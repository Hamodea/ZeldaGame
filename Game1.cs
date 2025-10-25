using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Collections.Generic;
using Zeldagame; // behåll om TextureManager ligger här

namespace Zelda_game
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

         
        private TileMap _map;       // kartan från CSV (id per ruta)
        private const int TILE_SIZE = 32; // ändra vid behov (måste matcha tileset-rutornas storlek)
        private int _tilesPerRow;       // hur många tiles på en rad i tileset-bilden

        private GameState _gameState;

        private List<Enemy> _enemies = new List<Enemy>();
        private Player _player;

        private List<Projectile> projectiles = new List<Projectile>();
        private KeyboardState prevKb;

        // simple score counter

        private int _score = 0;
        private const double PlayerHitCooldown = 1.0; // seconds of invulnerability after a hit
        private double _playerHitTimer = 0.0;



        // (valfritt) debug-font


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
            _graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

           
            // 1) Ladda texturer via din TextureManager
            TextureManager.LoadTexture(Content);
            
            _gameState = new GameState();
            _gameState.LoadContent(Content);

            _map = new TileMap();

            string csvPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Map", "ZeldaMap.csv");
            if (!File.Exists(csvPath)) throw new FileNotFoundException($"Hitta inte file: {csvPath}");
            _map.LoadWorld(csvPath);
            var start = _map.PlayerStart;
            int fW = 32;
            int fH = 40;


            // Load Player

            _player = new Player(TextureManager.playerSheet, _map, start.X, start.Y, TILE_SIZE, fW, fH);

            // Load Enemy

            _enemies.Clear();
            _enemies.Add(new Enemy(
                texture: TextureManager.enemyTex,
                startPixelPos: new Vector2(300, 50),
                tileSize: 32,
                axis: PatrolAxis.Horizontal,
                patrolTiles: 6,
                slowSpeed: 60f,
                fastSpeed: 120f,
                startFast: false
            ));
            _enemies.Add(new Enemy(
               texture: TextureManager.enemyTexRed,
               startPixelPos: new Vector2(390, 200),
               tileSize: 32,
               axis: PatrolAxis.Vertical,
               patrolTiles: 4,
               slowSpeed: 80f,
               fastSpeed: 140f,
               startFast: true
           ));
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _gameState.Update(gameTime);

            // honor GameState request to exit (menu Exit)
            if (_gameState.RequestExit)
                Exit();

            var kb = Keyboard.GetState();
            if (kb.IsKeyDown(Keys.Space) && prevKb.IsKeyUp(Keys.Space))
            {
                var proj = _player.TryShoot(TextureManager.playerAttackSheet);
                if (proj != null)
                {
                    projectiles.Add(proj);

                }
                else System.Diagnostics.Debug.WriteLine("TryShoot returned null (cooldown? texture null?)");
            }

            // Uppdatera projektiler
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Update(gameTime, _map);
                if (projectiles[i].IsDead)
                    projectiles.RemoveAt(i);
            }

            prevKb = kb;

            // decrement player hit timer (invulnerability window)
            if (_playerHitTimer > 0.0)
                _playerHitTimer -= gameTime.ElapsedGameTime.TotalSeconds;

            // update Enemy
            for (int e = 0; e < _enemies.Count; e++)
            {
                _enemies[e].Update(gameTime);
            }

            // Kollisionskontroll: projektil mot enemy
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                var proj = projectiles[i];
                if (proj.IsDead) { projectiles.RemoveAt(i); continue; }

                for (int e = _enemies.Count - 1; e >= 0; e--)
                {
                    var enemy = _enemies[e];
                    if (!enemy.Alive) continue;

                    if (proj.Bounds.Intersects(enemy.enemyBounds))
                    {
                        // markera enemy död och ta bort projektil
                        // if enemy drops key, that logic should be handled here (kept as before)
                        enemy.Kill();
                        proj.Kill();

                        // increment score for the kill
                        _score += 100;

                        // ta bort projektil direkt från listan
                        projectiles.RemoveAt(i);

                        // ta bort enemy från listan så den försvinner helt
                        _enemies.RemoveAt(e);

                        break;
                    }
                }
            }

            // Kollisionskontroll: enemy träffar player -> förlora 1 liv (enemy kvar)
            for (int e = _enemies.Count - 1; e >= 0; e--)
            {
                var enemy = _enemies[e];
                if (!enemy.Alive) continue;

                if (enemy.enemyBounds.Intersects(_player.Bounds))
                {
                    if (_playerHitTimer <= 0.0)
                    {
                        _player.Hurt();
                        _playerHitTimer = PlayerHitCooldown;

                        // check gameover
                        if (_player.Lives <= 0)
                        {
                            _gameState.ChangeState(GameState.State.GameOver);
                        }
                    }

                    // do not remove/killing the enemy here
                }
            }

            // update player once per frame (moved outside loops)
            _player.Update(gameTime);

            // --- Key pickup and door opening logic --- (outside loops)
            int px = _player.TileX;
            int py = _player.TileY;
            if (_map.IsInside(px, py))
            {
                var tileTex = _map.GetTileTexture(px, py);

                // pickup key
                if (tileTex == TextureManager.zeldaKey && !_player.HasKey)
                {
                    _player.PickupKey();
                    _map.SetTileTexture(px, py, TextureManager.grassTex, true);
                    System.Diagnostics.Debug.WriteLine("Key picked up.");
                }

                // open door if player has key
                if (tileTex == TextureManager.doorTex && _player.HasKey)
                {
                    _map.SetTileTexture(px, py, TextureManager.openDoor, true);
                    System.Diagnostics.Debug.WriteLine("Door opened.");
                }

                // If player stands on an opened door and we're playing -> Win
                var currentTex = _map.GetTileTexture(px, py);
                if (_gameState.CurrentState == GameState.State.Playing
                    && currentTex == TextureManager.openDoor
                    && _player.HasKey)
                {
                    _gameState.ChangeState(GameState.State.Win);
                    TextureManager.vectory?.Play();
                    System.Diagnostics.Debug.WriteLine("Player reached open door — WIN.");
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // Viktigt för skarpa pixlar
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _gameState.Draw(_spriteBatch);

            switch (_gameState.CurrentState)
            {
                case GameState.State.Playing:
                    _map.Draw(_spriteBatch);
                    _player.Draw(_spriteBatch);
                    foreach (var enemy in _enemies) { enemy.Draw(_spriteBatch); }
                    foreach(var p in projectiles) { p.Draw(_spriteBatch); }

                    // update GameState HUD values and draw HUD on top
                    _gameState.PlayerLives = _player.Lives;
                    _gameState.Score = _score;
                    _gameState.DrawHUD(_spriteBatch);
                    break;
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

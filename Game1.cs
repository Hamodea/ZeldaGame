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

        // ----- TILE/KARTA -----
        private Texture2D _tileset;     // hämtas från TextureManager
        private int[,] _map;            // kartan från CSV (id per ruta)
        private int _rows, _cols;
        private const int TILE_SIZE = 32; // ändra vid behov (måste matcha tileset-rutornas storlek)
        private int _tilesPerRow;       // hur många tiles på en rad i tileset-bilden

        // (valfritt) debug-font
        private SpriteFont _font;

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

            // 2) Hämta tilesheet från TextureManager
            // OBS: Byt namnet nedan till rätt fält/egenskap i din TextureManager
            // ex: TextureManager.tileset, TextureManager.terrainTex, etc.
            _tileset = TextureManager.grassTex; // <-- sätt detta till rätt property!

            if (_tileset == null)
                throw new Exception("Tileset saknas i TextureManager. Sätt t.ex. TextureManager.tileset i LoadTexture.");

            _tilesPerRow = _tileset.Width / TILE_SIZE;
            if (_tilesPerRow <= 0)
                throw new Exception($"Fel TILE_SIZE ({TILE_SIZE}) eller tileset-bredd ({_tileset.Width}).");

          

            // 3) Läs CSV-kartan (lägg filen i Content/Maps/ och 'Copy if newer')
            string csvPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Map", "ZeldaMap.csv");
            if (!File.Exists(csvPath))
                throw new FileNotFoundException($"Hittar inte kartfilen: {csvPath}");

            string[] lines = File.ReadAllLines(csvPath);
            _rows = lines.Length;
            _cols = lines[0].Split(',').Length;
            _map = new int[_rows, _cols];

            for (int y = 0; y < _rows; y++)
            {
                var parts = lines[y].Split(',');
                for (int x = 0; x < _cols; x++)
                    _map[y, x] = int.Parse(parts[x].Trim());
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // Viktigt för skarpa pixlar
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            for (int y = 0; y < _rows; y++)
            {
                for (int x = 0; x < _cols; x++)
                {
                    int id = _map[y, x];

                    Texture2D tex = TextureManager.grassTex; // standard = gräs
                    switch (id)
                    {
                        case 1: tex = TextureManager.waterTex; break;
                        case 2: tex = TextureManager.bridgeTex; break;
                        case 3: tex = TextureManager.wallTex; break;
                        case 4: tex = TextureManager.doorTex; break;
                        case 5: tex = TextureManager.treeTex; break;
                        case 6: tex = TextureManager.floorTex;break;
                        case 7: tex = TextureManager.zledaTex; break;


                       
                        
                    }

                    var pos = new Vector2(x * TILE_SIZE, y * TILE_SIZE);
                    _spriteBatch.Draw(tex, pos, Color.White);
                }
            }

            // (valfritt) debug-text
            // if (_font != null)
            //     _spriteBatch.DrawString(_font, $"rows={_rows} cols={_cols}", new Vector2(10,10), Color.White);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

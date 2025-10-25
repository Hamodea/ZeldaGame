using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeldagame
{
    public class Player
    {
        private readonly TileMap tileMap;
        private readonly Texture2D sheet;
        private readonly int tileSize;

        // Pixel-baserad position och mål
        private Vector2 position;      // aktuell pixelposition
        private Vector2 destination;   // pixelmål 
        private bool moving = false;
        private float speed = 200f;    // pixlar/sek — 

        // Sprite-sheet
        private readonly int frameWidth;
        private readonly int frameHeight;
        private readonly int framesPerRow;

        // Tile-koordinater (grid)
        public int TileX { get; private set; }
        public int TileY { get; private set; }

        // Animation
        private int dirRow = 0;                 // 0=Down,1=Left,2=Right,3=Up
        private int frame = 0;
        private double animTimer = 0;
        private double animSpeed = 100;         // ms per frame

        // Attack
        private float fireCooldown = 0.25f; // sekunder mellan skott
        private float fireTimer = 0f;
        public Vector2 CenterPixel => position + new Vector2(tileSize * 0.5f, tileSize * 0.5f);


        public Player(Texture2D playerSheet, TileMap tileMap, int startTileX, int startTileY,
                      int tileSize, int frameWidth, int frameHeight)
        {
            this.sheet = playerSheet;
            this.tileMap = tileMap;
            this.tileSize = tileSize;

            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            this.framesPerRow = sheet.Width / frameWidth;

            // init position från tile
            TileX = startTileX;
            TileY = startTileY;
            position = new Vector2(TileX * tileSize, TileY * tileSize);
            destination = position;
        }

        public Rectangle Bounds => new Rectangle((int)position.X, (int)position.Y, tileSize, tileSize);

        public int Lives { get; private set; } = 3; // Add this property

        public void Hurt()
        {
            if (Lives > 0)
            {
                Lives--;
                // play hurt / life lost sound
                TextureManager.playerDeathSfx?.Play();
            }
        }

        // New key state
        public bool HasKey { get; private set; } = false;
        public void PickupKey()
        {
            TextureManager.getKey?.Play();
            HasKey = true;
        }

        public void Update(GameTime gt)
        {
            var kb = Keyboard.GetState();

            float dt = (float)gt.ElapsedGameTime.TotalSeconds;
            // Decrement cooldown while it's positive; clamp to zero
            if (fireTimer > 0f)
            {
                fireTimer -= dt;
                if (fireTimer < 0f) fireTimer = 0f;
            }

            // Om vi inte rör oss: kolla input och försök starta en ny förflyttning
            if (!moving)
            {
                if (kb.IsKeyDown(Keys.Left))
                {
                    dirRow = 0;
                    TryStartMove(-1, 0);
                }
                else if (kb.IsKeyDown(Keys.Right))
                {
                    dirRow = 4;
                    TryStartMove(1, 0);
                }
                else if (kb.IsKeyDown(Keys.Up))
                {
                    dirRow = 2;
                    TryStartMove(0, -1);
                }
                else if (kb.IsKeyDown(Keys.Down))
                {
                    dirRow = 1;
                    TryStartMove(0, 1);
                }
            }

            // Om vi rör oss: glid mot destination
            if (moving)
            {
                float moveDt = (float)gt.ElapsedGameTime.TotalSeconds;
                Vector2 toDest = destination - position;
                float dist = toDest.Length();

                if (dist <= speed * moveDt) // framme nästa steg
                {
                    position = destination;
                    moving = false;

                    // uppdatera tile-koordinater när vi landat
                    TileX = (int)(position.X / tileSize);
                    TileY = (int)(position.Y / tileSize);

                    // håll idle-frame
                    frame = 0;
                    animTimer = 0;
                }
                else
                {
                    // gå vidare mot mål
                    Vector2 step = Vector2.Normalize(toDest) * speed * moveDt;
                    position += step;

                    // gånganimation
                    animTimer += gt.ElapsedGameTime.TotalMilliseconds;
                    if (animTimer >= animSpeed)
                    {
                        animTimer = 0;
                        frame = (frame + 1) % framesPerRow; // loopa kolumner
                    }
                }
            }
        }

        private void TryStartMove(int dx, int dy)
        {
            int nx = TileX + dx;
            int ny = TileY + dy;

            if (!tileMap.IsInside(nx, ny)) return;

            // If the target is a door and player has the key, open it before checking walkability
            var targetTex = tileMap.GetTileTexture(nx, ny);
            if (targetTex == TextureManager.doorTex && HasKey)
            {
                tileMap.SetTileTexture(nx, ny, TextureManager.openDoor, true);
            }

            // now check walkability (door will be walkable if it was opened above)
            if (!tileMap.IsWalkable(nx, ny)) return;

            destination = new Vector2(nx * tileSize, ny * tileSize);
            moving = true;

            // starta gånganimation från frame 0
            frame = 0;
            animTimer = 0;
        }

        public Vector2 FacingDir()
        {
            switch (dirRow)
            {
                case 0: return new Vector2(-1, 0); // Left
                case 4: return new Vector2(1, 0);  // Right
                case 2: return new Vector2(0, -1); // Up
                case 1: return new Vector2(0, 1);  // Down
                default: return new Vector2(0, 1);
            }
        }


        public Projectile TryShoot(Texture2D projTexture, float projSpeed = 300f, float life = 0.6f, int frameW = 16, int frameH = 16, float scale = 1f)
        {
            if (fireTimer > 0f || projTexture == null)
                return null;

            fireTimer = fireCooldown;

            var dir = FacingDir();
            if (dir == Vector2.Zero) dir = new Vector2(0, 1);

            // Spawna några pixlar framför spelaren
            float muzzleOffset = tileSize * 0.6f;
            var spawnPos = CenterPixel + dir * muzzleOffset;

            // play shoot sound
            TextureManager.shootSfx?.Play();
            const float rotationOffset = +MathF.PI / 2f;

            return new Projectile(
                projTexture,
                spawnPos,
                Vector2.Normalize(dir),
                projSpeed,
                life,
                frameW,
                frameH,
                scale,
                rotationOffset
            );
        }

        public void Draw(SpriteBatch sb)
        {
            int sx = frame * frameWidth;
            int sy = dirRow * frameHeight;
            var src = new Rectangle(sx, sy, frameWidth, frameHeight);

            float yOffset = frameHeight > tileSize ? -(frameHeight - tileSize) : 0f;

            sb.Draw(sheet, position + new Vector2(0, yOffset), src, Color.White);
        }
    }

}

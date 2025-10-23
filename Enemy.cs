using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeldagame
{
    public enum PatrolAxis {Horizontal, Vertical}
    public class Enemy
    {
        private readonly Texture2D texture;
        private Vector2 position;           // övre-vänstra hörnet i pixlar
        private readonly int tileSize;      // t.ex. 32
        private readonly PatrolAxis axis;   // H/V-patrull

        private readonly float slowSpeed;   // px/s
        private readonly float fastSpeed;
        private bool useFast;               // aktiv hastighet
        private double speedToggleTimer;    // växla mellan hastigheter

        private int dir = 1;                // +1 eller -1 längs axeln
        private readonly float minCoord;    // min x (H) / min y (V) i pixlar
        private readonly float maxCoord;    // max x (H) / max y (V) i pixlar


        public bool Alive {  get; private set; } = true;
        public Enemy(Texture2D texture, Vector2 startPixelPos, int tileSize,
                             PatrolAxis axis, int patrolTiles, float slowSpeed = 60f, float fastSpeed = 120f,
                             bool startFast = false)
        {
            this.texture = texture;
            this.position = startPixelPos;  // TOP-LEFT i pixlar
            this.tileSize = tileSize;
            this.axis = axis;
            this.slowSpeed = slowSpeed;
            this.fastSpeed = fastSpeed;
            this.useFast = startFast;

            if (axis == PatrolAxis.Horizontal)
            {
                minCoord = position.X;
                maxCoord = position.X + patrolTiles * tileSize;
            }
            else
            {
                minCoord = position.Y;
                maxCoord = position.Y + patrolTiles * tileSize;
            }
        }

        public Rectangle enemyBounds => new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);

        public void Update(GameTime gt)
        {
            if(!Alive) return;  
            float dt = (float)gt.ElapsedGameTime.TotalSeconds;
            float speed = useFast ? fastSpeed : slowSpeed;

            if(axis == PatrolAxis.Horizontal)
            {
                position.X += dir * speed * dt;
                if(position.X <= minCoord) { position.X = minCoord; dir = 1; }
                if (position.X >= maxCoord) { position.X = maxCoord; dir = -1; }
            }
            else
            {
                position.Y += dir * speed * dt;
                if (position.Y <= minCoord) { position.Y = minCoord; dir = 1; }
                if (position.Y >= maxCoord) { position.Y = maxCoord; dir = -1; }
            }

            // 2) Växla fart ibland
            speedToggleTimer += gt.ElapsedGameTime.TotalSeconds;
            if (speedToggleTimer >= 1.25)
            {
                speedToggleTimer = 0;
                useFast = !useFast;
            }

        }

        public void Draw(SpriteBatch sb)
        {
            if(!Alive) return;
            var dest = new Rectangle((int)position.X, (int)position.Y, tileSize, tileSize);
            sb.Draw(texture, dest, Color.White);

        }

        // Publicly allow killing the enemy (used by collision handling)
        public void Kill()
        {
            Alive = false;
            System.Diagnostics.Debug.WriteLine("Enemy killed");
        }
    }
}

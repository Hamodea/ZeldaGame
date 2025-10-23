using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeldagame
{
    public class Projectile
    {
        private Texture2D texture;
        private Vector2 position;
        private Vector2 velocity;
        private float speed;
        private float lifetime;       // återstående tid sekunder
        private readonly float maxLifetime;
        private readonly int frameWidth;
        private readonly int frameHeight;
        public float Scale = 2f;     
        public bool IsDead { get; private set; } = false;

        public float Depth = 0.6f;



        public Projectile(Texture2D texture, Vector2 startPos, Vector2 directionNormalized, float speed = 300f, float life = 1.0f, int frameWidth = 16, int frameHeight = 16, float scale = 2f)
        {
            this.texture = texture ?? throw new ArgumentNullException(nameof(texture));
            this.position = startPos;
            this.speed = speed;
            this.velocity = directionNormalized * speed;
            this.maxLifetime = life;
            this.lifetime = life;
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            this.Scale = scale;
        }


        public Rectangle Bounds
        {
            get
            {
                float w = frameWidth * Scale;
                float h = frameHeight * Scale;
                return new Rectangle((int)(position.X - w ), (int)(position.Y - h), (int)w, (int)h);
            }
        }

        public void Update(GameTime gt, TileMap tileMap)
        {
            float dt = (float)gt.ElapsedGameTime.TotalSeconds;
            position += velocity * dt;
            lifetime -= dt;

            if(tileMap != null && lifetime > 0f)
            {
                int tx = (int)Math.Floor(position.X / tileMap.TileSize);
                int ty = (int)Math.Floor(position.Y / tileMap.TileSize);

                if (!tileMap.IsInside(tx, ty) || !tileMap.IsWalkable(tx, ty))
                {
                    // döda projektilen direkt vid vägg
                    lifetime = 0f;
                }
            }

            if (lifetime <= 0f)
                IsDead = true;
        }


        public void Draw(SpriteBatch sb)
        {
            //Skippa ritning om projektilen är död
            if (IsDead)
                return;

            // Rita med destination-rect för pixelperfekt storlek
            float drawW = frameWidth * Scale;
            float drawH = frameHeight * Scale;
            var dest = new Rectangle(
                (int)Math.Floor(position.X - drawW / 2f),
                (int)Math.Floor(position.Y - drawH / 2f),
                (int)drawW, (int)drawH
            );

            var src = new Rectangle(0, 0, frameWidth, frameHeight); // antag enkel single-frame texture (16x16)
            sb.Draw(texture, dest, src, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Depth);
        }

        public void Kill()
        {

            IsDead = true;
            System.Diagnostics.Debug.WriteLine("Projectile killed");
        }




    }
}

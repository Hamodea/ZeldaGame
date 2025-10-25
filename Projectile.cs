using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

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

        // NYTT: rotation, origin och valfri offset om din sprite pekar "fel" i default
        private readonly float rotation;
        private readonly float rotationOffset;
        private readonly Vector2 origin;

        public Projectile(
            Texture2D texture,
            Vector2 startPos,
            Vector2 directionNormalized,
            float speed = 300f,
            float life = 1.0f,
            int frameWidth = 16,
            int frameHeight = 16,
            float scale = 2f,
            float rotationOffset = 0f // sätt t.ex. -MathF.PI/2 om din sprite pekar uppåt i default
        )
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

            this.rotationOffset = rotationOffset;

            // 0 rad = höger; atan2 ger rätt vinkel; lägg på valfri offset
            this.rotation = (float)Math.Atan2(directionNormalized.Y, directionNormalized.X) + rotationOffset;

            // rotera/rita runt mitten av källbilden
            this.origin = new Vector2(frameWidth / 2f, frameHeight / 2f);
        }

        public Rectangle Bounds
        {
            get
            {
                // Centrerad AABB (approx för kollision – duger i 2D)
                float w = frameWidth * Scale;
                float h = frameHeight * Scale;
                return new Rectangle(
                    (int)(position.X - w / 2f),
                    (int)(position.Y - h / 2f),
                    (int)w, (int)h
                );
            }
        }

        public void Update(GameTime gt, TileMap tileMap)
        {
            float dt = (float)gt.ElapsedGameTime.TotalSeconds;
            position += velocity * dt;
            lifetime -= dt;

            if (tileMap != null && lifetime > 0f)
            {
                int tx = (int)Math.Floor(position.X / tileMap.TileSize);
                int ty = (int)Math.Floor(position.Y / tileMap.TileSize);

                if (!tileMap.IsInside(tx, ty) || !tileMap.IsWalkable(tx, ty))
                {
                    lifetime = 0f; // träffar vägg → dö direkt
                }
            }

            if (lifetime <= 0f)
                IsDead = true;
        }

        public void Draw(SpriteBatch sb)
        {
            if (IsDead) return;

            var src = new Rectangle(0, 0, frameWidth, frameHeight);

            // VIKTIGT: center-baserad draw + rotation
            sb.Draw(
                texture,
                position,          // tolkas som centrum när 'origin' används
                src,
                Color.White,
                rotation,
                origin,            // rotera kring mitten
                Scale,
                SpriteEffects.None,
                Depth
            );
        }

        public void Kill()
        {
            IsDead = true;
            System.Diagnostics.Debug.WriteLine("Projectile killed");
        }
    }
}

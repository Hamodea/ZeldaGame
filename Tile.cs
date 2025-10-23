using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Zeldagame
{
    public class Tile
    {
        public Texture2D Texture { get; private set; }
        public Vector2 Position { get; private set; }
        public bool IsWalkable { get; private set; }

        public Tile(Texture2D texture, Vector2 position, bool isWalkable = true)
        {
            Texture = texture;
            Position = position;
            IsWalkable = isWalkable;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(Texture, Position, Color.White);
        }




    }
}

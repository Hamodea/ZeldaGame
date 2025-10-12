using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeldagame
{
  public static class TextureManager
    {


        public static Texture2D wallTex;
        public static Texture2D waterTex;
        public static Texture2D grassTex;
        public static Texture2D bridgeTex;
        public static Texture2D doorTex;
        public static Texture2D floorTex;
        public static Texture2D treeTex;
        public static Texture2D zledaTex;





        public static void LoadTexture(ContentManager content)
        {
            wallTex = content.Load<Texture2D>("wall");
            waterTex = content.Load<Texture2D>("water");
            grassTex = content.Load<Texture2D>("grass");
            bridgeTex = content.Load<Texture2D>("bridge");
            doorTex = content.Load<Texture2D>("door");
            floorTex = content.Load < Texture2D>("stonefloor");
            treeTex = content.Load<Texture2D>("bush");
            zledaTex = content.Load<Texture2D>("Zelda");





        }




    }

}





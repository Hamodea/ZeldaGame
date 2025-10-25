using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
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
        public static Texture2D playerTex;
        public static Texture2D playerSheet;
        public static Texture2D enemyTex;
        public static Texture2D enemyTexRed;
        public static Texture2D playerAttackSheet;
        public static Texture2D zeldaKey;
        public static Texture2D openDoor;



        public static SpriteFont font;

        public static Texture2D zledaBackground;

        public static Song menuMusic;

        // sound effects
        public static SoundEffect shootSfx;
        public static SoundEffect playerDeathSfx;
        public static SoundEffect getKey;
        public static SoundEffect vectory;

        public static void LoadTexture(ContentManager content)
        {
            wallTex = content.Load<Texture2D>("wall");
            waterTex = content.Load<Texture2D>("water");
            grassTex = content.Load<Texture2D>("grass");
            bridgeTex = content.Load<Texture2D>("bridge");
            doorTex = content.Load<Texture2D>("door");
            floorTex = content.Load<Texture2D>("stonefloor");
            treeTex = content.Load<Texture2D>("bush");
            zledaTex = content.Load<Texture2D>("Zelda");
            //playerTex = content.Load<Texture2D>("player");
            playerSheet = content.Load<Texture2D>("playerSheet2");
            enemyTex = content.Load<Texture2D>("OctorokBlue");
            enemyTexRed = content.Load<Texture2D>("OctorokRed");
            playerAttackSheet = content.Load<Texture2D>("zeldaSword1");
            zeldaKey = content.Load<Texture2D>("key32");
            openDoor = content.Load<Texture2D>("opendoor");

            font = content.Load<SpriteFont>("UiFont");

            // background
            zledaBackground = content.Load<Texture2D>("zeldaBackgrund");

            // song
            menuMusic = content.Load<Song>("Sonds/zeldaIntro");

            
            shootSfx = content.Load<SoundEffect>("Sonds/hitwav");
            playerDeathSfx = content.Load<SoundEffect>("Sonds/lifelostWav");
            getKey = content.Load<SoundEffect>("Sonds/key");
            vectory = content.Load<SoundEffect>("Sonds/Victory");

        }
    }
}














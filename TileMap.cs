using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace Zeldagame
{
    public class TileMap
    {
        private int[,] map;              // själva kartan (från CSV)
        private Texture2D tileset;       // spritesheet med alla tiles
        private int tileSize;            // t.ex. 32 pixlar
        private int tilesPerRow;         // hur många rutor per rad i spritesheetet

        public TileMap(Texture2D tileset, string csvPath, int tileSize)
        {
            this.tileset = tileset;
            this.tileSize = tileSize;
            this.tilesPerRow = tileset.Width / tileSize;

            //  Läs in CSV-filen (alla rader)
            string[] lines = File.ReadAllLines(csvPath);
            int rows = lines.Length;
            int cols = lines[0].Split(',').Length;

            map = new int[rows, cols];

            for (int y = 0; y < rows; y++)
            {
                var parts = lines[y].Split(',');
                for (int x = 0; x < cols; x++)
                {
                    map[y, x] = int.Parse(parts[x].Trim());
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    int gid = map[y, x]; // tile ID från CSV
                    if (gid < 0) continue;

                    // 🧩 Beräkna vilken ruta i spritesheetet vi ska använda
                    int sx = (gid % tilesPerRow) * tileSize;
                    int sy = (gid / tilesPerRow) * tileSize;

                    var src = new Rectangle(sx, sy, tileSize, tileSize);
                    var dst = new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);

                    sb.Draw(tileset, dst, src, Color.White);
                }
            }
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace Zeldagame
{
    public class TileMap
    {
        private Tile[,] tiles;
        private int rows, cols;
        private readonly int tileSize = 32;

        public Point PlayerStart { get; private set; } = new Point(5, 1);

        public int Rows => rows;
        public int Cols => cols;
        public int TileSize => tileSize;

        public void LoadWorld(string path)
        {
            var lines = new List<string>();
            using (var sr = new StreamReader(path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    lines.Add(line.Trim());
                }
            }

            rows = lines.Count;
            cols = lines[0].Split(',').Length;
            tiles = new Tile[rows, cols];

            for (int y = 0; y < rows; y++)
            {
                var parts = lines[y].Split(',');
                for (int x = 0; x < cols; x++)
                {
                    int id = int.Parse(parts[x].Trim());
                    Texture2D tex;
                    bool walkable = true;

                    switch (id)
                    {
                        case 0: tex = TextureManager.grassTex; break;
                        case 1: tex = TextureManager.waterTex; walkable = false; break;
                        case 2: tex = TextureManager.bridgeTex; break;
                        case 3: tex = TextureManager.wallTex; walkable = false; break;
                        case 4: tex = TextureManager.doorTex; walkable = false; break;
                        case 5: tex = TextureManager.treeTex; walkable = false; break;
                        case 6: tex = TextureManager.floorTex ?? TextureManager.grassTex; break;
                        case 7: tex = TextureManager.zledaTex ?? TextureManager.grassTex; break;
                        case 8: tex = TextureManager.grassTex; PlayerStart = new Point(x, y); break;
                        case 9: tex = TextureManager.zeldaKey; break;
                        

                        default: tex = TextureManager.grassTex; break;
                    }

                    tex ??= TextureManager.grassTex;

                    var pos = new Vector2(x * tileSize, y * tileSize);
                    tiles[y, x] = new Tile(tex, pos, walkable);
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (tiles == null) return;

            for (int y = 0; y < rows; y++)
                for (int x = 0; x < cols; x++)
                    tiles[y, x].Draw(sb);
        }

        public bool IsInside(int x, int y) => y >= 0 && y < rows && x >= 0 && x < cols;
        public bool IsWalkable(int x, int y) => IsInside(x, y) && tiles[y, x].IsWalkable;

        // Hjälpare: kolla walkable vid pixelposition
        public bool IsWalkableAtPixel(Vector2 pixel)
        {
            int tx = (int)(pixel.X / tileSize);
            int ty = (int)(pixel.Y / tileSize);
            return IsWalkable(tx, ty);
        }

        // New helpers to read and modify tiles at runtime
        public Texture2D GetTileTexture(int x, int y)
        {
            if (!IsInside(x, y)) return null;
            return tiles[y, x].Texture;
        }

        public void SetTileTexture(int x, int y, Texture2D texture, bool? isWalkable = null)
        {
            if (!IsInside(x, y)) return;
            var pos = tiles[y, x].Position;
            bool walk = isWalkable ?? tiles[y, x].IsWalkable;
            tiles[y, x] = new Tile(texture ?? TextureManager.grassTex, pos, walk);
        }
    }
}

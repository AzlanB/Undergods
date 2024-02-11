using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Undergods
{
    // Level Version 1.0
    // Using Tile Version 1.1
    class Level
    {
        private Tile[,,] arena;
        public float scale = 1.5f;
        private static Random rng = new Random();

        public Level(int x, int y, int z)
        {
            arena = new Tile[x, y, z];
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    for (int k = 0; k < z; k++)
                        arena[i, j, k] = new Tile(new Vector3(i, j, k));
                }
            }
        }

        public Level(int x, int y, int z, List<Texture2D> textures) : this(x, y, z)
        {
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    arena[i, j, 0] = new Tile(textures[rng.Next(0, textures.Count)], new Vector3(i, j, 0));

                    for (int k = 1; k < z; k++)
                    {
                        arena[i, 0, k] = new Tile(textures[rng.Next(0, textures.Count)], new Vector3(i, 0, k));
                        arena[0, j, k] = new Tile(textures[rng.Next(0, textures.Count)], new Vector3(0, j, k));
                    }
                }
            }
        }

        /*public Level(int x, int y, int z, Texture2D texture, string fileName) : this(x, y, z)
        {
            // Nonfunctional
        }*/

        /// <summary>
        /// Converts a point relative to the 3D tile grid into a position on
        /// the screen with the proper isometric progection
        /// </summary>
        public Vector2 GridToIsometric(Vector3 gridCoords, int screenWidth, int screenHeight)
        {
            float screenX = screenWidth / 2 + (Tile.rightCorner.X - Tile.backCorner.X) * scale * gridCoords.X + (Tile.leftCorner.X - Tile.backCorner.X) * scale * gridCoords.Y;
            float screenY = Tile.height * scale * (arena.GetLength(2)-0.25f) + (Tile.rightCorner.Y - Tile.backCorner.Y) * scale * gridCoords.X + (Tile.leftCorner.Y - Tile.backCorner.Y) * scale * gridCoords.Y - Tile.height * scale * gridCoords.Z;
            return new Vector2(screenX, screenY);
        }

        public void Draw(SpriteBatch spriteBatch, int screenWidth, int screenHeight)
        {
            List<Tile> tiles = new List<Tile>();
            foreach (Tile t in arena)
                tiles.Add(t);
            tiles.Sort();
            foreach (Tile t in tiles)
                t.Draw(spriteBatch, this, screenWidth, screenHeight);
        }

        public int GetX()
        {
            return arena.GetLength(0);
        }

        public int GetY()
        {
            return arena.GetLength(1);
        }

        public int GetZ()
        {
            return arena.GetLength(2);
        }
    }
}

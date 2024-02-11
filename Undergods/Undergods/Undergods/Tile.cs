using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Undergods
{
    // Tile Version 1.1
    // Using Object3D Version 1.0
    // Using Level 1.0
    class Tile : Object3D, IComparable<Tile>
    {
        public static Vector2 backCorner = new Vector2(54, 8);
        public static Vector2 frontCorner = new Vector2(54, 49);
        public static Vector2 leftCorner = new Vector2(13, 28);
        public static Vector2 rightCorner = new Vector2(94, 28);
        public static float height = 98 - frontCorner.Y;

        public Texture2D texture;

        public Tile(Vector3 gridCoords) : this(null, gridCoords) { }

        public Tile(Texture2D texture, Vector3 gridCoords) : base("Tile", gridCoords)
        {
            this.texture = texture;
        }

        public override int CompareTo(Object3D point)
        {
            if (point is Tile)
                return CompareTo((Tile)point);

            if (CompareByDepth(new Vector3(gridCoords.X, gridCoords.Y, gridCoords.Z - 1), point.gridCoords) > 0)
                return CompareByDepth(new Vector3(gridCoords.X, gridCoords.Y, gridCoords.Z - 1), point.gridCoords);
            if (CompareByDepth(new Vector3(gridCoords.X + 1, gridCoords.Y + 1, gridCoords.Z), point.gridCoords) <= 0)
                return CompareByDepth(new Vector3(gridCoords.X + 1, gridCoords.Y + 1, gridCoords.Z), point.gridCoords);
            return (point.gridCoords.X < gridCoords.X + 1 || point.gridCoords.Y < gridCoords.Y + 1 || point.gridCoords.Z < gridCoords.Z) ? 1 : -1;
        }

        public int CompareTo(Tile other)
        {
            return CompareByDepth(gridCoords, other.gridCoords);
        }

        /// <summary>
        /// Draws the tile at its appropriate screen position given its coordinates
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, Level arena, int screenWidth, int screenHeight)
        {
            if (texture != null)
                spriteBatch.Draw(texture, arena.GridToIsometric(gridCoords, screenWidth, screenHeight), null, Color.White, 0, backCorner, arena.scale, SpriteEffects.None, 0);
        }
    }
}

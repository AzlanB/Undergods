using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Undergods
{
    // Object3D Version 1.0
    class Object3D : IComparable<Object3D>
    {
        public string type;
        public Vector3 gridCoords;

        public Object3D(string type, Vector3 gridCoords)
        {
            this.type = type;
            this.gridCoords = gridCoords;
        }

        public static int CompareByDepth(Vector3 coord1, Vector3 coord2)
        {
            return (coord1.X + coord1.Y + coord1.Z).CompareTo(coord2.X + coord2.Y + coord2.Z);
        }

        public virtual int CompareTo(Object3D other)
        {
            return CompareByDepth(gridCoords, other.gridCoords);
        }
    }
}

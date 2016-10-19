using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpitecture.Maths
{
    public struct Vector3B
    {
        public byte X, Y, Z;

        public static readonly Vector3B Zero = new Vector3B(0, 0, 0);

        public Vector3B(byte X, byte Y, byte Z)
        {
            this.X = X; this.Y = Y; this.Z = Z;
        }

        public Vector3F Scale(float scale)
        {
            return new Vector3F(X * scale, Y * scale, Z * scale);
        }

        public static Vector3B operator -(Vector3B left, Vector3B right)
        {
            return new Vector3B(
                (byte)(left.X - right.X),
                (byte)(left.Y - right.Y),
                (byte)(left.Z - right.Z));
        }

        public static bool operator ==(Vector3B left, Vector3B right)
        {
            return left.X == right.X && left.Y == right.Y && left.Z == right.Z;
        }

        public static bool operator !=(Vector3B left, Vector3B right)
        {
            return left.X != right.X || left.Y != right.Y || left.Z != right.Z;
        }
    }
}

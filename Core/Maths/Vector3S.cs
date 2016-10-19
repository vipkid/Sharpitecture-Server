namespace Sharpitecture.Maths
{
    public struct Vector3S
    {
        public short X, Y, Z;

        public static readonly Vector3S Zero = new Vector3S(0, 0, 0);

        public Vector3S(short X, short Y, short Z)
        {
            this.X = X; this.Y = Y; this.Z = Z;
        }

        public Vector3F Scale(float scale)
        {
            return new Vector3F(scale * X, scale * Y, scale * Z);
        }

        public static Vector3S operator -(Vector3S left, Vector3S right)
        {
            return new Vector3S(
                (short)(left.X - right.X), 
                (short)(left.Y - right.Y), 
                (short)(left.Z - right.Z));
        }

        public static bool operator ==(Vector3S left, Vector3S right)
        {
            return left.X == right.X && left.Y == right.Y && left.Z == right.Z;
        }

        public static bool operator !=(Vector3S left, Vector3S right)
        {
            return left.X != right.X || left.Y != right.Y || left.Z != right.Z;
        }
    }
}

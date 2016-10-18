namespace Sharpitecture.Chatting
{
    public struct Colour
    {
        public char code;
        public readonly string name;
        public byte R, G, B, A;

        public Colour(string name, char c, byte r, byte g, byte b, byte a)
        {
            this.code = c;
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
            this.name = name;
        }
    }
}

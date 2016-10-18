using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpitecture.Chatting
{
    public class ColourDefinitions
    {
        public const string ReservedCharacters = "0123456789abcdef";

        private static List<Colour> _colours = new List<Colour>()
        {
            c.Black,
            c.Navy,
            c.Gray,
            c.Teal,
            c.Maroon,
            c.Purple,
            c.Gold,
            c.Silver,
            c.Gray,
            c.Blue,
            c.Lime,
            c.Aqua,
            c.Red,
            c.Pink,
            c.Yellow,
            c.White
        };

        public static bool IsValidColourCode(char c) => _colours.Any(col => col.code == c);

        public static Colour FindColour(string col)
        {
            if (col.StartsWith("&") && col.Length == 2)
                return _colours.FirstOrDefault(colour => colour.code == col[1]);
            return _colours.FirstOrDefault(colour => colour.name.CaselessEquals(col));
        }
    }

    public static class c
    {
        public static readonly Colour Black
            = new Colour("black", '0', 0, 0, 0, 255);
        public static readonly Colour Navy
            = new Colour("navy", '1', 0, 0, 191, 255);
        public static readonly Colour Green
            = new Colour("green", '2', 0, 191, 0, 255);
        public static readonly Colour Teal
            = new Colour("teal", '3', 0, 191, 191, 255);
        public static readonly Colour Maroon
            = new Colour("maroon", '4', 191, 0, 0, 255);
        public static readonly Colour Purple
            = new Colour("purple", '5', 191, 0, 191, 255);
        public static readonly Colour Gold
            = new Colour("gold", '6', 191, 191, 0, 255);
        public static readonly Colour Silver
            = new Colour("silver", '7', 191, 191, 191, 255);
        public static readonly Colour Gray
            = new Colour("gray", '8', 64, 64, 64, 255);
        public static readonly Colour Blue
            = new Colour("blue", '9', 64, 64, 255, 255);
        public static readonly Colour Lime
            = new Colour("lime", 'a', 64, 255, 64, 255);
        public static readonly Colour Aqua
            = new Colour("aqua", 'b', 64, 255, 255, 255);
        public static readonly Colour Red
            = new Colour("red", 'c', 255, 64, 64, 255);
        public static readonly Colour Pink
            = new Colour("pink", 'd', 255, 64, 255, 255);
        public static readonly Colour Yellow
            = new Colour("yellow", 'e', 255, 255, 64, 255);
        public static readonly Colour White
            = new Colour("white", 'f', 255, 255, 255, 255);
    }
}

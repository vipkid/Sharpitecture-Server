using System.Collections.Generic;

namespace Sharpitecture.Levels.Seeds
{
    public abstract class Seed
    {
		public abstract string Name { get; }
        public abstract void Generate(Level level, params object[] parameters);

        internal static List<Seed> Seeds = new List<Seed>();

		public static void Initialise()
        {
            Seeds.Add(new FlatSeed());
        }
    }
}

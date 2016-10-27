using System.Collections.Generic;

namespace Sharpitecture.Levels.Seeds
{
    public abstract class Seed
    {
        /// <summary>
        /// The name of the seed
        /// </summary>
		public abstract string Name { get; }

        /// <summary>
        /// Generates a level using the seed's algorithm
        /// </summary>
        public abstract void Generate(Level level, params object[] parameters);

        /// <summary>
        /// The list of seeds supported by the server
        /// </summary>
        internal static List<Seed> Seeds { get; private set; } = new List<Seed>();

        /// <summary>
        /// Initialises all the level seeds
        /// </summary>
		public static void Initialise()
        {
            Seeds.Add(new FlatSeed());
        }
    }
}

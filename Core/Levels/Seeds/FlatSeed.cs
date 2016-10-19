using Sharpitecture.Levels.Blocks;
using Sharpitecture.Utils.Logging;

namespace Sharpitecture.Levels.Seeds
{
    public class FlatSeed : Seed
    {
        public override string Name
        {
            get
            {
                return "Flat";
            }
        }

        public override void Generate(Level level, params object[] parameters)
        {
            short halfWay = (short)(level.Height / 2);

            byte surface = CoreBlock.Grass;
            byte underground = CoreBlock.Dirt;

            if (!(parameters.Length >= 2 && 
                byte.TryParse(parameters[0].ToString(), out surface) &&
                byte.TryParse(parameters[1].ToString(), out underground)))
            {
                Logger.Log("Invalid parameters passed on FlatSeed generation", LogType.Error);
                Logger.LogF("Parameters passed: {0}, {1}", LogType.Error, parameters[0], parameters[1]);
            }

            for (short x = 0; x < level.Width; ++x)
                for (short z = 0; z < level.Depth; ++z)
                    for (short y = 0; y <= halfWay; ++y)
                        level.SetTile(x, y, z, y == halfWay ? surface : underground);
        }
    }
}

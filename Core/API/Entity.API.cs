using Sharpitecture.Levels;

namespace Sharpitecture.Entities
{
    public partial class Entity
    {
        public delegate void LevelChange(Level from, Level to);

        /// <summary>
        /// What happens when a player changes levels
        /// </summary>
        public event LevelChange OnLevelChange = null;
    }
}

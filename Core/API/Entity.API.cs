using Sharpitecture.Levels;

namespace Sharpitecture.Entities
{
    public partial class Entity
    {
        public delegate void LevelChange(Level from, Level to);
        public delegate void Blockchange(Player player, short x, short y, short z, byte type);

        /// <summary>
        /// What happens when a player changes levels
        /// </summary>
        public event LevelChange OnLevelChange = null;

        /// <summary>
        /// What happens when a player changes a block
        /// </summary>
        public event Blockchange OnBlockchange = null;
    }
}

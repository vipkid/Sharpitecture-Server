using Sharpitecture.Entities;

namespace Sharpitecture.Levels
{
    public sealed partial class Level
    {
        public delegate void EntityJoin(Entity entity);
        public delegate void EntityLeave(Entity entity);

        /// <summary>
        /// What happens when an entity joins a level
        /// </summary>
        public event EntityJoin OnEntityJoin = null;

        /// <summary>
        /// What happens when an entity leaves a level
        /// </summary>
        public event EntityLeave OnEntityLeave = null;
    }
}

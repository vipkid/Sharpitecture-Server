using Sharpitecture.Entities;

namespace Sharpitecture.Levels
{
    public sealed partial class Level
    {
        public delegate void EntityAction(Entity entity);

        /// <summary>
        /// What happens when an entity joins a level
        /// </summary>
        public event EntityAction OnEntityJoin = null;

        /// <summary>
        /// What happens when an entity leaves a level
        /// </summary>
        public event EntityAction OnEntityLeave = null;
    }
}

using Sharpitecture.Levels;
using Sharpitecture.Networking;
using Sharpitecture.Tasks;
using System.Text;

namespace Sharpitecture.Entities
{
    public static class EntityHandler
    {
        /// <summary>
        /// Spawns "other" to "target"
        /// <para>This is soley used for the first spawn of a player</para>
        /// </summary>
        public static void SpawnEntity(Entity target, Entity other, bool mutual = true)
        {
            if (mutual) SpawnEntity(other, target, false);
            if (target.VisibleIDs.Contains(other.EntityID)) return;
            if (target is Player)
            {
                ByteBuffer buffer = new ByteBuffer(Opcodes.SpawnPlayer.length);
                buffer.WriteByte(Opcodes.SpawnPlayer.id);
                buffer.WriteByte(other.EntityID);
                buffer.WriteString(other.HoverName, Encoding.ASCII);
                buffer.WriteShort(other.Position.X);
                buffer.WriteShort(other.Position.Y);
                buffer.WriteShort(other.Position.Z);
                buffer.WriteByte(other.Rotation.X);
                buffer.WriteByte(other.Rotation.Y);
                ((Player)target).SendRaw(buffer.Data);
            }

            target.VisibleIDs.Add(other.EntityID);
            if (mutual) SpawnEntity(other, target, false);
        }

        /// <summary>
        /// Despawns "other" from "target"
        /// <para>This is soley used for when entities leave a level</para>
        /// </summary>
        public static void DespawnEntity(Entity target, Entity other, bool mutual = false)
        {
            if (mutual) DespawnEntity(other, target, false);
            if (!target.VisibleIDs.Contains(other.EntityID)) return;

            if (target is Player)
            {
                /*ByteBuffer buffer = new ByteBuffer(Opcodes.DespawnPlayer.length);
                buffer.WriteByte(Opcodes.DespawnPlayer.id);
                buffer.WriteByte(other.EntityID);*/
                ((Player)target).SendRaw(new byte[2] { Opcodes.DespawnPlayer.id, other.EntityID });
            }

            target.VisibleIDs.Remove(other.EntityID);
        }

        /// <summary>
        /// Spawns all entities on a specified level
        /// </summary>
        public static void SpawnAllEntities(Entity target, Level level, bool mutual = true)
        {
            foreach (Entity entity in level.Entities)
                SpawnEntity(target, entity, mutual);
        }

        /// <summary>
        /// Despawns all entities from "target"
        /// </summary>
        public static void DespawnAllEntities(Entity target, bool mutual = false)
        {
            lock (target.VisibleIDs)
                foreach (byte id in target.VisibleIDs)
                {
                    Entity entity = target.Level.GetEntityByID(id);
                    if (entity == null) continue;
                    DespawnEntity(target, entity, mutual);
                }
        }

        public static void Initialise()
        {
            Task EntityUpdateTask = new Task(EntityPositionUpdateHandler)
            {
                Name = "Entity.Update",
                IsRecurring = true,
                Timeout = 100
            };

            Server.QueueTask(EntityUpdateTask);
        }

        static void EntityPositionUpdateHandler()
        {
            Server.Players.ForEach(player => player.UpdatePosition());
        }
    }
}

using Sharpitecture.Levels.IO.NBT;
using System;
using System.IO;

namespace Sharpitecture.Levels.IO
{
    public class NbtLoader
    {
        public static void Save(Level level, bool compress = true)
        {
            if (!Directory.Exists("levels")) Directory.CreateDirectory("levels");
            if (!Directory.Exists("levels/nbt")) Directory.CreateDirectory("levels/nbt");

            string path = "levels/nbt/" + level.Name + (compress ? ".cw" : ".nbt");

            NbtFile file = new NbtFile(path);
            file.Root.Name = "ClassicWorld";
            file.Add(new NbtInt() { Name = "FormatVersion", Value = 1 });
            file.Add(new NbtString() { Name = "Name", Value = level.Name });
            file.Add(new NbtByteArray() { Name = "UUID", Value = level.UUID });
            file.Add(new NbtShort() { Name = "X", Value = level.Width });
            file.Add(new NbtShort() { Name = "Y", Value = level.Height });
            file.Add(new NbtShort() { Name = "Z", Value = level.Depth });

            NbtCompound CreatedBy = new NbtCompound() { Name = "CreatedBy" };
            CreatedBy.AddField(new NbtString() { Name = "Service", Value = "Sharpitecture" });
            CreatedBy.AddField(new NbtString() { Name = "Author", Value = string.Empty });

            file.Add(CreatedBy);

            NbtCompound MapGeneration = new NbtCompound() { Name = "MapGeneration" };
            CreatedBy.AddField(new NbtString() { Name = "Software", Value = "Sharpitecture" });
            CreatedBy.AddField(new NbtString() { Name = "MapGenerationName", Value = string.Empty });

            file.Add(new NbtLong() { Name = "TimeCreated", Value = DateTime.UtcNow.Ticks });
            file.Add(new NbtLong() { Name = "LastAccessed", Value = DateTime.UtcNow.Ticks });
            file.Add(new NbtLong() { Name = "LastModified", Value = DateTime.UtcNow.Ticks });

            NbtCompound Spawn = new NbtCompound() { Name = "Spawn" };
            Spawn.AddField(new NbtShort() { Name = "X", Value = 0 });
            Spawn.AddField(new NbtShort() { Name = "Y", Value = 0 });
            Spawn.AddField(new NbtShort() { Name = "Z", Value = 0 });
            Spawn.AddField(new NbtByte() { Name = "H", Value = 0 });
            Spawn.AddField(new NbtByte() { Name = "P", Value = 0 });

            file.Add(Spawn);

            file.Add(new NbtByteArray() { Name = "BlockArray", Value = level.Blocks });
            file.Add(new NbtCompound() { Name = "Metadata" });

            file.Save();
        }
    }
}

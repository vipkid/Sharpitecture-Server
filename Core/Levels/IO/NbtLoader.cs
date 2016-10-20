using Sharpitecture.Levels.IO.NBT;
using Sharpitecture.Utils.Logging;
using System.IO;

namespace Sharpitecture.Levels.IO
{
    public class NbtLoader
    {
        public static void Save(Level level, bool compress = true)
        {
            compress = true;
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
            CreatedBy.AddField(new NbtString() { Name = "Author", Value = level.Authors });

            file.Add(CreatedBy);

            NbtCompound MapGeneration = new NbtCompound() { Name = "MapGeneration" };
            CreatedBy.AddField(new NbtString() { Name = "Software", Value = "Sharpitecture" });
            CreatedBy.AddField(new NbtString() { Name = "MapGenerationName", Value = level.SeedName });

            file.Add(new NbtLong() { Name = "TimeCreated", Value = level.TimeCreated });
            file.Add(new NbtLong() { Name = "LastAccessed", Value = Extensions.GetUnixTimestamp() });
            file.Add(new NbtLong() { Name = "LastModified", Value = level.LastModified });

            NbtCompound Spawn = new NbtCompound() { Name = "Spawn" };
            Spawn.AddField(new NbtShort() { Name = "X", Value = level.Spawn.X });
            Spawn.AddField(new NbtShort() { Name = "Y", Value = level.Spawn.Y });
            Spawn.AddField(new NbtShort() { Name = "Z", Value = level.Spawn.Z });
            Spawn.AddField(new NbtByte() { Name = "H", Value = level.SpawnRot.X });
            Spawn.AddField(new NbtByte() { Name = "P", Value = level.SpawnRot.Y });

            file.Add(Spawn);

            file.Add(new NbtByteArray() { Name = "BlockArray", Value = level.Blocks });
            file.Add(new NbtCompound() { Name = "Metadata" });

            file.Save();
        }

        public static Level Load(string path, bool fullPath = false)
        {
            string lvlPath = fullPath ? path : "levels/nbt/" + path + ".cw";

            if (!File.Exists(lvlPath))
            {
                Logger.LogF("[NBTLoader] Level not found '{0}'", LogType.Error, lvlPath);
                return null;
            }

            NbtFile levelFile = new NbtFile(lvlPath);
            levelFile.Load();
            Level level;

            try {
                level = new Level(levelFile["Name"]?.StringValue ?? Path.GetFileNameWithoutExtension(path),
                levelFile["X"].ShortValue,
                levelFile["Y"].ShortValue,
                levelFile["Z"].ShortValue);
            }
            catch
            {
                Logger.LogF("Error when loading file '{0}'", LogType.Error, fullPath);
                return null;
            }

            level.Blocks = levelFile["BlockArray"].ByteArrayValue;
            level.UUID = levelFile["UUID"]?.ByteArrayValue ?? level.UUID;
            
            try
            {
                NbtCompound Spawn = (NbtCompound)levelFile["Spawn"];
                level.Spawn.X = Spawn.GetField("X")?.ShortValue ?? level.Spawn.X;
                level.Spawn.Y = Spawn.GetField("Y")?.ShortValue ?? level.Spawn.Y;
                level.Spawn.Z = Spawn.GetField("Z")?.ShortValue ?? level.Spawn.Z;
                level.SpawnRot.X = Spawn.GetField("H")?.ByteValue ?? level.SpawnRot.X;
                level.SpawnRot.Y = Spawn.GetField("P")?.ByteValue ?? level.SpawnRot.Y;
            }
            catch
            {
                Logger.LogF("Error when loading spawn for '{0}'", LogType.Error, level.Name);
            }

            level.TimeCreated = levelFile["TimeCreated"]?.LongValue ?? Extensions.GetUnixTimestamp();
            level.LastModified = levelFile["LastModified"]?.LongValue ?? Extensions.GetUnixTimestamp();
            level.LastAccessed = Extensions.GetUnixTimestamp();
            
            try
            {
                NbtCompound MapGen = (NbtCompound)levelFile["MapGeneration"];
                level.SeedName = MapGen.GetField("MapGenerationName")?.StringValue ?? "Unknown";
            }
            catch { }

            try
            {
                NbtCompound CreatedBy = (NbtCompound)levelFile["CreatedBy"];
                level.Authors = CreatedBy.GetField("Author")?.StringValue ?? "Unknown";
            } catch { }

            return level;
        }
    }
}

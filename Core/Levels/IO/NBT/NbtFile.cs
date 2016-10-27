using Sharpitecture.Utils.Logging;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace Sharpitecture.Levels.IO.NBT
{
    public class NbtFile
    {
        public readonly string Path;
        public NbtCompound Root { get; set; }

        public NbtFile(string Path)
        {
            this.Path = Path;
            Root = new NbtCompound();
        }

        public NbtField this[string index]
        {
            get
            {
                return Root.GetField(index);
            }
        }

        public void Add(NbtField field)
        {
            Root.AddField(field);
        }

        public void Load()
        {
            using (FileStream stream = File.OpenRead(Path))
            {
                using (GZipStream gs = new GZipStream(stream, CompressionMode.Decompress))
                {
                    BinaryReader reader = new BinaryReader(gs);
                    if (reader.ReadByte() != 10)
                        throw new FormatException("Start of file must be NbtCompound (ID: 10)");
                    Root = ReadCompound(reader);
                }
            }
        }

        private NbtCompound ReadCompound(BinaryReader reader)
        {
            NbtCompound compound = new NbtCompound()
            {
                Name = ReadTagName(reader)
            };
            byte typeID;
            while (true)
            {
                typeID = reader.ReadByte();

                switch (typeID)
                {
                    case 0:
                        return compound;
                    case 1:
                        compound.AddField(ReadByte(reader));
                        break;
                    case 2:
                        compound.AddField(ReadShort(reader));
                        break;
                    case 3:
                        compound.AddField(ReadInt(reader));
                        break;
                    case 4:
                        compound.AddField(ReadLong(reader));
                        break;
                    case 5:
                        compound.AddField(ReadFloat(reader));
                        break;
                    case 6:
                        compound.AddField(ReadDouble(reader));
                        break;
                    case 7:
                        compound.AddField(ReadByteArray(reader));
                        break;
                    case 8:
                        compound.AddField(ReadString(reader));
                        break;
                    case 10:
                        compound.AddField(ReadCompound(reader));
                        break;
                    default:
                        Logger.LogF("[NBT] Error loading file: Unknown type ID '{0}'", LogType.Error, typeID);
                        return null;
                }
            }
        }

        private NbtByte ReadByte(BinaryReader reader)
        {
            return new NbtByte() { Name = ReadTagName(reader), Value = reader.ReadByte() };
        }

        private NbtShort ReadShort(BinaryReader reader)
        {
            return new NbtShort() { Name = ReadTagName(reader), Value = IPAddress.NetworkToHostOrder(reader.ReadInt16()) };
        }

        private NbtInt ReadInt(BinaryReader reader)
        {
            return new NbtInt() { Name = ReadTagName(reader), Value = IPAddress.NetworkToHostOrder(reader.ReadInt32()) };
        }

        public NbtLong ReadLong(BinaryReader reader)
        {
            return new NbtLong() { Name = ReadTagName(reader), Value = IPAddress.NetworkToHostOrder(reader.ReadInt64()) };
        }

        public NbtFloat ReadFloat(BinaryReader reader)
        {
            return new NbtFloat() { Name = ReadTagName(reader), Value = reader.ReadSingle() };
        }
        
        public NbtDouble ReadDouble(BinaryReader reader)
        {
            return new NbtDouble() { Name = ReadTagName(reader), Value = reader.ReadDouble() };
        }

        public NbtByteArray ReadByteArray(BinaryReader reader)
        {
            string name = ReadTagName(reader);
            int length = IPAddress.NetworkToHostOrder(reader.ReadInt32());
            return new NbtByteArray() { Name = name, Value = reader.ReadBytes(length) };
        }

        public NbtString ReadString(BinaryReader reader)
        {
            return new NbtString() { Name = ReadTagName(reader), Value = ReadTagName(reader) };
        }

        private string ReadTagName(BinaryReader reader)
        {
            short length = reader.ReadInt16();

            if (BitConverter.IsLittleEndian)
                length = IPAddress.NetworkToHostOrder(length);

            byte[] name = new byte[length];
            reader.Read(name, 0, length);
            return Encoding.ASCII.GetString(name, 0, length);
        }

        public void Save()
        {
            byte[] data = Root.Serialize().GZip();
            File.WriteAllBytes(Path, data);
        }
    }
}

using System.IO;

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

        public void Add(INbtField field)
        {
            Root.AddField(field);
        }

        public void Save()
        {
            byte[] data = Root.Serialize().GZip();
            File.WriteAllBytes(Path, data);
        }
    }
}

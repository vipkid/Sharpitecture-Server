namespace Sharpitecture.Levels.IO.NBT
{
    public interface INbtField
    {
        byte TypeID { get; }
        int Size { get; }
        string Name { get; set; }
        object Value { get; set; }
        byte[] Serialize();
    }
}

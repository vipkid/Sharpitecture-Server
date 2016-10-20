namespace Sharpitecture.Levels.IO.NBT
{
    public interface INbtField
    {
        byte TypeID { get; }
        int Size { get; }
        string Name { get; set; }
        object Value { get; set; }
        byte[] Serialize();

        byte ByteValue { get; }
        short ShortValue { get; }
        int IntValue { get; }
        long LongValue { get; }
        float FloatValue { get; }
        double DoubleValue { get; }
        byte[] ByteArrayValue { get; }
        string StringValue { get; }
    }
}

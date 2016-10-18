namespace Sharpitecture.Networking
{
    public class Opcodes
    {
        #region ==CLASSIC OPCODES==
        public static readonly Opcode ServerIdentification
            = new Opcode(0x00, 131);

        public static readonly Opcode Ping
            = new Opcode(0x01, 1);

        public static readonly Opcode LevelInitialize
            = new Opcode(0x02, 1);

        public static readonly Opcode LevelDataChunk
            = new Opcode(0x03, 1028);

        public static readonly Opcode LevelFinalize
            = new Opcode(0x04, 7);

        public static readonly Opcode SetBlock
            = new Opcode(0x06, 8);

        public static readonly Opcode SpawnPlayer
            = new Opcode(0x07, 74);

        public static readonly Opcode PlayerTeleport
            = new Opcode(0x08, 10);

        public static readonly Opcode PosAndRotUpdate
            = new Opcode(0x09, 7);

        public static readonly Opcode PositionUpdate
            = new Opcode(0x0a, 5);

        public static readonly Opcode RotationUpdate
            = new Opcode(0x0b, 4);

        public static readonly Opcode DespawnPlayer
            = new Opcode(0x0c, 2);

        public static readonly Opcode Message
            = new Opcode(0x0d, 66);

        public static readonly Opcode Disconnect
            = new Opcode(0x0e, 65);

        public static readonly Opcode UpdateUserType
            = new Opcode(0x0f, 2);

#endregion
    }

    public struct Opcode
    {
        public readonly byte id;
        public readonly int length;

        public Opcode(byte id, int length)
        {
            this.id = id; this.length = length;
        }
    }
}

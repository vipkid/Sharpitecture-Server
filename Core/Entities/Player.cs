using Sharpitecture.Chatting;
using Sharpitecture.Levels;
using Sharpitecture.Maths;
using Sharpitecture.Networking;
using Sharpitecture.Utils.Logging;
using System;
using System.Text;

namespace Sharpitecture.Entities
{
    public class Player : Entity
    {
        private Connection _connection;
        private bool _useCP437 = false;
        private bool _canHack = true;
        private bool _useCustomBlocks;
        private bool _useBlockDefinitions;

        public Player(Connection conn)
        {
            _connection = conn;
            conn.OnDataRead += HandleMessages;
        }

        private void HandleMessages(ByteBuffer buffer)
        {
            //Logger.Log("Read start", LogType.Debug);
            while (buffer.Position > 0)
            {
                byte packetId = buffer.ReadByte();
                //Logger.LogF("Parsing packet ID: {0}", LogType.Debug, packetId);

                switch (packetId)
                {
                    case 0:
                        HandleLoginMessage(buffer);
                        break;
                    case 5:
                        HandleBlockchange(buffer);
                        break;
                    case 8:
                        HandlePositionUpdate(buffer);
                        break;
                    case 13:
                        HandleChatMessage(buffer);
                        break;
                }
            }
        }

        private void HandleLoginMessage(ByteBuffer buffer)
        {
            byte protocolVersion = buffer.ReadByte();
            string username = buffer.ReadString(Encoding.ASCII);
            string verificationKey = buffer.ReadString(Encoding.ASCII);
            byte clientId = buffer.ReadByte();

            HoverName = username;
            Name = username;

            SendLoadingScreen(Config.GetConfig<string>(Config.Name), Config.GetConfig<string>(Config.MOTD));
            SendToMap(Server.MainLevel);

            Server.Players.Add(this);
        }

        private void HandleBlockchange(ByteBuffer buffer)
        {
            short x = buffer.ReadShort();
            short y = buffer.ReadShort();
            short z = buffer.ReadShort();
            bool created = buffer.ReadBoolean();
            byte tile = buffer.ReadByte();

            if (Level != null) Level.HandleManualChange(this, x, y, z, tile, created);
        }

        private void HandlePositionUpdate(ByteBuffer buffer)
        {
            byte heldBlock = buffer.ReadByte();
            short targetX = buffer.ReadShort();
            short targetY = buffer.ReadShort();
            short targetZ = buffer.ReadShort();
            byte yaw = buffer.ReadByte();
            byte pitch = buffer.ReadByte();

            _position.X = targetX;
            _position.Y = targetY;
            _position.Z = targetZ;

            _binaryRotation.X = yaw;
            _binaryRotation.Y = pitch;
        }

        private void HandleChatMessage(ByteBuffer buffer)
        {
            byte unused = buffer.ReadByte();
            string rawMessage = buffer.ReadString(_useCP437 ? Server.CP437 : Encoding.ASCII);

            string fullMessage = ChatName + ": " + rawMessage;
            Chat.MessageAll(fullMessage);
        }

        public void SendLoadingScreen(string title, string caption)
        {
            ByteBuffer buffer = new ByteBuffer(Opcodes.ServerIdentification.length);
            buffer.WriteByte(Opcodes.ServerIdentification.id);
            buffer.WriteByte(0x07);
            buffer.WriteString(title, Encoding.ASCII);
            buffer.WriteString(caption, Encoding.ASCII);
            buffer.WriteBoolean(_canHack);
            SendRaw(buffer.Data);
        }

        public void SendPing() => SendRaw(new byte[] { Opcodes.Ping.id });
        public void SendLevelInitialize() => SendRaw(new byte[] { Opcodes.LevelInitialize.id });

        public override void SendToMap(Level level)
        {
            if (_level != null) EntityHandler.DespawnAllEntities(this, true);

            SendLevelInitialize();
            byte[] data = level.Serialize(_useCustomBlocks, _useBlockDefinitions).GZip();
            int position = 0;
            short length = 0;
            ByteBuffer buffer = new ByteBuffer(Opcodes.LevelDataChunk.length);
            buffer.WriteByte(Opcodes.LevelDataChunk.id);

            while (position < data.Length)
            {
                length = (short)Math.Min(1024, data.Length - position);
                buffer.WriteShort(length);
                buffer.Write(data, position, length);
                position += length;
                buffer.SetPosition(1027);
                buffer.WriteByte((byte)(position * 256F / data.Length));
                SendRaw(buffer.Data);
                buffer.SetPosition(1);
            }

            SendLevelFinalize(level);

            _level = level;
            _entityID = Level.GetFreeEntityID();
            EntityHandler.SpawnAllEntities(this, _level, true);
            SpawnSelf();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void SpawnSelf()
        {
            ByteBuffer buffer = new ByteBuffer(Opcodes.SpawnPlayer.length);
            buffer.WriteByte(Opcodes.SpawnPlayer.id);
            buffer.WriteByte(255);
            buffer.WriteString(HoverName, Encoding.ASCII);
            buffer.WriteShort(10);
            buffer.WriteShort(10);
            buffer.WriteShort(10);
            buffer.WriteByte(Rotation.X);
            buffer.WriteByte(Rotation.Y);
            SendRaw(buffer.Data);
        }

        public void SendLevelFinalize(Level level)
        {
            ByteBuffer buffer = new ByteBuffer(Opcodes.LevelFinalize.length);
            buffer.WriteByte(Opcodes.LevelFinalize.id);
            buffer.WriteShort((short)level.Width);
            buffer.WriteShort((short)level.Height);
            buffer.WriteShort((short)level.Depth);
            SendRaw(buffer.Data);
        }

        public void SendBlockchange(short X, short Y, short Z, byte Tile)
        {
            Tile = _level.BlockDefinitions.GetFallback(Tile, _useCustomBlocks, _useBlockDefinitions);
            ByteBuffer buffer = new ByteBuffer(Opcodes.SetBlock.length);
            buffer.WriteByte(Opcodes.SetBlock.id);
            buffer.WriteShort(X);
            buffer.WriteShort(Y);
            buffer.WriteShort(Z);
            buffer.WriteByte(Tile);
            SendRaw(buffer.Data);
        }

        public void SendRaw(byte[] data)
        {
            _connection.SendRaw(data);
            if (_connection.ErrorOccurred)
                Disconnect();
        }
        
        public void UpdatePosition()
        {
            ByteBuffer packet = null;

            if (Math.Abs(PositionChange.X) > 32 ||
                Math.Abs(PositionChange.Y) > 32 ||
                Math.Abs(PositionChange.Z) > 32)
            {
                packet = new ByteBuffer(Opcodes.PlayerTeleport.length);
                packet.WriteByte(Opcodes.PlayerTeleport.id);
                packet.WriteByte(EntityID);
                packet.WriteShort(Position.X);
                packet.WriteShort(Position.Y);
                packet.WriteShort(Position.Z);
                packet.WriteByte(Rotation.X);
                packet.WriteByte(Rotation.Y);
            } else {

                bool position = PositionChange != Vector3S.Zero;
                bool rotation = RotationChange != Vector3B.Zero;

                byte[] posChange = (byte[])(Array)(new sbyte[] { (sbyte)PositionChange.X, (sbyte)PositionChange.Y, (sbyte)PositionChange.Z });

                if (position && rotation) {
                    packet = new ByteBuffer(Opcodes.PosAndRotUpdate.length);
                    packet.WriteByte(Opcodes.PosAndRotUpdate.id);
                    packet.WriteByte(EntityID);
                    packet.Write(posChange, 0, 3);
                    packet.WriteByte(Rotation.X);
                    packet.WriteByte(Rotation.Y);
                } else if (position) {
                    packet = new ByteBuffer(Opcodes.PositionUpdate.length);
                    packet.WriteByte(Opcodes.PositionUpdate.id);
                    packet.WriteByte(EntityID);
                    packet.Write(posChange, 0, 3);
                } else if (rotation) {
                    packet = new ByteBuffer(Opcodes.RotationUpdate.length);
                    packet.WriteByte(Opcodes.RotationUpdate.id);
                    packet.WriteByte(EntityID);
                    packet.WriteByte(Rotation.X);
                    packet.WriteByte(Rotation.Y);
                }
            }

            if (packet != null)
                lock(Level.Players)
                    Level.Players.ForEach(p => { if (p != this) p.SendRaw(packet.Data); });

            _posChange = _position - _oldPosition;
            _rotChange = _binaryRotation - _oldRotation;
            _oldPosition = _position;
            _oldRotation = _binaryRotation;
        }

        public void SendMessage(string message)
        {
            ByteBuffer buffer = new ByteBuffer(Opcodes.Message.length);

            foreach (string line in LineWrapper.WrapLines(message))
            {
                buffer.WriteByte(Opcodes.Message.id);
                buffer.WriteByte(0);
                buffer.WriteString(message, _useCP437 ? Server.CP437 : Encoding.ASCII);
                SendRaw(buffer.Data);
                buffer.SetPosition(0);
            }
        }

        public void SendCpeMessage(CpeMessageType type, string message)
        {
            if (type == CpeMessageType.Chat) { SendMessage(message); return; }
            if (message.Length > 64) message = message.Substring(0, 64);

            ByteBuffer buffer = new ByteBuffer(Opcodes.Message.length);
            buffer.WriteByte(Opcodes.Message.id);
            buffer.WriteByte((byte)type);
            buffer.WriteString(message, _useCP437 ? Server.CP437 : Encoding.ASCII);
            SendRaw(buffer.Data);
        }

        public void Disconnect()
        {
            Server.Players.Remove(this);
        }
    }
}

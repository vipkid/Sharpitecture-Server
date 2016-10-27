using Sharpitecture.API.Commands;
using Sharpitecture.Chatting;
using Sharpitecture.Groups;
using Sharpitecture.Levels;
using Sharpitecture.Maths;
using Sharpitecture.Networking;
using Sharpitecture.Utils.Logging;
using System;
using System.Text;
using System.Threading;

namespace Sharpitecture.Entities
{
    public partial class Player
    {
        public Player(Connection conn)
        {
            Connection = conn;
            Connection.OnDataRead += HandleMessages;
        }

        protected Player() { } //Psuedo Player

        /// <summary>
        /// The connection object bound to this player
        /// </summary>
        private Connection Connection { get; set; }

        #region INCOMING MESSAGES

        /// <summary>
        /// Handles incoming packets
        /// </summary>
        private void HandleMessages(ByteBuffer buffer)
        {
            while (buffer.Position > 0)
            {
                byte packetId = buffer.ReadByte();

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

        /// <summary>
        /// Handles a login message
        /// </summary>
        private void HandleLoginMessage(ByteBuffer buffer)
        {
            byte protocolVersion = buffer.ReadByte();
            string username = buffer.ReadString(Encoding.ASCII);
            string verificationKey = buffer.ReadString(Encoding.ASCII);
            byte clientId = buffer.ReadByte();

            HoverName = username;
            Name = username;

            SendLoadingScreen(Config.Name, Config.MOTD);
            SendToMap(Server.MainLevel);

            Group = Group.FindPlayerRank(Name);
            ChatColour = Group.DefaultColour;
            Server.Players.Add(this);

            SendMessage("&eYou are a " + Group.DefaultColour + Group.Name + "&e!");
            Chat.MessageAll("&a+ " + ChatName + " &ejoined the game.");
        }
        
        /// <summary>
        /// Handles a blockchange packet
        /// </summary>
        private void HandleBlockchange(ByteBuffer buffer)
        {
            short x = buffer.ReadShort();
            short y = buffer.ReadShort();
            short z = buffer.ReadShort();
            bool created = buffer.ReadBoolean();
            byte tile = buffer.ReadByte();

            if (OnBlockchange != null)
                if (OnBlockchange(this, x, y, z, tile, created))
                    return;

            if (Level != null) Level.HandleManualChange(this, x, y, z, tile, created);
        }

        /// <summary>
        /// Handles a position change packet
        /// </summary>
        private void HandlePositionUpdate(ByteBuffer buffer)
        {
            byte heldBlock = buffer.ReadByte();
            short targetX = buffer.ReadShort();
            short targetY = buffer.ReadShort();
            short targetZ = buffer.ReadShort();
            byte yaw = buffer.ReadByte();
            byte pitch = buffer.ReadByte();

            Position.X = targetX;
            Position.Y = targetY;
            Position.Z = targetZ;

            Rotation.X = yaw;
            Rotation.Y = pitch;
        }

        /// <summary>
        /// Handles a chat message packet
        /// </summary>
        private void HandleChatMessage(ByteBuffer buffer)
        {
            byte unused = buffer.ReadByte();
            string rawMessage = buffer.ReadString(UseCP437 ? Server.CP437 : Encoding.ASCII);

            if (rawMessage.StartsWith("/"))
            {
                rawMessage = rawMessage.Remove(0, 1);
                string parameters = string.Empty;

                if (rawMessage.IndexOf(' ') != -1)
                {
                    string[] parts = rawMessage.Split(new char[] { ' ' }, 2);
                    rawMessage = parts[0];
                    parameters = parts[1];
                }

                Command cd;
                if ((cd = Command.Find(rawMessage)) == null)
                {
                    SendMessage("Could not find command '" + rawMessage + "'");
                    return;
                }

                try
                {
                    CommandThread = new Thread(() => cd.Handler(this, parameters));
                    CommandThread.Start();
                }
                catch (Exception ex)
                {
                    SendMessage("An error occurred when using the command");
                    SendMessage("Message: &c" + ex.Message);
                    Logger.LogF("{0} trigged a command error", LogType.Error, Name);
                    Logger.LogF("Message: {0}", LogType.Error, ex.Message);
                }

                return;
            }

            string fullMessage = ChatName + ": &f" + rawMessage;
            Chat.MessageAll(fullMessage);
        }

        #endregion

        #region OUTGOING MESSAGES

        /// <summary>
        /// Sent to the client to check if the client is still connected
        /// </summary>
        public void SendPing() => SendRaw(new byte[] { Opcodes.Ping.id });

        /// <summary>
        /// Sends the loading screen to a player
        /// </summary>
        public void SendLoadingScreen(string title, string caption)
        {
            ByteBuffer buffer = new ByteBuffer(Opcodes.ServerIdentification);
            buffer.WriteByte(0x07);
            buffer.WriteString(title, Encoding.ASCII);
            buffer.WriteString(caption, Encoding.ASCII);
            buffer.WriteBoolean(CanHack);
            SendRaw(buffer.Data);
        }

        /// <summary>
        /// Prepares the client for an incoming level change
        /// </summary>
        public void SendLevelInitialize() => SendRaw(new byte[] { Opcodes.LevelInitialize.id });

        /// <summary>
        /// Sends level data to the player
        /// </summary>
        /// <param name="level"></param>
        private void SendLevelData(Level level)
        {
            SendLevelInitialize();
            byte[] data = level.Serialize(UseCustomBlocks, UseBlockDefinitions).GZip();
            int position = 0;
            short length = 0;
            ByteBuffer buffer = new ByteBuffer(Opcodes.LevelDataChunk);

            while (position < data.Length)
            {
                length = (short)Math.Min(1024, data.Length - position);
                buffer.WriteShort(length);
                buffer.Write(data, position, length);
                position += length;
                buffer.WriteByte((byte)(position * 256F / data.Length));
                SendRaw(buffer.Data);
                buffer.SetPosition(1);
            }

            SendLevelFinalize(level);
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Signals the client that the level has finished loading
        /// </summary>
        public void SendLevelFinalize(Level level)
        {
            ByteBuffer buffer = new ByteBuffer(Opcodes.LevelFinalize);
            buffer.WriteShort((short)level.Width);
            buffer.WriteShort((short)level.Height);
            buffer.WriteShort((short)level.Depth);
            SendRaw(buffer.Data);
        }

        /// <summary>
        /// Sends a blockchange to the client
        /// </summary>
        public void SendBlockchange(short X, short Y, short Z, byte Tile)
        {
            Tile = Level.BlockDefinitions.GetFallback(Tile, UseCustomBlocks, UseBlockDefinitions);
            ByteBuffer buffer = new ByteBuffer(Opcodes.SetBlock);
            buffer.WriteShort(X);
            buffer.WriteShort(Y);
            buffer.WriteShort(Z);
            buffer.WriteByte(Tile);
            SendRaw(buffer.Data);
        }

        /// <summary>
        /// Sends the player to the level spawn
        /// </summary>
        public void SpawnSelf()
        {
            ByteBuffer buffer = new ByteBuffer(Opcodes.SpawnPlayer);
            buffer.WriteByte(255);
            buffer.WriteString(HoverName, Encoding.ASCII);
            buffer.WriteShort(Level.Spawn.X);
            buffer.WriteShort(Level.Spawn.Y);
            buffer.WriteShort(Level.Spawn.Z);
            buffer.WriteByte(Level.SpawnRot.X);
            buffer.WriteByte(Level.SpawnRot.Y);
            SendRaw(buffer.Data);
        }
        
        /// <summary>
        /// Updates the player's position to all clients
        /// </summary>
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
            }
            else
            {

                bool position = PositionChange != Vector3S.Zero;
                bool rotation = RotationChange != Vector3B.Zero;

                byte[] posChange = (byte[])(Array)(new sbyte[] { (sbyte)PositionChange.X, (sbyte)PositionChange.Y, (sbyte)PositionChange.Z });

                if (position && rotation)
                {
                    packet = new ByteBuffer(Opcodes.PosAndRotUpdate.length);
                    packet.WriteByte(Opcodes.PosAndRotUpdate.id);
                    packet.WriteByte(EntityID);
                    packet.Write(posChange, 0, 3);
                    packet.WriteByte(Rotation.X);
                    packet.WriteByte(Rotation.Y);
                }
                else if (position)
                {
                    packet = new ByteBuffer(Opcodes.PositionUpdate.length);
                    packet.WriteByte(Opcodes.PositionUpdate.id);
                    packet.WriteByte(EntityID);
                    packet.Write(posChange, 0, 3);
                }
                else if (rotation)
                {
                    packet = new ByteBuffer(Opcodes.RotationUpdate.length);
                    packet.WriteByte(Opcodes.RotationUpdate.id);
                    packet.WriteByte(EntityID);
                    packet.WriteByte(Rotation.X);
                    packet.WriteByte(Rotation.Y);
                }
            }

            if (packet != null)
                lock (Level.Players)
                    Level.Players.ForEach(p => { if (p != this) p.SendRaw(packet.Data); });

            PositionChange = Position - OldPosition;
            RotationChange = Rotation - OldRotation;
            OldPosition = Position;
            OldRotation = Rotation;
        }

        /// <summary>
        /// Sends a message to the player
        /// </summary>
        public void SendMessage(string message)
        {
            ByteBuffer buffer = new ByteBuffer(Opcodes.Message);

            foreach (string line in LineWrapper.WrapLines(message))
            {
                buffer.SetPosition(1);
                buffer.WriteByte(0);
                buffer.WriteString(line, UseCP437 ? Server.CP437 : Encoding.ASCII);
                SendRaw(buffer.Data);
                buffer.SetPosition(0);
            }
        }

        /// <summary>
        /// Sends a CpeMessage to the client
        /// <para>NOTE: CpeMessages are single lined, therefore cannot be wrapped</para>
        /// </summary>
        public void SendCpeMessage(CpeMessageType type, string message)
        {
            if (type == CpeMessageType.Chat) { SendMessage(message); return; }
            if (message.Length > 64) message = message.Substring(0, 64);

            ByteBuffer buffer = new ByteBuffer(Opcodes.Message);
            buffer.WriteByte((byte)type);
            buffer.WriteString(message, UseCP437 ? Server.CP437 : Encoding.ASCII);
            SendRaw(buffer.Data);
        }

        /// <summary>
        /// Sends the kick screen to the player
        /// </summary>
        public void Kick(string message)
        {
            ByteBuffer buffer = new ByteBuffer(Opcodes.Disconnect);
            buffer.WriteString(message, Encoding.ASCII);
            SendRaw(buffer.Data);

            Disconnect(message);
        }

        /// <summary>
        /// Disconnects the player from the server if still connected
        /// </summary>
        /// <param name="message"></param>
        public void Disconnect(string message = "")
        {
            Chat.MessageAll(
                string.Format("&c- &e{0} &edisconnected{1}", ChatName, message == string.Empty ? "." : " (" + message + ")")
                );

            Server.Players.Remove(this);
        }

        /// <summary>
        /// Sends a raw message to the client
        /// </summary>
        public void SendRaw(byte[] data)
        {
            Connection.SendRaw(data);
            if (Connection.ErrorOccurred)
                Disconnect();
        }

        #endregion
    }
}

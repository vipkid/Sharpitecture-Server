using Sharpitecture.Levels;

namespace Sharpitecture.Chatting
{
    public static class Chat
    {
        /// <summary>
        /// Sends a message to all players currently online
        /// </summary>
        public static void MessageAll(string message)
        {
            Server.Players.ForEach(player => player.SendMessage(message));
        }

        /// <summary>
        /// Sends a message to all players on the specified level
        /// </summary>
        public static void MessageLevel(Level level, string message)
        {
            Server.Players.ForEach(player => { if (player.Level == level) player.SendMessage(message); });
        }

        /// <summary>
        /// Sends a CpeMessage to all players currently online
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        public static void CpeMessageAll(CpeMessageType type, string message)
        {
            Server.Players.ForEach(player => player.SendCpeMessage(type, message));
        }

        /// <summary>
        /// Sends a CpeMessage to all players on the specified level
        /// </summary>
        /// <param name="level"></param>
        /// <param name="type"></param>
        /// <param name="message"></param>
        public static void CpeMessageLevel(Level level, CpeMessageType type, string message)
        {
            Server.Players.ForEach(player => { if (player.Level == level) player.SendCpeMessage(type, message); });
        }
    }
}

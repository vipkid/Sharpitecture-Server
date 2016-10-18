using Sharpitecture.Levels;

namespace Sharpitecture.Chatting
{
    public static class Chat
    {
        public static void MessageAll(string message)
        {
            Server.Players.ForEach(player => player.SendMessage(message));
        }

        public static void MessageLevel(Level level, string message)
        {
            Server.Players.ForEach(player => { if (player.Level == level) player.SendMessage(message); });
        }

        public static void CpeMessageAll(CpeMessageType type, string message)
        {
            Server.Players.ForEach(player => player.SendCpeMessage(type, message));
        }

        public static void CpeMessageLevel(Level level, CpeMessageType type, string message)
        {
            Server.Players.ForEach(player => { if (player.Level == level) player.SendCpeMessage(type, message); });
        }
    }
}

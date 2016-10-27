using Sharpitecture.Entities;

namespace Sharpitecture
{
    public static partial class Server
    {
        /// <summary>
        /// What happens when a player places a block
        /// </summary>
        public static event Blockchange OnPlayerBlockchange = null;

        /// <summary>
        /// What happens when a player joins or leaves the server
        /// </summary>
        public static event PlayerAction OnPlayerJoin = null;

        /// <summary>
        /// What happens when a player chats
        /// </summary>
        public static event ChatMessage OnPlayerChat = null;

        /// <summary>
        /// What happens when a player moves
        /// </summary>
        public static event PlayerMove OnPlayerMove = null;

        /// <summary>
        /// What happens when a player leaves
        /// </summary>
        public static event PlayerAction OnPlayerLeave = null;
    }
}

using Sharpitecture.Levels;

namespace Sharpitecture.Entities
{
    public delegate void LevelChange(Level from, Level to);
    public delegate bool Blockchange(Player player, short x, short y, short z, byte type, bool created);
    public delegate bool ChatMessage(Player sender, ref string message);
    public delegate void PlayerAction(Player player);
    public delegate bool PlayerMove(Player player);

    public partial class Player
    {
        /// <summary>
        /// What happens when a player changes levels
        /// </summary>
        public event LevelChange OnLevelChange = null;

        /// <summary>
        /// What happens when a player changes a block
        /// </summary>
        public event Blockchange OnBlockchange = null;

        /// <summary>
        /// What happens when a player chats
        /// </summary>
        public event ChatMessage OnChatMessage = null;

        /// <summary>
        /// What happens when a player joins the server
        /// </summary>
        public event PlayerAction OnPlayerJoin = null;

        /// <summary>
        /// What happens when a player leaves the server
        /// </summary>
        public event PlayerAction OnPlayerLeave = null;
    }
}

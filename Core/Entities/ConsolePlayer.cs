using Sharpitecture.Groups;
using Sharpitecture.Networking;

namespace Sharpitecture.Entities
{
    public sealed class ConsolePlayer : Player
    {
        public static ConsolePlayer Console { get; private set; }

        private ConsolePlayer(Connection conn) { }
        private ConsolePlayer() { }

        public static void Initialise()
        {
            Console = new ConsolePlayer();

            Console.Group = new Group()
            {
                DefaultColor = "&8",
                FilePath = string.Empty,
                Name = "Superuser",
                PermissionLevel = int.MaxValue,
                Prefix = string.Empty
            };
        }
    }
}

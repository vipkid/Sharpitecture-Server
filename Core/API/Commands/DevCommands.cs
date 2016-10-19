using Sharpitecture.Entities;

namespace Sharpitecture.API.Commands
{
    public static class DevCommands
    {
        public static Command TestCommand = new Command()
        {
            Name = "Test",
            Shortcuts = new string[] { "t", "e", "s" },
            Handler = TestCmd,
            Permission = 50,
        };

        public static void TestCmd(Player p, string parameters)
        {
            int pars = parameters.Split(' ').Length;
            p.SendMessage(pars + " parameters parsed in the command.");
            p.SendMessage("Parameters: " + parameters);
        }
    }
}

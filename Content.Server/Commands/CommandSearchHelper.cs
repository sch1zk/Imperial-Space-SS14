using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;


namespace Content.Server.Chat.Commands
{
    [AdminCommand(AdminFlags.Admin)]
    internal sealed class SearchHelperCommand : IConsoleCommand
    {
        public string Command => "searchelper";
        public string Description => "help you to find objects with prototype";
        public string Help => "searchhelper [-n for name, -p for prototype, -c for component] [uid of grid] [search name or prototype or component]";

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            if (args.Length != 3)
            {
                shell.WriteLine(Loc.GetString("shell-wrong-arguments-number"));
                return;
            }

            if (!int.TryParse(args[0], out var entityUid))
            {
                shell.WriteLine(Loc.GetString("shell-entity-uid-must-be-number"));
                return;
            }
            switch (args[1])
            {
                case "-n":
                    shell.ExecuteCommand("forall ongrid " + args[0] + " named " + args[2] + " do echo $ID $NAME");
                    break;
                case "-p":
                    shell.ExecuteCommand("forall ongrid " + args[0] + " prototyped " + args[2] + " do echo $ID $NAME");
                    break;
                case "-c":
                    shell.ExecuteCommand("forall ongrid " + args[0] + " with " + args[2] + " do echo $ID $NAME");
                    break;
                default:
                    shell.WriteLine(Loc.GetString("wrong argument, input -n for search name, -p for search prototype, -c for search component"));
                    return;
            }
        }
    }
}

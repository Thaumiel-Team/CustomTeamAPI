using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using LabApi.Features.Permissions;
using Thaumiel.API.Interfaces;

namespace Thaumiel.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Parent : ParentCommand
    {
        public Parent() => LoadGeneratedCommands();

        public override string Command => "tct";

        public override string Description => "Manage Thaumiel Custom Teams Features";

        public override string[] Aliases => [];

        public override void LoadGeneratedCommands()
        {
            Subcommands.Add(new List());
            Subcommands.Add(new Spawn());
        }

        private List<ISubcommandV2> Subcommands { get; } = [];

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count == 0)
            {
                response = $"Thaumiel CustomTeams v{TeamPlugin.Instance.Version} by Mr. Baguetter\n\nAvailable commands:";
                foreach (ISubcommandV2 command in Subcommands)
                    response += $"\n- tcr {command.Name}{(command.VisibleArgs != string.Empty ? $" {command.VisibleArgs}" : "")} - {command.Description}";

                return true;
            }

            ISubcommandV2 cmd = Subcommands.FirstOrDefault(cmd => cmd.Name == arguments.At(0));
            cmd ??= Subcommands.FirstOrDefault(cmd => cmd.Aliases.Contains(arguments.At(0)));

            if (cmd is null)
            {
                response = "Command not found!";
                return false;
            }

            if (!sender.HasPermissions(cmd.RequiredPermission))
            {
                response = "You don't have permission to access that command!";
                return false;
            }

            if (arguments.Count < cmd.RequiredArgsCount)
            {
                response = $"Wrong usage!\nCorrect usage: thaumiel {cmd.Name} {cmd.VisibleArgs}";
                return false;
            }

            List<string> args = [.. arguments];
            args.RemoveAt(0);

            return cmd.Execute(args, sender, out response);
        }
    }
}
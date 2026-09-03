using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandSystem;
using Thaumiel.API.CustomTeamsAPI;
using Thaumiel.API.Interfaces;

namespace Thaumiel.Commands
{
    public class List : ISubcommandV2
    {
        public string Name => "list";

        public string VisibleArgs => "";

        public int RequiredArgsCount => 0;

        public string Description => "Lists all registered Custom Teams";

        public string[] Aliases => ["ctlist", "ctli"];

        public string RequiredPermission => "tct.list";

        public bool Execute(List<string> arguments, ICommandSender sender, out string response)
        {
            StringBuilder sb = new();
            sb.AppendLine("Registered Thaumiel CustomTeams:");
            foreach (BaseCustomTeam team in CustomTeamsManager.RegisteredTeams.OrderBy(r => r.Id))
                sb.AppendLine($"ID: {team.Id} - Name: {team.Name}");

            response = sb.ToString();
            return true;
        }
    }
}
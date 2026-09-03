using System.Collections.Generic;
using CommandSystem;
using Thaumiel.API.CustomTeamsAPI;
using Thaumiel.API.Interfaces;

namespace Thaumiel.Commands
{
    public class Spawn : ISubcommandV2
    {
        public string Name => "spawn";

        public string VisibleArgs => "<CustomTeamId>";

        public int RequiredArgsCount => 1;

        public string Description => "Spawns the specified Custom Team";

        public string[] Aliases => ["ctspawn"];

        public string RequiredPermission => "tct.spawn";

        public bool Execute(List<string> arguments, ICommandSender sender, out string response)
        {
            BaseCustomTeam team = CustomTeamsManager.GetTeamById(uint.Parse(arguments[0]));
            if (team is null)
            {
                response = "Failed to find Custom Team. Is the Id correct?";
                return false;
            }

            CustomTeamsManager.TrySpawnWave(team);
            response = "Custom Team should have been spawned";
            return true;
        }
    }
}
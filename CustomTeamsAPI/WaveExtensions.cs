using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Wrappers;
using Thaumiel.API.CustomRolesAPI;

namespace Thaumiel.API.CustomTeamsAPI
{
    public static class WaveExtensions
    {
        public static bool IsCustomWave(this WaveRespawnedEventArgs? ev)
        {
            if (ev is null)
                return false;

            foreach (Player? player in ev.Players)
            {
                if (player is null)
                    continue;

                if (BaseCustomRole.GivenRoles.ContainsKey(player.PlayerId))
                    return true;
            }

            return false;
        }

        public static bool TryGetCustomTeam(this WaveRespawnedEventArgs? ev, out BaseCustomTeam? team)
        {
            team = null;
            if (ev is null)
                return false;

            foreach (BaseCustomTeam candidate in CustomTeamsManager.RegisteredTeams)
            {
                if (candidate.CustomRoleProperties is null)
                    continue;

                foreach (Spawning.CustomRoleProperties rosterEntry in candidate.CustomRoleProperties)
                {
                    BaseCustomRole? rosterRole = rosterEntry.CustomRole;
                    if (rosterRole is null)
                        continue;

                    foreach (Player? player in ev.Players)
                    {
                        if (player is null)
                            continue;

                        if (BaseCustomRole.GivenRoles.TryGetValue(player.PlayerId, out BaseCustomRole? given) && given.Id == rosterRole.Id)
                        {
                            team = candidate;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the registered team that took over the wave, or null when the wave is vanilla.
        /// </summary>
        /// <param name="ev">The wave event to inspect.</param>
        public static BaseCustomTeam? GetCustomTeam(this WaveRespawnedEventArgs? ev)
            => ev.TryGetCustomTeam(out BaseCustomTeam? team) ? team : null;
    }
}

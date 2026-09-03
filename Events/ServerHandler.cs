using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thaumiel.API.CustomRolesAPI;
using Thaumiel.API.CustomTeamsAPI;

namespace Thaumiel.Events
{
    public class ServerHandler
    {
        public static void Register()
        {
            ServerEvents.WaveRespawned += OnWaveRespawn;
        }

        public static void Unregister()
        {
            ServerEvents.WaveRespawned -= OnWaveRespawn;

        }

        private static void OnWaveRespawn(WaveRespawnedEventArgs ev)
        {
            if (!CustomTeamsManager.TrySpawnWave(ev))
            {
                foreach (Player player in ev.Players)
                {
                    CustomRolesManager.CalculateSpawn(player, ev);
                }
            }
        }
    }
}

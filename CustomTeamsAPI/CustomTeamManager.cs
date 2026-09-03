using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Wrappers;
using LabApi.Loader.Features.Yaml.CustomConverters;
using LabApiExtensions.Extensions;
using Thaumiel.API.CustomRolesAPI;
using Thaumiel.API.CustomTeamsAPI.Spawning;
using Thaumiel.API.Internal;
using Thaumiel.API.Extensions;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Thaumiel.API.CustomTeamsAPI
{
    public class CustomTeamsManager
    {
        /// <summary>
        /// Every custom team.
        /// </summary>
        public static List<BaseCustomTeam> RegisteredTeams = [];

        public static IDeserializer deserializer = new DeserializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .WithTypeConverter(new CustomVectorConverter())
            .WithTypeConverter(new CustomColor32Converter())
            .WithTypeConverter(new CustomColorConverter())
            .WithTypeConverter(new CustomQuaternionConverter())
            .Build();

        public static ISerializer serializer = new SerializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .WithTypeConverter(new CustomVectorConverter())
            .WithTypeConverter(new CustomColor32Converter())
            .WithTypeConverter(new CustomColorConverter())
            .WithTypeConverter(new CustomQuaternionConverter())
            .Build();

        private static readonly IDeserializer peekDeserializer = new DeserializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .WithTypeConverter(new CustomVectorConverter())
            .WithTypeConverter(new CustomColor32Converter())
            .WithTypeConverter(new CustomColorConverter())
            .WithTypeConverter(new CustomQuaternionConverter())
            .IgnoreUnmatchedProperties()
            .Build();

        /// <summary>
        /// Reads every yml file in the CustomTeams folder and applies the values onto the matching registered teams.
        /// </summary>
        public static void RegisterTeamsFromConfig()
        {
            ThaumielFileManager.TryCreateDirectory("CustomTeams");

            foreach (string filename in ThaumielFileManager.GetFilesInDirectory("CustomTeams", "*.yml"))
            {
                try
                {
                    string yaml = File.ReadAllText(filename);
                    CustomTeam peek = peekDeserializer.Deserialize<CustomTeam>(yaml);
                    BaseCustomTeam match = RegisteredTeams.FirstOrDefault(r => r.Id == peek.Id);
                    if (match is null)
                    {
                        LogManager.Warn($"No registered Team found with Id {peek.Id} for config '{peek.Name}'. Skipping.");
                        continue;
                    }

                    BaseCustomTeam? config = (BaseCustomTeam?)deserializer.Deserialize(yaml, match.GetType());
                    if (config != null)
                    {
                        match.ApplyConfig(config);
                        LogManager.Info($"Applied config to '{match.Name}' (Id: {match.Id}) as {match.GetType().Name}.");
                    }

                }
                catch (YamlException yamlex)
                {
                    LogManager.Error($"Error parsing yaml when reading CustomTeams from config {yamlex}");
                }
                catch (Exception ex)
                {
                    LogManager.Error($"Error when reading CustomTeams from config {ex}");
                }
            }
        }

        /// <summary>
        /// Registers one team and creates its default config file when none exists yet.
        /// </summary>
        /// <typeparam name="T">The team class to register.</typeparam>
        public static void RegisterTeam<T>() where T : BaseCustomTeam, new()
        {
            T team = new();
            if (RegisteredTeams.Any(t => t.Id == team.Id))
            {
                LogManager.Warn($"Team '{team.Name}' (Id: {team.Id}) is already registered. Skipping.");
                return;
            }

            CreateConfigForTeam(team);
            RegisteredTeams.Add(team);
        }

        /// <summary>
        /// Writes the default config file for a team, unless the file already exists.
        /// </summary>
        /// <param name="team">The team to write a config file for.</param>
        public static void CreateConfigForTeam(BaseCustomTeam team)
        {
            string path = ThaumielFileManager.Dir(["CustomTeams"]);
            string filePath = Path.Combine(path, $"{team.Name.Replace(' ', '_')}.yml");
            ThaumielFileManager.TryCreateDirectory(path);

            if (!File.Exists(filePath))
                File.WriteAllText(filePath, serializer.Serialize(team));
        }

        /// <summary>
        /// Tries to replace a respawn wave with a matching custom team. Roster entries split the wave between their roles and members are moved to the team spawn point.
        /// </summary>
        /// <param name="ev">The wave event to consider taking over.</param>
        /// <returns>True when a team took over the wave.</returns>
        public static bool TrySpawnWave(WaveRespawnedEventArgs ev)
        {
            foreach (BaseCustomTeam team in RegisteredTeams)
            {
                if (ev.Players.Count() < team.MinimumWaveSize)
                    continue;
                if (ev.Wave.Faction != team.ReplacedFaction)
                    continue;
                if (UnityEngine.Random.Range(0, 100) >= team.SpawnPercentage)
                    continue;

                List<Player> availablePlayers = ev.Players.ToList();
                int totalWaveSize = availablePlayers.Count;

                foreach (CustomRoleProperties roleProperties in team.CustomRoleProperties.Where(p => totalWaveSize >= p.MinimumWaveSizeForSpawn).OrderByDescending(p => p.PercentageOfWaveToConvertToRole))
                {
                    if (availablePlayers.Count == 0)
                        break;

                    int calculatedAmount = (int)Math.Floor(totalWaveSize * roleProperties.PercentageOfWaveToConvertToRole);
                    int finalAmount = Math.Min(calculatedAmount, (int)roleProperties.MaximumAmountPerWave);
                    finalAmount = Math.Min(finalAmount, availablePlayers.Count);

                    if (finalAmount <= 0)
                        continue;

                    List<Player> selectedPlayers = PickRandomPlayers(availablePlayers, finalAmount);

                    foreach (Player player in selectedPlayers)
                    {
                        CustomRolesManager.GiveRoleToPlayer(player, roleProperties.CustomRole);
                        availablePlayers.Remove(player);

                        ThaumielTiming.WaitForTwoFrames(() =>
                        {
                            SeperatePlayer(player, team);
                        });
                    }
                }

                foreach (string line in team.CassieAnnouncement.Split(' '))
                {
                    if (!Announcer.IsValid(line))
                        LogManager.Warn($"Line {line} in CassieAnnouncement for {team.Name} is not a valid CassieLine.");
                }

                Announcer.Message(team.CassieAnnouncement, team.CassieAnnouncementSubtitles, glitchScale: team.CassieGlitchScale);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Spawns a team right away using players who are waiting to respawn. Useful for commands and events that should force a team into the round.
        /// </summary>
        /// <param name="team">The team to spawn.</param>
        public static void TrySpawnWave(BaseCustomTeam team)
        {
            List<Player> availablePlayers = Player.ReadyList.Where(p => !p.IsAlive).ToList();
            int totalWaveSize = availablePlayers.Count;

            foreach (CustomRoleProperties roleProperties in team.CustomRoleProperties.Where(p => totalWaveSize >= p.MinimumWaveSizeForSpawn).OrderByDescending(p => p.PercentageOfWaveToConvertToRole))
            {
                if (availablePlayers.Count == 0)
                    break;

                int calculatedAmount = (int)Math.Floor(totalWaveSize * roleProperties.PercentageOfWaveToConvertToRole);
                int finalAmount = Math.Min(calculatedAmount, (int)roleProperties.MaximumAmountPerWave);
                finalAmount = Math.Min(finalAmount, availablePlayers.Count);

                if (finalAmount <= 0)
                    continue;

                List<Player> selectedPlayers = PickRandomPlayers(availablePlayers, finalAmount);

                foreach (Player player in selectedPlayers)
                {
                    CustomRolesManager.GiveRoleToPlayer(player, roleProperties.CustomRole);
                    availablePlayers.Remove(player);
                    ThaumielTiming.WaitForTwoFrames(() =>
                    {
                        SeperatePlayer(player, team);
                    });
                }
            }

            foreach (string line in team.CassieAnnouncement.Split(' '))
            {
                if (!Announcer.IsValid(line))
                    LogManager.Warn($"Line {line} in CassieAnnouncement for {team.Name} is not a valid CassieLine.");
            }

            Announcer.Message(team.CassieAnnouncement, team.CassieAnnouncement);
        }

        /// <summary>
        /// Finds a registered team by its id.
        /// </summary>
        /// <param name="id">The team id to look up.</param>
        /// <returns>The matching team.</returns>
        public static BaseCustomTeam GetTeamById(uint id) => RegisteredTeams.Where(t => t.Id == id).First();

        private static List<Player> PickRandomPlayers(List<Player> players, int count)
        {
            List<Player> selected = new(count);
            int n = players.Count;
            for (int i = 0; i < count && i < n; i++)
            {
                int j = UnityEngine.Random.Range(i, n);
                (players[i], players[j]) = (players[j], players[i]);
                selected.Add(players[i]);
            }

            return selected;
        }

        /// <summary>
        /// Moves a player to the spawn point of a team, with a small random offset so members do not stack.
        /// </summary>
        /// <param name="player">The player to move.</param>
        /// <param name="team">The team whose spawn point should be used.</param>
        public static void SeperatePlayer(Player player, BaseCustomTeam team)
        {
            Room? room = RoomExtensions.GetRoomByName(team.SpawnPoint.SpawnRoom).GetRandom();
            if (room is not null)
            {
                Vector3 basePosition = RoomExtensions.WorldPosition(room, team.SpawnPoint.Offset);
                Vector3 randomOffset = UnityEngine.Random.insideUnitSphere * 3f;
                randomOffset.y = 1f;
                player.Position = basePosition + randomOffset;
            }
        }
    }
}

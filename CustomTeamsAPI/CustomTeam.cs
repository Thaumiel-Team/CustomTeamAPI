using System.Collections.Generic;
using PlayerRoles;
using Thaumiel.API.CustomTeamsAPI.Spawning;

namespace Thaumiel.API.CustomTeamsAPI
{
    public class CustomTeam : BaseCustomTeam
    {
        /// <summary>
        /// Unique number for this team. Must match the coded team it configures.
        /// </summary>
        public override uint Id { get; set; }

        /// <summary>
        /// The display name of the team.
        /// </summary>
        public override string Name { get; set; } = string.Empty;

        /// <summary>
        /// Roster that decides which custom roles fill the wave and in what amounts.
        /// </summary>
        public override List<CustomRoleProperties> CustomRoleProperties { get; set; } = [];

        /// <summary>
        /// The vanilla wave this team replaces when it spawns.
        /// </summary>
        public override Faction ReplacedFaction { get; set; }

        /// <summary>
        /// Chance that this team takes over a matching wave, from 0 to 100.
        /// </summary>
        public override float SpawnPercentage { get; set; }

        /// <summary>
        /// The wave must hold at least this many players before the team can spawn.
        /// </summary>
        public override uint MinimumWaveSize { get; set; }

        /// <summary>
        /// Where team members appear after they get their roles.
        /// </summary>
        public override SpawnPoint SpawnPoint { get; set; } = new();

        /// <summary>
        /// Cassie line played when the team spawns.
        /// </summary>
        public override string CassieAnnouncement { get; set; } = string.Empty;

        /// <summary>
        /// Subtitle text shown with the announcement.
        /// </summary>
        public override string CassieAnnouncementSubtitles { get; set; } = string.Empty;

        /// <summary>
        /// Glitch flavor of the announcement.
        /// </summary>
        public override float CassieGlitchScale { get; set; }
    }
}

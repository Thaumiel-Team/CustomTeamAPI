using System.Collections.Generic;
using PlayerRoles;
using Thaumiel.API.CustomTeamsAPI.Spawning;

namespace Thaumiel.API.CustomTeamsAPI
{
    public abstract class BaseCustomTeam
    {
        /// <summary>
        /// Unique number for this team. Keep it stable once players have config files, since configs match on it.
        /// </summary>
        public abstract uint Id { get; set; }

        /// <summary>
        /// The display name of the team.
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        /// Roster that decides which custom roles fill the wave and in what amounts.
        /// </summary>
        public abstract List<CustomRoleProperties> CustomRoleProperties { get; set; }

        /// <summary>
        /// The vanilla wave this team replaces when it spawns.
        /// </summary>
        public abstract Faction ReplacedFaction { get; set; }

        /// <summary>
        /// Chance that this team takes over a matching wave, from 0 to 100.
        /// </summary>
        public abstract float SpawnPercentage { get; set; }

        /// <summary>
        /// The wave must hold at least this many players before the team can spawn.
        /// </summary>
        public abstract uint MinimumWaveSize { get; set; }

        /// <summary>
        /// Where team members appear after they get their roles.
        /// </summary>
        public abstract SpawnPoint SpawnPoint { get; set; }

        /// <summary>
        /// Cassie line played when the team spawns. Use spaces between words as usual.
        /// </summary>
        public abstract string CassieAnnouncement { get; set; }

        /// <summary>
        /// Subtitle text shown with the announcement.
        /// </summary>
        public abstract string CassieAnnouncementSubtitles { get; set; }

        /// <summary>
        /// Glitch flavor of the announcement. Higher values sound more broken.
        /// </summary>
        public virtual float CassieGlitchScale { get; set; } = 1;

        /// <summary>
        /// Copies tunable values from a config file version onto this team.
        /// </summary>
        /// <param name="team">The team instance read from the config file.</param>
        public void ApplyConfig(BaseCustomTeam team)
        {
            Id = team.Id;
            Name = team.Name;
            CustomRoleProperties = team.CustomRoleProperties;
            ReplacedFaction = team.ReplacedFaction;
            SpawnPercentage = team.SpawnPercentage;
            MinimumWaveSize = team.MinimumWaveSize;
            SpawnPoint = team.SpawnPoint;
            CassieAnnouncement = team.CassieAnnouncement;
            CassieAnnouncementSubtitles = team.CassieAnnouncementSubtitles;
            CassieGlitchScale = team.CassieGlitchScale;
        }
    }
}

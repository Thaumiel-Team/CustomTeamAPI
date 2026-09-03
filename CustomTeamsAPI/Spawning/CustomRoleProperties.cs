using System.Linq;
using Thaumiel.API.CustomRolesAPI;
using YamlDotNet.Serialization;

namespace Thaumiel.API.CustomTeamsAPI.Spawning
{
    public class CustomRoleProperties
    {
        /// <summary>
        /// Id of the custom role to give. Must match a registered role.
        /// </summary>
        public uint CustomRoleId { get; set; }

        /// <summary>
        /// This entry is skipped in waves smaller than this.
        /// </summary>
        public int MinimumWaveSizeForSpawn { get; set; }

        /// <summary>
        /// The most players this entry can convert in one wave.
        /// </summary>
        public uint MaximumAmountPerWave { get; set; }

        /// <summary>
        /// Share of the wave this entry converts, from 0 to 1.
        /// </summary>
        public float PercentageOfWaveToConvertToRole { get; set; }

        /// <summary>
        /// The resolved role for the id above. Empty when no registered role matches.
        /// </summary>
        [YamlIgnore]
        public BaseCustomRole CustomRole => BaseCustomRole.Roles.FirstOrDefault(r => r.Id == CustomRoleId);
    }
}

using MapGeneration;

namespace Thaumiel.API.CustomTeamsAPI.Spawning
{
    public class SpawnPoint
    {
        /// <summary>
        /// The room to spawn in.
        /// </summary>
        public RoomName SpawnRoom { get; set; }

        /// <summary>
        /// Local offset inside that room.
        /// </summary>
        public Vector3 Offset { get; set; }
    }
}

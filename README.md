# Thaumiel CustomTeamsAPI

Standalone custom teams API for LabAPI. A team takes over a respawn wave, hands its roster of custom roles to the wave players and moves everyone to the team spawn point.

## What it does

* Define custom teams in code by inheriting `BaseCustomTeam`
* Split a wave between several custom roles with weighted roster entries
* Replace a vanilla wave faction when the percentage roll hits
* Per team config files in yaml, applied on top of the coded defaults
* Cassie announcement with subtitles when a team spawns

## Config files

On first load the manager creates one yaml file per registered team under

```
configs/Thaumiel/CustomTeams
```

Edit those files to tune a team without recompiling. Values in the file win over the coded defaults. Keep the `Id` stable, since files are matched to teams by id. Role ids inside the roster must match registered custom roles.

## Minimal example

```csharp
using Thaumiel.API.CustomRolesAPI;
using Thaumiel.API.CustomTeamsAPI;
using Thaumiel.API.CustomTeamsAPI.Spawning;

public class SerpentsHand : BaseCustomTeam
{
    public override uint Id { get; set; } = 2001;
    public override string Name { get; set; } = "Serpents Hand";
    public override Faction ReplacedFaction { get; set; } = Faction.FoundationEnemy;
    public override float SpawnPercentage { get; set; } = 25;
    public override uint MinimumWaveSize { get; set; } = 5;
    public override SpawnPoint SpawnPoint { get; set; } = new()
    {
        SpawnRoom = RoomName.Lcz914,
        Offset = new Vector3(0, 1, 0),
    };
    public override List<CustomRoleProperties> CustomRoleProperties { get; set; } =
    [
        new()
        {
            CustomRoleId = 1001,
            MinimumWaveSizeForSpawn = 5,
            MaximumAmountPerWave = 3,
            PercentageOfWaveToConvertToRole = 0.5f
        },
    ];
    public override string CassieAnnouncement { get; set; } = "serpents hand has entered the facility";
    public override string CassieAnnouncementSubtitles { get; set; } = "Serpents Hand has entered the facility";
}

// once, on enable
CustomTeamsManager.RegisterTeam<SerpentsHand>();
CustomTeamsManager.RegisterTeamsFromConfig();

// take over a wave inside a WaveRespawned handler
if (CustomTeamsManager.TrySpawnWave(ev))
    return;
```
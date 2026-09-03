using LabApi.Loader.Features.Plugins;
using LabApi.Loader.Features.Plugins.Enums;
using System;
using System.Reflection;
using Thaumiel.Events;

namespace Thaumiel
{
    public class TeamPlugin : Plugin<TeamConfig>
    {
        public override string Name => "Thaumiel CustomTeams API";
        public override string Description => ":3";
        public override string Author => "Mr. Baguetter";
        public override Version RequiredApiVersion => LabApi.Features.LabApiProperties.CurrentVersion;
        public override Version Version => new(1, 3, 22);

        public override LoadPriority Priority => LoadPriority.High;
        public static TeamPlugin Instance { get; private set; }

        public override void Enable()
        {
            Instance = this;
            ServerHandler.Register();
        }

        public override void Disable()
        {
            Instance = null!;
            ServerHandler.Unregister();
        }
    }
}

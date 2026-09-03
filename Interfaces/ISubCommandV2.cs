using System.Collections.Generic;
using CommandSystem;

namespace Thaumiel.API.Interfaces
{
    internal interface ISubcommandV2
    {
        public string Name { get; }

        public string VisibleArgs { get; }

        public int RequiredArgsCount { get; }

        public string Description { get; }

        public string[] Aliases { get; }

        public string RequiredPermission { get; }

        public bool Execute(List<string> arguments, ICommandSender sender, out string response);
    }
}
using System.Collections.Generic;
using Eco.Gameplay.Systems.Chat;
using Eco.Core.Plugins.Interfaces;
using System.Reflection;
using Eco.EM.Framework.FileManager;
using Eco.Shared.Utils;
using System.Linq;
using System.IO;
using Eco.Shared.Localization;
using Eco.Gameplay.Systems.Messaging.Chat.Commands;
using Eco.EM.Framework.Utils;

namespace Eco.EM.Framework.Permissions
{
    public class CommandGroupsManager : IModKitPlugin, IInitializablePlugin
    {
        // The currently internally cached set of commands.
        private readonly IEnumerable<ChatCommand> _commands;
        private static HashSet<ChatCommandAdapter> Commands;
        private const string _configFile = "CommandGroupsConfig.json";
        internal static string protectorGroup = "command_admin";
        private const string _subPath = "/EM/CommandGroups";

        public static CommandGroupsConfig Config { get; private set; }

        public CommandGroupsManager()
        {
            _commands = LoadCommandsInternal();
            Commands = new HashSet<ChatCommandAdapter>();
            Config = LoadConfig();

            if (!File.Exists(Defaults.SaveLocation + _subPath + _configFile))
                SaveConfig();
        }

        // Retrieve a specific adapter based on an input string (may return null)
        public static ChatCommandAdapter FindAdapter(string dirtyCommand)
        {
            var cleanCommand = StringUtils.Sanitize(dirtyCommand);

            if (Commands.FirstOrDefault(adpt => adpt.Identifier == cleanCommand) != null)
                return Commands.FirstOrDefault(adpt => adpt.Identifier == cleanCommand);
            else
                return Commands.FirstOrDefault(adpt => adpt.ShortCut == cleanCommand);

        }

        // Permission system Server GUI Status
        public string GetStatus()
        {
            return "Chat Command Permissions Active";
        }

        public override string ToString()
        {
            return Localizer.DoStr("EM - Permissions");
        }

        public void Initialize(Core.Utils.TimedTask timer)
        {
            CreateAdapters();
        }

        private void CreateAdapters()
        {
            _commands.ForEach(c =>
            {
                if (!Commands.Any(adpt => adpt.Identifier == c.Name))
                    Commands.Add(new ChatCommandAdapter(c));
            });
        }

        // Internally cache all the commands.
        private IEnumerable<ChatCommand> LoadCommandsInternal()
        {
            ChatCommandService chatCommandService = new();
            IEnumerable<ChatCommand> commands = chatCommandService.GetAllCommands();

            return commands;
        }

        private CommandGroupsConfig LoadConfig()
        {
            return FileManager<CommandGroupsConfig>.ReadFromFile(Defaults.SaveLocation + _subPath, _configFile);
        }

        internal static void SaveConfig()
        {
            FileManager<CommandGroupsConfig>.WriteToFile(Config, Defaults.SaveLocation + _subPath, _configFile);
        }

        public string GetCategory() => "Elixr Mods";
    }
}

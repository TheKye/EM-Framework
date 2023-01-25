using Eco.EM.Framework.Groups;
using Eco.Gameplay.Players;
using Eco.Gameplay.Systems.Chat;
using Eco.EM.Framework.ChatBase;
using Eco.Shared.Localization;
using System.Linq;
using System;
using Eco.Core.Utils.Logging;
using Eco.Shared.Utils;
using Eco.Gameplay.Systems.Messaging.Chat.Commands;

namespace Eco.EM.Framework.Permissions
{
    // Custom chat command processor assists in overriding SLG defined Auth levels and allows us to assign standard command to our own processing logic
    public class EMCustomChatProcessor : ICommandProcessorHandler
    {
        private static readonly Func<ChatCommand, IChatClient, bool> commandProcessor;

        public EMCustomChatProcessor() { }

        [CommandProcessor]
        public static bool EMProcessCommand(ChatCommand command, IChatClient chatClient)
        {
            var level = chatClient.GetChatAuthLevel();
            var adapter = CommandGroupsManager.FindAdapter(command.Name);
            if (adapter == null)
            {
                chatClient.ErrorLocStr(string.Format(Defaults.appName + Localizer.DoStr("Command {0} not found"), command.Name));
                return false;
            }

            // if an admin or developer & we have not overridden this in our config return true;
            if ((level >= ChatAuthorizationLevel.Admin && CommandGroupsManager.Config.DefaultAdminBehaviour) || (command.AuthLevel == ChatAuthorizationLevel.User && CommandGroupsManager.Config.DefaultUserBehaviour))
            {
                // Check For Blacklisted commands
                if (GroupsManager.API.CommandPermitted(chatClient, adapter))
                {
                    commandProcessor?.Invoke(command, chatClient);
                    return true;
                }
            }

            // check the users groups permissions permissions
            if (GroupsManager.API.UserPermitted(chatClient, adapter))
            {
                commandProcessor?.Invoke(command, chatClient);
                return true;
            }

            // default behaviour is to deny if command or user is not set
            chatClient.ErrorLocStr(string.Format(Defaults.appName + Localizer.DoStr("You are not authorized to use the command {0}"), command.Name));

            commandProcessor?.Invoke(command, chatClient);
            return false;
        }
    }
}
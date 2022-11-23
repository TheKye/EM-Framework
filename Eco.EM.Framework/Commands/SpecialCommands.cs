using Eco.Server;
using Eco.Gameplay.Players;
using Eco.Gameplay.Systems.Messaging.Chat.Commands;
using Eco.ModKit;
using Eco.Shared.Localization;
using Eco.Shared.Utils;
using System;
using Eco.Gameplay.EcopediaRoot;
using Eco.Gameplay.Systems.Chat;
using Eco.EM.Framework.AdminUtils;
using Eco.Gameplay.Objects;
using Eco.Shared.Networking;

namespace Eco.EM.Framework.Commands
{
    [ChatCommandHandler]
    public class SpecialCommands
    {
        /*[ChatCommand("Opens the Admin Util UI", "admin-util",ChatAuthorizationLevel.Admin)]
        public static void AdminUtilities(User user)
        {
            user.Player.Client.RPC("OpenUI", user.Player.Client, "WorldObjectUI", typeof(AdminUiUtil));
        }
        */


        [ChatCommand("Reloads the Unity Data Files without needing to reboot the server", "rl-unity", ChatAuthorizationLevel.Admin)]
        public static void ReloadUnityData(IChatClient chatClient)
        {
            ModKitPlugin.ContentSync.RefreshContent();

            chatClient.MsgLocStr("Unity Files Refreshed, Please Re-log to get the new changes.");
        }

        [ChatCommand("Rebuilds the ecopedia", "rl-ecopedia", ChatAuthorizationLevel.Admin)]
        public static void RebuildEcopedia(IChatClient chatClient)
        {
            EcopediaManager.Build(ModKitPlugin.ModDirectory);
            chatClient.MsgLocStr("The Ecopedia Has been Rebuilt and should be automatically update. Please check the console for any logged issues with rebuilding the ecopedia");
        }

        [ChatCommand("Re-Opens the Server GUI If Possible", "rl-gui", ChatAuthorizationLevel.Admin)]
        public static void ReloadGUI(IChatClient chatClient)
        {
            try
            {
                PluginManager.Obj.OpenServerUI();
                chatClient.MsgLocStr("The Server GUI Should be available on the Server PC.");
            }
            catch (Exception e)
            {
                chatClient.ErrorLocStr(Localizer.Do($"Failed to open server GUI, it may not be supported on your OS.\r\nThe exception encountered was: {e.ToStringPretty()}"));
            }
        }
    }
}

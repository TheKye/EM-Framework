using Eco.Core;
using Eco.Core.Plugins;
using Eco.Core.Plugins.Interfaces;
using Eco.Core.Utils;
using Eco.EM.Framework.Utils;
using Eco.Gameplay.Aliases;
using Eco.Gameplay.GameActions;
using Eco.Gameplay.Players;
using Eco.Gameplay.Property;
using Eco.Gameplay.Systems.Messaging.Chat.Commands;
using Eco.Shared.Localization;
using Eco.Shared.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Eco.EM.Framework.Resolvers
{
    [LocDisplayName("EM Configure Plugin")]
    [ChatCommandHandler]
    [Priority(200)]
    public class EMConfigurePlugin : Singleton<EMConfigurePlugin>, IModKitPlugin, IConfigurablePlugin, IModInit
    {
        private static readonly PluginConfig<EMConfigureBaseConfig> config;
        public IPluginConfig PluginConfig => config;
        public static EMConfigureBaseConfig Config => config.Config;
        public ThreadSafeAction<object, string> ParamChanged { get; set; } = new ThreadSafeAction<object, string>();


        public object GetEditObject() => config.Config;
        public void OnEditObjectChanged(object o, string param) => this.SaveConfig();
        public string GetStatus() => $"Loaded EM Configure - Recipes Overriden: {EMRecipeResolver.Obj.ModRecipeOverrides?.Count: 0}";

        static EMConfigurePlugin()
        {
            config = new PluginConfig<EMConfigureBaseConfig>("EMConfigureBase");
        }


        public static void PostInitialize()
        {
            EMConfigureResolver.Initialize();
            config.SaveAsync();
        }



        public override string ToString() => Localizer.DoStr("EM Configure");

        [ChatCommand("Generates The EMConfigure.eco File for people who have headless server", "gen-emcon", ChatAuthorizationLevel.Admin)]
        public static void GenerateEmConfigure(User user)
        {
            config.SaveAsync();
            ChatBase.ChatBaseExtended.CBOkBox("Config File Generated, you can find it in: Configs/EMConfigure.eco", user);
        }

        [ChatCommand("Force the Rebuild of the EMConfigure.eco File", "fbuild-emcon", ChatAuthorizationLevel.Admin)]
        public static void ForceRebuildEmConfigure(User user)
        {
            config.ResetAsync();
            config.SaveAsync();
            ChatBase.ChatBaseExtended.CBOkBox("Config File Reset and Re-Generated, you can find it in: Configs/EMConfigure.eco", user);
        }

        public string GetCategory() => "EM Configure";
        
    }
}

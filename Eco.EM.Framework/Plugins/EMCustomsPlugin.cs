using Eco.Core.Plugins;
using Eco.Core.Plugins.Interfaces;
using Eco.Core.Utils;
using Eco.Gameplay.Players;
using Eco.Gameplay.Systems.Messaging.Chat.Commands;
using Eco.Shared.Localization;
using Eco.Shared.Utils;

namespace Eco.EM.Framework.Resolvers
{
    /// <summary>This is the EMConfigurePlugin with some minor edits</summary>
    [LocDisplayName("EM Customs Plugin")]
    [ChatCommandHandler]
    [Priority(200)]
    public class EMCustomsPlugin : Singleton<EMCustomsPlugin>, IModKitPlugin, IConfigurablePlugin, IModInit
    {
        private static readonly PluginConfig<EMCustomsConfig> config;
        public IPluginConfig PluginConfig => config;
        public static EMCustomsConfig Config => config.Config;
        public ThreadSafeAction<object, string> ParamChanged { get; set; } = new ThreadSafeAction<object, string>();

        public object GetEditObject() => config.Config;
        public void OnEditObjectChanged(object o, string param) => this.SaveConfig();
        public string GetStatus() => $"Loaded EM Customs - Overrides Used: {EMCustomsResolver.Obj.LoadedCustomsOverrides?.Count: 0}";

        static EMCustomsPlugin()
        {
            config = new PluginConfig<EMCustomsConfig>("EMCustoms");
        }

        public static void PostInitialize()
        {
            EMCustomsResolver.Obj.Initialize();
            config.SaveAsync();
        }

        public override string ToString() => Localizer.DoStr("EM Customs");

        [ChatCommand("Generates The EMCustoms.eco File for people who have headless server", "gen-emcustoms", ChatAuthorizationLevel.Admin)]
        public static void GenerateEmCustoms(User user)
        {
            config.SaveAsync();
            ChatBase.ChatBaseExtended.CBOkBox("Config File Generated, you can find it in: Configs/EMCustoms.eco", user);
        }

        [ChatCommand("Force the Rebuild of the EMCustoms.eco File", "fbuild-emcustoms", ChatAuthorizationLevel.Admin)]
        public static void ForceRebuildEmCustoms(User user)
        {
            config.ResetAsync();
            config.SaveAsync();
            ChatBase.ChatBaseExtended.CBOkBox("Config File Reset and Re-Generated, you can find it in: Configs/EMCustoms.eco", user);
        }

        public string GetCategory() => "Elixr Mods";
    }
}

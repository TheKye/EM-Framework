using Eco.Core.Plugins;
using Eco.Core.Plugins.Interfaces;
using Eco.Core.Utils;
using Eco.Gameplay.Players;
using Eco.Gameplay.Systems.Messaging.Chat.Commands;
using Eco.Shared.Localization;
using Eco.Shared.Utils;

namespace Eco.EM.Framework.Resolvers
{
    [LocDisplayName("EM Link Distances Plugin")]
    [ChatCommandHandler]
    [Priority(200)]
    public class EMLinkDistancesPlugin : Singleton<EMLinkDistancesPlugin>, IModKitPlugin, IConfigurablePlugin, IModInit
    {
        private static readonly PluginConfig<EMLinkDistancesConfig> config;
        public IPluginConfig PluginConfig => config;
        public static EMLinkDistancesConfig Config => config.Config;
        public ThreadSafeAction<object, string> ParamChanged { get; set; } = new ThreadSafeAction<object, string>();

        public object GetEditObject() => config.Config;
        public void OnEditObjectChanged(object o, string param) => this.SaveConfig();
        public string GetStatus() => "Loaded EM Link Distances";

        static EMLinkDistancesPlugin()
        {
            config = new PluginConfig<EMLinkDistancesConfig>("EMLinkDistances");
        }

        public static void Initialize()
        {
            EMLinkRadiusResolver.Obj.Initialize();
            config.SaveAsync();
        }

        public override string ToString() => Localizer.DoStr("EM Link Distances");

        [ChatCommand("Generates The EMLinkDistances.eco File for people who have headless server", "gen-emlinks", ChatAuthorizationLevel.Admin)]
        public static void GenerateEmLinkDistances(User user)
        {
            config.SaveAsync();
            ChatBase.ChatBaseExtended.CBOkBox("Config File Generated, you can find it in: Configs/EMLinkDistances.eco", user);
        }

        [ChatCommand("Force the Rebuild of the EMLinkDistances.eco File", "fbuild-emlinks", ChatAuthorizationLevel.Admin)]
        public static void ForceRebuildEmLinkDistances(User user)
        {
            config.ResetAsync();
            config.SaveAsync();
            ChatBase.ChatBaseExtended.CBOkBox("Config File Reset and Re-Generated, you can find it in: Configs/EMLinkDistances.eco", user);
        }

        public string GetCategory() => "Elixr Mods";
    }
}

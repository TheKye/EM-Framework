using Eco.Core.Plugins;
using Eco.Core.Plugins.Interfaces;
using Eco.Core.Utils;
using Eco.Gameplay.Players;
using Eco.Gameplay.Systems.Messaging.Chat.Commands;
using Eco.Shared.Localization;
using Eco.Shared.Utils;

namespace Eco.EM.Framework.Resolvers
{
    [LocDisplayName("EM Item Weight Plugin")]
    [ChatCommandHandler]
    [Priority(200)]
    public class EMItemWeightPlugin : Singleton<EMItemWeightPlugin>, IModKitPlugin, IConfigurablePlugin, IModInit
    {
        private static readonly PluginConfig<EMItemWeightConfig> config;
        public IPluginConfig PluginConfig => config;
        public static EMItemWeightConfig Config => config.Config;
        public ThreadSafeAction<object, string> ParamChanged { get; set; } = new ThreadSafeAction<object, string>();

        public object GetEditObject() => config.Config;
        public void OnEditObjectChanged(object o, string param) => this.SaveConfig();
        public string GetStatus() => "Loaded EM Item Weights";

        static EMItemWeightPlugin()
        {
            config = new PluginConfig<EMItemWeightConfig>("EMItemWeight");
        }

        public static void PostInitialize()
        {
            EMItemWeightResolver.Initialize();
            config.SaveAsync();
        }

        public override string ToString() => Localizer.DoStr("EM Item Weight");

        [ChatCommand("Generates The EMItemWeight.eco File for people who have headless server", "gen-emweights", ChatAuthorizationLevel.Admin)]
        public static void GenerateEmItemWeight(User user)
        {
            config.SaveAsync();
            ChatBase.ChatBaseExtended.CBOkBox("Config File Generated, you can find it in: Configs/EMItemWeight.eco", user);
        }

        [ChatCommand("Force the Rebuild of the EMItemWeight.eco File", "fbuild-emweights", ChatAuthorizationLevel.Admin)]
        public static void ForceRebuildEmItemWeight(User user)
        {
            config.ResetAsync();
            config.SaveAsync();
            ChatBase.ChatBaseExtended.CBOkBox("Config File Reset and Re-Generated, you can find it in: Configs/EMItemWeight.eco", user);
        }

        public string GetCategory() => "EM Configure";
    }
}

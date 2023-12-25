using Eco.Core;
using Eco.Core.Plugins;
using Eco.Core.Plugins.Interfaces;
using Eco.Core.Utils;
using Eco.Gameplay.Players;
using Eco.Gameplay.Systems.Messaging.Chat.Commands;
using Eco.Shared.Localization;
using Eco.Shared.Utils;

namespace Eco.EM.Framework.Resolvers
{
    [LocDisplayName("EM Stack Sizes Plugin")]
    [ChatCommandHandler]
    [Priority(200)]
    public class EMStackSizesPlugin : Singleton<EMStackSizesPlugin>, IModKitPlugin, IConfigurablePlugin, IModInit
    {
        private static readonly PluginConfig<EMStackSizesConfig> config;
        public IPluginConfig PluginConfig => config;
        public static EMStackSizesConfig Config => config.Config;
        public ThreadSafeAction<object, string> ParamChanged { get; set; } = new ThreadSafeAction<object, string>();

        public object GetEditObject() => config.Config;
        public void OnEditObjectChanged(object o, string param) => this.SaveConfig();
        public string GetStatus() => $"Loaded EM Stack Sizes: {EMStackSizeResolver.Overriden}";

        static EMStackSizesPlugin()
        {
            config = new PluginConfig<EMStackSizesConfig>("EMStackSizes");
        }

        public static void PostInitialize()
        {
            EMStackSizeResolver.Initialize();
            config.SaveAsync();
        }

        public override string ToString() => Localizer.DoStr("EM Stack Sizes");

        [ChatCommand("Generates The StackSizes.eco File for people who have headless server", "gen-emstacks", ChatAuthorizationLevel.Admin)]
        public static void GenerateEmStackSizes(User user)
        {
            config.SaveAsync();
            ChatBase.ChatBaseExtended.CBOkBox("Config File Generated, you can find it in: Configs/StackSizes.eco", user);
        }

        [ChatCommand("Force the Rebuild of the EMStackSizes.eco File", "fbuild-emstacks", ChatAuthorizationLevel.Admin)]
        public static void ForceRebuildEmStackSizes(User user)
        {
            config.ResetAsync();
            config.SaveAsync();
            ChatBase.ChatBaseExtended.CBOkBox("Config File Reset and Re-Generated, you can find it in: Configs/EMStackSizes.eco", user);
        }

        public string GetCategory() => "EM Configure";
    }
}

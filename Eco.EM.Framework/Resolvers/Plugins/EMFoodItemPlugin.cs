using Eco.Core.Plugins;
using Eco.Core.Plugins.Interfaces;
using Eco.Core.Utils;
using Eco.Gameplay.Players;
using Eco.Gameplay.Systems.Messaging.Chat.Commands;
using Eco.Shared.Localization;
using Eco.Shared.Utils;

namespace Eco.EM.Framework.Resolvers
{
    [LocDisplayName("EM Food Item Plugin")]
    [ChatCommandHandler]
    [Priority(200)]
    public class EMFoodItemPlugin : Singleton<EMFoodItemPlugin>, IModKitPlugin, IConfigurablePlugin, IModInit
    {
        private static readonly PluginConfig<EMFoodItemConfig> config;
        public IPluginConfig PluginConfig => config;
        public static EMFoodItemConfig Config => config.Config;
        public ThreadSafeAction<object, string> ParamChanged { get; set; } = new ThreadSafeAction<object, string>();

        public object GetEditObject() => config.Config;
        public void OnEditObjectChanged(object o, string param) => this.SaveConfig();
        public string GetStatus() => $"Loaded EM Food Items - Overrides Used: {EMFoodItemResolver.Obj.LoadedFoodOverrides?.Count: 0}";

        static EMFoodItemPlugin()
        {
            config = new PluginConfig<EMFoodItemConfig>("EMFoodItems");
        }

        public static void Initialize()
        {
            EMFoodItemResolver.Obj.Initialize();
            config.SaveAsync();
        }

        public override string ToString() => Localizer.DoStr("EM Configure - Food Config");

        [ChatCommand("Generates The EMFoodItems.eco File for people who have headless server", "gen-emfoods", ChatAuthorizationLevel.Admin)]
        public static void GenerateEmFoodItems(User user)
        {
            config.SaveAsync();
            ChatBase.ChatBaseExtended.CBOkBox("Configs File Generated, you can find it in: Configs/EMFoodItems.eco", user);
        }

        [ChatCommand("Force the Rebuild of the EMFoodItems.eco File", "fbuild-emfoods", ChatAuthorizationLevel.Admin)]
        public static void ForceRebuildEmFoodItems(User user)
        {
            config.ResetAsync();
            config.SaveAsync();
            ChatBase.ChatBaseExtended.CBOkBox("Config File Reset and Re-Generated, you can find it in: Configs/EMFoodItems.eco", user);
        }

        public string GetCategory() => "EM Configure";
    }
}

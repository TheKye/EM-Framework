using Eco.Core.Plugins;
using Eco.Core.Plugins.Interfaces;
using Eco.Core.Utils;
using Eco.Gameplay.Players;
using Eco.Gameplay.Systems.Messaging.Chat.Commands;
using Eco.Shared.Localization;
using Eco.Shared.Utils;

namespace Eco.EM.Framework.Resolvers
{
    [LocDisplayName("EM Recipes Plugin")]
    [ChatCommandHandler]
    [Priority(200)]
    public class EMRecipesPlugin : Singleton<EMRecipesPlugin>, IModKitPlugin, IConfigurablePlugin, IModInit
    {
        private static readonly PluginConfig<EMRecipesConfig> config;
        public IPluginConfig PluginConfig => config;
        public static EMRecipesConfig Config => config.Config;
        public ThreadSafeAction<object, string> ParamChanged { get; set; } = new ThreadSafeAction<object, string>();

        public object GetEditObject() => config.Config;
        public void OnEditObjectChanged(object o, string param) => this.SaveConfig();
        public string GetStatus() => $"Loaded EM Recipes - Overrides Used: {EMRecipeResolver.Obj.LoadedConfigRecipes?.Count: 0}";

        static EMRecipesPlugin()
        {
            config = new PluginConfig<EMRecipesConfig>("EMRecipes");
        }

        public static void Initialize()
        {
            EMRecipeResolver.Obj.Initialize();
            config.SaveAsync();
        }

        public override string ToString() => Localizer.DoStr("EM Recipes");

        [ChatCommand("Generates The EMRecipes.eco File for people who have headless server", "gen-emrecipes", ChatAuthorizationLevel.Admin)]
        public static void GenerateEmRecipes(User user)
        {
            config.SaveAsync();
            ChatBase.ChatBaseExtended.CBOkBox("Config File Generated, you can find it in: Configs/EMRecipes.eco", user);
        }

        [ChatCommand("Force the Rebuild of the EMRecipes.eco File", "fbuild-emrecipes", ChatAuthorizationLevel.Admin)]
        public static void ForceRebuildEmRecipes(User user)
        {
            config.ResetAsync();
            config.SaveAsync();
            ChatBase.ChatBaseExtended.CBOkBox("Config File Reset and Re-Generated, you can find it in: Configs/EMRecipes.eco", user);
        }

        public string GetCategory() => "EM Configure";
    }
}

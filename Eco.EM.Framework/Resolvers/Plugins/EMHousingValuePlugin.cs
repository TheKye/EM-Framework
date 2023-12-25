using Eco.Core.Plugins;
using Eco.Core.Plugins.Interfaces;
using Eco.Core.Utils;
using Eco.Gameplay.Housing;
using Eco.Gameplay.Players;
using Eco.Gameplay.Systems.Messaging.Chat.Commands;
using Eco.Shared.Localization;
using Eco.Shared.Utils;
using System.Threading.Tasks;

namespace Eco.EM.Framework.Resolvers
{
    [LocDisplayName("EM Housing Value Plugin")]
    [ChatCommandHandler]
    public class EMHousingValuePlugin : Singleton<EMHousingValuePlugin>, IModKitPlugin, IConfigurablePlugin, IModInit
    {
        private static readonly PluginConfig<EMHousingValueConfig> config;
        public IPluginConfig PluginConfig => config;
        public static EMHousingValueConfig Config => config.Config;
        public ThreadSafeAction<object, string> ParamChanged { get; set; } = new ThreadSafeAction<object, string>();

        public object GetEditObject() => config.Config;
        public void OnEditObjectChanged(object o, string param) => this.SaveConfig();
        public string GetStatus() => $"Loaded EM Housing Values - Overrides Used: {EMHousingResolver.Obj.LoadedHomeOverrides?.Count: 0}";

        static EMHousingValuePlugin()
        {
            config = new PluginConfig<EMHousingValueConfig>("EMHousingValue");
        }

        public static void PostInitialize()
        {
                EMHousingResolver.Initialize();
                config.SaveAsync();
        }

        public override string ToString() => Localizer.DoStr("EM Housing Value");

        [ChatCommand("Generates The EMHousingValue.eco File for people who have headless server", "gen-emhouse", ChatAuthorizationLevel.Admin)]
        public static void GenerateEmHousingValues(User user)
        {
            config.SaveAsync();
            ChatBase.ChatBaseExtended.CBOkBox("Config File Generated, you can find it in: Configs/EMHousingValue.eco", user);
        }

        [ChatCommand("Force the Rebuild of the EMHousingValue.eco File", "fbuild-emhouse", ChatAuthorizationLevel.Admin)]
        public static void ForceRebuildEmHousingValues(User user)
        {
            config.ResetAsync();
            config.SaveAsync();
            ChatBase.ChatBaseExtended.CBOkBox("Config File Reset and Re-Generated, you can find it in: Configs/EMHousingValue.eco", user);
        }

        public string GetCategory() => "EM Configure";
    }
}

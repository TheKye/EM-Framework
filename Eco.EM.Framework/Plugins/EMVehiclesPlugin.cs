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
    [LocDisplayName("EM Vehicles Plugin")]
    [ChatCommandHandler]
    [Priority(200)]
    public class EMVehiclesPlugin : Singleton<EMVehiclesPlugin>, IModKitPlugin, IConfigurablePlugin, IModInit
    {
        private static readonly PluginConfig<EMVehiclesConfig> config;
        public IPluginConfig PluginConfig => config;
        public static EMVehiclesConfig Config => config.Config;
        public ThreadSafeAction<object, string> ParamChanged { get; set; } = new ThreadSafeAction<object, string>();

        public object GetEditObject() => config.Config;
        public void OnEditObjectChanged(object o, string param) => this.SaveConfig();
        public string GetStatus() => $"Loaded EM Vehicles - Overrides Used: {EMVehicleResolver.Obj.LoadedVehicleOverrides?.Count: 0}";

        static EMVehiclesPlugin()
        {
            config = new PluginConfig<EMVehiclesConfig>("EMVehicles");
        }

        public static void Initialize()
        {
            EMVehicleResolver.Obj.Initialize();
            config.SaveAsync();
        }

        public override string ToString() => Localizer.DoStr("EM Vehicles");

        [ChatCommand("Generates The EMVehicles.eco File for people who have headless server", "gen-emvehicles", ChatAuthorizationLevel.Admin)]
        public static void GenerateEmVehicles(User user)
        {
            config.SaveAsync();
            ChatBase.ChatBaseExtended.CBOkBox("Config File Generated, you can find it in: Configs/EMVehicles.eco", user);
        }

        [ChatCommand("Force the Rebuild of the EMVehicles.eco File", "fbuild-emvehicles", ChatAuthorizationLevel.Admin)]
        public static void ForceRebuildEmVehicles(User user)
        {
            config.ResetAsync();
            config.SaveAsync();
            ChatBase.ChatBaseExtended.CBOkBox("Config File Reset and Re-Generated, you can find it in: Configs/EMVehicles.eco", user);
        }

        public string GetCategory() => "Elixr Mods";
    }
}

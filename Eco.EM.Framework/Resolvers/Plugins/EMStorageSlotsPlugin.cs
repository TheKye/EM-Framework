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
    [LocDisplayName("EM Storage Slots Plugin")]
    [ChatCommandHandler]
    [Priority(200)]
    public class EMStorageSlotsPlugin : Singleton<EMStorageSlotsPlugin>, IModKitPlugin, IConfigurablePlugin, IModInit
    {
        private static readonly PluginConfig<EMStorageSlotsConfig> config;
        public IPluginConfig PluginConfig => config;
        public static EMStorageSlotsConfig Config => config.Config;
        public ThreadSafeAction<object, string> ParamChanged { get; set; } = new ThreadSafeAction<object, string>();

        public object GetEditObject() => config.Config;
        public void OnEditObjectChanged(object o, string param) => this.SaveConfig();
        public string GetStatus() => "Loaded EM Storage Slots";

        static EMStorageSlotsPlugin()
        {
            config = new PluginConfig<EMStorageSlotsConfig>("EMStorageSlots");
        }

        public static void Initialize()
        {
            EMStorageSlotResolver.Obj.Initialize();
            config.SaveAsync();
        }

        public override string ToString() => Localizer.DoStr("EM Storage Slots");

        [ChatCommand("Generates The EMStorageSlots.eco File for people who have headless server", "gen-emslots", ChatAuthorizationLevel.Admin)]
        public static void GenerateEmStorageSlots(User user)
        {
            config.SaveAsync();
            ChatBase.ChatBaseExtended.CBOkBox("Config File Generated, you can find it in: Configs/EMStorageSlots.eco", user);
        }

        [ChatCommand("Force the Rebuild of the EMStorageSlots.eco File", "fbuild-emslots", ChatAuthorizationLevel.Admin)]
        public static void ForceRebuildEmStorageSlots(User user)
        {
            config.ResetAsync();
            config.SaveAsync();
            ChatBase.ChatBaseExtended.CBOkBox("Config File Reset and Re-Generated, you can find it in: Configs/EMStorageSlots.eco", user);
        }

        public string GetCategory() => "EM Configure";
    }
}

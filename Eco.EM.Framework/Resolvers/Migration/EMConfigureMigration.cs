using Eco.Core.Plugins;
using Eco.Core.Plugins.Interfaces;
using Eco.Core.Utils;
using Eco.EM.Framework.Utils;
using Eco.Gameplay.Players;
using Eco.Gameplay.Systems.Messaging.Chat.Commands;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using Eco.Shared.Utils;
using System.IO;
using System.Threading.Tasks;

namespace Eco.EM.Framework.Resolvers
{
    [ChatCommandHandler]
    [Priority(-200)]
    public class EMConfigureMigrationPlugin : Singleton<EMConfigureMigrationPlugin>
    {
        private static PluginConfig<EMConfigureConfig> config;
        public IPluginConfig PluginConfig => config;
        [Serialized]private static bool Migrated { get; set; }
        public static EMConfigureConfig Config => config.Config;
        public ThreadSafeAction<object, string> ParamChanged { get; set; } = new ThreadSafeAction<object, string>();
        public object GetEditObject() => config.Config;
        public string GetStatus() => "";

        static EMConfigureMigrationPlugin()
        {
                
        }

        public void Initialize()
        {
            config = new PluginConfig<EMConfigureConfig>("EMConfigureOld");

            if (Config.MigrationPerformed)
                Migrated = true;
            if (!Config.MigrationPerformed)
            {
                RunMigration();
                Migrated = true;

                config = new("EMConfigureOld");
                File.Delete(Path.Combine("Configs", "EMConfigure.eco"));
                Config.MigrationPerformed = true;
            }
            config.SaveAsync();
        }

        private static void RunMigration()
        {
            var configureconfig = EMConfigurePlugin.Config;
            configureconfig.DefaultMaxStackSize = Config.DefaultMaxStackSize;
            configureconfig.ForceSameStackSizes = Config.ForceSameStackSizes;
            configureconfig.ForcedSameStackAmount = Config.ForcedSameStackAmount;
            configureconfig.CarriedItemsOverride = Config.CarriedItemsOverride;
            configureconfig.CarriedItemsAmount = Config.CarriedItemsAmount;
            configureconfig.useConfigOverrides = Config.useConfigOverrides;
            configureconfig.OverrideVanillaStockpiles = Config.OverrideVanillaStockpiles;
            configureconfig.EnableGlobalLuckyStrike = Config.EnableGlobalLuckyStrike;

            EMFoodItemPlugin.Config.EMFoodItem = Config.EMFoodItem;
            EMHousingValuePlugin.Config.EMHousingValue = Config.EMHousingValue;
            EMItemWeightPlugin.Config.EMItemWeight = Config.EMItemWeight;
            EMLinkDistancesPlugin.Config.EMLinkDistances = Config.EMLinkDistances;
            EMRecipesPlugin.Config.EMRecipes = Config.EMRecipes;
            EMStackSizesPlugin.Config.EMStackSizes = Config.EMStackSizes;
            EMStorageSlotsPlugin.Config.EMStorageSlots = Config.EMStorageSlots;
            EMVehiclesPlugin.Config.EMVehicles = Config.EMVehicles;

        }

        public string GetCategory() => "";
    }
}

using Eco.Core.Plugins;
using Eco.Core.Plugins.Interfaces;
using Eco.Core.Utils;
using Eco.EM.Framework.Utils;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Players;
using Eco.Gameplay.Systems.Chat;
using Eco.Gameplay.Systems.Messaging.Chat.Commands;
using Eco.Mods.TechTree;
using Eco.Shared.Items;
using Eco.Shared.Localization;
using Eco.Shared.Utils;
using Eco.Simulation.WorldLayers.History;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Eco.EM.Framework.Resolvers
{
    [LocDisplayName("EM Configure Plugin")]
    [ChatCommandHandler]

    public class EMConfigurePlugin : Singleton<EMConfigurePlugin>, IModKitPlugin, IConfigurablePlugin, IModInit, ISaveablePlugin
    {
        private static readonly PluginConfig<EMConfigureConfig> config;
        public IPluginConfig PluginConfig => config;
        public static EMConfigureConfig Config => config.Config;
        public ThreadSafeAction<object, string> ParamChanged { get; set; } = new ThreadSafeAction<object, string>();

        public object GetEditObject() => config.Config;
        public void OnEditObjectChanged(object o, string param) => this.SaveConfig();
        public string GetStatus() => $"Loaded EM Configure - Recipes Overriden: {EMRecipeResolver.Obj.ModRecipeOverrides?.Count: 0}";

        static EMConfigurePlugin()
        {
            config = new PluginConfig<EMConfigureConfig>("EMConfigure");
        }

        public static void Initialize()
        {
            Task.Run(() =>
            {
                EMRecipeResolver.Obj.Initialize();
                EMLinkRadiusResolver.Obj.Initialize();
                EMStorageSlotResolver.Obj.Initialize();
                EMFoodItemResolver.Obj.Initialize();
                EMVehicleResolver.Obj.Initialize();
                RunStockpileResolver();
                RunLuckyStrikeResolver();
            });
        }

        private static void RunLuckyStrikeResolver()
        {
            if (!Config.EnableGlobalLuckyStrike) return;

            var alsdir = Path.DirectorySeparatorChar + "Mods" + Path.DirectorySeparatorChar + "UserCode" + Path.DirectorySeparatorChar + "Tools";

            if (!Directory.Exists(alsdir))
            {
                Directory.CreateDirectory(alsdir);
            }
            if (!File.Exists(Path.Combine(alsdir, "PickaxeItem.override.cs")))
                WritingUtils.WriteFromEmbeddedResource("Eco.EM.Framework.SpecialItems", "pickaxe.txt", alsdir, ".cs", specificFileName: "PickaxeItem.override");
        }

        private static void RunStockpileResolver()
        {
            if (!Config.OverrideVanillaStockpiles) return;

            var agdir = Path.DirectorySeparatorChar + "Mods"+ Path.DirectorySeparatorChar + "UserCode" + Path.DirectorySeparatorChar + "Objects";

            if (!Directory.Exists(agdir))
            {
                Directory.CreateDirectory(agdir);
            }

            if (!File.Exists(Path.Combine(agdir, "StockpileObject.override.cs")))
                WritingUtils.WriteFromEmbeddedResource("Eco.EM.Framework.SpecialItems", "Stockpile.txt", agdir, ".cs", specificFileName: "StockpileObject.override");
            if (!File.Exists(Path.Combine(agdir, "SmallStockpileObject..override.cs")))
                WritingUtils.WriteFromEmbeddedResource("Eco.EM.Framework.SpecialItems", "smallStockpile.txt", agdir, ".cs", specificFileName: "SmallStockpileObject.override");
            if (!File.Exists(Path.Combine(agdir, "LumberStockpileObject.override.cs")))
                WritingUtils.WriteFromEmbeddedResource("Eco.EM.Framework.SpecialItems", "lumberStockpile.txt", agdir, ".cs", specificFileName: "LumberStockpileObject.override");
            if (!File.Exists(Path.Combine(agdir, "LargeLumberStockpileObject.override.cs")))
                WritingUtils.WriteFromEmbeddedResource("Eco.EM.Framework.SpecialItems", "largelumberStockpile.txt", agdir, ".cs", specificFileName: "LargeLumberStockpileObject.override");

        }

        public static void PostInitialize()
        {
            Task.Run(() => { 
            EMHousingResolver.Obj.Initialize();
            EMStackSizeResolver.Initialize();
            EMItemWeightResolver.Initialize();
            config.SaveAsync();
            });
        }

        public override string ToString() => Localizer.DoStr("EM Configure");

        [ChatCommand("Generates The EMConfigure.eco File for people who have headless server", "gen-emcon", ChatAuthorizationLevel.Admin)]
        public static void GenerateEmConfigure(User user)
        {
            config.SaveAsync();
            ChatBase.ChatBaseExtended.CBOkBox("Config File Generated, you can find it in: Configs/EMConfigure.eco", user);
        }

        [ChatCommand("Force the Rebuild of the EMConfigure.eco File", "fbuild-emcon", ChatAuthorizationLevel.Admin)]
        public static void ForceRebuildEmConfigure(User user)
        {
            config.ResetAsync();
            config.SaveAsync();
            ChatBase.ChatBaseExtended.CBOkBox("Config File Reset and Re-Generated, you can find it in: Configs/EMConfigure.eco", user);
        }

        public string GetCategory() => "Elixr Mods";

        public void SaveAll() => this.SaveConfig();
    }
}

using Eco.Core.Plugins;
using Eco.Core.Plugins.Interfaces;
using Eco.Core.Utils;
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

namespace Eco.EM.Framework.Resolvers
{
    [LocDisplayName("EM Configure Plugin")]
    [ChatCommandHandler]
    [Priority(200)]
    
    public class EMConfigurePlugin : Singleton<EMConfigurePlugin>, IModKitPlugin, IConfigurablePlugin, IModInit
    {
        private static readonly PluginConfig<EMConfigureConfig> config;
        public IPluginConfig PluginConfig => config;
        public static EMConfigureConfig Config => config.Config;
        public ThreadSafeAction<object, string> ParamChanged { get; set; } = new ThreadSafeAction<object, string>();

        public object GetEditObject() => config.Config;
        public void OnEditObjectChanged(object o, string param) => this.SaveConfig();
        public string GetStatus() => $"Loaded EM Configure - Recipes Overriden: {EMRecipeResolver.Obj.ModRecipeOverrides?.Count : 0}";

        static EMConfigurePlugin()
        {
            config = new PluginConfig<EMConfigureConfig>("EMConfigure");          
        }

        public static void Initialize()
        {
            EMRecipeResolver.Obj.Initialize();
            EMLinkRadiusResolver.Obj.Initialize();
            EMStorageSlotResolver.Obj.Initialize();
            EMFoodItemResolver.Obj.Initialize();
            EMVehicleResolver.Obj.Initialize();
            RunStockpileResolver();
            RunLuckyStrikeResolver();
        }

        private static void RunLuckyStrikeResolver()
        {
            if (!Config.EnableGlobalLuckyStrike) return;
            var directory = Defaults.AssemblyLocation + "/Mods/UserCode";
            var alsdir = directory + "/Tools";

            if (!Directory.Exists(alsdir))
            {
                Directory.CreateDirectory(alsdir);
            }

            var assembly = Assembly.GetCallingAssembly();
            var pickaxeresourceName = "Eco.EM.Framework.SpecialItems." + "pickaxe.txt";
            string resource = null;
            try
            {
                using Stream stream = assembly.GetManifestResourceStream(pickaxeresourceName);
                using StreamReader reader = new(stream);
                resource = reader.ReadToEnd();
                File.WriteAllText(alsdir + "/" + "PickaxeItem.override.cs", resource);
                resource = null;
            }
            catch { }
        }

        private static void RunStockpileResolver()
        {
            if (!Config.OverrideVanillaStockpiles) return;
            var directory = Defaults.AssemblyLocation + "/Mods/UserCode";
            var agdir = directory + "/Objects";

            if (!Directory.Exists(agdir))
            {
                Directory.CreateDirectory(agdir);
            }

            var assembly = Assembly.GetCallingAssembly();
            var stockpileresourceName = "Eco.EM.Framework.SpecialItems." + "Stockpile.txt";
            var smallstockpileresourceName = "Eco.EM.Framework.SpecialItems." + "smallStockpile.txt";
            var lumberstockpileresourceName = "Eco.EM.Framework.SpecialItems." + "lumberStockpile.txt";
            var largelumberstockpileresourceName = "Eco.EM.Framework.SpecialItems." + "largelumberStockpile.txt";
            string resource = null;
            try
            {
                using Stream stream = assembly.GetManifestResourceStream(stockpileresourceName);
                using StreamReader reader = new(stream);
                resource = reader.ReadToEnd();
                File.WriteAllText(agdir + "/" + "StockpileObject.override.cs", resource);
                resource = null;
            }
            catch { }
            try
            {
                using Stream stream = assembly.GetManifestResourceStream(smallstockpileresourceName);
                using StreamReader reader = new(stream);
                resource = reader.ReadToEnd();
                File.WriteAllText(agdir + "/" + "SmallStockpileObject.override.cs", resource);
                resource = null;
            }
            catch { }
            try
            {
                using Stream stream = assembly.GetManifestResourceStream(lumberstockpileresourceName);
                using StreamReader reader = new(stream);
                resource = reader.ReadToEnd();
                File.WriteAllText(agdir + "/" + "LumberStockpileObject.override.cs", resource);
                resource = null;
            }
            catch { }
            try
            {
                using Stream stream = assembly.GetManifestResourceStream(largelumberstockpileresourceName);
                using StreamReader reader = new(stream);
                resource = reader.ReadToEnd();
                File.WriteAllText(agdir + "/" + "LargeLumberStockpileObject.override.cs", resource);
                resource = null;
            }
            catch { }
        }

        public static void PostInitialize()
        {
            EMHousingResolver.Obj.Initialize();
            EMStackSizeResolver.Initialize();
            EMItemWeightResolver.Initialize();

            config.SaveAsync();
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
    }
}

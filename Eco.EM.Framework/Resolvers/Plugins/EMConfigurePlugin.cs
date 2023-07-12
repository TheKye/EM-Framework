using Eco.Core.Plugins;
using Eco.Core.Plugins.Interfaces;
using Eco.Core.Utils;
using Eco.EM.Framework.Utils;
using Eco.Gameplay.Players;
using Eco.Gameplay.Systems.Messaging.Chat.Commands;
using Eco.Shared.Localization;
using Eco.Shared.Utils;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.IO;
using System.Threading.Tasks;

namespace Eco.EM.Framework.Resolvers
{
    [LocDisplayName("EM Configure Plugin")]
    [ChatCommandHandler]

    public class EMConfigurePlugin : Singleton<EMConfigurePlugin>, IModKitPlugin, IConfigurablePlugin, IModInit, ISaveablePlugin
    {
        private static readonly PluginConfig<EMConfigureBaseConfig> config;
        public IPluginConfig PluginConfig => config;
        public static EMConfigureBaseConfig Config => config.Config;
        public ThreadSafeAction<object, string> ParamChanged { get; set; } = new ThreadSafeAction<object, string>();

        public object GetEditObject() => config.Config;
        public void OnEditObjectChanged(object o, string param) => this.SaveConfig();
        public string GetStatus() => $"Loaded EM Configure - Recipes Overriden: {EMRecipeResolver.Obj.ModRecipeOverrides?.Count: 0}";

        static EMConfigurePlugin()
        {

            config = new PluginConfig<EMConfigureBaseConfig>("EMConfigureBase");
            Task.Run(() => {
                EMConfigureMigrationPlugin.Obj.Initialize();

            });
        }

        public static void Initialize()
        {
            RunStockpileResolver();
            RunLuckyStrikeResolver();
            config.SaveAsync();
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

            var agdir = Path.DirectorySeparatorChar + "Mods" + Path.DirectorySeparatorChar + "UserCode" + Path.DirectorySeparatorChar + "Objects";

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

        [ChatCommand("Adjust basic config settings for em configure", "config-em", ChatAuthorizationLevel.Admin)]
        public static void EMConfigureConfig(User user, string setting, string value)
        {
            value = value.Trim();
            setting = setting.ToLower();
            switch (setting)
            {
                case "defaultstacksize":
                   var s = int.TryParse(value, out var i);
                    if (s)
                    {
                        config.Config.DefaultMaxStackSize = i;
                        config.SaveAsync();
                        user.MsgLocStr($"Default Max Stack size changed to: {i}");
                        return;
                    }
                    else
                    {
                        user.ErrorLocStr($"{value} is not a valid number for this setting");
                        return;
                    }
                case "forcesamestacksize":
                    var su = bool.TryParse(value, out bool res);
                    if(su)
                    {
                        config.Config.ForceSameStackSizes = res;
                        config.SaveAsync();
                        user.MsgLocStr($"Force Same Stack Sizes changed to: {res}");
                        return;
                    }
                    else
                    {
                        user.ErrorLocStr($"{value} is not a valid for this setting, please use true or false");
                        return;
                    }
                case "forcedstackamount":
                    var succe = int.TryParse(value, out var it);
                    if (succe)
                    {
                        config.Config.ForcedSameStackAmount = it;
                        config.SaveAsync();
                        user.MsgLocStr($"Default Max Stack size changed to: {it}");
                        return;
                    }
                    else
                    {
                        user.ErrorLocStr($"{value} is not a valid number for this setting");
                        return;
                    }
                case "carrieditemsoverride":
                    var suc = bool.TryParse(value, out bool reso);
                    if (suc)
                    {
                        config.Config.CarriedItemsOverride = reso;
                        config.SaveAsync();
                        user.MsgLocStr($"Force Same Stack Sizes changed to: {reso}");
                        return;
                    }
                    else
                    {
                        user.ErrorLocStr($"{value} is not a valid for this setting, please use true or false");
                        return;
                    }
                case "carrieditemsamount":
                    var succes = int.TryParse(value, out var ite);
                    if (succes)
                    {
                        config.Config.CarriedItemsAmount = ite;
                        config.SaveAsync();
                        user.MsgLocStr($"Default Max Stack size changed to: {ite}");
                        return;
                    }
                    else
                    {
                        user.ErrorLocStr($"{value} is not a valid number for this setting");
                        return;
                    }
                case "useconfigoverrides":
                    var succ = bool.TryParse(value, out bool resou);
                    if (succ)
                    {
                        config.Config.useConfigOverrides = resou;
                        config.SaveAsync();
                        user.MsgLocStr($"Force Same Stack Sizes changed to: {resou}");
                        return;
                    }
                    else
                    {
                        user.ErrorLocStr($"{value} is not a valid for this setting, please use true or false");
                        return;
                    }
                default:
                    user.ErrorLocStr($"{setting} is not valid. Please use: DefaultStackSize, ForceSameStackSize, CarriedItemsOverride, CarriedItemsAmount or UseConfigOverrides.");
                    return;
            }
        }

        public string GetCategory() => "EM Configure";
        public void SaveAll()
        {
            RunLuckyStrikeResolver();
            RunStockpileResolver();
            this.SaveConfig();
        }
    }
}

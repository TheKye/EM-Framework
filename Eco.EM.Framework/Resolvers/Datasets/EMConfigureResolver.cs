using Eco.EM.Framework.Utils;
using Eco.Shared.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Eco.Shared.Utils.LimitMapper;

namespace Eco.EM.Framework.Resolvers
{
    internal class EMConfigureResolver
    {
        public static void Initialize()
        {
            if (EMConfigurePlugin.Config.OverrideVanillaStockpiles)
                RunStockpileResolver();
            if (EMConfigurePlugin.Config.EnableGlobalLuckyStrike)
                RunLuckyStrikeResolver();
        }

        private static void RunLuckyStrikeResolver()
        {

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
    }
}

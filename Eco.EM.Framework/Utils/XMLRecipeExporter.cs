using Eco.Core.Plugins.Interfaces;
using Eco.Core.Utils;
using Eco.Gameplay.Items;
using Eco.Gameplay.Items.Recipes;
using Eco.Shared.Localization;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco.EM.Framework.Utils
{
    [LocDisplayName("Vanilla Recipe Export (XML)")]
    public class XMLExporter : IModKitPlugin, ICommandablePlugin
    {
        private const string _subPath = "/EM/RecipeExport/";
        public const string saveLocation = "/Configs/Mods";
        public static string AssemblyLocation => Directory.GetCurrentDirectory();
        public static string SaveLocation => GetRelevantDirectory();

        static string GetRelevantDirectory()
        {
            if (saveLocation.StartsWith("/"))
            {
                return AssemblyLocation + saveLocation;
            }
            return saveLocation;
        }

        readonly static List<Recipe> VanillaRecipe = new();
        readonly static DataTable dt = new DataTable();

        private static void BuildRecipeLists()
        {
            var all = RecipeManager.AllRecipeFamilies;
            foreach (var family in all)
            {
                family.Recipes.ForEach(r => { if (!VanillaRecipe.Contains(r)) VanillaRecipe.Add(r); });
            }
        }

        public static void BuildRecipeTable()
        {
            dt.Clear();
            dt.TableName = "Recipes";
            dt.Columns.Add("Name");
            dt.Columns.Add("BaseLaborCost");
            dt.Columns.Add("Ingredient1");
            dt.Columns.Add("Amount1");
            dt.Columns.Add("Ingredient2");
            dt.Columns.Add("Amount2");
            dt.Columns.Add("Ingredient3");
            dt.Columns.Add("Amount3");
            dt.Columns.Add("Ingredient4");
            dt.Columns.Add("Amount4");
            dt.Columns.Add("Ingredient5");
            dt.Columns.Add("Amount5");
            dt.Columns.Add("Result");
            dt.Columns.Add("ResultAmount");
        }

        public static void BuildRecipeRows()
        {

            foreach (Recipe r in VanillaRecipe)
            {
                int count = 0;

                DataRow _dtrow = dt.NewRow();
                _dtrow["Name"] = r.DisplayName;
                _dtrow["BaseLaborCost"] = r.Family.Labor.ToString();

                // Ingredients required
                foreach (var e in r.Ingredients)
                {
                    if (count >= 5) break;
                    count++;
                    _dtrow["Ingredient" + count] = e.IsSpecificItem ? e.Item.DisplayName : e.Tag.DisplayName;
                    _dtrow["Amount" + count] = e.Quantity.GetBaseValue.ToString();


                }
                foreach (var e in r.Products)
                {
                    _dtrow["Result"] = e.Item.DisplayName;
                    _dtrow["ResultAmount"] = e.Quantity.GetBaseValue.ToString();

                }
                dt.Rows.Add(_dtrow);
            }
        }

        public static void ExportRecipesXML()
        {
            BuildRecipeLists();
            BuildRecipeTable();
            BuildRecipeRows();
            ExportXML();
        }

        public static void ExportXML()
        {
            dt.WriteXml(Path.Combine(SaveLocation + _subPath, "RecipeExport.xml"));
        }

        public string GetCategory() => "Utilities";

        public string GetStatus() => Localizer.DoStr($"XML Recipe Exporter");

        public override string ToString() => "XML Recipe Exporter";

        public void GetCommands(Dictionary<string, Action> nameToFunction)
        {
            nameToFunction.Add(Localizer.DoStr("Export Recipes to XML"), () => ExportRecipesXML());
        }
    }
}

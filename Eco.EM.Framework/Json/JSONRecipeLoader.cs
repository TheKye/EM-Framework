/*using Eco.EM.Framework.Utils;
using Eco.Gameplay.Items;
using Eco.Gameplay.Items.Recipes;
using Eco.Gameplay.Utils;
using Eco.Shared.Localization;
using Eco.Shared.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Eco.EM.Framework.Json
{
    internal class JSONRecipeLoader
    {
        string AbsolutePath = Defaults.GetRelevantDirectory("Recipes");


        public void Initialize()
        {

            InitalizeRecipes();
        }


        //Begin the creation of the recipes from the files
        private void InitalizeRecipes()
        {
            //All Files from the specified folder will be gained and then assigned to F then iterated through to construct the recipes, we also check sub directories too should a mod keep their recipes seperate
            foreach (var f in Directory.GetFiles(AbsolutePath))
            {
                //Each file is deserialized against the JsonRecipeClass in order to have the correct and proper formatting for an acurate recipe reading
                var dso = JsonConvert.DeserializeObject<List<JsonRecipeClass>>(f);
                //Check Each List inside the file and build the recipe based on the information present
                foreach (var r in dso)
                {
                    var recipe = new Recipe();

                    List<IngredientElement> ings = null;
                    List<CraftingElement> prod = null;

                    foreach (var ie in r.RecipeIngredients)
                    {
                        if (!string.IsNullOrEmpty(ie.TagName))
                        {
                            if(ie.RecipeIsStatic)
                                ings.Add(new IngredientElement(ie.TagName, ie.ItemAmount, ie.RecipeIsStatic));
                            if (r.UseCraftingCostMultipliers)
                                ings.Add(new IngredientElement(ie.TagName, ie.ItemAmount));
                        }

                        else
                        {
                            if (ie.RecipeIsStatic)
                            {
                                ings.Add(new IngredientElement(ItemUtil.TryGet(ie.ItemName).GetType(), ie.ItemAmount, ie.RecipeIsStatic));
                            }
                            if (r.UseCraftingCostMultipliers)
                            {
                                ings.Add(new IngredientElement(ItemUtil.TryGet(ie.ItemName).GetType(), ie.ItemAmount));
                            }
                        }
                    }


                    foreach (var c in r.RecipeProducts)
                    {
                        Item i = ItemUtil.TryGet(c.ItemName);
                        Type ce = typeof(CraftingElement<>);
                        var generic = ce.MakeGenericType(i.GetType());
                        CraftingElement element = Activator.CreateInstance(generic, c.OutputAmount) as CraftingElement;
                        prod.Add(element);
                    }



                    recipe.Init(
                        name: r.RecipeName,
                        Localizer.DoStr(r.RecipeName),
                        ingredients: ings,
                        prod



                        );

                }

            }
        }
    }
}
*/
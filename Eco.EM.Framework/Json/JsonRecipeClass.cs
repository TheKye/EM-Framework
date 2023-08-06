using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco.EM.Framework.Json
{
    class JsonRecipeClass
    {
        public string RecipeName { get; set; }
        public string RequiredSkill { get; set; }
        public int RequiredSkillLevel { get; set; }
        public List<Ingredients> RecipeIngredients { get; set; }
        public List<Products> RecipeProducts { get; set; } 
        public int CraftTime { get; set; }
        public int CalorieCost { get; set; }
        public string CraftingTable { get; set; }
        public bool UseSpeedModifiers { get; set; }
        public bool UseCraftingCostMultipliers {get; set;}
        public bool HasParentRecipe { get; set; }
        public List<Parents> ParentRecipe { get; set; }
    }

    internal class Ingredients
    {
        public string ItemName { get; set; }
        public string TagName { get; set; }
        public int ItemAmount { get; set; }
        public bool RecipeIsStatic { get; set; }

    }

    internal class Products
    {
        public string ItemName { get; set; }
        public int OutputAmount { get; set; }
    }
    internal class Parents
    {
        public string ParentRecipe { get; set; }
        public string ParentRecipeCraftingTable { get; set; }
    }
}

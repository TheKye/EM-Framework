using Eco.EM.Framework.Utils;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Eco.EM.Framework.Resolvers
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class EMIngredient : IEquatable<EMIngredient>
    {
        [LocDisplayName("Ingredient Item")] public string Item { get; set; }
        [LocDisplayName("Item is Tag")] public bool Tag { get; set; }
        [LocDisplayName("Ingredient Amount")] public float Amount { get; set; }
        [LocDisplayName("Static Ingredient")] public bool Static { get; set; }

        public EMIngredient(string item, bool isTag, float amount, bool isStatic = false)
        {
            Item = item;
            Tag = isTag;
            Amount = amount;
            Static = isStatic;
        }

        public EMIngredient() { }

        public override string ToString() => $"{Item}";

        public bool Equals(EMIngredient other)
        {
            if (other is null)
                return false;

            return this.Item == other.Item && this.Amount == other.Amount && this.Tag == other.Tag && this.Static == other.Static;
        }

        public override bool Equals(object obj) => Equals(obj as EMIngredient);

        public override int GetHashCode() => (Item, Amount, Tag, Static).GetHashCode();
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class EMCraftable : IEquatable<EMCraftable>
    {
        [LocDisplayName("Craftable Item")] public string Item { get; set; }
        [LocDisplayName("Crafting Amount")] public float Amount { get; set; }

        public EMCraftable(string item, float amount = 1)
        {
            Item = item;
            Amount = amount;
        }

        public EMCraftable() { }

        public override string ToString() => $"{Item}";

        public bool Equals(EMCraftable other)
        {
            if (other is null)
                return false;

            return this.Item == other.Item && this.Amount == other.Amount;
        }

        public override bool Equals(object obj) => Equals(obj as EMCraftable);

        public override int GetHashCode() => (Item, Amount).GetHashCode();
    }

    [LocDisplayName("Recipe Model")]
    public class RecipeModel : ModelBase
    {
        [LocDisplayName("Crafting Experience - Baseline Value")] public float BaseExperienceOnCraft { get; set; }
        [LocDisplayName("Labor - Baseline Value")] public float BaseLabor { get; set; }
        [LocDisplayName("Labor - Is Static")] public bool LaborIsStatic { get; set; }
        [LocDisplayName("Craft Time - Baseline Value")] public float BaseCraftTime { get; set; }
        [LocDisplayName("Craftime - Is Static")] public bool CraftTimeIsStatic { get; set; }
        [LocDisplayName("Crafting Station")] public string CraftingStation { get; set; }
        [LocDisplayName("Ingredient List")] public List<EMIngredient> IngredientList { get; set; }
        [LocDisplayName("Product List")] public List<EMCraftable> ProductList { get; set; }
        [LocDisplayName("Enable Recipe")] public bool EnableRecipe { get; set; } = true;

        public override string ToString() => $"{StringUtils.GetAssemblyNameFromAssemblyString(Assembly)} - {ModelType}";

        public static bool Compare(RecipeModel A, RecipeModel B)
        {
            return A.IngredientList.SequenceEqual(B.IngredientList) && A.ProductList.SequenceEqual(B.ProductList);
        }
    }

    [LocDisplayName("Default Recipe Model")]
    public class RecipeDefaultModel : RecipeModel
    {
        [JsonIgnore, ReadOnly(true)] public string HiddenName { get; set; }
        [LocDisplayName("Recipe Name")] public LocString LocalizableName { get; set; }
        [JsonIgnore, ReadOnly(true)] public Type RequiredSkillType { get; set; }
        [JsonIgnore, ReadOnly(true)] public int RequiredSkillLevel { get; set; }
        [JsonIgnore, ReadOnly(true)] public Type IngredientImprovementTalents { get; set; }
        [JsonIgnore, ReadOnly(true)] public Type[] SpeedImprovementTalents { get; set; }
    }
}

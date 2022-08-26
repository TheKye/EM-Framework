using Eco.EM.Framework.Utils;
using Eco.Shared.Localization;
using System;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Eco.EM.Framework.Resolvers
{

    public class FoodItemModel : ModelBase
    {
        [LocDisplayName("Carbs")]     public float Carbs          { get; set; }
        [LocDisplayName("Protein")]   public float Protein        { get; set; }
        [LocDisplayName("Fat")]       public float Fat            { get; set; }
        [LocDisplayName("Vitamins")]  public float Vitamins       { get; set; }
        [LocDisplayName("Calories")]  public float Calories       { get; set; }
        [LocDisplayName("Shelf Life")] public int ShelfLife { get; set; }
        [LocDisplayName("Display Name"), ReadOnly(true)]  public string DisplayName   { get; set; }

        public FoodItemModel(Type type, string displayName, float carbs, float fat, float protein, float vitamins, int shelflife, float calories)
        {
            ModelType = type.Name;
            Assembly = type.AssemblyQualifiedName;
            DisplayName = displayName;
            Carbs = carbs;
            Protein = protein;
            Fat = fat;
            Vitamins = vitamins;
            ShelfLife = shelflife;
            Calories = calories;
        }

        [JsonConstructor]
        public FoodItemModel(string modelType, string assembly, float carbs, float fat, float protein, float vitamins, int shelflife, float calories, string displayName)
        {
            ModelType = modelType;
            Assembly = assembly;
            Carbs = carbs;
            Protein = protein;
            Fat = fat;
            Vitamins = vitamins;
            ShelfLife = shelflife;
            Calories = calories;
            DisplayName = displayName;
        }

        public override string ToString() => $"{StringUtils.GetAssemblyNameFromAssemblyString(Assembly)} - {DisplayName}";
    }
}

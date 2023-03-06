using Eco.Core.Utils;
using Eco.Gameplay.Players;
using Eco.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Eco.EM.Framework.Resolvers
{
    public interface IConfigurableFoodItem { }

    public class EMFoodItemResolver : AutoSingleton<EMFoodItemResolver>
    {
        public Dictionary<string, FoodItemModel> DefaultFoodOverrides { get; private set; } = new();
        public Dictionary<string, FoodItemModel> LoadedFoodOverrides { get; private set; } = new();

        public Nutrients ResolveNutrients(IConfigurableFoodItem nutrients) => GetNutrients(nutrients);
        public float ResolveCalories(IConfigurableFoodItem calories) => GetCalories(calories);

        public int ResolveShelfLife(IConfigurableFoodItem shelflife) => GetshelfLife(shelflife);

        private int GetshelfLife(IConfigurableFoodItem shelflife)
        {
            var dModel = LoadedFoodOverrides[shelflife.GetType().Name];
            var loaded = LoadedFoodOverrides.TryGetValue(shelflife.GetType().Name, out FoodItemModel model);

            // check if config override
            if (loaded)
            {
                return (int)TimeUtil.HoursToSeconds(model.ShelfLife);
            }

            // return default
            return (int)TimeUtil.HoursToSeconds(dModel.ShelfLife);
        }

        public static void AddDefaults(FoodItemModel defaults)
        {
            Obj.DefaultFoodOverrides.Add(defaults.ModelType, defaults);
        }

        private Nutrients GetNutrients(IConfigurableFoodItem nutrient)
        {
            var dModel = LoadedFoodOverrides[nutrient.GetType().Name];
            var dNutrients = CreateDefaultNutritionFromModel(dModel);

            // check if config override
            var loaded = LoadedFoodOverrides.TryGetValue(nutrient.GetType().Name, out FoodItemModel model);
            if (loaded)
            {
                return CreateNutritionFromModel(model);
            }

            // return default
            return dNutrients;
        }

        private float GetCalories(IConfigurableFoodItem calories)
        {
            var dModel = LoadedFoodOverrides[calories.GetType().Name];
            var loaded = LoadedFoodOverrides.TryGetValue(calories.GetType().Name, out FoodItemModel model);

            // check if config override
            if (loaded)
            {
                return model.Calories;
            }

            // return default
            return dModel.Calories;
        }

        private static Nutrients CreateNutritionFromModel(FoodItemModel model)
        {
            var nutrients = new Nutrients()
            {
                Carbs = model.Carbs,
                Protein = model.Protein,
                Fat = model.Fat,
                Vitamins = model.Vitamins
            };

            return nutrients;
        }

        private static Nutrients CreateDefaultNutritionFromModel(FoodItemModel def)
        {
            var nutrients = new Nutrients()
            {
                Carbs = def.Carbs,
                Protein = def.Protein,
                Fat = def.Fat,
                Vitamins = def.Vitamins
            };

            return nutrients;
        }

        public void Initialize()
        {
            SerializedSynchronizedCollection<FoodItemModel> newModels = new();
            var previousModels = EMFoodItemPlugin.Config.EMFoodItem;
            foreach (var type in typeof(IConfigurableFoodItem).ConcreteTypes())
            {
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            }

            var loadtypes = DefaultFoodOverrides.Values.ToList();

            // for each type that exists that we are trying to load
            foreach (var lModel in loadtypes)
            {
                var m = previousModels.SingleOrDefault(x => x.ModelType == lModel.ModelType);

                if (m != null)
                    newModels.Add(m);
                else
                    newModels.Add(lModel);
            }

            EMFoodItemPlugin.Config.EMFoodItem = newModels;

            foreach (var model in newModels)
            {
                if (!LoadedFoodOverrides.ContainsKey(model.ModelType))
                    LoadedFoodOverrides.Add(model.ModelType, model);
            }
        }
    }
}

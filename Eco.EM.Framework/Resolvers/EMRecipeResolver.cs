using Eco.Gameplay.Items;
using Eco.Shared.Utils;
using System.Collections.Generic;
using System;
using Eco.Gameplay.DynamicValues;
using Eco.Shared.Localization;
using System.Linq;
using Eco.Core.Utils;
using Eco.EM.Framework.Utils;
using Eco.Gameplay.Skills;

// This mod is created by Elixr Mods for Eco under the SLG TOS. 
// Please feel free to join our community Discord which aims to brings together modders of Eco to share knowledge, 
// collaborate on projects and improve the overall experience for Eco modders.
// https://discord.gg/69UQPD2HBR

namespace Eco.EM.Framework.Resolvers
{
    /// <summary>
    /// For classes that override an EMRecipeResolver compatible recipe.
    /// OverrideType (Type):        The Recipe Type that is to be overidden.
    /// NewRecipe: (List<Recipe>):  The new default recipe in a list.
    /// </summary>
    public interface IRecipeOverride
    {
        string OverrideType { get; }
        RecipeModel Model { get; }
        bool debug { get; }
    }

    public interface IConfigurableRecipe { }

    public class EMRecipeResolver : AutoSingleton<EMRecipeResolver>
    {
        // for default recipes
        public Dictionary<string, RecipeDefaultModel> LoadedDefaultRecipes { get; private set; } = new();
        // for overrides from mods
        public Dictionary<string, RecipeModel> ModRecipeOverrides { get; private set; } = new();
        // for overrides from config
        public Dictionary<string, RecipeModel> LoadedConfigRecipes { get; private set; } = new();

        public static void AddDefaults(RecipeDefaultModel defaults)
        {
            Obj.LoadedDefaultRecipes.Add(defaults.ModelType, defaults);
        }

        // Individual RecipeFamily part resolvers.
        public List<Recipe> ResolveRecipe(IConfigurableRecipe recipe) => GetRecipe(recipe);
        public IDynamicValue ResolveLabor(IConfigurableRecipe recipe) => GetLaborValue(recipe);
        public IDynamicValue ResolveCraftMinutes(IConfigurableRecipe recipe) => GetCraftTime(recipe);
        public Type ResolveStation(IConfigurableRecipe recipe) => GetConfigStation(recipe);

        public float ResolveExperience(IConfigurableRecipe recipe) => GetExperience(recipe);

        private float GetExperience(IConfigurableRecipe recipe)
        {
            var dModel = LoadedDefaultRecipes[recipe.GetType().Name];
            // check if config override
            var loaded = LoadedConfigRecipes.TryGetValue(recipe.GetType().Name, out RecipeModel model);
            if (loaded && (model.BaseExperienceOnCraft != dModel.BaseExperienceOnCraft))
                return model.BaseExperienceOnCraft;

            // check if mod override
            loaded = ModRecipeOverrides.TryGetValue(recipe.GetType().Name, out model);
            if (loaded)
                return model.BaseExperienceOnCraft;

            // return default
            return dModel.BaseExperienceOnCraft;
        }

        #region Decison Methods
        // checks to decide which version of the recipe should be loaded in
        private List<Recipe> GetRecipe(IConfigurableRecipe recipe)
        {
            var dModel = LoadedDefaultRecipes[recipe.GetType().Name];
            var dRecipe = CreateDefaultRecipeFromModel(dModel);

            // check if config override
            var loaded = LoadedConfigRecipes.TryGetValue(recipe.GetType().Name, out RecipeModel model);
            if (loaded && !RecipeModel.Compare(dModel, model) || loaded && !model.EnableRecipe)
            {
                ConsoleColors.PrintConsoleMultiColored("[EM Framework] ", ConsoleColor.Magenta, Localizer.DoStr(string.Format("Loaded Server Override - {0}", dModel.HiddenName)), ConsoleColor.Yellow);
                return new List<Recipe>() { CreateRecipeFromModel(model, dModel) };
            }

            // check if mod override
            loaded = ModRecipeOverrides.TryGetValue(recipe.GetType().Name, out model);
            if (loaded)
            {
                ConsoleColors.PrintConsoleMultiColored("[EM Framework] ", ConsoleColor.Magenta, Localizer.DoStr(string.Format("Loaded Mod Override - {0}", dModel.HiddenName)), ConsoleColor.Yellow);
                return new List<Recipe>() { CreateRecipeFromModel(model, dModel) };
            }

            // return default
            return new List<Recipe>() { dRecipe };
        }

        private static Recipe CreateRecipeFromModel(RecipeModel model, RecipeDefaultModel def)
        {
            var recipe = new Recipe();
            recipe.Init(
            def.HiddenName,
            def.LocalizableName,
            CreateIngredientList(model, def.RequiredSkillType, def.IngredientImprovementTalents),
            CreateProductList(model));
            return recipe;
        }

        private static Recipe CreateDefaultRecipeFromModel(RecipeDefaultModel def)
        {
            var recipe = new Recipe();
            recipe.Init(
            def.HiddenName,
            def.LocalizableName,
            CreateIngredientList(def, def.RequiredSkillType, def.IngredientImprovementTalents),
            CreateProductList(def));
            return recipe;
        }

        private static List<IngredientElement> CreateIngredientList(RecipeModel model, Type skill, Type talent)
        {
            List<IngredientElement> ingredients = new();
            foreach (var value in model.IngredientList)
            {
                try
                {
                    if (value.Static)
                    {
                        if (!value.Tag)
                            ingredients.Add(new IngredientElement(Item.Get(value.Item), value.Amount, true));
                        else
                            ingredients.Add(new IngredientElement(value.Item, value.Amount, true));
                    }
                    else if (skill != null && talent != null)
                    {
                        if (!value.Tag)
                            ingredients.Add(new IngredientElement(Item.Get(value.Item), value.Amount, skill, talent));
                        else
                            ingredients.Add(new IngredientElement(value.Item, value.Amount, skill, talent));
                    }
                    else if(skill !=null && talent == null)
                    {
                        if (!value.Tag)
                            ingredients.Add(new IngredientElement(Item.Get(value.Item), value.Amount, skill));
                        else
                            ingredients.Add(new IngredientElement(value.Item, value.Amount, skill));
                    }
                    else
                    {
                        if (!value.Tag)
                            ingredients.Add(new IngredientElement(Item.Get(value.Item), value.Amount, false));
                        else
                            ingredients.Add(new IngredientElement(value.Item, value.Amount, false));
                        
                    }
                }
                catch
                {
                    ConsoleColors.PrintConsoleMultiColored("[EM Framework] ", ConsoleColor.Magenta, Localizer.DoStr(string.Format("{0} was an invalid ingredient lookup for an Item or Tag. Please review the recipe config for {1}", value.Item, model.ModelType)), ConsoleColor.Red);
                    throw;
                }
            }
            return ingredients;
        }

        private static List<CraftingElement> CreateProductList(RecipeModel model)
        {
            List<CraftingElement> products = new();
            foreach (var value in model.ProductList)
            {
                try
                {
                    Item i = Item.Get(value.Item);
                    Type ce = typeof(CraftingElement<>);
                    var generic = ce.MakeGenericType(i.GetType());
                    CraftingElement element = Activator.CreateInstance(generic, value.Amount) as CraftingElement;
                    products.Add(element);
                }
                catch
                {
                    ConsoleColors.PrintConsoleMultiColored("[EM Framework] ", ConsoleColor.Magenta, Localizer.DoStr(string.Format("{0} was an invalid product lookup for an Item or Tag. Please review the recipe config for {1}", value.Item, model.ModelType)), ConsoleColor.Red);
                    throw;
                }
            }
            return products;
        }

        private IDynamicValue GetCraftTime(IConfigurableRecipe recipe)
        {
            var dModel = LoadedDefaultRecipes[recipe.GetType().Name];
            // check if config override
            var loaded = LoadedConfigRecipes.TryGetValue(recipe.GetType().Name, out RecipeModel model);
            if (loaded && (model.BaseCraftTime != dModel.BaseCraftTime || model.CraftTimeIsStatic != dModel.CraftTimeIsStatic))
                return CreateTimeValue(model, dModel);

            // check if mod override
            loaded = ModRecipeOverrides.TryGetValue(recipe.GetType().Name, out model);
            if (loaded)
                return CreateTimeValue(model, dModel);

            // return default
            return CreateDefaultTimeValue(dModel);
        }

        private Type GetConfigStation(IConfigurableRecipe recipe)
        {
            try
            {
                var dModel = LoadedDefaultRecipes[recipe.GetType().Name];

                // check if config override
                var loaded = LoadedConfigRecipes.TryGetValue(recipe.GetType().Name, out RecipeModel model);

                if (loaded && !model.EnableRecipe)
                    return string.Empty.GetType();

                if (loaded && model.CraftingStation != dModel.CraftingStation)
                    return Item.GetCreatedObj(Item.GetType(model.CraftingStation));

                // check if mod override
                loaded = ModRecipeOverrides.TryGetValue(recipe.GetType().Name, out model);

                if (loaded && !model.EnableRecipe)
                    return string.Empty.GetType();

                if (loaded)
                    return Item.GetCreatedObj(Item.GetType(model.CraftingStation));

                // return default
                return Item.GetCreatedObj(Item.GetType(dModel.CraftingStation));
            }
            catch
            {
                ConsoleColors.PrintConsoleMultiColored("[EM Framework] ", ConsoleColor.Magenta, Localizer.DoStr(string.Format("{0} had an invalid crafting station lookup. Check and make sure the CREATING ITEM and not OBJECT is referenced.", recipe.GetType().Name)), ConsoleColor.Red);
                throw;
            }

        }

        private IDynamicValue GetLaborValue(IConfigurableRecipe recipe)
        {
            var dModel = LoadedDefaultRecipes[recipe.GetType().Name];

            // check if config override
            var loaded = LoadedConfigRecipes.TryGetValue(recipe.GetType().Name, out RecipeModel model);
            if (loaded && (model.BaseLabor != dModel.BaseLabor || model.LaborIsStatic != dModel.LaborIsStatic))
                return CreateLaborValue(model, dModel);

            // check if mod override
            loaded = ModRecipeOverrides.TryGetValue(recipe.GetType().Name, out model);
            if (loaded)
                return CreateLaborValue(model, dModel);

            // return default
            return CreateDefaultLaborValue(dModel);
        }
        #endregion

        #region Value Creators
        private static IDynamicValue CreateLaborValue(RecipeModel model, RecipeDefaultModel def)
        {
            bool isStatic = model.LaborIsStatic || def.RequiredSkillType == null;

            return isStatic
                ? CreateLaborInCaloriesValue(model.BaseLabor)
                : CreateLaborInCaloriesValue(model.BaseLabor, def.RequiredSkillType);
        }

        private static IDynamicValue CreateDefaultLaborValue(RecipeDefaultModel def)
        {
            bool isStatic = def.LaborIsStatic || def.RequiredSkillType == null;
            return isStatic
                ? CreateLaborInCaloriesValue(def.BaseLabor)
                : CreateLaborInCaloriesValue(def.BaseLabor, def.RequiredSkillType);
        }

        private static IDynamicValue CreateTimeValue(RecipeModel model, RecipeDefaultModel def)
        {
            Type ModelType = Type.GetType(model.Assembly);
            bool isStatic = model.CraftTimeIsStatic || def.RequiredSkillType == null;
            return isStatic
                ? CreateCraftTimeValue(model.BaseCraftTime)
                : CreateCraftTimeValue(ModelType, model.BaseCraftTime, def.RequiredSkillType, def.SpeedImprovementTalents);
        }

        private static IDynamicValue CreateDefaultTimeValue(RecipeDefaultModel def)
        {
            Type ModelType = Type.GetType(def.Assembly);
            bool isStatic = def.CraftTimeIsStatic || def.RequiredSkillType == null;
            return isStatic
                ? CreateCraftTimeValue(def.BaseCraftTime)
                : CreateCraftTimeValue(ModelType, def.BaseCraftTime, def.RequiredSkillType, def.SpeedImprovementTalents);
        }


        // SLG code for creating IDynamicValues of RecipeFamilies
        private static IDynamicValue CreateCraftTimeValue(float start) => new ConstantValue(start * RecipeFamily.CraftTimeModifier);
        private static IDynamicValue CreateCraftTimeValue(Type beneficiary, float start, Type skillType, params Type[] talents)
        {
            var smv = new ModuleModifiedValue(start * RecipeFamily.CraftTimeModifier, skillType, DynamicValueType.Speed);
            return talents != null
                ? new MultiDynamicValue(MultiDynamicOps.Multiply, talents.Select(x => new TalentModifiedValue(beneficiary, x) as IDynamicValue).Concat(new[] { smv }).ToArray())
                : (IDynamicValue)smv;
        }

        private static IDynamicValue CreateLaborInCaloriesValue(float start) => new ConstantValue(start);
        private static IDynamicValue CreateLaborInCaloriesValue(float start, Type skillType)
        {
            var strategy = (ModificationStrategy)skillType.GetField("MultiplicativeStrategy")!.GetValue(null);
            return new SkillModifiedValue(start, strategy, skillType, Localizer.DoStr("calories of labor"), DynamicValueType.LaborEfficiency);
        }
        #endregion

        // Load
        public void Initialize()
        {
            ConsoleColors.PrintConsoleMultiColored("[EM Framework] ", ConsoleColor.Magenta, Localizer.DoStr("Checking for Recipe Overrides from the Config..."), ConsoleColor.Yellow);
            LoadConfigOverrides();
            ConsoleColors.PrintConsoleMultiColored("[EM Framework] ", ConsoleColor.Magenta, Localizer.DoStr("Checking for Recipe Overrides from Mods..."), ConsoleColor.Yellow);
            InitModOverrides();
        }

        // Load overrides from config changes.
        private void LoadConfigOverrides()
        {
            SerializedSynchronizedCollection<RecipeModel> newModels = new();
            var previousModels = EMConfigurePlugin.Config.EMRecipes;

            foreach (var type in typeof(IConfigurableRecipe).ConcreteTypes())
            {
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            }

            var loadtypes = LoadedDefaultRecipes.Values.ToList();
            // for each type that exists that we are trying to load
            foreach (var dModel in loadtypes)
            {
                var m = previousModels.SingleOrDefault(x => x.ModelType == dModel.ModelType);
                if (m != null && EMConfigurePlugin.Config.useConfigOverrides && !m.Equals(dModel))
                {
                    newModels.Add(m);
#if DEBUG
                    ConsoleColors.PrintConsoleMultiColored("[EM Framework] ", ConsoleColor.Magenta, Localizer.DoStr($"Loaded Config Override For: {m.ModelType}"), ConsoleColor.Yellow);
#endif
                }
                else
                    newModels.Add(dModel);
            }
            EMConfigurePlugin.Config.EMRecipes = newModels;

            foreach (var model in EMConfigurePlugin.Config.EMRecipes)
            {
                if (!LoadedConfigRecipes.ContainsKey(model.ModelType))
                {
                    LoadedConfigRecipes.Add(model.ModelType, model);
                }
            }
        }

        // Load overrides from other mods.
        private void InitModOverrides()
        {
            foreach (var type in typeof(IRecipeOverride).ConcreteTypes())
            {
                try
                {
                    IRecipeOverride t = Activator.CreateInstance(type) as IRecipeOverride;
                    AddRecipeOverride(t.OverrideType, t.Model);
                    ConsoleColors.PrintConsoleMultiColored("[EM Framework] ", ConsoleColor.Magenta, Localizer.DoStr($"Loaded Mod Override For: {t.Model.ModelType}"), ConsoleColor.Yellow);
                }
                catch (Exception e)
                {
#if DEBUG
                    Log.WriteErrorLine(Localizer.DoStr(string.Format("Unable to add override recipe {0}. Invalid implementation for IRecipeOverride.", type.Name)));
                    Log.WriteErrorLine(Localizer.DoStr($"{e.Message}"));
#endif
                }
            }

            /*
            var bindings = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            var tagMan = typeof(GeneratedRegistrarWrapper<TagManager>);
            var whenRdy = (WhenReady)tagMan.GetField("whenSetupDone", bindings).GetValue(tagMan);
            whenRdy.Do(() =>
            {
                foreach (var type in typeof(IRecipeOverride).ConcreteTypes())
                    AddRecipeOverride((string)type.GetProperty("OverrideType").GetValue(type), (RecipeModel)type.GetProperty("Model").GetValue(type));
            });
            */
        }

        private void AddRecipeOverride(string recipeType, RecipeModel r)
        {
            if (!ModRecipeOverrides.ContainsKey(recipeType))
            {
                ModRecipeOverrides.Add(recipeType, r);
            }
        }
    }
}

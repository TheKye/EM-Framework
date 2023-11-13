using Eco.Shared.IoC;
using Eco.Core.Utils;
using Eco.Gameplay.Components;
using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.Shared.Serialization;
using Eco.Simulation.Settings;
using System;
using System.Collections.Generic;
using Eco.Shared.Localization;
using Eco.Shared.Utils;
using Eco.Gameplay.Components.Storage;
using Eco.Gameplay.Items.Recipes;

// This mod is created by Elixr Mods for Eco under the SLG TOS. 
// Please feel free to join our community Discord which aims to brings together modders of Eco to share knowledge, 
// collaborate on projects and improve the overall experience for Eco modders.
// https://discord.gg/69UQPD2HBR

namespace Eco.EM.Framework.Components
{
    // Passively generate set list of products over time.
    // Optionally require a list of ingredients to be available and consumed each "passive craft"
    [Serialized]
    [Priority(-2)]
    [RequireComponent(typeof(PublicStorageComponent))]
    [RequireComponent(typeof(StatusComponent))]
    public class PassiveCraftingComponent : WorldObjectComponent
    {
        // a status element will allow this component to describe it's enabled/disabled conditions to the user of the world object
        private StatusElement status;
        private string failString = "";

        // Allow callbacks when passive craft fires
        public Action OutputCrafted;

        // craft time tracking
        private double passiveCraftTime;
        private double timeEnabledSinceLastCraft = 0;

        // crafting checks and output
        private List<(Item, float)> Outputs = new();

        // this component is only Enabled if all its conditions are met
        public override bool Enabled => CheckConditions();
        private readonly List<IPassiveCraftCondition> Conditions = new();

        public PassiveCraftingComponent() { }

        // Allows initialization based on an existing recipe, it will uses the base values of the recipe and be uneffected by the recipes "Skills & Talents"
        public void Initialize(double passiveCraftTime, Recipe recipe)
        {
            List<(Item, float)> prods = new();
            foreach (var e in recipe.Products) { prods.Add((e.Item, e.Quantity.GetBaseValue)); };

            List<(Item, float)> ings = new();
            foreach (var e in recipe.Ingredients) { ings.Add((e.Item, e.Quantity.GetBaseValue)); };

            Initialize(passiveCraftTime, prods, ings);
        }

        // Initializes the products and potential ingredient requirements of the passive crafting
        public void Initialize(double passiveCraftTime, List<(Item, float)> products, List<(Item, float)> ingredients = null)
        {
            base.Initialize();
            this.Outputs = products;
            this.passiveCraftTime = passiveCraftTime;

            if (ingredients != null)
                Conditions.Add(new IngredientCraftCondition(this, ingredients));

            this.status = this.Parent.GetComponent<StatusComponent>().CreateStatusElement();
            this.Parent.GetComponent<PublicStorageComponent>().Inventory.AddInvRestriction(new SpecificItemTypesRestriction(Outputs.ConvertAll<Type>(x => x.Item1.GetType()).ToArray()));
        }

        public override void Tick() => this.Tick(ServiceHolder<IWorldObjectManager>.Obj.TickDeltaTime * EcoDef.Obj.TimeMult);

        private void Tick(float deltaTime)
        {
            Result result = Result.Succeeded;
            // checks if the parent is enabled (includes this component) and will only continue to passively craft if so
            if (Parent.Owners == null)
            {
                status.SetStatusMessage(false, Localizer.DoStr("This station is unowned and unable to passively produce"));
                return;
            }

            if (Parent.Enabled)
            {
                if (timeEnabledSinceLastCraft < passiveCraftTime)
                    timeEnabledSinceLastCraft += deltaTime;
                else
                {
                    result = Craft();
                }
            }
            else
                result = Result.FailedNoMessage;

            status.SetStatusMessage(result, Localizer.DoStr("Passively Crafting"), Localizer.DoStr(failString == "" ? "Unable to craft while inventory is full" : failString));
        }

        // loop through all conditions registered to see if crafting should continue;
        private bool CheckConditions()
        {
            bool allowCraft = true;
            failString = "";

            foreach (var condition in Conditions)
            {
                if (!condition.AllowCraft())
                {
                    if (allowCraft) allowCraft = false;
                    failString += condition.FailString;
                }
            }
            return allowCraft;
        }

        // Craft the outputs and fire callbacks virtual to allow extension behaviour
        protected virtual Result Craft() 
        {
            var inv = Parent.GetComponent<PublicStorageComponent>().Inventory;
            var changes = InventoryChangeSet.New(inv);

            foreach (var e in Outputs) { changes.AddItems(e.Item1.GetType(), (int)Math.Ceiling(e.Item2)); }
            var result = changes.CanApplyNonDisposing();

            if (result)
            {
                try
                {
                    OutputCrafted?.Invoke();
                    changes.Apply();                  
                }
                catch (InvalidOperationException e)
                {
                    Log.WriteLine(Localizer.DoStr(e.Message));
                }
                
                timeEnabledSinceLastCraft -= passiveCraftTime;
            }

            return result;
        }

        // Adds a crafting condition to the passive generation, no Outputs will be created if all conditions are not met
        public void AddCraftCondition(IPassiveCraftCondition condition) => Conditions.Add(condition);
    }
}


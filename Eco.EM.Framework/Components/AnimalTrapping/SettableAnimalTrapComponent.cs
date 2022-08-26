using Eco.Core.Utils;
using Eco.Gameplay.Components;
using Eco.Gameplay.Interactions;
using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.Shared.Localization;
using Eco.Shared.Math;
using Eco.Shared.Serialization;
using Eco.Shared.Utils;

namespace Eco.EM.Framework.Components
{
    // Animal Trap that requires no bait for operation but needs to be cleared on each catch.
    [Serialized]
    public class SettableAnimalTrapComponent : BaseAnimalTrapComponent, IInteractable
    {
        protected bool enabled;
        public override bool Enabled => enabled;
        [Serialized]  public bool TrapSet { get; set; } = false;
        public override LocString FailStatusMessage => Localizer.DoStr($"Trap is currently not set.");

        protected override void UpdateEnabled()
        {
            enabled = TrapSet;
            base.UpdateEnabled();
        }

        protected override void InteractWithLayers()
        {
            if (!Enabled) return;
            foreach (var layer in animalLayers)
            {
                var layerPos = layer.WorldPosToLayerPos(Vector3Extensions.XZi(Parent.Position));
                var layerValue = layer.SafeEntry(layerPos);

                // Chance of catch is the equal to the % of Animals in layer area compared to their maximum, reduced by a factor of 2
                // Since layers update every 15 minutes or so a layer at full capcity should hit approximately every half hour.
                if (RandomUtil.Chance(layerValue / (layer.Settings.RenderRange.Max * 1)))
                {   
                    var organisms = layer.SafePopMapEntry(layerPos);
                    if (organisms == null) return;
                    var animal = organisms.Random();

                    // give items
                    var changeSet = InventoryChangeSet.New(storage.Inventory);
                    OrganismItemManager.AddRandomResourcesToChangeSet(ref changeSet, animal.Species);
                    if (changeSet.TryApply().Success) 
                    { 
                        animal.Kill(); 
                        TrapSet = false;
                        UpdateTrap();
                    }
                    return;
                }
            }
        }

        public virtual InteractResult OnActInteract(InteractionContext context) => InteractResult.NoOp;
        public virtual InteractResult OnActLeft(InteractionContext context) => InteractResult.NoOp;
        public virtual InteractResult OnActRight(InteractionContext context)
        {
            if (Enabled) return InteractResult.FailureLoc($"This Trap is already Set.");
            
            if (!storage.Inventory.IsEmpty)
            {
                var result = storage.Inventory.MoveAsManyItemsAsPossible(context.Player.User.Inventory,null,null);
                if (result.Val < 1) return InteractResult.FailureLoc($"Unable to clear and set trap, please make room in your inventory"); 
            }

            TrapSet = true;
            UpdateTrap();
            return InteractResult.SuccessLoc($"Trap set");
        }

        public virtual InteractResult OnPreInteractionPass(InteractionContext context) => InteractResult.NoOp;
    }
}

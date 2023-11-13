using Eco.Core.Utils;
using Eco.EM.Framework.Text;
using Eco.Gameplay.Components;
using Eco.Gameplay.Interactions;
using Eco.Gameplay.Interactions.Interactors;
using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Players;
using Eco.Shared.Localization;
using Eco.Shared.Math;
using Eco.Shared.Serialization;
using Eco.Shared.SharedTypes;
using Eco.Shared.Utils;

namespace Eco.EM.Framework.Components
{
    // Animal Trap that requires no bait for operation but needs to be cleared on each catch.
    [Serialized]
    public class SettableAnimalTrapComponent : BaseAnimalTrapComponent
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

        [Interaction(InteractionTrigger.RightClick, "Set Trap")]
        public void SetTrap(Player context, InteractionTarget target, InteractionTriggerInfo info)
        {
            if (Enabled) return;

            if (!storage.Inventory.IsEmpty)
            {
                var result = storage.Inventory.MoveAsManyItemsAsPossible(context.User.Inventory, null, null);
                if (result.Val < 1)
                {

                    context.ErrorLocStr($"Unable to clear and set trap, please make room in your inventory");
                    return;
                }
            }

            context.User.Inventory.RemoveItem(context.User.ToolbarSelected.Item.Type);
            TrapSet = true;
            UpdateTrap();
            context.MsgLocStr($"Trap set".Green());
        }
    }
}

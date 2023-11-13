using Eco.EM.Framework.Text;
using Eco.Gameplay.Interactions;
using Eco.Gameplay.Interactions.Interactors;
using Eco.Gameplay.Items;
using Eco.Gameplay.Players;
using Eco.Shared.Networking;
using Eco.Shared.Serialization;
using Eco.Shared.SharedTypes;
using System.Collections.Generic;

namespace Eco.EM.Framework.Components
{
    // Animal Trap that requires no bait for operation but needs to be cleared on each catch.
    [Serialized]
    public class BaitableAnimalTrapComponent : SettableAnimalTrapComponent
    {
        private readonly HashSet<Item> baitTypes = new();

        public void AddBait(Item item) { baitTypes.Add(item); }

        [Interaction(InteractionTrigger.RightClick, "Set Trap")]
        public void SetTrap(Player context, InteractionTarget target, InteractionTriggerInfo info)
        {
            if (Enabled) return;
            
            if (!storage.Inventory.IsEmpty)
            {
                var result = storage.Inventory.MoveAsManyItemsAsPossible(context.User.Inventory,null,null);
                if (result.Val < 1)
                {

                    context.ErrorLocStr($"Unable to clear and set trap, please make room in your inventory");
                    return;
                }
            }

            if (context.User.ToolbarSelected.Item == null || !baitTypes.Contains(context.User.ToolbarSelected.Item))
            {
                context.ErrorLocStr($"You require the correct bait in your hand to reset this trap");
                return;
            }

            context.User.Inventory.RemoveItem(context.User.ToolbarSelected.Item.Type);
            TrapSet = true;
            UpdateTrap();
            context.MsgLocStr($"Trap set".Green());
        }
    }
}

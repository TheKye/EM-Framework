using Eco.Gameplay.Interactions;
using Eco.Gameplay.Items;
using Eco.Shared.Serialization;
using System.Collections.Generic;

namespace Eco.EM.Framework.Components
{
    // Animal Trap that requires no bait for operation but needs to be cleared on each catch.
    [Serialized]
    public class BaitableAnimalTrapComponent : SettableAnimalTrapComponent, IInteractable
    {
        private readonly HashSet<Item> baitTypes = new();

        public void AddBait(Item item) { baitTypes.Add(item); }

        public override InteractResult OnActRight(InteractionContext context)
        {
            if (Enabled) return InteractResult.NoOp;
            
            if (!storage.Inventory.IsEmpty)
            {
                var result = storage.Inventory.MoveAsManyItemsAsPossible(context.Player.User.Inventory,null,null);
                if (result.Val < 1) return InteractResult.FailureLoc($"Unable to clear and set trap, please make room in your inventory"); 
            }

            if (context.SelectedItem == null || !baitTypes.Contains(context.SelectedItem))
            {
                return InteractResult.FailureLoc($"You require the correct bait in your hand to reset this trap");
            }

            context.Player.User.Inventory.RemoveItem(context.SelectedItem.Type);
            TrapSet = true;
            UpdateTrap();
            return InteractResult.SuccessLoc($"Trap set");
        }
    }
}

using Eco.Gameplay.Items;
using Eco.Mods.TechTree;
using Eco.Shared.Localization;
using Eco.World.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco.EM.Framework.Utils
{
    // Lets you multiply the Max Stack size by a number to be able to hold more for all items at once instead of making all stack to a specific stack size
    public class StackAllLimitRestriction : InventoryRestriction
    {
        public override bool SurpassStackSize => true;
        public virtual float MaxItems { get; set; } = 1;

        public override LocString Message => Localizer.DoStr("Not enough room in inventory.");
        public virtual bool Enabled => this.MaxItems > 0;

        public StackAllLimitRestriction(float multiplier)
        {
            this.MaxItems = multiplier;
        }

        public override int MaxAccepted(Item item, int currentQuantity) => (int)(item.MaxStackSize * this.MaxItems);
    }

    // Limits stack sizes to a different quantity other than item.maxstacksize, while restricting Tools and Tailings/Garbage.
    public class CustomStackLimitRestriction : InventoryRestriction
    {
        public override bool SurpassStackSize => true;
        public virtual int MaxItems { get; protected set; }

        public override LocString Message => Localizer.DoStr("Not able to store in this Storage.");
        public virtual bool Enabled => this.MaxItems > 0;

        public CustomStackLimitRestriction(int maxItems) => this.MaxItems = maxItems;

        public override int MaxAccepted(Item item, int currentQuantity)
        {
            if (item is ToolItem)
            {
                return item.MaxStackSize;
            }
            else if (item is TailingsItem || item is WetTailingsItem || item is GarbageItem)
            {
                return 0;
            }
            else
            {
                return this.MaxItems;
            }
        }
    }
}

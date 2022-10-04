using Eco.Gameplay.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco.EM.Framework.Extentsions.Items
{
    public static class @Inventory
    {
#nullable enable    
        public static Gameplay.Items.Item? FindInInventory(this Gameplay.Items.Inventory inventory, Gameplay.Items.Item searchItem)
        {
            var query = inventory.AllInventories
                .AllStacks()
                .Where(stack => stack.Item.ItemsEqual(searchItem));
            return query.Any() ? query.First().Item : null;
        }
#nullable disable
    }
}

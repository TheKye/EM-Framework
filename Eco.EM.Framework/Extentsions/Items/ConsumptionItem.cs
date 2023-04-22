using Eco.Gameplay.Items;
using Eco.Gameplay.Players;
using Eco.Gameplay.Systems.TextLinks;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco.EM.Framework.Extentsions.Items
{
    [Serialized]
    public partial class ConsumptionItem : DurabilityItem
    {
        public virtual float Durability { get; set; } = 100f;
        public override float GetDurability() => Durability;
        public virtual float UseDurability(float multiplier, Player player)
        {
            if (!this.Broken)
            {
                this.Durability = Math.Max(0, this.Durability - (this.DurabilityRate * multiplier));
                if (this.Durability == 0) player.ErrorLocStr(Localizer.DoStr(string.Format($"Your {0} broke!  It will be much less efficient until repaired.", this.UILink())));
            }

            return this.DurabilityMultiplier;
        }
    }
}

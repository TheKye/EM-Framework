using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco.EM.Framework.Models
{
    public struct OfferedItem
    {
        public int Quantity;
        public float Price;
        public string tagItemName;
        public string Currency;
        public string StoreName;
        public string StoreOwner;
        public bool ForSale;
    }
}

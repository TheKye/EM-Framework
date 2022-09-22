using Eco.Gameplay.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco.EM.Framework.Extentsions.Items
{
    public static partial class @Item
    {
#nullable enable
        public static bool ItemsEqual(this Gameplay.Items.Item x, Gameplay.Items.Item y) => x.DisplayName.NotTranslated.Equals(y.DisplayName.NotTranslated);
#nullable disable
    }
}

using Eco.Gameplay.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco.EM.Framework.Extentsions.Items
{
    public partial class NPhysicsWorldObject : PhysicsWorldObject
    {
        public virtual Type BaseType { get; set; }
    }

    public static class VehicleExtentsions
    {
        public static Type GetBaseType(this NPhysicsWorldObject npWorldObject)
        {
            return npWorldObject.BaseType;
        }
    }
}

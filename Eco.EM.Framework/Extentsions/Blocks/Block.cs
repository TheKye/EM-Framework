using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eco.World.Blocks;
using System.Threading.Tasks;

namespace Eco.EM.Framework.Extentsions
{
    public partial class NBlock : Block
    {
        public virtual Type BaseType { get; set; }
    }

    public static class BlockExtentsions
    {
        public static Type GetBaseType(this NBlock block)
        {
            return block.BaseType;
        }

    }
}

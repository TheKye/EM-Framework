using Eco.Gameplay.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco.EM.Framework.Utils
{
    public static class ItemUtil
    {
        /// <summary>
        /// Use TryGet instead of Get to protect against Exception without having to implement try catch
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static Item TryGet(string item)
        {
            try 
            {
                return Item.Get(item);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}

using Eco.Core.Utils;
using Eco.Gameplay.Items;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Eco.EM.Framework.Resolvers
{
    public class EMItemWeightResolver
    {
        public static void Initialize()
        {
            IEnumerable<Item> locals;

            locals = Item.AllItems.Where(x => x.Category != "Hidden" && ItemAttribute.Has<WeightAttribute>(x.Type) && x.DisplayName != "Hands");
            locals = locals.OrderBy(x => x.DisplayName);

            BuildStackSizeList(locals);
            OverrideStackSizes(locals);
        }

        // Goes through and loads new items for stack sizes into the dictionary.
        private static void BuildStackSizeList(IEnumerable<Item> locals)
        {
            var config = EMItemWeightPlugin.Config.EMItemWeight;
            // Go through and keep items that are still referenced in the namespace
            SerializedSynchronizedCollection<ItemWeightModel> cleanList = new SerializedSynchronizedCollection<ItemWeightModel>();
            for (int i = 0; i < config.Count; i++)
            {
                if (locals.Any(x => x.DisplayName == config[i].DisplayName))
                {
                    if (!cleanList.Any(x => x.DisplayName == config[i].DisplayName))
                        cleanList.Add(config[i]);
                }
            }

            // Now add anything that is new
            foreach (var i in locals)
            {
                if (!cleanList.Any(x => x.DisplayName == i.DisplayName))
                    cleanList.Add(new ItemWeightModel(i.GetType(), i.DisplayName, i.Weight, false));
            }

            EMItemWeightPlugin.Config.EMItemWeight = cleanList;
        }

        // Overrides the preset stacksizes to those set in the config on load before adding newly created items
        private static void OverrideStackSizes(IEnumerable<Item> locals)
        {
            foreach (var i in locals)
            {
                // Check for the items in the stack size list
                var element = EMItemWeightPlugin.Config.EMItemWeight.SingleOrDefault(x => x.DisplayName == i.DisplayName);
                if (element == null) continue;
                var orThis = element.OverrideThis;
                // Get the stacksize attribute and override it.
                var wa = ItemAttribute.Get<WeightAttribute>(i.Type);

                // currently we only change items that have a MaxStackSizeAttribute
                if (wa != null && orThis)
                    wa.GetType().GetProperty("Weight", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).SetValue(wa, (int)element.ItemWeight);
            }
        }
    }
}

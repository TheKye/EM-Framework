using Eco.Core.Utils;
using Eco.Gameplay.Items;
using Eco.Shared.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Eco.EM.Framework.Resolvers
{
    public class EMStackSizeResolver : AutoSingleton<EMStackSizeResolver>
    {
        public static void Initialize()
        {
            IEnumerable<Item> locals;

            locals = Item.AllItems.Where(x => x.Category != "Hidden" && ItemAttribute.Has<MaxStackSizeAttribute>(x.Type) && !ItemAttribute.Has<IgnoreStackSizeAttribute>(x.Type) && x.DisplayName != "Hands");
            locals = locals.OrderBy(x => x.DisplayName);
            MaxStackSizeAttribute.Default = EMConfigurePlugin.Config.ForceSameStackSizes ? EMConfigurePlugin.Config.ForcedSameStackAmount : EMConfigurePlugin.Config.DefaultMaxStackSize;
            BuildStackSizeList(locals);
            OverrideStackSizes(locals);
        }

        // Goes through and loads new items for stack sizes into the dictionary.
        private static void BuildStackSizeList(IEnumerable<Item> locals)
        {
            var config = EMConfigurePlugin.Config.EMStackSizes;
            // Go through and keep items that are still referenced in the namespace
            SerializedSynchronizedCollection<StackSizeModel> cleanList = new SerializedSynchronizedCollection<StackSizeModel>();
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
                    cleanList.Add(new StackSizeModel(i.GetType(), i.DisplayName, i.MaxStackSize, false));
            }

            EMConfigurePlugin.Config.EMStackSizes = cleanList;
        }

        // Overrides the preset stacksizes to those set in the config on load before adding newly created items
        private static void OverrideStackSizes(IEnumerable<Item> locals)
        {
            foreach (var i in locals)
            {
                // Check for the items in the stack size list
                var element = EMConfigurePlugin.Config.EMStackSizes.SingleOrDefault(x => x.DisplayName == i.DisplayName);
                if (element == null) continue;
                var orThis = element.OverrideThis;
                var forced = EMConfigurePlugin.Config.ForceSameStackSizes;
                var bforced = EMConfigurePlugin.Config.CarriedItemsOverride;
                // Get the stacksize attribute and override it.
                var mss = ItemAttribute.Get<MaxStackSizeAttribute>(i.Type);

                // If using Carried Items Override get the attribute
                var bmss = ItemAttribute.Get<CarriedAttribute>(i.Type);

                // currently we only change items that have a MaxStackSizeAttribute
                switch (forced)
                {
                    case false:
                        if (mss != null && orThis)
                            mss.GetType().GetProperty("MaxStackSize", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).SetValue(mss, element.StackSize);
                        else if (mss != null && bmss != null && bforced)
                            mss.GetType().GetProperty("MaxStackSize", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).SetValue(mss, EMConfigurePlugin.Config.CarriedItemsAmount);
                        break;

                    case true:
                        if (mss != null)
                            mss.GetType().GetProperty("MaxStackSize", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).SetValue(mss, EMConfigurePlugin.Config.ForcedSameStackAmount);
                        break;
                }
            }
        }
    }
}
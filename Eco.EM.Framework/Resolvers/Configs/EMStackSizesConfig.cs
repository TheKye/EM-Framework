using Eco.Core.Utils;
using Eco.Shared.Localization;

namespace Eco.EM.Framework.Resolvers
{
    public class EMStackSizesConfig
    {
        [LocDisplayName("Default Max Stack Size")]
        [LocDescription("Set the default max stack size of those that don't have the max stack size attribute (Default is 100)")]
        public int DefaultMaxStackSize { get; set; } = 100;

        [LocDisplayName("Force All Items to have the same Stack Size")]
        [LocDescription("Force all items to share the same stack size, this should work for almost any mod and all vanilla items as well")]
        public bool ForceSameStackSizes { get; set; }

        [LocDisplayName("Forced Stack Size Amount")]
        [LocDescription("Set the stack size amount")]
        public int ForcedSameStackAmount { get; set; } = 100;

        [LocDisplayName("Change Carried Items Stack Size Amount")]
        [LocDescription("Enable the setting of Carried Items Stack Sizes only - Not to be used with forced as it will override forced stack sizes and use the below value instead.")]
        public bool CarriedItemsOverride { get; set; }

        [LocDisplayName("Forced Stack Size Amount for carried items")]
        [LocDescription("Carried Items Stack size amount, Forced Same Stack amount will override this value")]
        public int CarriedItemsAmount { get; set; } = 20;

        [LocDescription("Stack sizes by modules. ANY change to this list will require a server restart to take effect.")]
        [LocDisplayName("Stack Sizes")]
        public SerializedSynchronizedCollection<StackSizeModel> EMStackSizes { get; set; } = new();
    }
}

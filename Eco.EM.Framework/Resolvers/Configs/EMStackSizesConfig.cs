using Eco.Core.Utils;
using Eco.Shared.Localization;

namespace Eco.EM.Framework.Resolvers
{
    public class EMStackSizesConfig
    {
        [LocDescription("Stack sizes by modules. ANY change to this list will require a server restart to take effect.")]
        [LocDisplayName("Stack Sizes")]
        public SerializedSynchronizedCollection<StackSizeModel> EMStackSizes { get; set; } = new();
    }
}

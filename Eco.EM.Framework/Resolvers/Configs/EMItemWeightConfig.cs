using Eco.Core.Utils;
using Eco.Shared.Localization;

namespace Eco.EM.Framework.Resolvers
{
    public class EMItemWeightConfig
    {
        [LocDescription("Item weight by modules. ANY change to this list will require a server restart to take effect.")]
        [LocDisplayName("Item Weight")]
        public SerializedSynchronizedCollection<ItemWeightModel> EMItemWeight { get; set; } = new();
    }
}

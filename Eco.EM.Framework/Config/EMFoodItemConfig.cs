using Eco.Core.Utils;
using Eco.Shared.Localization;

namespace Eco.EM.Framework.Resolvers
{
    public class EMFoodItemConfig
    {
        [LocDescription("Food Item Values by modules. ANY change to this list will require a server restart to take effect.")]
        [LocDisplayName("Food Items")]
        public SerializedSynchronizedCollection<FoodItemModel> EMFoodItem { get; set; } = new();
    }
}

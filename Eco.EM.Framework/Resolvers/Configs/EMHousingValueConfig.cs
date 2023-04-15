using Eco.Core.Utils;
using Eco.Shared.Localization;

namespace Eco.EM.Framework.Resolvers
{
    public class EMHousingValueConfig
    {
        [LocDescription("Home Furnishing Values by modules. ANY change to this list will require a server restart to take effect.")]
        [LocDisplayName("Home Furnishing Values")]
        public SerializedSynchronizedCollection<HousingModel> EMHousingValue { get; set; } = new();
    }
}

using Eco.Core.Utils;
using Eco.Shared.Localization;

namespace Eco.EM.Framework.Resolvers
{
    public class EMVehiclesConfig
    {
        [LocDescription("Vehicle Configuration Values by modules. ANY change to this list will require a server restart to take effect.")]
        [LocDisplayName("Vehicle Configuration Values")]
        public SerializedSynchronizedCollection<VehicleModel> EMVehicles { get; set; } = new();
    }
}

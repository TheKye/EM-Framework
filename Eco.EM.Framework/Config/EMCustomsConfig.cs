using Eco.Core.Utils;
using Eco.Shared.Localization;

namespace Eco.EM.Framework.Resolvers
{
    public class EMCustomsConfig
    {
        [LocDescription("Custom Configuration Values by modules. ANY change to this list will require a server restart to take effect.")]
        [LocDisplayName("Custom Configuration Values")]
        public SerializedSynchronizedCollection<CustomsModel> EMCustoms { get; set; } = new();
    }
}
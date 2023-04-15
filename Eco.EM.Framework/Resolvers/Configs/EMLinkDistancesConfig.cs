using Eco.Core.Utils;
using Eco.Shared.Localization;

namespace Eco.EM.Framework.Resolvers
{
    public class EMLinkDistancesConfig
    {
        [LocDescription("EM Link radius loaded by modules. ANY change to this list will require a server restart to take effect.")]
        [LocDisplayName("Link Distances")]
        public SerializedSynchronizedCollection<LinkModel> EMLinkDistances { get; set; } = new();
    }
}

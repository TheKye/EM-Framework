using Eco.Core.Utils;
using Eco.Shared.Localization;

namespace Eco.EM.Framework.Resolvers
{
    public class EMStorageSlotsConfig
    {
        [LocDescription("Storage slots by modules. ANY change to this list will require a server restart to take effect.")]
        [LocDisplayName("Storage Slots")]
        public SerializedSynchronizedCollection<StorageSlotModel> EMStorageSlots { get; set; } = new();
    }
}

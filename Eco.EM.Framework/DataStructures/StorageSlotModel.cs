using Eco.EM.Framework.Utils;
using Eco.Mods.TechTree;
using Eco.Shared.Localization;
using Newtonsoft.Json;
using System;

namespace Eco.EM.Framework.Resolvers
{
    [LocDisplayName("Storage Slot Model")]
    public class StorageSlotModel : ModelBase
    {
        [LocDisplayName("Storage Slots")] public float StorageSlots { get; set; }
        [LocDisplayName("Storage Stack Multiplier")] public float StackMultiplier { get; set; } = 1;

        public StorageSlotModel(Type type, float storageSlots, float stackMultiplier)
        {
            ModelType = type.Name;
            Assembly = type.AssemblyQualifiedName;
            StorageSlots = storageSlots;
            StackMultiplier = stackMultiplier;
        }

        [JsonConstructor]
        public StorageSlotModel(string modelType, string assembly, float storageSlots, float stackMultiplier)
        {
            ModelType = modelType;
            Assembly = assembly;
            StorageSlots = storageSlots;
            StackMultiplier = stackMultiplier;
        }

        public override string ToString() => $"{StringUtils.GetAssemblyNameFromAssemblyString(Assembly)} - {ModelType}";
    }
}

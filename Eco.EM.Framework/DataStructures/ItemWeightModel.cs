using Eco.EM.Framework.Utils;
using Eco.Shared.Localization;
using Newtonsoft.Json;
using System;

namespace Eco.EM.Framework.Resolvers
{
    public class ItemWeightModel: ModelBase
    {
        [LocDisplayName("Item Display Name")] public string DisplayName { get; set; }
        [LocDisplayName("Item Weight")] public float ItemWeight { get; set; }
        [LocDisplayName("Override This")] public bool OverrideThis { get; set; }

        public ItemWeightModel(Type type, string n, int w, bool or)
        {
            ModelType = type.Name;
            Assembly = type.AssemblyQualifiedName;
            DisplayName = n;
            ItemWeight = w;
            OverrideThis = or;
        }

        [JsonConstructor]
        public ItemWeightModel(string modelType, string assembly, string displayName, float itemWeight, bool or)
        {
            ModelType = modelType;
            Assembly = assembly;
            DisplayName = displayName;
            ItemWeight = itemWeight;
            OverrideThis = or;
        }

        public override string ToString() => $"{StringUtils.GetAssemblyNameFromAssemblyString(Assembly)} - {DisplayName}";
    }
}

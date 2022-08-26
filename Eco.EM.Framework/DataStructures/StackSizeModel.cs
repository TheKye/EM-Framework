using Eco.EM.Framework.Utils;
using Eco.Shared.Localization;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Eco.EM.Framework.Resolvers
{
    public class StackSizeModel: ModelBase
    {
        [LocDisplayName("Item Display Name")] public string DisplayName { get; set; }
        [LocDisplayName("Stacksize")] public int StackSize { get; set; }
        [LocDisplayName("Override This")] public bool OverrideThis { get; set; }

        public StackSizeModel(Type type, string n, int s, bool or)
        {
            ModelType = type.Name;
            Assembly = type.AssemblyQualifiedName;
            DisplayName = n;
            StackSize = s;
            OverrideThis = or;
        }

        [JsonConstructor]
        public StackSizeModel(string modelType, string assembly, string displayModel, int stackSize, bool or)
        {
            ModelType = modelType;
            Assembly = assembly;
            DisplayName = displayModel;
            StackSize = stackSize;
            OverrideThis = or;
        }

        public override string ToString() => $"{StringUtils.GetAssemblyNameFromAssemblyString(Assembly)} - {DisplayName}";
    }
}
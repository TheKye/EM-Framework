using Eco.Shared.Localization;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Eco.EM.Framework.Resolvers
{
    public class CustomsModel : ModelBase
    {
        [LocDisplayName("Customs")]
        public Dictionary<string, object> Customs { get; set; }
        public CustomsModel(Type type, Dictionary<string, object> customs)
        {
            ModelType = type.Name;
            Assembly = type.AssemblyQualifiedName;
            Customs = customs;

        }

        [JsonConstructor]
        public CustomsModel(string modelType, string assembly, Dictionary<string, object> customs)
        {
            ModelType = modelType;
            Assembly = assembly;
            Customs = customs;
        }

        public override string ToString() => $"{Eco.EM.Framework.Utils.StringUtils.GetAssemblyNameFromAssemblyString(Assembly)} - {ModelType}";
    }
}

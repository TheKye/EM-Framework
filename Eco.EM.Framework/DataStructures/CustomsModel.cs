using Eco.Shared.Localization;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Eco.EM.Framework.Resolvers
{
    /// <summary>Stores a configurable dictionary</summary>
    public class CustomsModel : ModelBase
    {
        [LocDisplayName("Customs")]
        public Dictionary<string, object> Customs { get; set; }
        /// <summary>creates a new CustomsModel for use in the EMCustomsResolver.AddDefaults()</summary>
        /// <param name="type">the type of the object that the data is for</param>
        /// <param name="customs">a dictionary holding the configurable data withs keys repersenting the name of the property</param>
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

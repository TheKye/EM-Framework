using Eco.EM.Framework.Utils;
using Eco.Shared.Localization;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Eco.EM.Framework.Resolvers
{
    [LocDisplayName("Link Model")]
    public class LinkModel: ModelBase
    {
        [LocDisplayName("Link Radius")] public float LinkRadius { get; set; }

        public LinkModel(Type type)
        {
            ModelType = type.Name;
            Assembly = type.AssemblyQualifiedName;
        }

        [JsonConstructor]
        public LinkModel(string modelType, string assembly, float linkRadius)
        {
            ModelType = modelType;
            Assembly = assembly;
            LinkRadius = linkRadius;
        }

        public override string ToString() => $"{StringUtils.GetAssemblyNameFromAssemblyString(Assembly)} - {ModelType}";
    }
}

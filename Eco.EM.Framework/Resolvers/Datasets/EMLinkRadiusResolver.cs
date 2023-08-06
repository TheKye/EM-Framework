using Eco.Core.Utils;
using Eco.Shared.Localization;
using Eco.Shared.Utils;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Eco.EM.Framework.Resolvers
{
    public interface ILinkRadiusObject { }

    public class EMLinkRadiusResolver : AutoSingleton<EMLinkRadiusResolver>
    {
        public ConcurrentDictionary<string, LinkModel> DefaultLinkOverrides { get; private set; } = new();
        public ConcurrentDictionary<string, LinkModel> LoadedLinkOverrides { get; private set; } = new();

        public static void AddDefaults(LinkModel defaults)
        {
            Obj.DefaultLinkOverrides.TryAdd(defaults.ModelType, defaults);
        }

        public float ResolveLinkRadius(ILinkRadiusObject obj) => GetRadius(obj);

        private float GetRadius(ILinkRadiusObject obj)
        {
            try
            {
                return LoadedLinkOverrides[obj.GetType().Name].LinkRadius;
            }
            catch
            {
                Log.WriteErrorLine(Localizer.DoStr(string.Format("Unable to load link distance for {0}. Please check config.", obj.GetType().Name)));
                throw;
            }
        }

        public void Initialize()
        {
            SerializedSynchronizedCollection<LinkModel> newModels = new();
            var config = EMLinkDistancesPlugin.Config.EMLinkDistances;

            foreach (var type in typeof(ILinkRadiusObject).ConcreteTypes())
            {
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            }

            var loadtypes = DefaultLinkOverrides.Values.ToList();

            // for each type that exists that we are trying to load
            foreach (var lModel in loadtypes)
            {
                var m = config.SingleOrDefault(x => x.ModelType == lModel.ModelType);
                if (m != null)
                    newModels.Add(m);
                else
                    newModels.Add(lModel);
            }

            EMLinkDistancesPlugin.Config.EMLinkDistances = newModels;

            foreach (var model in newModels)
            {
                if (!LoadedLinkOverrides.ContainsKey(model.ModelType))
                    LoadedLinkOverrides.TryAdd(model.ModelType, model);
            }
        }
    }
}

using Eco.Core.Utils;
using Eco.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Eco.EM.Framework.Resolvers
{
    public interface IConfigurableCustoms { }
    public class EMCustomsResolver : AutoSingleton<EMCustomsResolver>
    {
        public Dictionary<string, CustomsModel> DefaultCustomsOverrides { get; private set; } = new();
        public Dictionary<string, CustomsModel> LoadedCustomsOverrides { get; private set; } = new();
        public static void AddDefaults(CustomsModel defaults)
        {
            Obj.DefaultCustomsOverrides.Add(defaults.ModelType, defaults);
        }
        public static object GetCustom(Type objectType, string key)
        {
            CustomsModel objectModel = Obj.LoadedCustomsOverrides.GetValueOrDefault(objectType.Name);
            if (objectModel == null) return null;
            return objectModel.Customs.GetValueOrDefault(key);
        }

        public void Initialize()
        {
            SerializedSynchronizedCollection<CustomsModel> newModels = new();
            var previousModels = newModels;
            try
            {
                previousModels = EMConfigurePlugin.Config.EMCustoms;
            }
            catch
            {
                previousModels = new();
            }
            foreach (var type in typeof(IConfigurableCustoms).ConcreteTypes())
            {
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            }

            var loadtypes = DefaultCustomsOverrides.Values.ToList();

            // for each type that exists that we are trying to load
            foreach (var lModel in loadtypes)
            {
                var m = previousModels.SingleOrDefault(x => x.ModelType == lModel.ModelType);



                if (m != null) newModels.Add(m);
                else newModels.Add(lModel);
            }

            EMConfigurePlugin.Config.EMCustoms = newModels;

            foreach (var model in newModels)
            {
                if (!LoadedCustomsOverrides.ContainsKey(model.ModelType))
                    LoadedCustomsOverrides.Add(model.ModelType, model);
            }
        }
    }
}

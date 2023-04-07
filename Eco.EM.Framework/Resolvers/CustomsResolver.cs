using Eco.Core.Utils;
using Eco.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Eco.EM.Framework.Resolvers
{
    /// <summary>Used by the EMCustomsResolver to find all classes that implement Customs</summary>
    public interface IConfigurableCustoms { }
    /// <summary>Uses CustomsModels to make a configurable section in the EMConfigure.eco file</summary>
    public class EMCustomsResolver : AutoSingleton<EMCustomsResolver>
    {
        public Dictionary<string, CustomsModel> DefaultCustomsOverrides { get; private set; } = new();
        public Dictionary<string, CustomsModel> LoadedCustomsOverrides { get; private set; } = new();
        /// <summary>Adds a Model to the DefaultCustomsOverrides that can be configured and accesed later on</summary>
        /// <param name="defaults">the default Model to use</param>
        public static void AddDefaults(CustomsModel defaults)
        {
            Obj.DefaultCustomsOverrides.Add(defaults.ModelType, defaults);
        }
        /// <summary>Uses a key to filter the object's data and returns the value that its set to</summary>
        /// <param name="objectType">the type of the object holding the data used to filter though the LoadedCustomsOverrides to get that objects data</param>
        /// <param name="key">the same key used to add the default is used to get the data back</param>
        /// <returns>returns the value of the key in object form which means a cast is needed when using the data as its intended form</returns>
        public static object GetCustom(Type objectType, string key)
        {
            var objectModel = Obj.LoadedCustomsOverrides.GetValueOrDefault(objectType.Name);
            if (objectModel == null) return null;
            return objectModel.Customs.GetValueOrDefault(key);
        }

        public void Initialize()
        {
            SerializedSynchronizedCollection<CustomsModel> newModels = new();
            var previousModels = newModels;
            // gets configs if they exist
            try
            {
                previousModels = EMCustomsPlugin.Config.EMCustoms;
            }
            catch
            {
                previousModels = new();
            }
            // executes the constructors so they can load the defaults
            foreach (var type in typeof(IConfigurableCustoms).ConcreteTypes())
            {
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            }

            var loadtypes = DefaultCustomsOverrides.Values.ToList();
            // reloads the configured customs or the defaults if they do not exist
            foreach (var lModel in loadtypes)
            {
                var m = previousModels.SingleOrDefault(x => x.ModelType == lModel.ModelType);

                if (m != null) newModels.Add(m);
                else newModels.Add(lModel);
            }

            EMCustomsPlugin.Config.EMCustoms = newModels;

            // adds the models to the overrides for use by GetCustom()
            foreach (var model in newModels)
            {
                if (!LoadedCustomsOverrides.ContainsKey(model.ModelType)) 
                    LoadedCustomsOverrides.Add(model.ModelType, model);
            }
        }
    }
}

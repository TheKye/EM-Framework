using Eco.Core.Utils;
using Eco.Shared.Localization;
using Eco.Shared.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Eco.EM.Framework.Resolvers
{
    public interface IStorageSlotObject { }

    public class EMStorageSlotResolver : AutoSingleton<EMStorageSlotResolver>
    {
        private Dictionary<string, StorageSlotModel> DefaultSlotOverrides { get; set; } = new();
        private Dictionary<string, StorageSlotModel> LoadedSlotOverrides { get; set; } = new();

        public static void AddDefaults(StorageSlotModel defaults)
        {
            Obj.DefaultSlotOverrides.Add(defaults.ModelType, defaults);
        }

        public int ResolveSlots(IStorageSlotObject obj) => GetSlots(obj);

        public float ResolveStackMultiplier(IStorageSlotObject obj) => GetMultiplier(obj);

        private float GetMultiplier(IStorageSlotObject obj)
        {
            try
            {
                return LoadedSlotOverrides[obj.GetType().Name].StackMultiplier;
            }
            catch
            {
                Log.WriteErrorLine(Localizer.DoStr(string.Format("Unable to load storage slots for {0}. Please check config.", obj.GetType().Name)));
                throw;
            }
        }

        private int GetSlots(IStorageSlotObject obj)
        {
            try
            {
                return (int)LoadedSlotOverrides[obj.GetType().Name].StorageSlots;
            }
            catch
            {
                Log.WriteErrorLine(Localizer.DoStr(string.Format("Unable to load storage slots for {0}. Please check config.", obj.GetType().Name)));
                throw;
            }
        }

        public void Initialize()
        {
            SerializedSynchronizedCollection<StorageSlotModel> newModels = new();
            var config = EMConfigurePlugin.Config.EMStorageSlots;

            foreach (var type in typeof(IStorageSlotObject).ConcreteTypes())
            {
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            }

            var loadtypes = DefaultSlotOverrides.Values.ToList();

            // for each type that exists that we are trying to load
            foreach (var lModel in loadtypes)
            {
                var m = config.SingleOrDefault(x => x.ModelType == lModel.ModelType);
                if (m != null)
                    newModels.Add(m);
                else
                    newModels.Add(lModel);
            }

            EMConfigurePlugin.Config.EMStorageSlots = newModels;

            foreach (var model in newModels)
            {
                if (!LoadedSlotOverrides.ContainsKey(model.ModelType))
                    LoadedSlotOverrides.Add(model.ModelType, model);
            }
        }
    }
}

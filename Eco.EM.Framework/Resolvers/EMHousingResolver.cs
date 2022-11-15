using Eco.Core.Utils;
using Eco.Gameplay.Housing.PropertyValues;
using Eco.Shared.Localization;
using Eco.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Eco.EM.Framework.Resolvers
{
    public interface IConfigurableHousing { }

    public class EMHousingResolver : AutoSingleton<EMHousingResolver>
    {
        public Dictionary<string, HousingModel> DefaultHomeOverrides { get; private set; } = new();
        public Dictionary<string, HousingModel> LoadedHomeOverrides { get; private set; } = new();

        public HomeFurnishingValue ResolveHomeValue(Type housing) => GetHomeValue(housing);

        public static void AddDefaults(HousingModel defaults)
        {
            Obj.DefaultHomeOverrides.Add(defaults.ModelType, defaults);
        }

        private HomeFurnishingValue GetHomeValue(Type housingItem)
        {
            var dModel = LoadedHomeOverrides[housingItem.Name];
            var dHomeFurnishingValue = CreateDefaultHomeValueFromModel(dModel);

            // check if config override
            var loaded = LoadedHomeOverrides.TryGetValue(housingItem.Name, out HousingModel model);
            if (loaded)
            {
                return CreateHomeValueFromModel(model);
            }

            // return default
            return dHomeFurnishingValue;
        }

        private static HomeFurnishingValue CreateHomeValueFromModel(HousingModel model)
        {
            var HomeValue = new HomeFurnishingValue()
            {
                Category = model.RoomType,     
                HouseValue = model.SkillValue,
                TypeForRoomLimit = Localizer.DoStr(model.TypeForRoomLimit),
                DiminishingReturnPercent = model.DiminishingReturn
            };

            return HomeValue;
        }

        private static HomeFurnishingValue CreateDefaultHomeValueFromModel(HousingModel def)
        {
            var HomeValue = new HomeFurnishingValue()
            {
                Category = def.RoomType,
                HouseValue = def.SkillValue,
                TypeForRoomLimit = Localizer.DoStr(def.TypeForRoomLimit),
                DiminishingReturnPercent = def.DiminishingReturn
            };

            return HomeValue;
        }

        public void Initialize()
        {
            SerializedSynchronizedCollection<HousingModel> newModels = new();
            var previousModels = EMConfigurePlugin.Config.EMHousingValue;
            foreach (var type in typeof(IConfigurableHousing).ConcreteTypes())
            {
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            }

            var loadtypes = DefaultHomeOverrides.Values.ToList();

            // for each type that exists that we are trying to load
            foreach (var lModel in loadtypes)
            {
                var m = previousModels.SingleOrDefault(x => x.ModelType == lModel.ModelType);

                if (m != null)
                    newModels.Add(m);
                else
                    newModels.Add(lModel);
            }

            EMConfigurePlugin.Config.EMHousingValue = newModels;

            foreach (var model in newModels)
            {
                if (!LoadedHomeOverrides.ContainsKey(model.ModelType))
                    LoadedHomeOverrides.Add(model.ModelType, model);
            }
        }
    }
}

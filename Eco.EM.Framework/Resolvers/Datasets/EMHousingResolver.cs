using Eco.Core.Utils;
using Eco.EM.Framework.Utils;
using Eco.Gameplay.Housing.PropertyValues;
using Eco.Mods.TechTree;
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
            Obj.DefaultHomeOverrides.TryAdd(defaults.ModelType, defaults);
        }

        private HomeFurnishingValue GetHomeValue(Type housingItem)
        {
            var dModel = DefaultHomeOverrides[housingItem.Name];
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
                Category = HousingConfig.GetRoomCategory(model.RoomType.Name),
                BaseValue = model.SkillValue,
                TypeForRoomLimit = Localizer.DoStr(model.TypeForRoomLimit),
                DiminishingReturnMultiplier = model.DiminishingReturn
            };

            return HomeValue;
        }

        private static HomeFurnishingValue CreateDefaultHomeValueFromModel(HousingModel def)
        {
            var HomeValue = new HomeFurnishingValue()
            {
                Category = HousingConfig.GetRoomCategory(def.RoomType.Name),
                BaseValue = def.SkillValue,
                TypeForRoomLimit = Localizer.DoStr(def.TypeForRoomLimit),
                DiminishingReturnMultiplier = def.DiminishingReturn
            };

            return HomeValue;
        }

        public static void Initialize()
        {
            SerializedSynchronizedCollection<HousingModel> newModels = new();
            var previousModels = newModels;
            try
            {
                previousModels = EMHousingValuePlugin.Config.EMHousingValue;
            }
            catch
            {
                previousModels = new();
            }
            foreach (var type in typeof(IConfigurableHousing).ConcreteTypes())
            {
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            }

            var loadtypes = Obj.DefaultHomeOverrides.Values.ToList();

            // for each type that exists that we are trying to load
            foreach (var lModel in loadtypes)
            {
                var m = previousModels.SingleOrDefault(x => x.ModelType == lModel.ModelType);

                if (m != null)
                {
                    newModels.Add(m);
                }
                else
                {
                    newModels.Add(lModel);
                }
            }

            EMHousingValuePlugin.Config.EMHousingValue = newModels;

            foreach (var model in newModels)
            {
                if (!Obj.LoadedHomeOverrides.ContainsKey(model.ModelType))
                    Obj.LoadedHomeOverrides.Add(model.ModelType, model);
            }
        }
    }
}
using Eco.Core.Utils;
using Eco.Shared.Localization;
using Eco.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco.EM.Framework.Resolvers
{
    public interface IConfigurableVehicle { }

    // ToDo, Implement Vehicle Resolver
    // It should include Values for: 
    // Storage Slots And Vehicle Weight - This comes under the same thing (PublicStorgaeComponent)
    // If Using Vehicle Component: MaxSpeed, Efficiency - Do not add Vehicle Seats as this will break things
    // If using Fuel Supply Component: Allow for the Addition of new Fuels and removal of fuels, Setting the number of fuel slots
    // If using the Fuel Consumption Component : Set Fuel Consumption Rate
    public class EMVehicleResolver : AutoSingleton<EMVehicleResolver>
    {
        public Dictionary<string, VehicleModel> DefaultVehicleOverrides { get; private set; } = new();
        public Dictionary<string, VehicleModel> LoadedVehicleOverrides { get; private set; } = new();

        public static void AddDefaults(VehicleModel defaults)
        {
            Obj.DefaultVehicleOverrides.Add(defaults.ModelType, defaults);
        }

        public string[] ResolveFuelTagList(IConfigurableVehicle obj) => GetFuelTagList(obj);

        public float ResolveFuelConsumption(IConfigurableVehicle obj) => GetFuelConsumption(obj);
        public float ResolveAirPollution(IConfigurableVehicle obj) => GetAirPollution(obj);
        public float ResolveMaxSpeed(IConfigurableVehicle obj) => GetMaxSpeed(obj);
        public float ResolveEfficiencyMultiplier(IConfigurableVehicle obj) => GetEfficiencyMultiplier(obj);

        public int ResolveFuelSlots(IConfigurableVehicle obj) => GetFuelSlots(obj);
        public int ResolveStorageSlots(IConfigurableVehicle obj) => GetStorageSlots(obj);
        public int ResolveMaxWeight(IConfigurableVehicle obj) => GetMaxWeight(obj);
        public int ResolveSeats(IConfigurableVehicle obj) => GetSeats(obj);

        public string ResolveControlHints(IConfigurableVehicle obj) => GetControlHints(obj);

        private string[] GetFuelTagList(IConfigurableVehicle obj)
        {
            try
            {
                return DefaultVehicleOverrides[obj.GetType().Name].FuelTagList;
            }
            catch
            {
                Log.WriteErrorLine(Localizer.DoStr(string.Format("Unable to load Fuel List for {0}. Please check config.", obj.GetType().Name)));
                throw;
            }
        }

        private float GetFuelConsumption(IConfigurableVehicle obj)
        {
            try
            {
                return LoadedVehicleOverrides[obj.GetType().Name].FuelConsumption;
            }
            catch
            {
                Log.WriteErrorLine(Localizer.DoStr(string.Format("Unable to load Fuel Consumption for {0}. Please check config.", obj.GetType().Name)));
                throw;
            }
        }
        private float GetAirPollution(IConfigurableVehicle obj)
        {
            try
            {
                return LoadedVehicleOverrides[obj.GetType().Name].AirPollution;
            }
            catch
            {
                Log.WriteErrorLine(Localizer.DoStr(string.Format("Unable to load Air Pollution for {0}. Please check config.", obj.GetType().Name)));
                throw;
            }
        }
        private float GetMaxSpeed(IConfigurableVehicle obj)
        {
            try
            {
                return LoadedVehicleOverrides[obj.GetType().Name].MaxSpeed;
            }
            catch
            {
                Log.WriteErrorLine(Localizer.DoStr(string.Format("Unable to load Max Speed for {0}. Please check config.", obj.GetType().Name)));
                throw;
            }
        }
        private float GetEfficiencyMultiplier(IConfigurableVehicle obj)
        {
            try
            {
                return LoadedVehicleOverrides[obj.GetType().Name].EfficiencyMultiplier;
            }
            catch
            {
                Log.WriteErrorLine(Localizer.DoStr(string.Format("Unable to load Efficiency Multiplier for {0}. Please check config.", obj.GetType().Name)));
                throw;
            }
        }

        private int GetFuelSlots(IConfigurableVehicle obj)
        {
            try
            {
                return (int)LoadedVehicleOverrides[obj.GetType().Name].FuelSlots;
            }
            catch
            {
                Log.WriteErrorLine(Localizer.DoStr(string.Format("Unable to load Fuel Slots for {0}. Please check config.", obj.GetType().Name)));
                throw;
            }
        }
        private int GetStorageSlots(IConfigurableVehicle obj)
        {
            try
            {
                return (int)LoadedVehicleOverrides[obj.GetType().Name].StorageSlots;
            }
            catch
            {
                Log.WriteErrorLine(Localizer.DoStr(string.Format("Unable to load Storage Slots for {0}. Please check config.", obj.GetType().Name)));
                throw;
            }
        }
        private int GetMaxWeight(IConfigurableVehicle obj)
        {
            try
            {
                return (int)LoadedVehicleOverrides[obj.GetType().Name].MaxWeight;
            }
            catch
            {
                Log.WriteErrorLine(Localizer.DoStr(string.Format("Unable to load Max Weight for {0}. Please check config.", obj.GetType().Name)));
                throw;
            }
        }
        private int GetSeats(IConfigurableVehicle obj)
        {
            try
            {
                return (int)LoadedVehicleOverrides[obj.GetType().Name].Seats;
            }
            catch
            {
                Log.WriteErrorLine(Localizer.DoStr(string.Format("Unable to load Seats for {0}. Please check config.", obj.GetType().Name)));
                throw;
            }
        }

        private string GetControlHints(IConfigurableVehicle obj)
        {
            try
            {
                return LoadedVehicleOverrides[obj.GetType().Name].ControlHints;
            }
            catch
            {
                Log.WriteErrorLine(Localizer.DoStr(string.Format("Unable to load Control Hints for {0}. Please check config.", obj.GetType().Name)));
                throw;
            }
        }

        public void Initialize()
        {
            SerializedSynchronizedCollection<VehicleModel> newModels = new();
            var config = EMVehiclesPlugin.Config.EMVehicles;

            foreach (var type in typeof(IConfigurableVehicle).ConcreteTypes())
            {
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            }

            var loadtypes = DefaultVehicleOverrides.Values.ToList();

            // for each type that exists that we are trying to load
            foreach (var lModel in loadtypes)
            {
                var m = config.SingleOrDefault(x => x.ModelType == lModel.ModelType);
                if (m != null)
                    newModels.Add(m);
                else
                    newModels.Add(lModel);
            }

            EMVehiclesPlugin.Config.EMVehicles = newModels;

            foreach (var model in newModels)
            {
                if (!LoadedVehicleOverrides.ContainsKey(model.ModelType))
                    LoadedVehicleOverrides.Add(model.ModelType, model);
            }
        }
    }
}

using Eco.EM.Framework.Utils;
using Eco.Shared.Localization;
using System;
using System.ComponentModel;
using Newtonsoft.Json;
using Eco.Gameplay.Components;
using Eco.Gameplay.Items;

namespace Eco.EM.Framework.Resolvers
{
    public class VehicleModel : ModelBase
    {
        [LocDisplayName("Fuel Tag List")] 
        public string[] FuelTagList                 { get; set; }
        [LocDisplayName("Fuel Consumption Rate")] 
        public float FuelConsumption                { get; set; }
        [LocDisplayName("Air Pollution")] 
        public float AirPollution                   { get; set; }
        [LocDisplayName("Max Speed")] 
        public float MaxSpeed                       { get; set; }
        [LocDisplayName("Efficiency Multiplier")] 
        public float EfficiencyMultiplier           { get; set; }
        [LocDisplayName("Fuel Slots")] 
        public float FuelSlots                        { get; set; }
        [LocDisplayName("Storage Slots")] 
        public float StorageSlots                         { get; set; }
        [LocDisplayName("Max Weight")] 
        public float MaxWeight                        { get; set; }
        [LocDisplayName("Seats"), LocDescription("USE THIS WITH CARE! This can really mess with the Total amount of seats in a vehicle and can cause issues.")] 
        public float Seats                            { get; set; } = 1;
        [LocDisplayName("Control Hints"), LocDescription("USE THIS WITH ABSOULTE CARE! This can hide the Vehicle controls and cause alot of confusion. Basic Usage Not Really Known.")] 
        public string ControlHints                  { get; set; } = null;
        [LocDisplayName("Item Name"), ReadOnly(true)] 
        public string DisplayName                   { get; set; }

        public VehicleModel(Type type, string displayName, string[] fuelTagList, int fuelSlots, float fuelConsumption, float airPollution, float maxSpeed, float efficencyMultiplier, float storageSlots, float maxWeight, float seats = 1, string controlHints = null)
        {
            ModelType = type.Name;
            Assembly = type.AssemblyQualifiedName;
            DisplayName = displayName;
            FuelTagList = fuelTagList;
            FuelSlots = fuelSlots;
            FuelConsumption = fuelConsumption;
            AirPollution = airPollution;
            MaxSpeed = maxSpeed;
            EfficiencyMultiplier = efficencyMultiplier;
            StorageSlots = storageSlots;
            MaxWeight = maxWeight;
            Seats = seats;
            ControlHints = controlHints;
        }

        public VehicleModel(Type type, string displayName, float maxSpeed, float efficencyMultiplier, float storageSlots, float maxWeight)
        {
            ModelType = type.Name;
            Assembly = type.AssemblyQualifiedName;
            DisplayName = displayName;
            MaxSpeed = maxSpeed;
            EfficiencyMultiplier = efficencyMultiplier;
            StorageSlots = storageSlots;
            MaxWeight = maxWeight;
        }

        [JsonConstructor]
        public VehicleModel(string modelType, string assembly, string displayName, string[] fuelTagList, int fuelSlots, float fuelConsumption, float airPollution, float maxSpeed, float efficencyMultiplier, float storageSlots, float maxWeight, float seats = 1, string controlHints = null)
        {
            ModelType = modelType;
            Assembly = assembly;
            DisplayName = displayName;
            FuelTagList = fuelTagList;
            FuelSlots = fuelSlots;
            FuelConsumption = fuelConsumption;
            AirPollution = airPollution;
            MaxSpeed = maxSpeed;
            EfficiencyMultiplier = efficencyMultiplier;
            StorageSlots = storageSlots;
            MaxWeight = maxWeight;
            Seats = seats;
            ControlHints = controlHints;
        }

        public override string ToString() => $"{StringUtils.GetAssemblyNameFromAssemblyString(Assembly)} - {ModelType}";
    }
}

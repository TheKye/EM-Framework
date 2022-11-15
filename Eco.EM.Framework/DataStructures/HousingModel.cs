using Eco.EM.Framework.Utils;
using Eco.Gameplay.Housing.PropertyValues;
using Eco.Shared.Localization;
using System;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Eco.EM.Framework.Resolvers
{
    public class HousingModel : ModelBase
    {
        [LocDisplayName("Room Type")]                     public RoomCategory RoomType          { get; set; }
        [LocDisplayName("Skill Value")]                   public float SkillValue                                   { get; set; }
        [LocDisplayName("Item Type")]                     public string TypeForRoomLimit                            { get; set; }
        [LocDisplayName("Diminishing Percent")]           public float DiminishingReturn                            { get; set; }
        [LocDisplayName("Item Name"), ReadOnly(true)]     public string DisplayName                                 { get; set; }

        public HousingModel(Type type, string displayName, RoomCategory roomType, float skillValue, string typeForRoomLimit, float diminishingReturn)
        {
            ModelType = type.Name;
            Assembly = type.AssemblyQualifiedName;
            DisplayName = displayName;
            RoomType = roomType;
            SkillValue = skillValue;
            TypeForRoomLimit = typeForRoomLimit;
            DiminishingReturn = diminishingReturn;
        }

        [JsonConstructor]
        public HousingModel(string modelType, string assembly, RoomCategory roomType, int skillValue, string typeForRoomLimit, float diminishingReturn, string displayName)
        {
            ModelType = modelType;
            Assembly = assembly;
            RoomType = roomType;
            SkillValue = skillValue;
            TypeForRoomLimit = typeForRoomLimit;
            DiminishingReturn = diminishingReturn;
            DisplayName = displayName;
        }

        public override string ToString() => $"{StringUtils.GetAssemblyNameFromAssemblyString(Assembly)} - {DisplayName}";
    }
}

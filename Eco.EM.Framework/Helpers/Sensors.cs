using Eco.Gameplay.Components;
using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Skills;
using Eco.Gameplay.Systems.TextLinks;
using Eco.Gameplay.Systems.Tooltip;
using Eco.Shared.Serialization;
using Eco.Mods.TechTree;
using Eco.Shared.Networking;
using Eco.Gameplay.Rooms;
using Eco.Shared.Utils;
using System;
using Eco.Gameplay.Interactions;
using Eco.Gameplay.Components.Auth;
using Eco.Gameplay.Housing;
using Eco.Gameplay.Players;
using System.Linq;
using Eco.Gameplay.Systems.Chat;

namespace Eco.EM.Framework.Helpers
{
    public static class Sensors
    {
        public static void MotionSensor(WorldObject sensor, float range = 20f)
        {
            if (!sensor.HasComponent<OnOffComponent>())
                return;
            if (!sensor.GetComponent<OnOffComponent>().On)
                return;

            var objRoom = RoomData.Obj.GetNearestRoom(sensor.Position);
            var anyoneNear = (PlayerSensor.AnyoneInSameRoom(sensor));
            NetObjectManager.Default
                .GetObjectsWithin(sensor.Position, range)
                .ForEach(obj =>
                {
                    if (obj is WorldObject wo &&
                        wo.HasComponent<OnOffComponent>() &&
                        wo.HasComponent<HousingComponent>() &&
                        Compare.IsLike(wo.GetComponent<HousingComponent>().HomeValue.TypeForRoomLimit, "Lights"))
                    {
                        var lightRoom = RoomData.Obj.GetNearestRoom(wo.Position);

                        if (lightRoom.Id == objRoom.Id)
                        {
                            wo.SetAnimatedState("IsOn", anyoneNear);
                            wo.GetComponent<OnOffComponent>().On = anyoneNear;
                            return;
                        }
                    }
                });
        }

        public static void ProximitySensor(WorldObject sensor, float range = 20f)
        {
            if (!sensor.HasComponent<OnOffComponent>())
                return;
            if (!sensor.GetComponent<OnOffComponent>().On)
                return;

            var anyoneNear = (PlayerSensor.AnyoneNear(sensor));
            NetObjectManager.Default
                .GetObjectsWithin(sensor.Position, range)
                .ForEach(obj =>
                {
                    if (obj is WorldObject wo &&
                        wo.HasComponent<OnOffComponent>() &&
                        wo.HasComponent<HousingComponent>() &&
                        Compare.IsLike(wo.GetComponent<HousingComponent>().HomeValue.TypeForRoomLimit, "Lights"))
                    {
                        wo.SetAnimatedState("IsOn", anyoneNear);
                        wo.GetComponent<OnOffComponent>().On = anyoneNear;
                        return;
                    }
                });
        }

        public static void AuthorizedSensor(WorldObject sensor, float range = 20f)
        {
            if (!sensor.HasComponent<OnOffComponent>())
                return;
            if (!sensor.GetComponent<OnOffComponent>().On)
                return;

            var anyoneNear = (PlayerSensor.AuthorizedPersonnelNear(sensor, (int)range));
            NetObjectManager.Default
                .GetObjectsWithin(sensor.Position, range)
                .ForEach(obj =>
                {
                    if (obj is WorldObject wo &&
                        wo.HasComponent<OnOffComponent>() &&
                        wo.HasComponent<HousingComponent>() &&
                        Compare.IsLike(wo.GetComponent<HousingComponent>().HomeValue.TypeForRoomLimit, "Lights"))
                    {
                        wo.SetAnimatedState("IsOn", anyoneNear);
                        wo.GetComponent<OnOffComponent>().On = anyoneNear;
                        return;
                    }
                });
        }

        public static void DaylightSensor(WorldObject sensor, float range = 20f)
        {
            if (!sensor.HasComponent<OnOffComponent>())
                return;
            if (!sensor.GetComponent<OnOffComponent>().On)
                return;

            var isDaylight = DaylightSensorHelper.IsDayLight();
            NetObjectManager.Default
                .GetObjectsWithin(sensor.Position, range)
                .ForEach(obj =>
                {
                    if (obj is WorldObject wo &&
                        wo.HasComponent<OnOffComponent>() &&
                        wo.HasComponent<HousingComponent>() &&
                        Compare.IsLike(wo.GetComponent<HousingComponent>().HomeValue.TypeForRoomLimit, "Lights") && !isDaylight)
                    {
                        wo.SetAnimatedState("IsOn", true);
                        wo.GetComponent<OnOffComponent>().On = true;
                        return;
                    }
                    else if (obj is WorldObject wob &&
                         wob.HasComponent<OnOffComponent>() &&
                         wob.HasComponent<HousingComponent>() &&
                         Compare.IsLike(wob.GetComponent<HousingComponent>().HomeValue.TypeForRoomLimit, "Lights") && isDaylight)
                    {
                        wob.SetAnimatedState("IsOn", false);
                        wob.GetComponent<OnOffComponent>().On = false;
                        return;
                    }
                });
        }

        public static void PowerSensor(WorldObject sensor, float range = 20f)
        {
            if (!sensor.HasComponent<OnOffComponent>())
                return;

            var objRoom = RoomData.Obj.GetNearestRoom(sensor.Position);

            NetObjectManager.Default
                .GetObjectsWithin(sensor.Position, range)
                .ForEach(obj =>
                {
                    if (obj is WorldObject wo &&
                        wo.HasComponent<OnOffComponent>() && wo.HasComponent<PowerConsumptionComponent>())
                    {
                        var lightRoom = RoomData.Obj.GetNearestRoom(wo.Position);

                        if (lightRoom.Id == objRoom.Id)
                        {
                            if (sensor.GetComponent<OnOffComponent>().On)
                            {
                                wo.GetComponent<OnOffComponent>().On = true;
                                wo.UpdateEnabledAndOperating();
                            }
                            if (!sensor.GetComponent<OnOffComponent>().On)
                            {
                                wo.GetComponent<OnOffComponent>().On = false;
                                wo.UpdateEnabledAndOperating();
                            }

                            wo.SetAnimatedState("IsOn", sensor.GetComponent<OnOffComponent>().On);
                            return;
                        }
                    }
                });
        }

        public static void BuildingPowerSensor(WorldObject sensor, float range = 20f)
        {
            if (!sensor.HasComponent<OnOffComponent>())
                return;

            var allRooms = RoomData.Obj.Rooms.Snapshot;

            NetObjectManager.Default
                .GetObjectsWithin(sensor.Position, range)
                .ForEach(obj =>
                {
                    if (obj is WorldObject wo &&
                        wo.HasComponent<OnOffComponent>() && wo.HasComponent<PowerConsumptionComponent>())
                    {
                        var rooms = allRooms.Where(x => x.Valid);
                        foreach (var r in rooms)
                        {
                            if (sensor.GetComponent<OnOffComponent>().On)
                            {
                                wo.GetComponent<OnOffComponent>().On = true;
                                wo.UpdateEnabledAndOperating();
                            }
                            if (!sensor.GetComponent<OnOffComponent>().On)
                            {
                                wo.GetComponent<OnOffComponent>().On = false;
                                wo.UpdateEnabledAndOperating();
                            }

                            wo.SetAnimatedState("IsOn", sensor.GetComponent<OnOffComponent>().On);
                            return;
                        }
                    }
                });
        }

        public static void LightsSensor(WorldObject sensor, float range = 20f)
        {
            if (!sensor.HasComponent<OnOffComponent>())
                return;

            var objRoom = RoomData.Obj.GetNearestRoom(sensor.Position);

            NetObjectManager.Default
                .GetObjectsWithin(sensor.Position, range)
                .ForEach(obj =>
                {
                    if (obj is WorldObject wo &&
                        wo.HasComponent<OnOffComponent>() &&
                        wo.HasComponent<HousingComponent>() &&
                        Compare.IsLike(wo.GetComponent<HousingComponent>().HomeValue.TypeForRoomLimit, "Lights"))
                    {
                        var lightRoom = RoomData.Obj.GetNearestRoom(wo.Position);

                        if (lightRoom.Id == objRoom.Id)
                        {
                            wo.SetAnimatedState("IsOn", sensor.GetComponent<OnOffComponent>().On);
                            return;
                        }
                    }
                });
        }
    }
}

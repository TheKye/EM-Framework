namespace Eco.EM.Framework.Helpers
{
    using Eco.Gameplay.Auth;
    using Eco.Gameplay.Components.Auth;
    using Eco.Gameplay.GameActions;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Players;
    using Eco.Gameplay.Rooms;
    using Eco.Shared.Items;
    using Eco.Shared.Math;
    using Eco.Shared.Networking;
    using System;
    using Eco.Shared.IoC;
    using Eco.EM.Framework.Utils;
    using System.Numerics;

    public static class PlayerSensor
    {
        public static bool AnyoneNear(WorldObject obj, int MaxSensorDistance = 20)
        {
            foreach (var user in PlayerUtils.OnlineUsers)
            {
                float dist = Vector3.Distance(obj.Position, user.Position);
                if (dist <= MaxSensorDistance)
                    return true;
            }

            return false;
        }

        public static User UserNear(WorldObject obj, int MaxSensorDistance = 20)
        {
            foreach (var user in PlayerUtils.OnlineUsers)
            {

                float dist = Vector3.Distance(obj.Position, user.Position);
                if (dist <= MaxSensorDistance)
                    return user;
            }
            return null;
        }

        public static bool AuthorizedPersonnelNear(WorldObject obj, int MaxSensorDistance = 20)
        {
            if (obj.HasComponent<AuthComponent>())
            {
                foreach (var user in PlayerUtils.OnlineUsers)
                {
                    if (ServiceHolder<IAuthManager>.Obj.IsAuthorized(obj, user, (AccessType.ConsumerAccess | AccessType.FullAccess | AccessType.OwnerAccess)))
                    {
                        float dist = Vector3.Distance(obj.Position, user.Position);
                        if (dist <= MaxSensorDistance)
                            return true;
                    }
                }
            }

            return false;
        }
        public static bool AnyoneInSameRoom(WorldObject obj, int MaxSensorDistance = 20)
        {
            var objRoom = RoomData.Obj.GetNearestRoom(obj.Position);

            foreach (var user in PlayerUtils.OnlineUsers)
            {
                float dist = Vector3.Distance(obj.Position, user.Position);
                if (dist <= MaxSensorDistance)
                {
                    int x = (int)user.Position.X,
                        y = (int)user.Position.Y,
                        z = (int)user.Position.Z;

                    var userRoom = RoomData.Obj.GetNearestRoom(new Vector3(x, y, z));

                    if (userRoom != null &&
                        userRoom.Id == objRoom.Id)
                        return true;
                }
            }

            return false;
        }

        public static User GetNear(WorldObject obj, int MaxSensorDistance = 20)
        {
            foreach (var user in PlayerUtils.OnlineUsers)
            {
                float dist = Vector3.Distance(obj.Position, user.Position);
                if (dist <= MaxSensorDistance)
                {
                    return user;
                }
            }
            return null;
        }
    }
}

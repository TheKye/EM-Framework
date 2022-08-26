using Eco.Gameplay.Players;
using Eco.Shared.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eco.EM.Framework.Utils
{
    public static class DirectionUtils
    {
        /// <summary>
        /// Get Looking Direction of User 
        /// <para>Can be Left, Right, Forward, Backward, Up, Down</para>
        /// </summary>
        public static Direction GetLookingDirection(User pUser)
        {
            float yDirection = pUser.Rotation.Forward.Y;

            if (yDirection > 0.85)
                return Direction.Up;
            else if (yDirection < -0.85)
                return Direction.Down;

            return pUser.FacingDir;
        }

        /// <summary>
        /// For example "Up" or "u" to  <see cref="Direction.Up"/>. <see cref="Direction.Unknown"/> if direction is unkown
        /// </summary>
        public static Direction GetDirection(string pDirection)
        {
            if (string.IsNullOrWhiteSpace(pDirection))
                return Direction.Unknown;

            switch (pDirection.Trim().ToLower())
            {
                case "up":
                case "u":
                    return Direction.Up;

                case "down":
                case "d":
                    return Direction.Down;

                case "left":
                case "l":
                    return Direction.Left;

                case "right":
                case "r":
                    return Direction.Right;

                case "back":
                case "b":
                    return Direction.Back;

                case "forward":
                case "f":
                    return Direction.Forward;

                default:
                    return Direction.Unknown;
            }
        }

        public static Direction GetDirectionOrLooking(User pUser, string pDirection = "")
        {
            if (string.IsNullOrWhiteSpace(pDirection))
                return GetLookingDirection(pUser);

            return GetDirection(pDirection);
        }
    }
}

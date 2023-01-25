using Eco.Shared.IoC;
using Eco.Core.Utils;
using Eco.Gameplay.Aliases;
using Eco.Gameplay.Auth;
using Eco.Gameplay.Components.Auth;
using Eco.Gameplay.GameActions;
using Eco.Gameplay.Interactions;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Players;
using Eco.Shared.Items;
using Eco.Shared.Math;
using Eco.Simulation;
using Eco.Simulation.Settings;
using Eco.Simulation.Types;
using Eco.World;
using Eco.World.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Eco.Shared.Voxel;

namespace Eco.EM.Framework.Utils
{
    public static class WorldUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PlantSpecies GetPlantSpecies(Type pBlockType)
        {
            return EcoSim.AllSpecies.OfType<PlantSpecies>().First(ps => ps.BlockType == pBlockType);
        }

        public static IEnumerable<Block> GetTopBlocks(Vector3i pPosition, int pRange)
        {
            for (int x = pPosition.X - pRange; x < pPosition.X + pRange; x++)
                for (int z = pPosition.Z - pRange; z < pPosition.Z + pRange; z++)
                {
                    yield return World.World.GetTopBlock(new Vector2i(x, z));
                }
        }

        //https://codereview.stackexchange.com/questions/70540/get-all-points-on-a-uniform-discrete-grid-inside-a-circles-radius
        public static IEnumerable<Vector2> GetPointsInCircle(Vector2 pCircleCenter, float pRadius, Vector2 pGridCenter, Vector2? pGridStep = null)
        {
            Vector2 gridStep = pGridStep ?? Vector2.one;

            if (pRadius <= 0)
                throw new ArgumentOutOfRangeException("radius", "Argument must be positive.");

            if (gridStep.x <= 0 || gridStep.y <= 0)
                throw new ArgumentOutOfRangeException("gridStep", "Argument must contain positive components only.");

            // Loop bounds for X dimension:
            int i1 = (int)Math.Ceiling((pCircleCenter.x - pGridCenter.x - pRadius) / gridStep.x);
            int i2 = (int)Math.Floor((pCircleCenter.x - pGridCenter.x + pRadius) / gridStep.x);

            // Constant square of the radius:
            float radius2 = pRadius * pRadius;

            for (int i = i1; i <= i2; i++)
            {
                // X-coordinate for the points of the i-th circle segment:
                float x = pGridCenter.x + i * gridStep.x;

                // Local radius of the circle segment (half-length of chord) calulated in 3 steps.
                // Step 1. Offset of the (x, *) from the (circleCenter.x, *):
                float localRadius = pCircleCenter.x - x;
                // Step 2. Square of it:
                localRadius *= localRadius;
                // Step 3. Local radius of the circle segment:
                localRadius = (float)Math.Sqrt(radius2 - localRadius);

                // Loop bounds for Y dimension:
                int j1 = (int)Math.Ceiling((pCircleCenter.y - pGridCenter.y - localRadius) / gridStep.y);
                int j2 = (int)Math.Floor((pCircleCenter.y - pGridCenter.y + localRadius) / gridStep.y);

                for (int j = j1; j <= j2; j++)
                {
                    yield return new Vector2(x, pGridCenter.y + j * gridStep.y);
                }
            }
        }

        /**
         *from Kirthos Core Mod 
         **/

        /// <summary>
        /// Return a list of world object where position are one block away from the position given
        /// </summary>
        public static List<WorldObject> GetNeightbourgWorldObject<T>(Vector3i position) where T : WorldObject
        {
            List<WorldObject> objects = new();
            WorldObjectManager.ForEach(x =>
            {
                if (x is T)
                {
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            for (int k = -1; k <= 1; k++)
                            {
                                Vector3i newPos = new(position.x + i, position.y + j, position.z + k);
                                if (x.Position3i == newPos)
                                {
                                    objects.Add(x);
                                }
                            }
                        }
                    }
                }
            });
            return objects;

        }

        /// <summary>
        /// Get a list of top Block that match the type T. Usefull for getting tree debris or plants
        /// </summary>
        public static List<T> GetTopBlockAroundPoint<T>(User user, Vector3i position, int range) where T : Block
        {
            try
            {
                List<T> blockLists = new();
                for (int i = -range; i < range; i++)
                {
                    for (int j = -range; j < range; j++)
                    {
                        if (i == 0 && j == 0) continue;
                        Vector3i positionAbove = World.World.GetTopGroundPos(new Vector2i(position.x + i, position.z + j)) + Vector3i.Up;
                        Block blockAbove = World.World.GetBlockProbablyTop(positionAbove);
                        if (blockAbove is T)
                        {
                            if (positionAbove != position && Vector3i.Distance(positionAbove, position) < range)
                            {
                                InteractionInfo info = new();
                                info.Method = InteractionMethod.Right;
                                info.BlockPosition = positionAbove;
                                InteractionContext context = info.MakeContext(user.Player);
                                var authResult = ServiceHolder<IAuthManager>.Obj.IsAuthorized(PlotUtil.ToPlotPos(context.Player.User.Position.XZi()), context.Player.User, (AccessType.ConsumerAccess | AccessType.FullAccess | AccessType.OwnerAccess), null);
                                var blockAuthResult = ServiceHolder<IAuthManager>.Obj.IsAuthorized(PlotUtil.ToPlotPos(context.TargetPosition.XZ), context.Player.User, (AccessType.ConsumerAccess | AccessType.FullAccess | AccessType.OwnerAccess), null);
                                if (!authResult.Success && !blockAuthResult.Success || !authResult.Success || !blockAuthResult.Success)
                                {
                                    blockLists.Add(blockAbove as T);
                                }
                            }
                        }
                    }
                }
                return blockLists;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static List<Block> GetTopBlocksInRadius(Vector3i position, int radius)
        {
            List<Block> blocks = new();

            for (int i = -radius; i < radius; i++)
            {
                for (int j = -radius; j < radius; j++)
                {
                    if (i == 0 && j == 0) continue; // ignore my origin

                    Vector3i positionAbove = World.World.GetTopGroundPos(new Vector2i(position.x + i, position.z + j)) + Vector3i.Up;
                    var block = World.World.GetBlock(positionAbove);

                    if (block != null) blocks.Add(block);
                }
            }

            return blocks;
        }

        public static List<WorldObject> GetSurfaceObjectsInRadius(Vector3i position, int radius)
        {
            var blocks = WorldUtils.GetTopBlocksInRadius(position, radius);
            var objects = new List<WorldObject>();

            foreach (var block in blocks)
            {
                if (block is WorldObjectBlock)
                {
                    var handle = (block as WorldObjectBlock).WorldObjectHandle;
                    var obj = handle.Object;

                    if (obj == null) continue;
                    if (!objects.Contains(obj)) objects.AddUnique(obj);
                }
            }
            return objects;
        }

        /// <summary>
        /// Get a list of position of top Block that match the type T. Usefull when wanted to destroy plants or tree debris
        /// </summary>
        public static List<Vector3i> GetTopBlockPositionAroundPoint<T>(User user, Vector3i position, int range) where T : Block
        {
            try
            {
                List<Vector3i> blockLists = new();
                for (int i = -range; i < range; i++)
                {
                    for (int j = -range; j < range; j++)
                    {
                        if (i == 0 && j == 0) continue;
                        Vector3i positionAbove = World.World.GetTopGroundPos(new Vector2i(position.x + i, position.z + j)) + Vector3i.Up;
                        Block blockAbove = World.World.GetBlockProbablyTop(positionAbove);
                        if (blockAbove is T)
                        {
                            if (positionAbove != position && Vector3i.Distance(positionAbove, position) < range)
                            {
                                InteractionInfo info = new();
                                info.Method = InteractionMethod.Right;
                                info.BlockPosition = positionAbove;
                                InteractionContext context = info.MakeContext(user.Player);
                                var authResult = ServiceHolder<IAuthManager>.Obj.IsAuthorized(PlotUtil.ToPlotPos(context.Player.User.Position.XZi()), context.Player.User, (AccessType.ConsumerAccess | AccessType.FullAccess | AccessType.OwnerAccess), null);
                                var blockAuthResult = ServiceHolder<IAuthManager>.Obj.IsAuthorized(PlotUtil.ToPlotPos(context.TargetPosition.XZ), context.Player.User, (AccessType.ConsumerAccess | AccessType.FullAccess | AccessType.OwnerAccess), null);
                                if (!authResult.Success && !blockAuthResult.Success || !authResult.Success || !blockAuthResult.Success)
                                {
                                    blockLists.Add(positionAbove);
                                }
                            }
                        }
                    }
                }
                return blockLists;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Get the position of all block in a sphere radius
        /// </summary>
        public static List<Vector3i> GetSphereBlock(Vector3i centerPosition, int radius)
        {
            List<Vector3i> blocks = new();

            int size = radius * 2 + 1;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    for (int k = 0; k < size; k++)
                    {
                        double distance = (radius - i) * (radius - i)
                                  + (radius - j) * (radius - j)
                                  + (radius - k) * (radius - k);

                        if (distance < (radius * radius))
                        {
                            Vector3i pos = centerPosition + new Vector3i(i, j, k) - new Vector3i(radius, radius, radius);
                            if (pos.x < 0 || pos.y < 0 || pos.z < 0)
                                continue;
                            blocks.Add(pos);
                        }
                    }
                }
            }
            return blocks;
        }

        public static WorldObject WorldObjectsAtPos(Vector3i pos)
        {
            WorldObject objAtPos = null;
            WorldObjectManager.ForEach(worldObject =>
            {
                foreach (Vector3i vector in worldObject.WorldOccupancy)
                {
                    if (vector == pos)
                    {
                        objAtPos = worldObject;
                        break;
                    }
                }
            });
            return objAtPos;
        }
    }
}

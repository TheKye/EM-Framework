using Eco.Gameplay.Components;
using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Players;
using Eco.Gameplay.Skills;
using Eco.Gameplay.Systems.Messaging.Chat.Commands;
using Eco.Mods.TechTree;
using Eco.Shared.Math;
using Eco.Shared.Services;
using Eco.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Eco.EM.Framework.ChatBase.ChatBase;
/// <summary>
/// Testing Commands for Elixr Mods
/// </summary>
namespace Eco.EM.Framework.ModTools.Testing
{
    [ChatCommandHandler]
    public class TestingCommands
    {
        [ChatCommand("Elixr Mods Testing Group", ChatAuthorizationLevel.Admin)]
        public static void EMTest() { }

        [ChatSubCommand("EMTest", "Basic Echo command for testing the chatbase", "/echo-chatbasic", ChatAuthorizationLevel.Admin)]
        public static void TestCBChatSMTA(User user, string content)
        {
            ChatBase.ChatBaseExtended.CBChat(content, user, MessageType.Permanent, true);
        }

        [ChatSubCommand("EMTest", "Retrieve all objects from the same namespace as an object spawn them in world.", "em-lineup", ChatAuthorizationLevel.Admin)]
        public static void NamespaceLineUp(User user, string itemlookup, bool _namespace = true)
        {
            // Find the item by the given string
            var item = Item.Get(itemlookup);

            // If nothing is found report it and break
            if (item == null)
            {
                user.Player.MsgLoc($"Couldn't find any item matching that value", NotificationStyle.Error);
                return;
            }

            var t = item.Type; ;

            // Declare the list to be populated
            List<Type> objList;
            List<Type> itemList;

            if (_namespace)
            {
                // Get the namespace of the object & cast all objects in that namespace to a list
                var ns = t.Namespace;
                itemList = typeof(Item).CreatableTypes().Where(x => x.Namespace == ns && Item.Get(x).Category != "Hidden" && !(Item.Get(x) is Skill)).ToList();
                objList = typeof(WorldObject).CreatableTypes().Where(x => x.Namespace == ns).ToList();

            }
            else
            {
                // Get the assembly of the object & cast all objects in that assembly to a list
                var asm = t.Assembly;
                itemList = typeof(Item).CreatableTypes().Where(x => x.Assembly == asm && Item.Get(x).Category != "Hidden" && !(Item.Get(x) is Skill)).ToList();
                objList = typeof(WorldObject).CreatableTypes().Where(x => x.Assembly == asm).ToList();
            }

            // Line up the objects
            if (objList.Count != 0) LineUp(user, objList);
            else user.Player.MsgLoc($"No World Objects found in the {(_namespace ? "Namespace" : "Assembly")}", NotificationStyle.Error);

            if (itemList.Count != 0) FillChests(user, itemList);
            else user.Player.MsgLoc($"No Items found in the {(_namespace ? "Namespace" : "Assembly")}", NotificationStyle.Error);
        }

        #region functions
        private static void FillChests(User user, List<Type> itemList)
        {
            var center = (Vector3i)user.Position;
            Quaternion standard = user.FacingDir.ToQuat();
            var count = 0;

            var length = itemList.Count;

            for (int i = 0; i < length;)
            {
                try
                {
                    var chestpos = center - (user.FacingDir.ToVec() * 2 * (count + 1));
                    var chest = WorldObjectsAtPos(chestpos) ?? WorldObjectManager.ForceAdd(typeof(StorageChestObject), user, chestpos, standard, false);
                    var inv = chest.GetComponent<PublicStorageComponent>().Storage;

                    if (!inv.Stacks.Any(x => x.Empty())) { count++; continue; }

                    inv.AddItem(Item.Get(itemList[i]));
                    i++;
                }
                catch
                {
                    user.MsgLocStr($"Unable to place {itemList[i].Name}. Skipped placement, make report.");
                    i++;
                }
            }
        }

        internal static void LineUp(User user, List<Type> list)
        {
            var center = (Vector3i)user.Position;
            Quaternion standard = user.FacingDir.ToQuat();
            var count = 0;

            foreach (Type obj in list)
            {
                try
                {
                    var spawnPos = center + (user.FacingDir.ToVec() * 2 * (count + 1));
                    WorldObjectManager.ForceAdd(obj, user, spawnPos, standard, false);
                    count++;
                }
                catch
                {
                    user.MsgLocStr($"Unable to place {obj.Name}. Skipped placement, make report.");
                }
            }
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
        #endregion
    }
}

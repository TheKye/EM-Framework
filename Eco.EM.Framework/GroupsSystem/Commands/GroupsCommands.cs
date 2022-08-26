using Eco.Gameplay.Players;
using Eco.Gameplay.Systems.Chat;
using Eco.Shared.Utils;
using System.Linq;
using System.Text;
using Eco.EM.Framework.ChatBase;
using Eco.Shared.Localization;
using Eco.Gameplay.Systems.Messaging.Chat.Commands;
using Eco.EM.Framework.Utils;

namespace Eco.EM.Framework.Groups
{
    [ChatCommandHandler]
    public class GroupsCommands
    {
        [ChatCommand("Groups System Commands", ChatAuthorizationLevel.Admin)]
        public static void Groups(User user) { }

        [ChatSubCommand("Groups", "Used to Create a New Group", "grp-add", ChatAuthorizationLevel.Admin)]
        public static void AddGroup(User user, string groupName)
        {
            Group group = GroupsManager.Data.GetorAddGroup(groupName, true);
            ChatBaseExtended.CBInfo(string.Format(Defaults.appName + Localizer.DoStr("Group {0} was created"), group.GroupName), user);

            GroupsManager.API.SaveData();
        }

        [ChatSubCommand("Groups", "Used to Delete an Existing Group", "grp-del", ChatAuthorizationLevel.Admin)]
        public static void DeleteGroup(User user, string groupName)
        {
            var maingroups = StringUtils.Sanitize(groupName);
            if (maingroups == "admin" || maingroups == "default")
            {
                ChatBaseExtended.CBError(string.Format(Defaults.appName + Localizer.DoStr("Group {0} can not be deleted, it is a core group"), maingroups), user);
                return;
            }


            if (GroupsManager.Data.DeleteGroup(groupName))
                ChatBaseExtended.CBInfo(string.Format(Defaults.appName + Localizer.DoStr("Group {0} was deleted"), StringUtils.Sanitize(groupName)), user);
            else
                ChatBaseExtended.CBError(string.Format(Defaults.appName + Localizer.DoStr("Group {0} was unable to be found"), StringUtils.Sanitize(groupName)), user);

            GroupsManager.API.SaveData();
        }

        [ChatSubCommand("Groups", "Used to print a list of groups to the chat window", "grp-list", ChatAuthorizationLevel.Admin)]
        public static void ListGroups(User user)
        {
            StringBuilder sb = new();
            var groups = GroupsManager.Data.Groups;
            groups.ForEach(g =>
            {
                sb.Append(g.GroupName + "\n");

                if (g != groups.Last())
                    sb.Append(", ");
            });

            ChatBaseExtended.CBInfoPane(Defaults.appName + "Groups:", sb.ToString(), "EMGroupList",user);
        }

        [ChatSubCommand("Groups", "Used to print a list permissions assigned to a group", "grp-perms", ChatAuthorizationLevel.Admin)]
        public static void GroupPermissions(User user, string groupName)
        {
            Group group = GroupsManager.Data.GetorAddGroup(groupName, true);

            StringBuilder sb = new();
            group.Permissions.ForEach(perm =>
            {
                sb.Append(perm.Identifier);

                if (perm != group.Permissions.Last())
                    sb.Append(", ");
            });

            ChatBaseExtended.CBInfoPane(string.Format(Localizer.DoStr("Permissions for Group: {0}"),group.GroupName), string.Format("Group {0}:\nPermissions: {1}", group.GroupName, sb.ToString()), "EMGroupList", user);
        }

        [ChatSubCommand("Groups", "Used to add a user to a group", "grp-adduser", ChatAuthorizationLevel.Admin)]
        public static void AddUserToGroup(User user, string userName, string groupName)
        {
            Group group = GroupsManager.Data.GetorAddGroup(groupName, true);
            User toAdd = PlayerUtils.GetUserByName(userName);
            if (toAdd == null)
            {
                ChatBaseExtended.CBError(Defaults.appName + string.Format(Localizer.DoStr("User {0} was unable to be found"), userName), user);
                return;
            }

            if (group.AddUser(toAdd))
                ChatBaseExtended.CBInfo(Defaults.appName + string.Format(Localizer.DoStr("User {0} was added to Group {1}"), toAdd.Name, group.GroupName), user);
            else
                ChatBaseExtended.CBError(Defaults.appName + string.Format(Localizer.DoStr("User {0} already exists in Group {1}"), toAdd.Name, group.GroupName), user);

            GroupsManager.API.SaveData();
        }

        [ChatSubCommand("Groups", "Used to remove a user from a group", "grp-remuser", ChatAuthorizationLevel.Admin)]
        public static void RemoveUserFromGroup(User user, string userName, string groupName)
        {
            Group group = GroupsManager.Data.GetorAddGroup(groupName, false);
            if (group == null)
            {
                ChatBaseExtended.CBError(Defaults.appName + string.Format(Localizer.DoStr("Group {0} was unable to be found"), groupName), user);
            }

            User toRemove = PlayerUtils.GetUser(userName);
            if (toRemove == null)
            {
                ChatBaseExtended.CBError(Defaults.appName + string.Format(Localizer.DoStr("User {0} was unable to be found"), userName), user);
                return;
            }

            if (group.RemoveUser(toRemove))
                ChatBaseExtended.CBInfo(Defaults.appName + string.Format(Localizer.DoStr("User {0} was removed from Group {1}"), userName, group.GroupName), user);
            else
                ChatBaseExtended.CBError(Defaults.appName + string.Format(Localizer.DoStr("User {0} was unable to be found in Group {1}"),userName, group.GroupName), user);

            GroupsManager.API.SaveData();
        }

        [ChatSubCommand("Groups", "Used to force save the groups just incase it didn't auto save.", "grp-fs", ChatAuthorizationLevel.Admin)]
        public static void ForceSave(User user)
        {
            GroupsManager.API.SaveData();
            ChatBaseExtended.CBInfo("Save Done.", user);
        }
    }
}

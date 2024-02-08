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
        public static void AddGroup(IChatClient user, string groupName)
        {
            Group group = GroupsManager.Data.GetorAddGroup(groupName, true);
            user.MsgLocStr($"Group {groupName} was created");

            GroupsManager.API.SaveData();
        }

        [ChatSubCommand("Groups", "Used to Delete an Existing Group", "grp-del", ChatAuthorizationLevel.Admin)]
        public static void DeleteGroup(IChatClient user, string groupName)
        {
            var maingroups = StringUtils.Sanitize(groupName);
            if (maingroups == "admin" || maingroups == "default")
            {
                user.ErrorLocStr($"Group {groupName} is a default group and cannot be deleted");
                return;
            }

            if (GroupsManager.Data.DeleteGroup(groupName))
                user.ErrorLocStr($"Group {groupName} was deleted");
            else
                user.ErrorLocStr($"Group {groupName} was unable to be found.");

            GroupsManager.API.SaveData();
        }

        [ChatSubCommand("Groups", "Used to print a list of groups to the chat window", "grp-list", ChatAuthorizationLevel.Admin)]
        public static void ListGroups(IChatClient client)
        {
            StringBuilder sb = new();
            var groups = GroupsManager.Data.Groups;
            groups.ForEach(g =>
            {
                sb.Append(g.GroupName + "\n");

                if (g != groups.Last())
                    sb.Append(", ");
            });

            var user = UserManager.FindUser(client.Name);
            if (user is null)
            {
                client.ErrorLocStr($"Groups: {sb}");
            }
            else
                ChatBaseExtended.CBInfoPane(Defaults.appName + "Groups:", sb.ToString(), "EMGroupList",user);
        }
        
        [ChatSubCommand("Groups", "Used to print a list of groups for rcon use", "rcongrp-list", ChatAuthorizationLevel.Admin)]
        public static void RconGroupPermissions(IChatClient client, string groupName)
        {
        }
        
        [ChatSubCommand("Groups", "Used to print a list permissions assigned to a group", "grp-perms", ChatAuthorizationLevel.Admin)]
        public static void GroupPermissions(IChatClient client, string groupName)
        {
            Group group = GroupsManager.Data.GetorAddGroup(groupName, true);

            StringBuilder sb = new();
            group.Permissions.ForEach(perm =>
            {
                sb.Append(perm.Identifier);

                if (perm != group.Permissions.Last())
                    sb.Append(", ");
            });

            var user = UserManager.FindUser(client.Name);
            if(user is null)
            {
                client.ErrorLocStr($"Permissions for Group: {group.GroupName}: {sb}");
            }
            else
                ChatBaseExtended.CBInfoPane(string.Format(Localizer.DoStr("Permissions for Group: {0}"),group.GroupName), string.Format("Group {0}:\nPermissions: {1}", group.GroupName, sb.ToString()), "EMGroupList", user);
        }

        [ChatSubCommand("Groups", "Used to add a user to a group", "grp-adduser", ChatAuthorizationLevel.Admin)]
        public static void AddUserToGroup(IChatClient user, string userName, string groupName)
        {
            Group group = GroupsManager.Data.GetorAddGroup(groupName, true);
            User toAdd = PlayerUtils.GetUserByName(userName);
            if (toAdd == null)
            {
                user.ErrorLocStr($"User {userName} was unable to be found.");
                return;
            }

            if (group.AddUser(toAdd))
                user.MsgLocStr($"User {toAdd.Name} was added to Group {group.GroupName}");
            else
                user.ErrorLocStr($"User {toAdd.Name} Already Exists in Group: {group.GroupName}");
            
            GroupsManager.API.SaveData();
        }

        [ChatSubCommand("Groups", "Used to remove a user from a group", "grp-remuser", ChatAuthorizationLevel.Admin)]
        public static void RemoveUserFromGroup(IChatClient user, string userName, string groupName)
        {
            Group group = GroupsManager.Data.GetorAddGroup(groupName, false);
            if (group == null)
            {
                user.ErrorLocStr($"Group {groupName} was unable to be found.");
            }

            User toRemove = PlayerUtils.GetUser(userName);
            if (toRemove == null)
            {
                user.ErrorLocStr($"User {userName} was unable to be found.");
                return;
            }

            if (group.RemoveUser(toRemove))
                user.MsgLocStr($"User {toRemove.Name} was removed from Group {group.GroupName}");
            else
                user.ErrorLocStr($"User {toRemove.Name} was unable to be found in Group: {group.GroupName}");

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

using Eco.Core.Controller;
using Eco.EM.Framework.ChatBase;
using Eco.EM.Framework.Groups;
using Eco.EM.Framework.Utils;
using Eco.Gameplay.Components;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Players;
using Eco.Gameplay.Systems.NewTooltip.TooltipLibraryFiles;
using Eco.Plugins.Networking;
using Eco.Shared.Localization;
using Eco.Shared.Networking;
using Eco.Shared.Serialization;
using Eco.Shared.Utils;
using Eco.Simulation.Time;
using Eco.World.Blocks;
using NLog.Targets;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Eco.EM.Framework.AdminUtils
{
    [Serialized, AutogenClass, LocDisplayName("Admin Utils")]
    public class AdminUiUtil : WorldObjectComponent, INotifyPropertyChanged, IController
    {

        #region IController
        public event PropertyChangedEventHandler PropertyChanged;
        int controllerID;
        public ref int ControllerID => ref this.controllerID;
        #endregion

        public AdminUiUtil() { }

        private User SelectedUser { get; set; }
        private User user { get; set; }

        private bool UserChanged;
        private bool GroupChanged;

        [Eco, ClientInterfaceProperty, GuestHidden]
        public User Users
        {
            get => user;
            set
            {
                if (value == user) return;
                user = value;
                UserChanged = true;
                SelectedUser = value;
                this.Changed(nameof(Users));
                this.Changed(nameof(UserChanged));
            }
        }

        string group { get; set; }
        [Eco, ClientInterfaceProperty, GuestHidden]
        public string Group
        {
            get => group;
            set
            {
                if (value == group) return;
                group = value;
                GroupChanged = true;
                this.Changed(nameof(Group));
                this.Changed(nameof(GroupChanged));
            }
        }

        [Eco, ClientInterfaceProperty, GuestHidden]
        public string ReasonForKickOrBan { get; set; }

        [RPC, Autogen, GuestHidden]
        public void Ban(Player player)
        {
            if (SelectedUser == null)
            {
                player.ErrorLocStr("Please Select a User to Ban.");
                return;
            }
            UserManagerCommands.Ban(SelectedUser, reason: string.IsNullOrEmpty(ReasonForKickOrBan) ? "No Reason Provided." : ReasonForKickOrBan);
            player.OkBoxLocStr(string.Format($"{SelectedUser} Was Banned For: {0}", string.IsNullOrEmpty(ReasonForKickOrBan) ? "No Reason Provided." : ReasonForKickOrBan));
            ReasonForKickOrBan = string.Empty;
            this.Changed(nameof(ReasonForKickOrBan));
        }

        [RPC, Autogen, GuestHidden]
        public void Kick(Player player)
        {
            if (SelectedUser == null)
            {
                player.ErrorLocStr("Please Select a User to Kick.");
                return;
            }
            UserManagerCommands.Kick(player.User, SelectedUser, reason: string.IsNullOrEmpty(ReasonForKickOrBan) ? "No Reason Provided." : ReasonForKickOrBan);
            player.OkBoxLocStr(string.Format($"{SelectedUser} Was Kicked For: {0}", string.IsNullOrEmpty(ReasonForKickOrBan) ? "No Reason Provided." : ReasonForKickOrBan));
            ReasonForKickOrBan = string.Empty;
            this.Changed(nameof(ReasonForKickOrBan));
        }

        [RPC, Autogen, GuestHidden]
        public void ListAllGroups(Player player)
        {
            StringBuilder sb = new();
            var groups = GroupsManager.Data.Groups;
            groups.ForEach(g =>
            {
                sb.Append(g.GroupName + ",\n");
            });

            ChatBaseExtended.CBInfoPane(Defaults.appName + "Groups:", sb.ToString(), "EMGroupList", player.User);
        }

        [RPC, Autogen, GuestHidden]
        public void ListCurrentUserGroups(Player player)
        {
            if (SelectedUser == null)
                return;
            StringBuilder sb = new();
            var groups = GroupsManager.Data.Groups;
            groups.ForEach(g =>
            {
                var sgu = GroupsManager.Data.GetGroupUser(SelectedUser);
                if (sgu != null)
                {
                    foreach (var gu in g.GroupUsers)
                    {
                        if (sgu.Equals(gu))
                        {
                            sb.Append(g.GroupName + "\n");
                            break;
                        }

                    }
                }
            });

            ChatBaseExtended.CBInfoPane(Defaults.appName + "Groups:", sb.ToString(), "EMGroupList", player.User);
        }

        [RPC, Autogen, GuestHidden]
        public void AddToGroup(Player player)
        {
            if (SelectedUser == null)
            {
                player.ErrorLocStr("Please Select a User to Kick.");
                return;
            }
            if (Group == null)
            {
                player.ErrorLocStr("Please Select a Group");
                return;
            }
            Group group = GroupsManager.Data.GetorAddGroup(Group, false);
            if (group == null)
            {
                player.ErrorLocStr("Group doesn't Exist, please check the spelling, or click the Add New Group Button");
                return;
            }
            if (group.AddUser(SelectedUser))
                ChatBaseExtended.CBInfo(Defaults.appName + string.Format(Localizer.DoStr("User {0} was added to Group {1}"), SelectedUser.Name, group.GroupName), player.User);
            else
                ChatBaseExtended.CBError(Defaults.appName + string.Format(Localizer.DoStr("User {0} already exists in Group {1}"), SelectedUser.Name, group.GroupName), player.User);

            GroupsManager.API.SaveData();
        }

        [RPC, Autogen, GuestHidden]
        public void RemoveFromGroup(Player player)
        {
            if (SelectedUser == null)
            {
                player.ErrorLocStr("Please Select a User to Kick.");
                return;
            }
            if (string.IsNullOrEmpty(Group))
            {
                player.ErrorLocStr("Please Select a Group");
                return;
            }
            Group group = GroupsManager.Data.GetorAddGroup(Group, false);
            if (group == null)
            {
                player.MsgLocStr("Group Doesn't Exist, Please check the group name");
            }
            if (group.RemoveUser(SelectedUser))
                ChatBaseExtended.CBInfo(Defaults.appName + string.Format(Localizer.DoStr("User {0} was removed from Group {1}"), SelectedUser, group.GroupName), player.User);
            else
                ChatBaseExtended.CBError(Defaults.appName + string.Format(Localizer.DoStr("User {0} was unable to be found in Group {1}"), SelectedUser, group.GroupName), player.User);

            GroupsManager.API.SaveData();
        }

        [RPC, Autogen, GuestHidden]
        public void AddNewGroup(Player player)
        {
            if (string.IsNullOrEmpty(Group))
            {
                player.ErrorLocStr("You need to give the group a name.");
            }
            Group group = GroupsManager.Data.GetorAddGroup(Group, false);
            if (group != null)
            {
                player.ErrorLocStr("Group Already Exists, Please pick a different name for the group");
                return;
            }
            else
            {
                GroupsManager.Data.GetorAddGroup(Group, true);
                player.MsgLocStr("Group has been created");
            }
        }

        [RPC, Autogen, GuestHidden]
        public void DeleteGroup(Player player)
        {
            if (string.IsNullOrEmpty(Group))
            {
                player.ErrorLocStr("You need to provide a group name to delete.");
            }
            Group group = GroupsManager.Data.GetorAddGroup(Group, false);
            if (group != null)
            {
                GroupsManager.Data.DeleteGroup(Group);
                player.MsgLocStr("Group Already Exists, Please pick a different name for the group");
                Group = string.Empty;
                this.Changed(nameof(Group));
                return;
            }
            else
                player.MsgLocStr("Group Doesn't Exists, Please Check the name");
        }

        [RPC, Autogen, GuestHidden]
        public void RemoveAllDevTools(Player player)
        {
            int count = 0;
            WorldObjectManager.ForEach(x =>
            {
                if (x.HasComponent<PublicStorageComponent>())
                {
                    var storage = x.GetComponent<PublicStorageComponent>();
                    if (storage.Inventory.Stacks.Any(stack => stack.Item is DevtoolItem))
                    {
                        foreach (var dt in storage.Inventories)
                        {
                            int qty = dt.TotalNumberOfItems(typeof(DevtoolItem));
                            dt.TryRemoveItems(typeof(DevtoolItem), qty);
                            count += qty;
                        }
                    }
                }
            });
            foreach (var usr in UserManager.Users)
            {
                if (usr.Inventory.ToolbarBackpack.Stacks.Any(stack => stack.Item is DevtoolItem))
                {
                    int qty = usr.Inventory.TotalNumberOfItems(typeof(DevtoolItem));
                    usr.Inventory.RemoveItems(typeof(DevtoolItem), qty);
                    count += qty;

                }
            }
            ChatBaseExtended.CBInfoBox(string.Format(Localizer.DoStr("All Found Dev tools have been reclaimed. Reclaimed {0} Dev Tools."), count), player.User);
        }

        [RPC, Autogen, GuestHidden]
        public void ListPlayerInfo(Player player)
        {
            StringBuilder sb = new();
            if (SelectedUser == null)
            {
                player.MsgLocStr("Please Select a user");
                return;
            }

            //Compile Basic Info:
            sb.Append($"Username: {SelectedUser.Name}\n");
            sb.Append($"Current Stars Earned: {SelectedUser.UserXP.TotalStarsEarned}\n");
            sb.Append($"Current Stars Left: {SelectedUser.UserXP.StarsAvailable}\n");
            sb.Append($"XP Till Next Star: {SelectedUser.UserXP.NextStarCost}\n");
            sb.Append($"Banned? {UserManager.Config.UserPermission.BlackList.Contains(SelectedUser)}\n");

            //Other info
            sb.Append($"Food Skill Rate: {SelectedUser.Stomach.NutrientSkillRate()}\n");
            sb.Append($"House Skill Rate: {SelectedUser.HomePropertyValue?.TotalSkillPoints ?? 0}\n");
            sb.Append($"Total Skill Rate: {SelectedUser.UserXP.SkillRate}\n\n");
            sb.Append($"---------------------\n");
            sb.Append($"Skills Taken:\n");
            foreach (var s in SelectedUser.Skillset.Skills)
                sb.Append($"- {s.DisplayName}\n");
            sb.Append($"\n---------------------\n\n");
            sb.Append($"Talents Taken:\n");
            foreach (var t in SelectedUser.Talentset.Talents)
                sb.Append($"- {t.Name}\n");
            sb.Append($"\n---------------------\n\n");

            sb.Append(string.Format(Localizer.DoStr("Online Time (Total): {0}"), TimeFormatter.FormatSpan(SelectedUser.TotalPlayTime)) + "\n");

            sb.Append(string.Format(Localizer.DoStr("Online Time (Session): {0}"), SelectedUser.LoggedIn ? TimeFormatter.FormatSpan(WorldTime.Seconds - SelectedUser.LoginTime) : $"Offline") + "\n");

            ChatBaseExtended.CBInfoPane(Defaults.appName + $"Player Stats For: {SelectedUser.Name}", sb.ToString(), "EMGroupList", player.User);
        }
    }
}

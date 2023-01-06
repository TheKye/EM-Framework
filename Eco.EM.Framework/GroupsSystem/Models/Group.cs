﻿using Eco.Core.Controller;
using Eco.EM.Framework.Logging;
using Eco.Gameplay.Players;
using Eco.Gameplay.Utils;
using Eco.Shared.Networking;
using Eco.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using static Eco.Shared.Networking.EcoTextLimitAttribute;

namespace Eco.EM.Framework.Groups
{
    [Serializable]
    public class Group : SimpleEntry
    {
        public static event Action<string, string> PlayerAdded;
        public static event Action<string, string> PlayerRemoved;

        public string GroupName { get; private set; }

        public HashSet<SimpleGroupUser> GroupUsers { get; private set; }
        public List<IGroupAuthorizable> Permissions { get; private set; }

        public override string ToString() => Name;

        public Group(string grpName, HashSet<SimpleGroupUser> users = null, List<IGroupAuthorizable> perms = null)
        {
            GroupName = grpName;
            Permissions = perms;
            GroupUsers = users;
            this.Name = grpName;
            this.Changed(nameof(this.Name));
            GroupUsers ??= new HashSet<SimpleGroupUser>();

            Permissions ??= new List<IGroupAuthorizable>();
        }

        public bool AddPermission(IGroupAuthorizable perm)
        {
            if (!Permissions.Any(p => p.Identifier == perm.Identifier))
            {
                perm.BlackListed = false;
                Permissions.Add(perm);
                return true;
            }

            return false;
        }

        public void DeletePermission(IGroupAuthorizable perm)
        {
            var found = Permissions.RemoveAll(p => p.Identifier == perm.Identifier);
        }

        public bool AddUser(SimpleGroupUser user)
        {
            var result = GroupUsers.Add(user);
            if (PlayerAdded != null) PlayerAdded.Invoke(GroupName, user.Name);
            return result;
        }

        public bool BlacklistCommand(IGroupAuthorizable perm)
        {
            if (!Permissions.Any(p => p.Identifier == perm.Identifier))
            {
                Permissions.Add(perm);
                return true;
            }
            LoggingUtils.Debug(perm.BlackListed.ToString());
            return false;
        }

        public bool AddUser(User user)
        {
            var sgu = GroupsManager.Data.GetGroupUser(user);
            if (sgu != null)
                return AddUser(sgu);
            else
                return false;
        }

        public bool RemoveUser(SimpleGroupUser user)
        {
            var result = GroupUsers.Remove(user);
            if (PlayerRemoved != null) PlayerRemoved.Invoke(GroupName, user.Name);
            return result;
        }

        public bool RemoveUser(User user)
        {
            var entry = GroupUsers.FirstOrDefault(u => u.Name == user.Name);

            if (entry == null)
                return false;

            return RemoveUser(entry);
        }
    }
}

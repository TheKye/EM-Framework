using Eco.Core.Plugins.Interfaces;
using Eco.Core.Utils;
using Eco.Gameplay.Players;
using Eco.Shared.Utils;
using System.IO;
using System.Linq;
using Eco.EM.Framework.FileManager;
using Eco.Shared.Localization;
using Eco.EM.Framework.Utils;
using System.Threading;
using Eco.WorldGenerator;
using Eco.EM.Framework.Plugins;

namespace Eco.EM.Framework.Groups
{
    public class GroupsManager : Singleton<GroupsManager>, IModKitPlugin, IInitializablePlugin
    {
        internal const string _dataFile = "ElixrMods-GroupsData.json";
        internal const string _subPath = "/EM/Groups";
        public Timer Timer;

        public static GroupsData Data { get; internal set; }

        public static GroupsAPI API { get; private set; }

        public GroupsManager()
        {
            Data = LoadData();
            API = new GroupsAPI();

            if (!File.Exists(Defaults.SaveLocation + _subPath + _dataFile))
                SaveData();
        }

        public void Initialize(TimedTask timer)
        {
            if (Data.Groups.Count == 0)
            {
                Data.GetorAddGroup("admin", true);
                Data.GetorAddGroup("default", true);
            }

            foreach (var usr in PlayerUtils.Users)
            {
                lock (Data.AllUsers)
                {
                    if (!Data.AllUsers.Any(entry => entry.Name == usr.Name || entry.SteamID == usr.SteamId || entry.SlgID == usr.SlgId))
                    {
                        Data.AllUsers.Add(new SimpleGroupUser(usr.Name, usr.SlgId ?? "", usr.SteamId ?? ""));
                        SaveData();
                    }

                    /* Only implement if we think this is necessary , it will basically disable server configs for TP's, Homes and Warps*/
                    Group group;

                    if (usr.IsAdminOrDev)
                        group = Data.GetorAddGroup("admin");
                    else
                        group = Data.GetorAddGroup("default");

                    if (!group.GroupUsers.Any(entry => entry.Name == usr.Name || entry.SteamID == usr.SteamId || entry.SlgID == usr.SlgId))
                    {
                        group.AddUser(usr);
                        SaveData();
                    }
                }
            }

            UserManager.OnUserLoggedIn.Add(u =>
            {
                lock (Data.AllUsers)
                {
                    if (!Data.AllUsers.Any(entry => entry.Name == u.Name || entry.SteamID == u.SteamId || entry.SlgID == u.SlgId))
                    {
                        Data.AllUsers.Add(new SimpleGroupUser(u.Name, u.SlgId ?? "", u.SteamId ?? ""));
                        SaveData();
                    }

                    /* Only implement if we think this is necessary , it will basically disable server configs for TP's, Homes and Warps*/
                    Group group;

                    if (u.IsAdmin)
                        group = Data.GetorAddGroup("admin");
                    else
                        group = Data.GetorAddGroup("default");

                    if (!group.GroupUsers.Any(entry => entry.Name == u.Name && entry.SteamID == u.SteamId || entry.SlgID == u.SlgId))
                    {
                        group.AddUser(u);
                        SaveData();
                    }
                }
            });

            Timer = new(Timer_tick, null, 10000, 10000);
        }

        static void Timer_tick(object state)
        {
            var users = PlayerUtils.Users;
            lock (Data.AllUsers)
            {
                foreach (var u in users)
                {
                    var agroup = Data.GetorAddGroup("admin");

                    if (!u.IsAdmin && agroup.GroupUsers.Any(entry => entry.Name == u.Name && entry.SteamID == u.SteamId || entry.SlgID == u.SlgId))
                    {
                        agroup.RemoveUser(u);
                        SaveData();
                    }

                    if (u.IsAdmin && !agroup.GroupUsers.Any(entry => entry.Name == u.Name && entry.SteamID == u.SteamId || entry.SlgID == u.SlgId))
                    {
                        agroup.AddUser(u);
                        SaveData();
                    }
                }
            }
        }

        public string GetStatus() => "Groups System Active";

        public override string ToString() => Localizer.DoStr("EM - Groups System");

        private static GroupsData LoadData()
        {
            return FileManager<GroupsData>.ReadTypeHandledFromFile(Defaults.SaveLocation + _subPath, _dataFile);
        }

        internal static void SaveData()
        {
            FileManager<GroupsData>.WriteTypeHandledToFile(Data, Defaults.SaveLocation + _subPath, _dataFile);
        }

        public string GetCategory() => "Elixr Mods";
    }

    [Priority(PriorityAttribute.High)]
    public class GroupsHandler : IModKitPlugin, IInitializablePlugin
    {
        public string GetStatus() => "";

        public override string ToString() => "EM - Groups Handler";

        public void Initialize(TimedTask timer)
        {
            //Reset Warp Points on new world generation to prevent adding of old warp points on a new world
            if (BasePlugin.Obj.Config.WipeGroupsFileOnFreshWorld)
                WorldGeneratorPlugin.OnFinishGenerate.AddUnique(HandleWorldReset);
        }

        public void HandleWorldReset()
        {
            ConsoleColors.PrintConsoleMultiColored(Defaults.appNameCon, System.ConsoleColor.Magenta, "New World Detected - Deleting Old Groups Data", System.ConsoleColor.White);
            if (File.Exists(Defaults.SaveLocation + GroupsManager._subPath + GroupsManager._dataFile))
                File.Delete(Defaults.SaveLocation + GroupsManager._subPath + GroupsManager._dataFile);
            GroupsManager.Data = new();
            GroupsManager.SaveData();
        }

        public string GetCategory() => "Elixr Mods";
    }
}

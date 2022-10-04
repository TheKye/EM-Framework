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
using System;
using System.Threading.Tasks;
using Eco.EM.Framework.Helpers;

namespace Eco.EM.Framework.Groups
{
    public class GroupsManager : Singleton<GroupsManager>, IModKitPlugin, IInitializablePlugin, IShutdownablePlugin
    {
        internal const string _dataFile = "ElixrMods-GroupsData.json";
        internal const string _dataBackupFile = "ElixrMods-GroupsData-Bakup.json";
        internal const string _subPath = "/EM/Groups";

        //Declaring the background task to periodically check for new admins added since server has been started.
        //Proposing the naming pattern for ids: ActionName_BackgroundTask and for actions to do methods: ActionName_BackgroundTaskRoutine
        //Lowering the time to 5 seconds from previously existing 10 seconds.
        public BackgroundTask AdminChecker_BackgroundTask = new(TimeSpan.FromMilliseconds(5000));

        public static GroupsData Data { get; internal set; }
        public static GroupsData DataBackup { get; internal set; }

        public static GroupsAPI API { get; private set; }

        public GroupsManager()
        {
            Data = ValidateDataFile();
            API = new GroupsAPI();

            if (!File.Exists(Defaults.SaveLocation + _subPath + _dataFile))
                SaveData();
        }

        private GroupsData ValidateDataFile()
        {

            Data = LoadData();

            if (Data != null)
            {
                return Data;
            }

            else
            {
                ConsoleColors.PrintConsoleMultiColored(Defaults.appNameCon, ConsoleColor.Magenta, "The Main Groups file was found to be corrupted, Loading from backup", ConsoleColor.Red);
                Data = LoadBackupData();
                SaveData();
                if (Data != null)
                    return Data;
                else
                {
                    ConsoleColors.PrintConsoleMultiColored(Defaults.appNameCon, ConsoleColor.Magenta, "There was an issue loading from the backup file. generating new files.", ConsoleColor.Red);
                    Data = new();
                    SaveData();
                    return Data;
                }
            }
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
                    if (!Data.AllUsers.Any(entry => entry.Name == usr.Name && (entry.SlgID == usr.SlgId || entry.SteamID == usr.SteamId)))
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

                    if (!group.GroupUsers.Any(entry => entry.Name == usr.Name && (entry.SlgID == usr.SlgId || entry.SteamID == usr.SteamId)))
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
                    if (!Data.AllUsers.Any(entry => entry.Name == u.Name && (entry.SlgID == u.SlgId || entry.SteamID == u.SteamId)))
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

                    if (!group.GroupUsers.Any(entry => entry.Name == u.Name && (entry.SlgID == u.SlgId || entry.SteamID == u.SteamId)))
                    {
                        group.AddUser(u);
                        SaveData();
                    }
                }
            });

            //Starting the background task and passing the method with the backgound task routine.
            AdminChecker_BackgroundTask.Start(AdminChecker_BackgroundTaskRoutine);
        }

        void AdminChecker_BackgroundTaskRoutine()
        {
            var users = PlayerUtils.Users;
            lock (Data.AllUsers)
            {
                foreach (var u in users)
                {
                    var agroup = Data.GetorAddGroup("admin");

                    if (!u.IsAdmin && agroup.GroupUsers.Any(entry => entry.Name == u.Name && (entry.SlgID == u.SlgId || entry.SteamID == u.SteamId)))
                    {
                        agroup.RemoveUser(u);
                        SaveData();
                    }

                    if (u.IsAdmin && !agroup.GroupUsers.Any(entry => entry.Name == u.Name && (entry.SlgID == u.SlgId || entry.SteamID == u.SteamId)))
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

        private static GroupsData LoadBackupData()
        {
            return FileManager<GroupsData>.ReadTypeHandledFromFile(Defaults.SaveLocation + _subPath, _dataBackupFile, ".bak");
        }

        internal static void SaveData()
        {
            FileManager<GroupsData>.WriteTypeHandledToFile(Data, Defaults.SaveLocation + _subPath, _dataFile);
            Task.Delay(2000);
            FileManager<GroupsData>.WriteTypeHandledToFile(Data, Defaults.SaveLocation + _subPath, _dataBackupFile, ".bak");
        }

        public string GetCategory() => "Elixr Mods";

        //We can add backgorund task here to ensure they are cancelled and disposed of on shutdown.
        //Not required on simple tasks, but if some more complex routine is present it is always safer.
        public async Task ShutdownAsync()
        {
            SaveData();
            await AdminChecker_BackgroundTask.StopAsync();
            await Task.CompletedTask;
        }
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
            if (File.Exists(Defaults.SaveLocation + GroupsManager._subPath + GroupsManager._dataBackupFile))
                File.Delete(Defaults.SaveLocation + GroupsManager._subPath + GroupsManager._dataBackupFile);
            GroupsManager.Data = new();
            GroupsManager.SaveData();
        }

        public string GetCategory() => "Elixr Mods";
    }
}

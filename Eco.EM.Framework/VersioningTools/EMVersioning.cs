using Eco.Core.Utils.Logging;
using Eco.EM.Framework.ChatBase;
using Eco.EM.Framework.Logging;
using Eco.EM.Framework.Networking;
using Eco.EM.Framework.Plugins;
using Eco.EM.Framework.VersioningTools;
using Eco.Gameplay.Players;
using Eco.Gameplay.Systems.Chat;
using Eco.Gameplay.Systems.Messaging.Chat.Commands;
using Eco.ModKit;
using Eco.Shared.Localization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Eco.EM.Framework
{
    [ChatCommandHandler]
    class BaseCommands
    {
        [ChatCommand("Display the current installed version of EM", "em-version")]
        public static void EMVersion(User user)
        {
            ChatBaseExtended.CBChat(string.Format(Defaults.appName) + " " + EMVersioning.GetChat(), user);
        }

        [ChatCommand("Display the current installed version of mods Using the EM Framework Mod Version Checker", "mod-versions")]
        public static void ModVersions(User user)
        {
            ChatBaseExtended.CBChat(string.Format(Defaults.appName) + " " + ModVersioning.GetChat(), user);
        }
    }

    public class EMVersioning
    {
        public static string modVersion;
        public static string needsUpdate;
        #region VersionChecking
        // Loops through all assemblies loaded and return the packInfo of those that contain "Elixr Solutions" as the Company name.
        private static Dictionary<string, string> GetEMInstalledInfo()
        {
            Dictionary<string, string> installedPacks = new();

            // recursively find all *.dll files in the mods folder.
            string[] filepaths = Directory.GetFiles(ModKitPlugin.ModDirectory, "*.dll", SearchOption.AllDirectories);

            foreach (var file in filepaths)
            {
                var info = FileVersionInfo.GetVersionInfo(file);

                if (info != null && info.CompanyName == "Elixr Solutions")
                {
                    if (!installedPacks.ContainsKey(info.ProductName))
                        installedPacks.Add(info.ProductName, info.ProductVersion);
                }
            }

            return installedPacks;
        }

        private class EMRequestObject
        {
            public string Status { get; set; }
            public Dictionary<string, string> rows { get; set; }
        }

        // returns a dictionary of the up to date EM packs and their Versions.
        private static EMRequestObject GetEMMasterInfo()
        {
            const string emWeb = "https://elixrmods.com/api/v1/GetVersions?apikey=uaRVGlDndFUIlwKJ";
            var request = Network.GetRequest(emWeb);
            return JsonConvert.DeserializeObject<EMRequestObject>(request.Result);
        }

        // Returns a formatted string of the installed EMPack versions for either console or chat.
        private static void EMVersion(bool console, out string chat)
        {
            chat = "";
            var logFile = NLogManager.GetLogWriter("Eco");
            try
            {
                var packs = GetEMInstalledInfo();
                var check = GetEMMasterInfo();
                string latestPack = "";

                if (check.Status != "200" || check.rows == null || check.rows.Count == 0)
                {
                    System.Console.ForegroundColor = ConsoleColor.Magenta;
                    System.Console.WriteLine(Localizer.DoStr(string.Format("Unable to contact webserver for latest pack info")));

                }
                else
                    latestPack = check.rows["EM Framework"];


                logFile.Write($"EM Framework - Installed Version: {packs["EM Framework"]} - Latest Version: {latestPack}");
                if (console)
                    PrintSinglePackConsole("EM Framework", packs["EM Framework"], latestPack);
                else
                    chat += PrintSingleStringChat("EM Framework", packs["EM Framework"], latestPack);

                int count = 0;
                // Go through other packs skipping EM Framework
                foreach (var p in packs)
                {
                    count++;
                    if (p.Key == "EM Framework")
                        continue;

                    if (check.rows.ContainsKey(p.Key))
                        latestPack = check.rows[p.Key];
                    else
                        latestPack = null;
                    logFile.Write($"{p.Key} - Installed Version: {p.Value} - Latest Version: {latestPack}");
                    if (console)
                        PrintSinglePackConsole(p.Key, p.Value, latestPack);
                    else
                        chat += PrintSingleStringChat(p.Key, p.Value, latestPack);
                }
            }
            catch (Exception)
            {
                if (console)
                {
                    System.Console.ForegroundColor = ConsoleColor.Magenta;
                    System.Console.WriteLine(Localizer.DoStr(string.Format("An error occured while attempting to check versions.")));
                    System.Console.WriteLine(Localizer.DoStr(string.Format("Current installed versions")));
                    System.Console.ResetColor();
                    //print the versions anyway so people know even on fail
                    var packs = GetEMInstalledInfo();
                    int count = 0;
                    foreach (var p in packs)
                    {
                        count++;
                        logFile.Write($"{p.Key} - Installed Version: {p.Value}");
                        if (console)
                            PrintSinglePackConsole(p.Key, p.Value, p.Value);
                        else
                            chat += PrintSingleStringChat(p.Key, p.Value, p.Value);
                    }
                }
                else
                {
                    var packs = GetEMInstalledInfo();
                    int count = 0;
                    foreach (var p in packs)
                    {
                        count++;
                        logFile.Write($"{p.Key} - Installed Version: {p.Value}");
                        if (console)
                            PrintSinglePackConsole(p.Key, p.Value, p.Value);
                        else
                        {
                            chat += string.Format("<color=red>{0}</color>", "An error occured while attempting to check versions.Please contact the EM Development Team");
                            chat += PrintSingleStringChat(p.Key, p.Value, p.Value);
                        }
                    }
                }
            }
        }

        private async static void CheckEMVersion()
        {
            await Task.Run(() =>
            {
                try
                {
                    var packs = GetEMInstalledInfo();
                    var check = GetEMMasterInfo();
                    string latestPack = "";

                    if (check.Status != "200" || check.rows == null || check.rows.Count == 0)
                    {
                        System.Console.ForegroundColor = ConsoleColor.Magenta;
                        System.Console.WriteLine(Localizer.DoStr(string.Format("Unable to contact webserver for latest pack info")));
                    }
                    else
                        latestPack = check.rows["EM Framework"];

                    int count = 0;
                    foreach (var p in packs)
                    {
                        count++;

                        if (check.rows.ContainsKey(p.Key))
                            latestPack = check.rows[p.Key];
                        else
                            latestPack = null;

                        CheckModVersion(p.Key, p.Value, latestPack);
                        if (!BasePlugin.Obj.Config.PostToDiscord && !CheckPackVersion(p.Value, latestPack).Result)
                            PrintSinglePackConsole(p.Key, p.Value, latestPack);
                    }
                }
                catch (Exception)
                {
                    System.Console.ForegroundColor = ConsoleColor.Magenta;
                    System.Console.WriteLine(Localizer.DoStr(string.Format("An error occured while attempting to check versions. Please contact the EM Development Team")));
                    System.Console.ResetColor();
                }
            });
        }

        private static async Task<bool> CheckPackVersion(string iVers, string mVers)
        {
            return (iVers == mVers);
        }

        // Formats the console output for the EM Pack Info.
        private static void PrintSinglePackConsole(string name, string iVers, string mVers)
        {
            bool match = CheckPackVersion(iVers, mVers).Result;
            // EM Pack Names should always be in Magenta
            // normal Text in Yellow
            // version text in Green if current version matches master version
            // version text in Red if current version lower than master version
            // Colors should also reset so it doesn't overwrite default colors
            // Added in time log to match in
            System.Console.ForegroundColor = ConsoleColor.DarkGreen;
            System.Console.Write(Localizer.DoStr($"[{DateTime.Now:hh:mm:ss}] "));

            System.Console.ForegroundColor = ConsoleColor.Magenta;
            System.Console.Write(Localizer.DoStr(string.Format("{0}", name)));

            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.Write(Localizer.DoStr(string.Format(" - Installed version ")));

            if (match && !string.IsNullOrWhiteSpace(mVers))
            {
                System.Console.ForegroundColor = ConsoleColor.Green;
                System.Console.Write(string.Format("({0}) \n", iVers));
            }
            else if (string.IsNullOrWhiteSpace(mVers))
            {
                System.Console.ForegroundColor = ConsoleColor.Green;
                System.Console.Write(string.Format("({0}) \n", iVers));
            }
            else
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.Write(string.Format("({0}) ", iVers));
            }
            if (!match && !string.IsNullOrWhiteSpace(mVers))
            {
                System.Console.ForegroundColor = ConsoleColor.Yellow;
                System.Console.Write(Localizer.DoStr(string.Format(": Latest Version ({0}) \n", mVers)));
                modVersion += "Mod Has an Update Available\n";
                modVersion += $"{name} - Installed Version: {iVers} - Latest Version: {mVers} \n\n";

            }
            System.Console.ResetColor();
        }

        private static void CheckModVersion(string name, string iVers, string mVers)
        {
            bool match = CheckPackVersion(iVers, mVers).Result;

            if (!match)
            {
                needsUpdate += "Mod Has an Update Available\n";
                needsUpdate += $"{name} - Installed Version: {iVers} - Latest Version: {mVers} \n\n";
            }
        }
        // Formats the game text output for the EM Pack Info.
        private static string PrintSingleStringChat(string name, string iVers, string mVers)
        {
            bool match = CheckPackVersion(iVers, mVers).Result;

            // EM Pack Names should always be in Magenta
            // normal Text in Yellow
            // version text in Green if current version matches master version
            // version text in Red if current version lower than master version
            StringBuilder sb = new();

            sb.Append(Localizer.DoStr(string.Format("<color=purple>{0}</color>", name)));
            sb.Append(Localizer.DoStr(string.Format(" <color=yellow>- Installed version </color>")));
            if (match)
                sb.Append(string.Format("<color=green>({0}) </color>\n", iVers));
            else
                sb.Append(string.Format("<color=red>({0}) </color>", iVers));

            if (!match && mVers != "")
                sb.Append(Localizer.DoStr(string.Format("<color=yellow>: Latest Version ({0}) </color>\n", mVers)));

            return sb.ToString();
        }

        public static void GetInit()
        {
            EMVersion(true, out string _);
            WebHook.PostToWebhook();
            LoggingUtils.Write(modVersion);
        }

        public static void CheckUpdate()
        {
            needsUpdate = string.Empty;
            modVersion = string.Empty;
            CheckEMVersion();
            if (BasePlugin.Obj.Config.PostToDiscord && !string.IsNullOrWhiteSpace(needsUpdate))
                WebHook.PostToWebhook();
            LoggingUtils.Write(needsUpdate);
        }

        public static string GetChat()
        {
            EMVersion(false, out string chat);

            return chat;
        }
        #endregion
    }
}

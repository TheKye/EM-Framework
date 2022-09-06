using Eco.EM.Framework.Networking;
using Eco.EM.Framework.ChatBase;
using Eco.Gameplay.Players;
using Eco.ModKit;
using Eco.Shared.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Eco.EM.Framework.VersioningTools
{
    public class ModVersioning
    {
        internal static HashSet<Dictionary<string, string>> ModVersions = new();
        private static Dictionary<string, string> GetModInstalledInfo(string ModIdentity)
        {
            Dictionary<string, string> installedPacks = new Dictionary<string, string>();

            string[] filepaths = Directory.GetFiles(ModKitPlugin.ModDirectory, "*.dll", SearchOption.AllDirectories);

            foreach (var file in filepaths)
            {
                var info = FileVersionInfo.GetVersionInfo(file);

                if (info != null && info.CompanyName == ModIdentity)
                {
                    if (!installedPacks.ContainsKey(info.ProductName))
                        installedPacks.Add(info.ProductName, info.ProductVersion);
                }
            }

            return installedPacks;
        }

        private class ModRequestObject
        {
            public int status { get; set; }
            public Dictionary<string, object> modfile { get; set; }
        }

        private static ModRequestObject GetModMasterInfo(string modId, string modApi)
        {
            try
            {
                string EcoModIo = $"https://api.mod.io/v1/games/6/mods/{modId}?api_key={modApi}";
                var request = Network.GetRequest(EcoModIo);
                return JsonConvert.DeserializeObject<ModRequestObject>(request);
            }
            catch
            {
                return new ModRequestObject() { status = 0};
            }
        }

        private static void ModVersion(string modId, string modApi, string ModIdentity, string displayName, ConsoleColor mainColor, string appName, ConsoleColor textColor, bool sendChat, bool overrideCheck)
        {
            var packs = GetModInstalledInfo(ModIdentity);
            var check = GetModMasterInfo(modId, modApi);
            string latestPack = "";
            if (check.status != 1)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine(Localizer.DoStr(string.Format("Unable to contact webserver for latest pack info")));
                return;
            }
            else
            latestPack = (string)check.modfile["version"];
            PrintSinglePackConsole(displayName, packs[appName], latestPack, overrideCheck, mainColor, textColor);
            if (sendChat)
            {
                UserManager.OnUserLoggedIn.Add(u =>
                {
                    if (u.IsAdmin)
                    {
                        ChatBaseExtended.CBError(string.Format("<color=red>Attention Admin!</color>" + " " + PrintSingleStringChat(displayName, packs[appName], latestPack, mainColor.ToString(), textColor.ToString(), overrideCheck)),u);
                    }
                });
            }
        }

        private static void PrintSinglePackConsole(string name, string iVers, string mVers, bool overrideCheck, ConsoleColor mainColor, ConsoleColor textColor = ConsoleColor.Yellow)
        {
            bool match = true;
            if(!overrideCheck)
                match = CheckPackVersion(iVers, mVers);

            Dictionary<string, string> mod = new();
            mod.Add(name, iVers );
            ModVersions.Add(mod);
            System.Console.ForegroundColor = ConsoleColor.DarkGreen;
            System.Console.Write(Localizer.DoStr($"[{DateTime.Now:hh:mm:ss}] "));

            System.Console.ForegroundColor = mainColor;
            System.Console.Write(Localizer.DoStr(string.Format("{0}", name)));

            System.Console.ForegroundColor = textColor;
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
            if (!match)
            {
                System.Console.ForegroundColor = textColor;
                System.Console.Write(Localizer.DoStr(string.Format(": Latest Version ({0}) \n", mVers)));
                EMVersioning.modVersion += "\n\nOther Mods Installed\n\n";
                EMVersioning.modVersion += "Mod Has an Update Available\n";
                EMVersioning.modVersion += "1." + name + " Installed Version: " + iVers + " Latest Version: " + mVers + "\n";
            }
            System.Console.ResetColor();
        }

        // Formats the game text output for the Mod Pack Info.
        private static string PrintSingleStringChat(string name, string iVers, string mVers, string mainColor, string secondColor, bool overrideCheck)
        {
            bool match = true;
            if(!overrideCheck)
                match = CheckPackVersion(iVers, mVers);

            StringBuilder sb = new();

            sb.Append(Localizer.DoStr(string.Format("<color={0}>{1}</color>", mainColor, name)));
            sb.Append(Localizer.DoStr(string.Format(" <color={0}>- Installed version </color>", secondColor)));
            if (match && !string.IsNullOrWhiteSpace(mVers))
            {
                sb.Append(string.Format("<color=green>({0}) </color>", iVers));
            }
            else if (string.IsNullOrWhiteSpace(mVers))
            {
                System.Console.ForegroundColor = ConsoleColor.Green;
                System.Console.Write(string.Format("({0}) \n", iVers));
            }
            else
            {
                sb.Append(string.Format("<color=red>({0}) </color>", iVers));
            }

            if (!match && !string.IsNullOrWhiteSpace(mVers))
                sb.Append(Localizer.DoStr(string.Format("<color=yellow>: Latest Version ({0}) </color>\n", mVers)));

            return sb.ToString();
        }

        private static bool CheckPackVersion(string iVers, string mVers)
        {
            return (iVers == mVers);
        }

        public static string GetChat()
        {
            StringBuilder sb = new();
            if(ModVersions.Count > 0)
            foreach (var i in ModVersions)
            {
                if(i != null)
                    foreach(var s in i)
                    {
                        if (s.Key != null) 
                        {
                            sb.Append(Localizer.DoStr(string.Format("<color=green>{0}</color>", s.Key)));
                            sb.Append(Localizer.DoStr(string.Format(" - Installed version: {0}\n", s.Value)));
                        }
                    }
            }
            else
            {
                sb.Append(Localizer.DoStr("You have no mods installed that Use EM Versioning Tools."));
            }
            return sb.ToString();
        }

        public static void GetModInit(string modId, string modApi, string ModIdentity, string displayName, ConsoleColor mainColor, string appName, ConsoleColor textColor = ConsoleColor.Yellow, bool sendChat = false, bool overrideCheck = false)
        {
            ModVersion(modId, modApi, ModIdentity, displayName, mainColor, appName, textColor, sendChat, overrideCheck);
        }
    }
}

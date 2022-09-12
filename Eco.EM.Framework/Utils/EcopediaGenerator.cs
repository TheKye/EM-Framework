using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Eco.Gameplay.EcopediaRoot;
using Eco.Shared.Utils;

namespace Eco.EM.Framework.Utils
{
    /// <summary>
    /// EcopediaGenerator, Automatically Generates the EcoPedia Files for your mods so you don't have to!
    /// </summary>
    public static class EcopediaGenerator
    {
        internal static Dictionary<string, string> subPages = new();
        internal static Dictionary<string, Dictionary<string, string>> pages = new();
        internal const string SavePath = "Mods/UserCode/Ecopedia/Mods";
        /// <summary>
        /// This method will autogenerate a File in a folder Called Ecopedia inside the usercode folder, the folder it will make will be the modName param
        /// This will present as: Mods/Usercode/Ecopedia/modName/
        /// When using this method please use a String Builder to create the entry
        /// 
        /// </summary>
        /// <param name="information"></param>
        /// <param name="categoryName"></param>
        /// <param name="pageName"></param>
        /// <param name="modName"></param>
        public static bool GenerateEcopediaPage(string information, string categoryName, string pageName, string modName, bool isSubPage = false, string mainPageName = "")
        {
            string fileName;
            if (isSubPage)
                fileName = categoryName + ";" + mainPageName + ";" + pageName;
            else
                fileName = categoryName + ";" + pageName;

            StringBuilder sb = new();

            sb.Append($"<ecopedia icon=\"gear\">\n");
            sb.Append($"<section type=\"header\">{pageName}</section>\n");
            sb.Append($"{information}\n");
            sb.Append($"</section>\n");
            sb.Append($"</ecopedia>");

            Random rnd = new();
            Dictionary<string, string> details = new()
            {
                { modName, sb.ToString() }
            };
            if (pages.ContainsKey(fileName))
                pages.Add(fileName + "-" + modName + rnd.ToString(), details);
            else
                pages.Add(fileName, details);

            Logging.LoggingUtils.Debug($"Added new Ecopedia file at {SavePath}{modName}");
            return true;
        }

        /// <summary>
        /// 
        /// ModNamespace: This is your DLL root namespace + folder structure
        /// IE: DiscordLink.Ecopedia.CustomFile
        /// or
        /// DiscordLink.Documents.Ecopedia
        /// Don't add a period at the end of your namespace this is handled by this method
        /// 
        /// fileName is the files fully qualified name. IE: EcopediaPage.txt
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="modNamespace"></param>
        /// <returns></returns>
        public static bool GenerateEcopediaPageFromFile(string fileName, string modNamespace, string modName, bool isSubPage = false)
        {
            var assembly = Assembly.GetCallingAssembly();
            var resourceName = modNamespace + "." + fileName;
            string resource = null;
            var cleanName = fileName.Split(".")[0];
            try
            {
                using Stream stream = assembly.GetManifestResourceStream(resourceName);
                using StreamReader reader = new(stream);
                resource = reader.ReadToEnd();
            }
            catch (Exception e)
            {
                //debugging
                Logging.LoggingUtils.Error($"There was an error adding the ecopedia file for this mod: {modName}, Error was: \n{e}\n\nIf the error was about a null reference for the stream, then your path to your file is not Correct. Please Check the path: {resourceName}");
                Log.WriteErrorLineLoc($"There was an error adding an ecopedia file for this mod: {modName}.");
                return false;
            }
            Random rnd = new();
            Dictionary<string, string> details = new()
            {
                { modName, resource }
            };

            if (pages.ContainsKey(cleanName))
                pages.Add(cleanName + "-" + modName + rnd.ToString(), details);
            else
                pages.Add(cleanName, details);
            if (isSubPage)
                subPages.Add(cleanName.Split(";")[1], cleanName.Split(";")[2]);

            Logging.LoggingUtils.Debug($"Added new Ecopedia file at {SavePath}{modName}");
            return true;
        }

        internal static void ClearOld()
        {
            if (Directory.Exists(SavePath))
                Directory.Delete(SavePath);
        }

        internal static void BuildPages()
        {
            foreach (var mod in pages)
            {
                foreach (var p in mod.Value)
                {
                    var fileName = mod.Key.Split("-")[0];
                    if (!File.Exists(SavePath + p.Key + "/" + fileName + ".xml"))
                    {
                        FileManager.FileManager.WriteToFile(p.Value, SavePath + p.Key, fileName, ".xml");

                        Logging.LoggingUtils.Debug($"Added new Ecopedia file");
                    }
                }

            }
        }

        internal static void BuildSubPages()
        {
            foreach (var p in subPages)
            {
                var parentPage = Ecopedia.Obj.GetPage(p.Key);
                var page = Ecopedia.Obj.GetPage(p.Value);

                parentPage.SubPages.Add(p.Value, page);

                Logging.LoggingUtils.Debug($"Added new Ecopedia sub page");
            }
        }
    }
}

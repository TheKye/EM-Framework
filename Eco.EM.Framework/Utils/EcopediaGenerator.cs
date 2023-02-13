using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Eco.Gameplay.EcopediaRoot;
using Eco.Shared.Localization;
using Eco.Shared.Utils;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Eco.EM.Framework.Utils
{
    /// <summary>
    /// EcopediaGenerator, Automatically Generates the EcoPedia Files for your mods so you don't have to!
    /// </summary>
    public class EcopediaGenerator : Singleton<EcopediaGenerator>
    {
        internal static Dictionary<string, string> subPages = new();
        internal static Dictionary<string, Dictionary<string, string>> pages = new();
        internal const string SavePath = "Mods\\UserCode\\Ecopedia\\";
        private static readonly Ecopedia eco;

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
        public static bool GenerateEcopediaPage(string information, string pageName, string modName, bool isSubPage = false, string categoryName = "", string mainPageName = "", string icon = "")
        {
            if (string.IsNullOrWhiteSpace(categoryName))
                categoryName = "Documentation";
            if (string.IsNullOrWhiteSpace(icon))
                icon = "GearboxItem";

            string fileName;
            if (isSubPage)
                fileName = categoryName + ";" + mainPageName + ";" + pageName;
            else
                fileName = categoryName + ";" + pageName;

            StringBuilder sb = new();

            sb.Append($"<ecopedia icon=\"{icon}\">\n");
            sb.Append($"<section type=\"header\">{pageName}</section>\n");
            sb.Append($"{information}\n");
            sb.Append($"</section>\n");
            sb.Append($"</ecopedia>");

            Random rnd = new();
            Dictionary<string, string> details = new()
            {
                { modName, sb.ToString() }
            };

            pages.Add(fileName + "-" + modName + rnd.ToString(), details);

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
            var cleanName = fileName.Split(".")[0];
            string resource;
            try
            {
                resource = WritingUtils.ReadFromEmbeddedResource(assembly, modNamespace, fileName);
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

            pages.Add(cleanName + "-" + modName + rnd.ToString(), details);

            if (isSubPage)
                subPages.Add(cleanName.Split(";")[1], cleanName.Split(";")[2]);

            if (File.Exists(SavePath + modName + "\\" + fileName + ".xml"))
                File.Delete(SavePath + modName + "\\" + fileName + ".xml");

            Logging.LoggingUtils.Debug($"Added new Ecopedia file at {SavePath}{modName}");
            return true;
        }

        internal static async void BuildPages()
        {
            await Task.Run(() =>
            {
                foreach (var mod in pages)
                {
                    foreach (var p in mod.Value)
                    {
                        //lets try using XML Writer to fix an issue
                        var fileName = mod.Key.Split("-")[0];
                        var sb = new StringBuilder();
                        sb.Append(p.Value);
                        if (!File.Exists(SavePath + p.Key + "\\" + fileName + ".xml"))
                        {
                            try
                            {
                                FileManager.FileManager.WriteToFile(sb.ToString(), SavePath + p.Key, fileName, ".xml");
                            }
                            catch
                            {

                            }
                            Logging.LoggingUtils.Debug($"Added new Ecopedia file");
                        }

                        if (subPages.Count > 0)
                        {
                            foreach (var sp in subPages)
                            {
                                try
                                {
                                    var modname = mod.Key.Split("-")[0];
                                    var cleanName = modname.Split(";");
                                    if (sp.Key.ToLower() == cleanName[1].ToLower())
                                    {
                                        var mainpage = Ecopedia.Obj.GetPage(cleanName[1]);
                                        var cat = eco.Categories.GetOrAdd("Mods");
                                        var page = cat.Pages.GetOrAdd(mainpage.Name);

                                        var subpage = page.SubPages.GetOrAdd(cleanName[2]);
                                        subpage.Name = sp.Value;
                                        subpage.FullName = page.FullName + ';' + sp.Value;
                                        subpage.DisplayName = Localizer.DoStr(sp.Value);

                                        mainpage.SubPages.Add(subpage.Name, subpage);
                                    }
                                }
                                catch
                                {
                                    continue;
                                }
                            }
                        }
                    }

                }
            });
        }

        internal static Task ShutDown()
        {
            foreach (var mod in pages)
            {
                foreach (var p in mod.Value)
                {
                    var fileName = mod.Key.Split("-")[0];
                    if (File.Exists(SavePath + p.Key + "\\" + fileName + ".xml"))
                        File.Delete(SavePath + p.Key + "\\" + fileName + ".xml");
                }
            }

            return Task.CompletedTask;
        }

    }
}

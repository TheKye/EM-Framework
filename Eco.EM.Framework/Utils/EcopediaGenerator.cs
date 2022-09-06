using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Eco.EM.Framework.FileManager;

namespace Eco.EM.Framework.Utils
{
    /// <summary>
    /// EcopediaGenerator, Automatically Generates the EcoPedia Files for your mods so you don't have to!
    /// </summary>
    public static class EcopediaGenerator
    {
        internal const string SavePath = "Mods/UserCode/Ecopedia/";
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
        public static bool GenerateEcopediaPage(string information, string categoryName, string pageName, string modName)
        {
            var fileName = categoryName + ";" + pageName;
            if(!File.Exists(SavePath + modName + "/" + fileName + ".xml"))
            {
                FileManager.FileManager.WriteToFile(information, SavePath + modName, fileName, ".xml");
                Logging.LoggingUtils.Debug($"Added new Ecopedia file at {SavePath}{modName}");
                return true;
            }
            return false;
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
        public static bool GenerateEcopediaPageFromFile(string fileName, string modNamespace, string modName)
        {
            var assembly = Assembly.GetCallingAssembly();
            var resourceName = modNamespace + "." + fileName;

            string resource = null;
            try
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        resource = reader.ReadToEnd();
                    }
                }
                if (!File.Exists(SavePath + modName + "/" + fileName.Split(".")[0] + ".xml"))
                {
                    FileManager.FileManager.WriteToFile(resource, SavePath + modName, fileName.Split(".")[0], ".xml");
                    Logging.LoggingUtils.Debug($"Added new Ecopedia file at {SavePath}{modName}");

                    return true;
                }

                return false;
            }
            catch(Exception e)
            {
                //debugging
                Logging.LoggingUtils.Warning($"{assembly}");
                Logging.LoggingUtils.Error($"There was an error adding the ecopedia file for this mod: {modName}, Error was: \n{e}\n\nIf the error was about a null reference for the stream, then your path to your file is not Correct. Please Check the path: {resourceName}");
                return false;
            }
        }
    }
}

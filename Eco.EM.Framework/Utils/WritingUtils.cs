using Eco.Shared.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Eco.EM.Framework.Utils
{
    public class WritingUtils
    {
        /// <summary>
        /// Read From Embedded Resource only reads from an Embedded File. this can be used for storing special values in a text file and reading from the file to use those values
        /// <para>Example: EcopediaGenerator Util</para>
        /// <para>Exception Handling is taken care of by this method so you only need to check for string.IsNullOrWhiteSpace(); Error Reporting will be Logged to the Console only if you set debug to true</para>
        /// 
        /// <para>usingnamespace: this is the fully Qualified Assembly name to the resource (Folder included) Example: Eco.EM.Framework.SpecialItems (Here SpecialItems is the folder name)</para>
        /// <para>filename: the name of the file and its file extension example: HiddenValues.txt</para>
        /// 
        /// 
        /// In order for this system to work, the files properties must be set to EmbeddedResource in your IDE
        /// </summary>
        /// <param name="usingnamespace"></param>
        /// <param name="filename"></param>
        /// <returns>string or empty string</returns>
        private static string ReadFromEmbeddedResource(string usingNamespace, string fileName, bool debug = false)
        {
            var assembly = Assembly.GetCallingAssembly();
            var resourceName = usingNamespace + "." + fileName;
            try
            {
                using Stream stream = assembly.GetManifestResourceStream(resourceName);
                using StreamReader reader = new(stream);
                return reader.ReadToEnd();
            }
            catch (Exception e)
            {
                if (debug)
                    Log.WriteErrorLineLocStr($"[EM Framework] (Writing Utils) There was an error reading the Resource Provided: Exception was: \n{e}\nEmpty String was returned to prevent server breaking.");
                return "";
            }
        }

        /// <summary>
        /// Read From Embedded Resource only reads from an Embedded File. this can be used for storing special values in a text file and reading from the file to use those values
        /// <para>Example: EcopediaGenerator Util</para>
        /// <para>Exception Handling is taken care of by this method so you only need to check for string.IsNullOrWhiteSpace(); Error Reporting will be Logged to the Console only if you set debug to true</para>
        /// <para>Assembly is your Assembly, so just do: var assem = Assembly.GetExecutingAssembly();</para> 
        /// <para>usingnamespace: this is the fully Qualified Assembly name to the resource (Folder included) Example: Eco.EM.Framework.SpecialItems (Here SpecialItems is the folder name)</para>
        /// <para>filename: the name of the file and its file extension example: HiddenValues.txt</para>
        /// 
        /// 
        /// In order for this system to work, the files properties must be set to EmbeddedResource in your IDE
        /// </summary>
        /// <param name="usingnamespace"></param>
        /// <param name="filename"></param>
        /// <returns>string or empty string</returns>
        public static string ReadFromEmbeddedResource(Assembly assembly, string usingNamespace, string fileName, bool debug = false)
        {
            var resourceName = usingNamespace + "." + fileName;
            try
            {
                using Stream stream = assembly.GetManifestResourceStream(resourceName);
                using StreamReader reader = new(stream);
                return reader.ReadToEnd();
            }
            catch (Exception e)
            {
                if (debug)
                    Log.WriteErrorLineLocStr($"[EM Framework] (Writing Utils) There was an error reading the Resource Provided: Exception was: \n{e}\nEmpty String was returned to prevent server breaking.");
                return "";
            }
        }
        /// <summary>
        /// WriteFromEmbeddedResource will read the embedded resource from the location passed to this method and then write it out to file with the Extension provided at the location specified.
        /// This can be good for doing easy vanilla overrides without doing it by hand and just embedding resources in your DLL
        /// 
        /// <para>usingNamespace: this is the fully Qualified Assembly name to the resource (Folder included) Example: Eco.EM.Framework.SpecialItems (Here SpecialItems is the folder name)</para>
        /// <para>fileName: the name of the file and its file extension example: HiddenValues.txt</para>
        /// <para>fileExtension: the extension you want the file to have, this could be anything as the FileManager Handles writing out the file and supports many different writeout types</para>
        /// <para>locationToWriteTo: What Directory you want the file written to, it gets the servers root folder by default then you just specify the folder path. Example: Mods/UserCode/EcoPedia</para>
        /// <para>SpecificFileName: The filename without the Extension, as this is handled by the fileExtension param.</para>
        /// 
        /// It makes use of the ReadFromEmbeddedResource which handles Exception catching and error reporting (Error reporting only happens if Debug is set to true)
        /// </summary>
        /// <param name="usingNamespace"></param>
        /// <param name="fileName"></param>
        /// <param name="locationToWriteTo"></param>
        /// <param name="fileExtension"></param>
        /// <param name="specificFileName"></param>
        public static void WriteFromEmbeddedResource(string usingNamespace, string fileName, string locationToWriteTo, string fileExtension, bool debug = false, string specificFileName = "")
        {
            string resource = "";
            locationToWriteTo = Directory.GetCurrentDirectory() + "\\" + locationToWriteTo;
            var cleanName = fileName.Split(".")[0];
            try
            {
                resource = ReadFromEmbeddedResource(usingNamespace, fileName, debug);
            }
            catch { }
            if (!string.IsNullOrWhiteSpace(resource))
            {
                if (string.IsNullOrWhiteSpace(specificFileName))
                    FileManager.FileManager.WriteToFile(resource, locationToWriteTo, cleanName, fileExtension);
                else
                    FileManager.FileManager.WriteToFile(resource, locationToWriteTo, specificFileName, fileExtension);
            }
        }
    }
}

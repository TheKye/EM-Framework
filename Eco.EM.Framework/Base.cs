using Eco.EM.Framework.FileManager;
using System;
using System.IO;
using System.Reflection;

namespace Eco.EM.Framework
{
    public static class Defaults
    {
        internal const string oldLocation = "ElixrMods";
        internal const string fileFormat = ".json";
        private static readonly string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        private static readonly string saveLocation = Path.Combine("Configs","Mods");
        public const string appName = "<color=purple>[EM Framework]:</color> ";
        public const string appNameCon = "[EM Framework]: ";
        public static string Version => version;

        public static string SaveLocation => GetRelevantDirectory();

        public static string AssemblyLocation => Directory.GetCurrentDirectory();

        public static double UTime => DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

        static string GetRelevantDirectory()
        {
            if (saveLocation.StartsWith(Path.DirectorySeparatorChar))
            {
                return Path.Combine(AssemblyLocation, saveLocation);
            }
            return saveLocation;
        }

        static void CreateDirectoryIfNotExist() => CreateDirectoryIfNotExist(SaveLocation);
        public static void CreateDirectoryIfNotExist(string Path)
        {
            if (!Directory.Exists(SaveLocation))
            {
                Directory.CreateDirectory(SaveLocation);
            }
            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }
        }

        static string GetPathOf(string FileName)
        {
            if (FileName.Contains(fileFormat))
            {
                return Path.Combine(SaveLocation, FileName);
            }

            return Path.Combine(SaveLocation, FileName + fileFormat);
        }

        public static bool ConfigExists(string FileName) => File.Exists(GetPathOf(FileName));

        static string GetPath(string FileName)
        {
            if (!FileName.EndsWith(fileFormat))
            {
                FileName += fileFormat;
            }

            return Path.Combine(SaveLocation, FileName);
        }

        public static BaseConfig Config
        {
            get
            {
                return FileManager<BaseConfig>.ReadFromFile(SaveLocation, "Base.EM");
            }
            set
            {
                FileManager<BaseConfig>.WriteToFile(value, SaveLocation, "Base.EM");
            }
        }     
    }
}
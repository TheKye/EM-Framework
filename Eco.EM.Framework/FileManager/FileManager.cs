using Eco.Core.Serialization;
using Eco.EM.Framework.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;

namespace Eco.EM.Framework.FileManager
{
using System;
    public static class FileManager<T> where T : class, new()
    {
        public static bool WriteToFile(T input, string SavePath, string FileName)
        {
            try
            {
                var stringContent = SerializationUtils.SerializeRawJsonIndented(input);

                FileManager.WriteToFile(stringContent, SavePath, FileName);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("FileManager: " + e.Message);

                return false;
            }
        }

        public static bool WriteToFile(T input, string SavePath, string FileName, string extension)
        {
            try
            {
                var stringContent = SerializationUtils.SerializeRawJsonIndented(input);

                FileManager.WriteToFile(stringContent, SavePath, FileName, extension);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("FileManager: " + e.Message);

                return false;
            }
        }

        public static T ReadFromFile(string SavePath, string FileName)
        {
            T content = new T();

            var stringContent = FileManager.ReadFromFile(SavePath, FileName);

            if (string.IsNullOrWhiteSpace(stringContent))
                return new T();
            else
                content = Eco.Core.Serialization.SerializationUtils.DeserializeJson<T>(stringContent);

            return content;
        }

        public static T ReadFromFile(string SavePath, string FileName, string extension)
        {
            T content = new T();

            var stringContent = FileManager.ReadFromFile(SavePath, FileName, extension);

            if (string.IsNullOrWhiteSpace(stringContent))
                return new T();
            else
                content = Eco.Core.Serialization.SerializationUtils.DeserializeJson<T>(stringContent);

            return content;
        }

        public static bool WriteTypeHandledToFile(T input, string savePath, string fileName)
        {
            try
            {
                var stringContent = JsonConvert.SerializeObject(input, Formatting.Indented, CreateSerializerSettings());

                FileManager.WriteToFile(stringContent, savePath, fileName);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("FileManager: " + e.Message);

                return false;
            }
        }

        public static bool WriteTypeHandledToFile(T input, string savePath, string fileName, string extension)
        {
            try
            {
                var stringContent = JsonConvert.SerializeObject(input, Formatting.Indented, CreateSerializerSettings());

                FileManager.WriteToFile(stringContent, savePath, fileName, extension);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("FileManager: " + e.Message);

                return false;
            }
        }

        public static T ReadTypeHandledFromFile(string SavePath, string FileName)
        {
            T content = new T();
            
            var stringContent = FileManager.ReadFromFile(SavePath, FileName);

            if (string.IsNullOrWhiteSpace(stringContent))
                return new T();
            else
                content = JsonConvert.DeserializeObject<T>(stringContent, CreateSerializerSettings());

            return content;
        }

        public static T ReadTypeHandledFromFile(string SavePath, string FileName, string extension)
        {
            T content = new T();

            var stringContent = FileManager.ReadFromFile(SavePath, FileName, extension);

            if (string.IsNullOrWhiteSpace(stringContent))
                return new T();
            else
                content = JsonConvert.DeserializeObject<T>(stringContent, CreateSerializerSettings());

            return content;
        }

        private static JsonSerializerSettings CreateSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Error,
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                Converters = { new StringEnumConverter(), new JavaScriptDateTimeConverter(), new GuidConverter() },
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                ContractResolver = new EMJsonResolver(),
            };
        }
    }

    public class FileManager
    {
        const string format = ".json";
        const string ecoFormat = ".eco";

        public static void WriteToFile(string Input, string SavePath, string FileName)
        {
            if (!Directory.Exists(SavePath))
                Directory.CreateDirectory(SavePath);

            if (!FileName.EndsWith(format) && !FileName.EndsWith(ecoFormat))
                FileName += format;

            using (var file = new StreamWriter(Path.Combine(SavePath, FileName)))
            {
                file.Write(Input);
            }
        }
        //Allows for custom Extensions
        public static void WriteToFile(string Input, string SavePath, string FileName, string extension)
        {
            if (!Directory.Exists(SavePath))
                Directory.CreateDirectory(SavePath);

            using (var file = new StreamWriter(Path.Combine(SavePath, FileName + extension)))
            {
                file.Write(Input);
            }
        }

        public static string ReadFromFile(string SavePath, string FileName)
        {
            if (!FileName.EndsWith(format) && !FileName.EndsWith(ecoFormat))
                FileName += format;

            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
                return string.Empty;
            }

            if (!File.Exists(Path.Combine(SavePath, FileName)))
                return string.Empty;

            var content = string.Empty;

            using (var file = new StreamReader(Path.Combine(SavePath, FileName)))
            {
                content = file.ReadToEnd();
                file.Close();
                file.Dispose();
            }

            return content;
        }

        public static string ReadFromFile(string SavePath, string FileName, string extension)
        {

            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
                return string.Empty;
            }

            if (!File.Exists(Path.Combine(SavePath, FileName + extension)))
                return string.Empty;

            var content = string.Empty;

            using (var file = new StreamReader(Path.Combine(SavePath, FileName + extension)))
            {
                content = file.ReadToEnd();
                file.Close();
                file.Dispose();
            }

            return content;
        }
    }
}

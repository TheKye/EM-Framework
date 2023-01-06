using Eco.Core.Utils.Logging;
using Eco.Shared.Localization;
using Eco.Shared.Utils;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Eco.EM.Framework.Logging
{
    public class LoggingUtils
    {
        private static string emLogger = "EM-Framework";

        private static NLogWriter Logging = NLogManager.GetLogWriter(emLogger);
        private static Dictionary<string, string> loged = new();

        public static NLogWriter Logger => GetLogger(Assembly.GetCallingAssembly());

        public static void RegisterNewLogger(string log)
        {
            var assembly = Assembly.GetCallingAssembly().GetName().Name;
            if (loged.ContainsKey(assembly))
            {
                Log.WriteWarningLineLoc($"[EM Framework]: Mod Error: Duplicate Key Entry: Logger Already Registered: Loggers can only be registered once per assembly, please use the logger you already register (This can be ignored)");
                return;
            }
            loged.Add(assembly, log);
        }

        private static NLogWriter GetLogger(Assembly assembly)
        {
            var assem = assembly.GetName().Name;
            foreach (var l in loged)
            {
                if (assem == l.Key)
                    return NLogManager.GetLogWriter(l.Value);
            }
            return Logging;
        }

        public static void Write(string s) => Logger.Write($"[{DateTime.Now:hh:mm:ss}] " + s);

        public static void Debug(string s) => Logger.Debug($"[{DateTime.Now:hh:mm:ss}] " + s);

        public static void Warning(string s) => Logger.WriteWarning($"[{DateTime.Now:hh:mm:ss}] " + s);

        public static void Error(string s) => Logger.WriteError($"[{DateTime.Now:hh:mm:ss}] " + s);

        public static void LogTypeSelect(string message, LogType logType)
        {
            message = Localizer.DoStr(message);

            switch (logType)
            {
                case LogType.Info:
                    Logger.Write($"[{DateTime.Now:hh:mm:ss}] " + message);
                    return;
                case LogType.Error:
                    Logger.WriteError($"[{DateTime.Now:hh:mm:ss}] " + message);
                    return;
                case LogType.Warn:
                    Logger.WriteWarning($"[{DateTime.Now:hh:mm:ss}] " + message);
                    return;
                case LogType.Debug:
                    Logger.Debug($"[{DateTime.Now:hh:mm:ss}] " + message);
                    return;
                case LogType.Important:
                    Logger.WriteError($"[{DateTime.Now:hh:mm:ss}] " + message);
                    Log.WriteError(Localizer.DoStr(message));
                    return;
                default:
                    Logger.Write($"[{DateTime.Now:hh:mm:ss}] " + message);
                    return;
            }
        }

        public enum LogType
        {
            Info,
            Error,
            Debug,
            Warn,
            Important
        }
    }
}

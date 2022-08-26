using Eco.Core.Utils.IO;
using Eco.Core.Utils.Logging;
using Eco.Shared.Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eco.EM.Framework.Utils
{
    public static class ConsoleColors
    {
        public static void PrintConsoleColored(string input, ConsoleColor color, bool logfile = true)
        {
            lock (ConsoleLogWriter.Instance)
            {
                ConsoleSynchronizationContext.Instance.Post(state =>
                {
                    System.Console.ForegroundColor = ConsoleColor.DarkGreen;
                    System.Console.Write(Localizer.DoStr($"[{DateTime.Now:hh:mm:ss}] "));

                    System.Console.ForegroundColor = color;
                    System.Console.Write(Localizer.DoStr(string.Format("{0}\n", input)));

                    System.Console.ForegroundColor = ConsoleColor.White;
                    System.Console.ResetColor();
                }, input);
            }

            var logFile = NLogManager.GetLogWriter("Eco");
            if (logfile)
                logFile.Write(string.Format("{0}", input));
        }

        public static void PrintConsoleMultiColored(string input, ConsoleColor color, string input2, ConsoleColor color2, bool logfile = true)
        {
            var state = input + input2;
            lock (ConsoleLogWriter.Instance)
            {
                ConsoleSynchronizationContext.Instance.Post(state =>
                {
                    System.Console.ForegroundColor = ConsoleColor.DarkGreen;
                    System.Console.Write(Localizer.DoStr($"[{DateTime.Now:hh:mm:ss}] "));

                    System.Console.ForegroundColor = color;
                    System.Console.Write(Localizer.DoStr(string.Format("{0}", input)));

                    System.Console.ForegroundColor = color2;
                    System.Console.Write(Localizer.DoStr(string.Format("{0}\n", input2)));

                    System.Console.ForegroundColor = ConsoleColor.White;
                    System.Console.ResetColor();
                }, state);
            }
            var logFile = NLogManager.GetLogWriter("Eco");
            if (logfile)
                logFile.Write(string.Format("{0}{1}", input, input2));
        }

        public static void PrintConsoleMultiColored(string input, ConsoleColor color, string input2, ConsoleColor color2, string input3, ConsoleColor color3, bool logfile = true)
        {
            var state = input + input2 + input3;
            lock (ConsoleLogWriter.Instance)
            {
                ConsoleSynchronizationContext.Instance.Post(state =>
                {
                    System.Console.ForegroundColor = ConsoleColor.DarkGreen;
                    System.Console.Write(Localizer.DoStr($"[{DateTime.Now:hh:mm:ss}] "));

                    System.Console.ForegroundColor = color;
                    System.Console.Write(Localizer.DoStr(string.Format("{0}", input)));

                    System.Console.ForegroundColor = color2;
                    System.Console.Write(Localizer.DoStr(string.Format("{0}", input2)));

                    System.Console.ForegroundColor = color3;
                    System.Console.Write(Localizer.DoStr(string.Format("{0}\n", input3)));

                    System.Console.ForegroundColor = ConsoleColor.White;
                    System.Console.ResetColor();
                }, state);
            }

            var logFile = NLogManager.GetLogWriter("Eco");
            if (logfile)
                logFile.Write(string.Format("{0}{1}{2}", input, input2, input3));
        }

        public static void PrintConsoleMultiColored(string input, ConsoleColor color, string input2, ConsoleColor color2, string input3, ConsoleColor color3, string input4, ConsoleColor color4, bool logfile = true)
        {
            var state = input + input2 + input3 + input4;
            lock (ConsoleLogWriter.Instance)
            {
                ConsoleSynchronizationContext.Instance.Post(state =>
                {
                    System.Console.ForegroundColor = ConsoleColor.DarkGreen;
                    System.Console.Write(Localizer.DoStr($"[{DateTime.Now:hh:mm:ss}] "));

                    System.Console.ForegroundColor = color;
                    System.Console.Write(Localizer.DoStr(string.Format("{0}", input)));

                    System.Console.ForegroundColor = color2;
                    System.Console.Write(Localizer.DoStr(string.Format("{0}", input2)));

                    System.Console.ForegroundColor = color3;
                    System.Console.Write(Localizer.DoStr(string.Format("{0}", input3)));

                    System.Console.ForegroundColor = color4;
                    System.Console.Write(Localizer.DoStr(string.Format("{0}\n", input4)));

                    System.Console.ForegroundColor = ConsoleColor.White;
                    System.Console.ResetColor();
                }, state);
            }

            var logFile = NLogManager.GetLogWriter("Eco");
            if (logfile)
                logFile.Write(string.Format("{0}{1}{2}{3}", input, input2, input3, input4));
        }
    }
}

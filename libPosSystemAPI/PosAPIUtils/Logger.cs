using System;

namespace fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIUtils
{
    public interface ILoggerTarget
    {
        void LogDebug(string message);
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message);
    }

    public class ConsoleLoggerTarget : ILoggerTarget
    {
        public void LogDebug(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("[DEBUG] " + message);
            Console.ResetColor();
        }

        public void LogInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("[INFO] " + message);
            Console.ResetColor();
        }

        public void LogWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("[WARNING] " + message);
            Console.ResetColor();
        }

        public void LogError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[ERROR] " + message);
            Console.ResetColor();
        }
    }

    public static class Logger
    {
        private static ILoggerTarget? _target = new ConsoleLoggerTarget();

        /// <summary>
        /// Sets the logger target for logging messages.
        /// Default, if never called is ConsoleLoggerTarget.
        /// </summary>
        public static void SetLoggerTarget(ILoggerTarget target)
        {
            _target = target;
        }

        public static void LogDebug(string message)
        {
            _target?.LogDebug(message);
        }

        public static void LogError(string message)
        {
            _target?.LogError(message);
        }

        public static void LogInfo(string message)
        {
            _target?.LogInfo(message);
        }

        public static void LogWarning(string message)
        {
            _target?.LogWarning(message);
        }
    }
}
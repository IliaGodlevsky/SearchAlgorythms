﻿using NLog;
using Pathfinding.Logging.Interface;

namespace Pathfinding.Logging.Loggers
{
    public sealed class FileLog : ILog
    {
        private readonly ILogger infoLogger;
        private readonly ILogger debugLogger;
        private readonly Logger errorLogger;
        private readonly ILogger traceLogger;

        public FileLog()
        {
            infoLogger = LogManager.GetLogger("Info");
            errorLogger = LogManager.GetLogger("Error");
            debugLogger = LogManager.GetLogger("Debug");
            traceLogger = LogManager.GetLogger("Trace");
        }

        public void Trace(string message)
        {
            Write(message, traceLogger.Trace);
        }

        public void Warn(Exception ex, string message = null)
        {
            Write(ex, message, errorLogger.Warn);
        }

        public void Warn(string message)
        {
            errorLogger.Warn(message);
        }

        public void Error(Exception ex, string message = null)
        {
            Write(ex, message, errorLogger.Error);
        }

        public void Error(string message)
        {
            errorLogger.Error(message);
        }

        public void Fatal(Exception ex, string message = null)
        {
            Write(ex, message, errorLogger.Fatal);
        }

        public void Fatal(string message)
        {
            errorLogger.Fatal(message);
        }

        public void Info(string message)
        {
            Write(message, infoLogger.Info);
        }

        public void Debug(string message)
        {
            Write(message, debugLogger.Debug);
        }

        private static void Write(string message, Action<string> action) => action(message);

        private static void Write(Exception ex, string message,
            Action<Exception, string> action)
        {
            action(ex, string.IsNullOrEmpty(message) ? string.Empty : message);
        }
    }
}

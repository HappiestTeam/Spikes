using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Aquiles.Logging
{
    internal class LoggerHelper
    {
        private static ILogger LOGGERBRIDGE;
        private static Dictionary<Type, LoggerHelper> LOGGERHELPERS;
        static LoggerHelper()
        {
            LOGGERHELPERS = new Dictionary<Type, LoggerHelper>();
        }
        public static void CreateBridge(Type bridgeClassType)
        {
            LOGGERBRIDGE = (ILogger)Activator.CreateInstance(bridgeClassType);
        }
        public static LoggerHelper CreateLogger(Type classType)
        {
            LoggerHelper helper = null;
            lock (LOGGERHELPERS)
            {
                if (!LOGGERHELPERS.TryGetValue(classType, out helper))
                {
                    helper = new LoggerHelper(classType.ToString());
                    LOGGERHELPERS.Add(classType, helper);
                }
            }
            return helper;
        }

        private string className;

        protected LoggerHelper(string className)
        {
            this.className = className;
        }

        public string StringFormatInvariantCulture(string pattern, params object[] obj) 
        {
            return String.Format(CultureInfo.InvariantCulture, pattern, obj);
        }

        public void Fatal(object message)
        {
            if (LOGGERBRIDGE != null)
            {
                LOGGERBRIDGE.Fatal(className, message);
            }
        }

        public void Fatal(object message, Exception exception)
        {
            if (LOGGERBRIDGE != null)
            {
                LOGGERBRIDGE.Fatal(className, message, exception);
            }
        }

        public void Error(object message)
        {
            if (LOGGERBRIDGE != null)
            {
                LOGGERBRIDGE.Error(className, message);
            }
        }

        public void Error(object message, Exception exception)
        {
            if (LOGGERBRIDGE != null)
            {
                LOGGERBRIDGE.Error(className, message, exception);
            }
        }

        public void Warn(object message)
        {
            if (LOGGERBRIDGE != null)
            {
                LOGGERBRIDGE.Warn(className, message, null);
            }
        }

        public void Warn(object message, Exception exception)
        {
            if (LOGGERBRIDGE != null)
            {
                LOGGERBRIDGE.Warn(className, message, exception);
            }
        }

        public void Info(object message)
        {
            if (LOGGERBRIDGE != null)
            {
                LOGGERBRIDGE.Info(className, message);
            }
        }

        public void Info(object message, Exception exception)
        {
            if (LOGGERBRIDGE != null)
            {
                LOGGERBRIDGE.Info(className, message, exception);
            }
        }

        public void Trace(object message)
        {
            if (LOGGERBRIDGE != null)
            {
                LOGGERBRIDGE.Info(className, message);
            }
        }

        public void Trace(object message, Exception exception)
        {
            if (LOGGERBRIDGE != null)
            {
                LOGGERBRIDGE.Info(className, message, exception);
            }
        }

        public void Debug(object message)
        {
            if (LOGGERBRIDGE != null)
            {
                LOGGERBRIDGE.Debug(className, message, null);
            }
        }

        public void Debug(object message, Exception exception)
        {
            if (LOGGERBRIDGE != null)
            {
                LOGGERBRIDGE.Debug(className, message, exception);
            }
        }
    }
}

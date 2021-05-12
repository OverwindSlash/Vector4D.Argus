using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RosSharp.Utility
{
    public class LogHelper
    {
        public static readonly ILog loginfo = LogManager.GetLogger("loginfo");
        public static readonly ILog logerror = LogManager.GetLogger("logerror");
        public static readonly ILog logdebug = LogManager.GetLogger("logdebug");
        public static readonly ILog logwarn = LogManager.GetLogger("logwarn");

        public static void Init(string configName)
        {
            XmlConfigurator.Configure(new FileInfo(configName));
        }

        public static void Info(string message)
        {
            if (loginfo.IsInfoEnabled)
            {
                loginfo.Info(message);
            }
        }

        public static void InfoFormat(string message, params object[] parms)
        {
            if (loginfo.IsInfoEnabled)
            {
                loginfo.Info(string.Format(message, parms));
            }
        }

        public static void Info(string message, Exception ex)
        {
            if (loginfo.IsInfoEnabled)
            {
                loginfo.Info(message, ex);
            }
        }

        public static void Debug(string message)
        {
            if (logdebug.IsDebugEnabled)
            {
                logdebug.Debug(message);
            }
        }
        public static void Debug(string message, Exception ex)
        {
            if (logdebug.IsDebugEnabled)
            {
                logdebug.Debug(message, ex);
            }
        }

        public static void DebugFormat(string message, params object[] parms)
        {
            if (logdebug.IsDebugEnabled)
            {
                logdebug.Debug(string.Format(message, parms));
            }
        }

        public static void Warn(string message)
        {
            if (logwarn.IsWarnEnabled)
            {
                logwarn.Warn(message);
            }
        }
        public static void WarnFormat(string message, params object[] parms)
        {
            if (logwarn.IsWarnEnabled)
            {
                logwarn.Warn(string.Format(message, parms));
            }
        }

        public static void Warn(string message, Exception ex)
        {
            if (logwarn.IsWarnEnabled)
            {
                logwarn.Warn(message, ex);
            }
        }


        public static void Error(string message)
        {
            if (logerror.IsErrorEnabled)
            {
                logerror.Error(message);
            }
        }
        public static void ErrorFormat(string message, params object[] parms)
        {
            if (logerror.IsErrorEnabled)
            {
                logerror.Error(string.Format(message, parms));
            }
        }
        public static void Error(string message, Exception ex)
        {
            if (logerror.IsErrorEnabled)
            {
                logerror.Error(message, ex);
            }
        }


    }
}

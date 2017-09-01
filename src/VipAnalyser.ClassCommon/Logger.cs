using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VipAnalyser.ClassCommon
{
    public class Logger
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();

        public static void Error(string msg, Exception ex = null)
        {
            logger.Error(ex, msg);
        }

        public static void Info(string msg)
        {
            logger.Info(msg);
        }

        public static void Fatal(string msg, Exception ex = null)
        {
            logger.Fatal(ex, msg);
        }

        public static void Debug(string msg, Exception ex = null)
        {
            logger.Debug(ex, msg);
        }

        public static void Trace(string msg, Exception ex = null)
        {
            logger.Trace(ex, msg);
        }

        public static void Warn(string msg, Exception ex = null)
        {
            logger.Warn(ex, msg);
        }
    }
}

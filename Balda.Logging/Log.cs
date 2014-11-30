using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NLog;

namespace Balda
{
    public class Log
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static void Debug(string message)
        {
            logger.Debug(message);
        }
        public static void Exception(string mes, Exception ex)
        {
            logger.Debug(mes, ex);
        }
    }
}
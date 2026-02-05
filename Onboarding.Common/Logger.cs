using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace Onboarding.Common
{
    public static class Logger
    {
        static ICollection _log4netConfiguration = null;

        static ILog _logger = null;

        static ILog GetLogger()
        {
            if (_log4netConfiguration == null)
            {
                _log4netConfiguration = log4net.Config.XmlConfigurator.Configure();
            }
            if (_logger == null)
            {
                _logger = LogManager.GetLogger("Logger");
            }
            return _logger;
        }

        public static void LogError(Exception ex)
        {
            GetLogger().Error(ex.Message, ex);
        }

        public static void LogError(string errMsg)
        {
            GetLogger().Error(errMsg);
        }

        public static void LogInfo(string infoMsg)
        {
            GetLogger().Info(infoMsg);
        }

    }
}

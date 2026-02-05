using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onboarding.DataProvider;
using Onboarding.Entity;
using System.Threading;

namespace Onboarding.EmailAlert
{
    class Program
    {
        private static DateTime _startDt = DateTime.MinValue;
        private static DateTime _endDt = DateTime.MaxValue;

        static void Main(string[] args)
        {
            _startDt = DateTime.Now;
            try
            {
                NotificationSender sender = new NotificationSender();
                sender.SendOutstandingTaskAlert();
                sender.SendOutstandingRequestAlert();
                SendReport(true, null);
            }
            catch (Exception ex)
            {
                SendReport(false, ex);
                throw ex;
            }
        }

        private static void SendReport(bool success, Exception ex)
        {
            _endDt = DateTime.Now;

            string rpt = string.Format("Start: {0:dd-MMM-yyyy HH:mm:ss}\r\n", _startDt);
            rpt += string.Format("End: {0:dd-MMM-yyyy HH:mm:ss}\r\n", _endDt);
            rpt += string.Format("Duration: {0} seconds\r\n\r\n", (long)(_endDt - _startDt).TotalSeconds);

            string from = ConfigurationManager.AppSettings["emailFrom"];
            string to = ConfigurationManager.AppSettings["BatchJobNotificationTo"];
            string subject = ConfigurationManager.AppSettings["BatchJobNotificationSubject"];

            if (success)
            {
                rpt += "Onboarding Email Alert ran successfully\r\n";
                subject += " (SUCCESS)";
            }
            else
            {
                rpt += "Onboarding Email Alert encountered error:\r\n";
                rpt += ex.ToString();
                subject += " (ERROR)";
            }
            rpt = rpt.Replace("\r\n", "<br/>");
            Common.Email.SendEmail(to, null, null, subject, rpt);
        }

    }
}
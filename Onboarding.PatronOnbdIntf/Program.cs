using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onboarding.DataProvider;
using Onboarding.DataProvider.Model;
using Onboarding.Entity;
using System.Threading;

namespace Onboarding.EmailAlert
{
    class Program
    {
        private static DateTime _startDt = DateTime.MinValue;
        private static DateTime _endDt = DateTime.MaxValue;
        private static string _summRpt = string.Empty;

        static void Main(string[] args)
        {
            _startDt = DateTime.Now;
            StringBuilder sbRpt = new StringBuilder();
            try
            {
                using (OnboardingEntities obCtx = new OnboardingEntities())
                {
                    DbContextTransaction tx = null;
                    try
                    {
                        tx = obCtx.Database.BeginTransaction();

                        CostCentreProvider costCtrProv = new CostCentreProvider();
                        _summRpt +=  SummarizeReport(costCtrProv.SynchronizeWithPatron(obCtx), "Cost Centre");
                        GenderProvider genderProv = new GenderProvider();
                        _summRpt += SummarizeReport(genderProv.SynchronizeWithPatron(obCtx), "Gender");
                        RaceProvider raceProv = new RaceProvider();
                        _summRpt += SummarizeReport(raceProv.SynchronizeWithPatron(obCtx), "Race");
                        SalutationProvider salutProv = new SalutationProvider();
                        _summRpt += SummarizeReport(salutProv.SynchronizeWithPatron(obCtx), "Salutation");

                        tx.Commit();
                    }
                    catch (Exception ex1)
                    {
                        if (tx != null)
                        {
                            tx.Rollback();
                        }
                        throw ex1;
                    }
                    SendReport(true, null);
                }
            }
            catch (Exception ex2)
            {
                SendReport(false, ex2);
                throw ex2;
            }
        }

        private static string SummarizeReport(PatronOnbdIntfRpt rpt, string recName)
        {
            string summRpt = string.Empty;
            summRpt += string.Format("{0}:\r\n", recName);
            summRpt += "-------------------------------------------------------\r\n";
            summRpt += string.Format("Inserted: {0}\r\n", rpt.Inserted);
            summRpt += string.Format("Updated: {0}\r\n", rpt.Updated);
            summRpt += string.Format("Removed: {0}\r\n", rpt.Removed);
            summRpt += "-------------------------------------------------------\r\n";
            return summRpt;
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
                rpt += "Patron - Onboarding interface ran successfully\r\n";
                subject += " (SUCCESS)";
            }
            else
            {
                rpt += "Patron - Onboarding interface encountered error:\r\n";
                rpt += ex.ToString();
                subject += " (ERROR)";
            }
            rpt += "\r\n\r\n";
            rpt += _summRpt;
            rpt = rpt.Replace("\r\n", "<br/>");
            Common.Email.SendEmail(to, null, null, subject, rpt);
        }

    }
}
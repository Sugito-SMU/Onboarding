using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Common
{
    public class Email
    {

        public static void SendEmail(string to, string cc, string bcc, string subject, string body)
        {

            string fromAddress = ConfigurationManager.AppSettings["EmailFrom"];

            MailMessage mailMessage = new MailMessage();

            if (!string.IsNullOrWhiteSpace(fromAddress))
            {
                mailMessage.From = new MailAddress(fromAddress);
            }

            if (!string.IsNullOrWhiteSpace(to))
            {
                foreach (string s in to.Trim().Split(','))
                {
                    mailMessage.To.Add(new MailAddress(s.Trim()));
                }
            }

            if (!string.IsNullOrWhiteSpace(cc))
            {
                foreach (string s in cc.Trim().Split(','))
                {
                    mailMessage.CC.Add(new MailAddress(s.Trim()));
                }
            }

            if (!string.IsNullOrWhiteSpace(bcc))
            {
                foreach (string s in bcc.Trim().Split(','))
                {
                    mailMessage.Bcc.Add(new MailAddress(s.Trim()));
                }
            }

            mailMessage.IsBodyHtml = true;

            mailMessage.Body = body;

            mailMessage.Subject = subject;

            SmtpClient smtp = new SmtpClient();

            int maxTry = 5;
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    smtp.Send(mailMessage);
                    i = maxTry;
                }
                catch (Exception ex)
                {
                    if (i == maxTry - 1)
                        throw ex;
                    System.Threading.Thread.Sleep(500);
                }
            }
            mailMessage.Dispose();


            string emailLog = string.Format("Email sent\r\nTo: {0}\r\nCC: {1}\r\nBCC: {2}\r\nSubject: {3}\r\n", to, cc, bcc, subject);
            Common.Logger.LogInfo(emailLog);

        }

        public static void SendEmailForError(Exception ex, string url, string userId)
        {
            string to = ConfigurationManager.AppSettings["ErrorMsgTo"];
            string subject = ConfigurationManager.AppSettings["ErrorMsgSubject"];
            string body = string.Format(@" 
                  <html> 
                  <body> 
                  <table cellpadding=""5"" cellspacing=""0"" border=""1""> 
                  <tr> 
                  <tdtext-align: right;font-weight: bold"">URL:</td> 
                  <td>{0}</td> 
                  </tr> 
                  <tr> 
                  <tdtext-align: right;font-weight: bold"">User:</td> 
                  <td>{1}</td> 
                  </tr> 
                  <tr> 
                  <tdtext-align: right;font-weight: bold"">Message:</td> 
                  <td>{2}</td> 
                  </tr> 
                  <tr> 
                  <tdtext-align: right;font-weight: bold"">Details:</td> 
                  <td>{3}</td> 
                  </tr>  
                  </table> 
                </body> 
                </html>",
                url,
                userId,
                ex.Message,
                ex.ToString().Replace(Environment.NewLine, "<br />"));
            SendEmail(to, null, null, subject, body);
        }

        public static void SendEmailForError(string exMsg, string url, string userId)
        {
            string to = ConfigurationManager.AppSettings["ErrorMsgTo"];
            string subject = ConfigurationManager.AppSettings["ErrorMsgSubject"];
            string body = string.Format(@" 
                  <html> 
                  <body> 
                  <table cellpadding=""5"" cellspacing=""0"" border=""1""> 
                  <tr> 
                  <tdtext-align: right;font-weight: bold"">URL:</td> 
                  <td>{0}</td> 
                  </tr> 
                  <tr> 
                  <tdtext-align: right;font-weight: bold"">User:</td> 
                  <td>{1}</td> 
                  </tr> 
                  <tr> 
                  <tdtext-align: right;font-weight: bold"">Details:</td> 
                  <td>{2}</td> 
                  </tr>  
                  </table> 
                </body> 
                </html>",
                url,
                userId,
                exMsg.ToString().Replace(Environment.NewLine, "<br />"));
            SendEmail(to, null, null, subject, body);
        }

        public static void SendEmailForNetworkIdUpdate(string employeeName, int employeeId, string requestedBy, string taskUrl)
        {
            string to = ConfigurationManager.AppSettings["HRAdminEmail"];
            string subject = ConfigurationManager.AppSettings["NetworkIdUpdateSubject"];

            if (string.IsNullOrWhiteSpace(to))
            {
                return;
            }
            string body = string.Format(@"
                <html>
                <body>
                <p>A new task requires HR Admin action to update the Network ID.</p>

                <table cellpadding='5' cellspacing='0' border='1'>
                    <tr>
                        <td style='text-align:right;font-weight:bold'>Employee Name:</td>
                        <td>{0}</td>
                    </tr>
                    <tr>
                        <td style='text-align:right;font-weight:bold'>Employee ID:</td>
                        <td>{1}</td>
                    </tr>
                    <tr>
                        <td style='text-align:right;font-weight:bold'>Requested By:</td>
                        <td>{4}</td>
                    </tr>
                    <tr>
                        <td style='text-align:right;font-weight:bold'>Task Link:</td>
                        <td><a href='{5}'>Open Request</a></td>
                    </tr>
                </table>

                <br/>
                <p>Please click the link above to review and complete the request.</p>

                </body>
                </html>",
                employeeName,
                employeeId,
                requestedBy,
                taskUrl
            );

            SendEmail(to, null, null, subject, body);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Onboarding;
using Onboarding.Entity;

namespace Onboarding.DataProvider
{
    public class NotificationSender
    {

        public void SendOutstandingTaskAlert()
        {
            EmailAlertProvider emailAlertProv = new EmailAlertProvider();
            OutstandingTaskAlert outTaskAlert = emailAlertProv.GetOutstandingTaskAlert();
            string rowTemplateFile = "config\\OutstandingTaskAlert_Row_Template.html";
            string rowTemplate = File.ReadAllText(rowTemplateFile);
            string emailTemplateFile = "config\\OutstandingTaskAlert_Template.html";
            string emailTemplate = File.ReadAllText(emailTemplateFile);

            if (outTaskAlert != null)
            {
                List<string> agentEmailList = new List<string>();
                foreach (var ta in outTaskAlert.Rows)
                {
                    if ((!string.IsNullOrWhiteSpace(ta.AgentEmail)) && (!agentEmailList.Contains(ta.AgentEmail)))
                    {
                        agentEmailList.Add(ta.AgentEmail);
                    }
                }

                foreach (string agentEmail in agentEmailList)
                {
                    string emailTo = agentEmail;
                    string emailCC = ConfigurationManager.AppSettings["OutstandingTaskAlert _Cc"];
                    string emailSubject = ConfigurationManager.AppSettings["OutstandingTaskAlert_Subject"];
                    string reqUrlTemplate = ConfigurationManager.AppSettings["OutstandingTaskAlert_RequestUrl"];

                    string taskRows = string.Empty;

                    var taskList = from a in outTaskAlert.Rows
                                   where a.AgentEmail == agentEmail
                                   orderby a.EmplStartDt ascending, a.ReqId ascending, a.TaskDesc
                                   select a;

                    foreach (var tsk in taskList)
                    {
                        string bgColor = string.Empty;
                        if (tsk.IsUrgent == true)
                        {
                            bgColor = ConfigurationManager.AppSettings["UrgentBackgroundColor"];
                        }
                        else
                        {
                            bgColor = ConfigurationManager.AppSettings["NonUrgentBackgroundColor"];
                        }
                        string taskRow = (string)rowTemplate.Clone();
                        string reqUrl = (string)reqUrlTemplate.Clone();
                        reqUrl = reqUrl.Replace("{RequestId}", tsk.ReqId.ToString());
                        taskRow = taskRow.Replace("{RequestId}", tsk.ReqId.ToString());
                        taskRow = taskRow.Replace("{RequestUrl}", reqUrl);
                        taskRow = taskRow.Replace("{FirstName}", string.Format("{0}", tsk.FirstName));
                        taskRow = taskRow.Replace("{LastName}", string.Format("{0}", tsk.LastName));
                        taskRow = taskRow.Replace("{CostCentre}", string.Format("{0}", tsk.CostCtrDesc));
                        taskRow = taskRow.Replace("{WorkStartDate}", string.Format("{0:dd MMM yyyy}", tsk.EmplStartDt));
                        taskRow = taskRow.Replace("{Requestor}", string.Format("{0}", tsk.RequestorName));
                        taskRow = taskRow.Replace("{RequestDate}", string.Format("{0:dd MMM yyyy}", tsk.RequestDt));
                        taskRow = taskRow.Replace("{Task}", string.Format("{0}", tsk.TaskDesc));
                        taskRow = taskRow.Replace("{Agent}", string.Format("{0}", tsk.AgentName));
                        taskRow = taskRow.Replace("{BackgroundColor}", string.Format("{0}", bgColor));
                        taskRows += taskRow + Environment.NewLine;
                    }

                    string emailBody = (string)emailTemplate.Clone();
                    emailBody = emailBody.Replace("{TaskListTable}", taskRows);

                    Onboarding.Common.Email.SendEmail(emailTo, emailCC, null, emailSubject, emailBody);
                }
            }
        }


        public void SendOutstandingRequestAlert()
        {
            EmailAlertProvider emailAlertProv = new EmailAlertProvider();
            OutstandingRequestAlert outReqAlert = emailAlertProv.GetOutstandingRequestAlert();

            string taskRowTemplateFile = "config\\OutstandingRequestAlert_TaskRow_Template.html";
            string taskRowTemplate = File.ReadAllText(taskRowTemplateFile);
            string reqRowTemplateFile = "config\\OutstandingRequestAlert_RequestRow_Template.html";
            string reqRowTemplate = File.ReadAllText(reqRowTemplateFile);
            string emailTemplateFile = "config\\OutstandingRequestAlert_Template.html";
            string emailTemplate = File.ReadAllText(emailTemplateFile);

            if (outReqAlert != null)
            {
                var reqList = from a in outReqAlert.Rows
                              group a by new
                              {
                                  a.ReqId,
                                  a.EmplStartDt,
                                  a.IsUrgent
                              };

                var reqList2 = from a in reqList
                               orderby a.Key.EmplStartDt ascending, a.Key.ReqId ascending
                               select a;

                string emailTo = ConfigurationManager.AppSettings["OutstandingRequestAlert_To"];
                string emailCC = ConfigurationManager.AppSettings["OutstandingRequestAlert_Cc"];
                string emailSubject = ConfigurationManager.AppSettings["OutstandingRequestAlert_Subject"];
                string reqUrlTemplate = ConfigurationManager.AppSettings["OutstandingRequestAlert_RequestUrl"];

                string emailBody = (string)emailTemplate.Clone();
                string reqRows = string.Empty;
                foreach (var reqItem in reqList2)
                {

                    List<OutstandingRequestAlertRow> taskList = outReqAlert.Rows.Where(x => x.ReqId == reqItem.Key.ReqId).OrderBy(x => x.TaskDesc).ToList();
                    string reqRow = (string)reqRowTemplate.Clone();
                    string reqUrl = (string)reqUrlTemplate.Clone();
                    string bgColor = string.Empty;
                    if (taskList[0].IsUrgent == true)
                    {
                        bgColor = ConfigurationManager.AppSettings["UrgentBackgroundColor"];
                    }
                    else
                    {
                        bgColor = ConfigurationManager.AppSettings["NonUrgentBackgroundColor"];
                    }

                    reqUrl = reqUrl.Replace("{RequestId}", taskList[0].ReqId.ToString());
                    reqRow = reqRow.Replace("{RequestUrl}", reqUrl);
                    reqRow = reqRow.Replace("{RequestId}", taskList[0].ReqId.ToString());
                    reqRow = reqRow.Replace("{FirstName}", string.Format("{0}", taskList[0].FirstName));
                    reqRow = reqRow.Replace("{LastName}", string.Format("{0}", taskList[0].LastName));
                    reqRow = reqRow.Replace("{CostCentre}", string.Format("{0}", taskList[0].CostCtrDesc));
                    reqRow = reqRow.Replace("{WorkStartDate}", string.Format("{0:dd MMM yyyy}", taskList[0].EmplStartDt));
                    reqRow = reqRow.Replace("{Requestor}", string.Format("{0}", taskList[0].RequestorName));
                    reqRow = reqRow.Replace("{RequestDate}", string.Format("{0:dd MMM yyyy}", taskList[0].RequestDt));
                    reqRow = reqRow.Replace("{TaskCount}", string.Format("{0}", taskList.Count));
                    reqRow = reqRow.Replace("{BackgroundColor}", string.Format("{0}", bgColor));

                    for (int i = 0; i < taskList.Count; i++)
                    {
                        string taskRow = (string)taskRowTemplate.Clone();
                        taskRow = taskRow.Replace("{Task}", string.Format("{0}", taskList[i].TaskDesc));
                        taskRow = taskRow.Replace("{Agent}", string.Format("{0}", taskList[i].AgentNameList));
                        taskRow = taskRow.Replace("{BackgroundColor}", string.Format("{0}", bgColor));

                        if (i == 0)
                        {
                            reqRows += "<tr>" + reqRow + taskRow + "</tr>" + Environment.NewLine;
                        }
                        else
                        {
                            reqRows += "<tr>" + taskRow + "</tr>" + Environment.NewLine;
                        }
                    }
                }


                emailBody = emailBody.Replace("{RequestCount}", reqList.Count().ToString());
                emailBody = emailBody.Replace("{UrgentRequestCutoff}", ConfigurationManager.AppSettings["UrgentRequestCutOff"]);
                emailBody = emailBody.Replace("{UrgentRequestCount}", reqList.Where(x => x.Key.IsUrgent == true).Count().ToString());
                emailBody = emailBody.Replace("{RequestListTable}", reqRows);

                Onboarding.Common.Email.SendEmail(emailTo, emailCC, null, emailSubject, emailBody);
            }
        }
    }
}

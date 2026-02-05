using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Onboarding.DataProvider;
using Onboarding.DataProvider.Model;

namespace Onboarding.ServiceManager
{
    public class NotificationManager
    {
        public void NotifyNextTask(Entity.Request req, List<Entity.Task> nextTaskList)
        {
            try
            {
                try
                {
                    if (nextTaskList != null)
                    {
                        AgentProvider agentProv = new AgentProvider();
                        TaskCodeProvider taskCodeProv = new TaskCodeProvider();
                        List<Entity.Agent> agentList = agentProv.GetAgentAll();
                        List<Entity.TaskCode> taskCodeList = taskCodeProv.GetTaskCodeAll();
                        UserProfileProvider userProv = new UserProfileProvider();
                        Entity.UserProfile usr = userProv.GetUserProfile(req.SubmittedBy);
                        string requestedByName = null;
                        if (usr != null)
                        {
                            requestedByName = usr.Name;
                        }


                        var taskAgents = from a in nextTaskList
                                         join b in agentList
                                         on a.TaskCd equals b.TaskCd
                                         join c in taskCodeList
                                         on a.TaskCd equals c.TaskCd
                                         select new
                                         {
                                             b.AgentName,
                                             b.Email,
                                             c.TaskDesc
                                         };

                        List<string> agentEmailList = new List<string>();
                        foreach (var ta in taskAgents)
                        {
                            if ((!string.IsNullOrWhiteSpace(ta.Email)) && (!agentEmailList.Contains(ta.Email)))
                            {
                                agentEmailList.Add(ta.Email);
                            }
                        }


                        foreach (string agentEmail in agentEmailList)
                        {
                            var taskList1 = from a in taskAgents
                                            where a.Email == agentEmail
                                            select new
                                            {
                                                a.AgentName,
                                                a.TaskDesc
                                            };
                            string agentName = null;

                            string taskList2 = "<ul>" + Environment.NewLine;
                            foreach (var t in taskList1.OrderBy(x => x.TaskDesc))
                            {
                                agentName = t.AgentName;
                                taskList2 += string.Format("<li>{0}</li>", t.TaskDesc) + Environment.NewLine;
                            }
                            taskList2 += "</ul>" + Environment.NewLine;

                            string emailTo = agentEmail;
                            string emailCC = ConfigurationManager.AppSettings["NextTaskNotification_CC"];
                            string emailBCC = ConfigurationManager.AppSettings["NextTaskNotification_BCC"];

                            string emailSubject = ConfigurationManager.AppSettings["NextTaskNotification_Subject"];
                            emailSubject = emailSubject.Replace("{RequestId}", req.ReqId.ToString());
                            emailSubject = emailSubject.Replace("{FirstName}", string.Format("{0}", req.FirstName));
                            emailSubject = emailSubject.Replace("{LastName}", string.Format("{0}", req.LastName));

                            string templateFile = System.AppDomain.CurrentDomain.BaseDirectory + "\\config\\NextTaskNotification_Template.html";
                            string emailBody = File.ReadAllText(templateFile);
                            string reqUrl = ConfigurationManager.AppSettings["NextTaskNotification_Url"];
                            reqUrl = reqUrl.Replace("{RequestId}", req.ReqId.ToString());
                            emailBody = emailBody.Replace("{RequestId}", req.ReqId.ToString());
                            emailBody = emailBody.Replace("{RequestUrl}", reqUrl);
                            emailBody = emailBody.Replace("{RequestType}", string.Format("{0}", req.ReqTypeDesc));
                            emailBody = emailBody.Replace("{FirstName}", string.Format("{0}", req.FirstName));
                            emailBody = emailBody.Replace("{LastName}", string.Format("{0}", req.LastName));
                            emailBody = emailBody.Replace("{StaffType}", string.Format("{0}", req.EmploymentInfo.UserTypeDesc));
                            emailBody = emailBody.Replace("{StaffSubType}", string.Format("{0}", req.EmploymentInfo.UserSubTypeDesc));
                            emailBody = emailBody.Replace("{StartDate}", string.Format("{0:dd MMM yyyy}", req.EmploymentInfo.EmplStartDt));
                            emailBody = emailBody.Replace("{EndDate}", string.Format("{0:dd MMM yyyy}", req.EmploymentInfo.EmplEndDt));
                            emailBody = emailBody.Replace("{CostCentre}", string.Format("{0}", req.EmploymentInfo.CostCtrDesc));
                            emailBody = emailBody.Replace("{RequestedBy}", string.Format("{0}", requestedByName));
                            emailBody = emailBody.Replace("{RequestedDate}", string.Format("{0:dd-MMM-yyyy}", req.SubmittedDt));
                            string reqSts = req.ReqStsDesc;
                            if ((req.IsCancelled != true) && (req.IsAmended == true))
                            {
                                reqSts += " (with amendment)";
                            }
                            emailBody = emailBody.Replace("{RequestStatus}", string.Format("{0}", reqSts));
                            emailBody = emailBody.Replace("{AgentName}", string.Format("{0}", agentName));
                            emailBody = emailBody.Replace("{TaskList}", taskList2);

                            string urgency = string.Empty;
                            if (Onboarding.Common.Validation.IsUrgentRequest(req.ReqStsCd, req.EmploymentInfo.EmplStartDt))
                            {
                                urgency = "(URGENT)";
                            }
                            emailSubject = emailSubject.Replace("{Urgency}", urgency);
                            emailBody = emailBody.Replace("{Urgency}", urgency);

                            Onboarding.Common.Email.SendEmail(emailTo, emailCC, emailBCC, emailSubject, emailBody);

                        }

                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            catch (ArgumentException ex2)
            {
                Common.Logger.LogError(ex2);
            }
        }

        public void NotifySubmittedRequest(Entity.Request req)
        {
            try
            {
                try
                {

                    if ((req != null) &&
                        ((req.ReqStsCd == Entity.Constant.RequestStatusCode.Submitted) || (req.ReqStsCd == Entity.Constant.RequestStatusCode.Cancelled)))
                    {
                        UserProfileProvider userProv = new UserProfileProvider();
                        Entity.UserProfile usr = userProv.GetUserProfile(req.SubmittedBy);
                        string requestedByName = null;
                        string requestorEmail = null;
                        if (usr != null)
                        {
                            requestedByName = usr.Name;
                            requestorEmail = usr.Email;
                        }

                        if (!string.IsNullOrWhiteSpace(requestorEmail))
                        {
                            string emailTo = requestorEmail;
                            string emailCC = ConfigurationManager.AppSettings["RequestNotification_CC"];
                            string emailBCC = ConfigurationManager.AppSettings["RequestNotification_BCC"];

                            string action = "submitted";
                            if (req.IsCancelled == true)
                            {
                                action = "cancelled";
                            }
                            else if ((req.IsCancelled != true) && (req.IsAmended == true))
                            {
                                action = "re-submitted (with amendment)";
                            }
                            else
                            {
                                action = "submitted";
                            }

                            string emailSubject = ConfigurationManager.AppSettings["RequestNotification_Subject"];
                            emailSubject = emailSubject.Replace("{RequestId}", req.ReqId.ToString());
                            emailSubject = emailSubject.Replace("{FirstName}", string.Format("{0}", req.FirstName));
                            emailSubject = emailSubject.Replace("{LastName}", string.Format("{0}", req.LastName));
                            emailSubject = emailSubject.Replace("{Action}", string.Format("{0}", action));

                            string templateFile = System.AppDomain.CurrentDomain.BaseDirectory + "\\config\\RequestSubmittedNotification_Template.html";
                            string emailBody = File.ReadAllText(templateFile);
                            string reqUrl = ConfigurationManager.AppSettings["RequestNotification_Url"];
                            reqUrl = reqUrl.Replace("{RequestId}", req.ReqId.ToString());
                            emailBody = emailBody.Replace("{RequestId}", req.ReqId.ToString());
                            emailBody = emailBody.Replace("{RequestUrl}", reqUrl);
                            emailBody = emailBody.Replace("{RequestType}", string.Format("{0}", req.ReqTypeDesc));
                            emailBody = emailBody.Replace("{FirstName}", string.Format("{0}", req.FirstName));
                            emailBody = emailBody.Replace("{LastName}", string.Format("{0}", req.LastName));
                            emailBody = emailBody.Replace("{StaffType}", string.Format("{0}", req.EmploymentInfo.UserTypeDesc));
                            emailBody = emailBody.Replace("{StaffSubType}", string.Format("{0}", req.EmploymentInfo.UserSubTypeDesc));
                            emailBody = emailBody.Replace("{StartDate}", string.Format("{0:dd MMM yyyy}", req.EmploymentInfo.EmplStartDt));
                            emailBody = emailBody.Replace("{EndDate}", string.Format("{0:dd MMM yyyy}", req.EmploymentInfo.EmplEndDt));
                            emailBody = emailBody.Replace("{CostCentre}", string.Format("{0}", req.EmploymentInfo.CostCtrDesc));
                            emailBody = emailBody.Replace("{RequestedBy}", string.Format("{0}", requestedByName));
                            emailBody = emailBody.Replace("{RequestedDate}", string.Format("{0:dd-MMM-yyyy}", req.SubmittedDt));

                            string reqSts = req.ReqStsDesc;
                            if ((req.IsCancelled != true) && (req.IsAmended == true))
                            {
                                reqSts += " (with amendment)";
                            }
                            emailBody = emailBody.Replace("{RequestStatus}", string.Format("{0}", reqSts));

                            emailBody = emailBody.Replace("{Action}", string.Format("{0}", action));

                            string urgency = string.Empty;
                            if (Onboarding.Common.Validation.IsUrgentRequest(req.ReqStsCd, req.EmploymentInfo.EmplStartDt))
                            {
                                urgency = "(URGENT)";
                            }
                            emailSubject = emailSubject.Replace("{Urgency}", urgency);
                            emailBody = emailBody.Replace("{Urgency}", urgency);

                            //Copy FA Admin
                            emailCC = CopyForFacultyRequest(req, emailCC);

                            //Copy to whoever needs to know when request is submitted
                            if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["RequestNotification_Submitted_CC"]))
                            {
                                if (!string.IsNullOrWhiteSpace(emailCC))
                                {
                                    emailCC += ",";
                                }
                                emailCC += ConfigurationManager.AppSettings["RequestNotification_Submitted_CC"];
                            }

                            Onboarding.Common.Email.SendEmail(emailTo, emailCC, emailBCC, emailSubject, emailBody);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            catch (ArgumentException ex2)
            {
                Common.Logger.LogError(ex2);
            }

        }

        public void NotifyClosedRequest(Entity.Request req)
        {
            try
            {
                try
                {

                    if ((req != null) && (req.ReqStsCd == Entity.Constant.RequestStatusCode.Closed))
                    {
                        UserProfileProvider userProv = new UserProfileProvider();
                        Entity.UserProfile usr = userProv.GetUserProfile(req.SubmittedBy);
                        string requestedByName = null;
                        string requestorEmail = null;
                        if (usr != null)
                        {
                            requestedByName = usr.Name;
                            requestorEmail = usr.Email;
                        }

                        if (!string.IsNullOrWhiteSpace(requestorEmail))
                        {
                            string emailTo = requestorEmail;
                            string emailCC = ConfigurationManager.AppSettings["RequestNotification_CC"];
                            string emailBCC = ConfigurationManager.AppSettings["RequestNotification_BCC"];

                            string action = "submitted";
                            if (req.IsCancelled == true)
                            {
                                action = "cancelled";
                            }
                            else if ((req.IsCancelled != true) && (req.IsAmended == true))
                            {
                                action = "re-submitted (with amendment)";
                            }
                            else
                            {
                                action = "submitted";
                            }

                            string emailSubject = ConfigurationManager.AppSettings["RequestNotification_Subject"];
                            emailSubject = emailSubject.Replace("{RequestId}", req.ReqId.ToString());
                            emailSubject = emailSubject.Replace("{FirstName}", string.Format("{0}", req.FirstName));
                            emailSubject = emailSubject.Replace("{LastName}", string.Format("{0}", req.LastName));
                            emailSubject = emailSubject.Replace("{Action}", string.Format("{0}", action));

                            string templateFile = System.AppDomain.CurrentDomain.BaseDirectory + "\\config\\RequestClosedNotification_Template.html";
                            string emailBody = File.ReadAllText(templateFile);
                            string reqUrl = ConfigurationManager.AppSettings["RequestNotification_Url"];
                            reqUrl = reqUrl.Replace("{RequestId}", req.ReqId.ToString());
                            emailBody = emailBody.Replace("{RequestId}", req.ReqId.ToString());
                            emailBody = emailBody.Replace("{RequestUrl}", reqUrl);
                            emailBody = emailBody.Replace("{RequestType}", string.Format("{0}", req.ReqTypeDesc));
                            emailBody = emailBody.Replace("{FirstName}", string.Format("{0}", req.FirstName));
                            emailBody = emailBody.Replace("{LastName}", string.Format("{0}", req.LastName));
                            emailBody = emailBody.Replace("{StaffType}", string.Format("{0}", req.EmploymentInfo.UserTypeDesc));
                            emailBody = emailBody.Replace("{StaffSubType}", string.Format("{0}", req.EmploymentInfo.UserSubTypeDesc));
                            emailBody = emailBody.Replace("{StartDate}", string.Format("{0:dd MMM yyyy}", req.EmploymentInfo.EmplStartDt));
                            emailBody = emailBody.Replace("{EndDate}", string.Format("{0:dd MMM yyyy}", req.EmploymentInfo.EmplEndDt));
                            emailBody = emailBody.Replace("{CostCentre}", string.Format("{0}", req.EmploymentInfo.CostCtrDesc));
                            emailBody = emailBody.Replace("{RequestedBy}", string.Format("{0}", requestedByName));
                            emailBody = emailBody.Replace("{RequestedDate}", string.Format("{0:dd-MMM-yyyy}", req.SubmittedDt));

                            string reqSts = req.ReqStsDesc;
                            if ((req.IsCancelled != true) && (req.IsAmended == true))
                            {
                                reqSts += " (with amendment)";
                            }
                            emailBody = emailBody.Replace("{RequestStatus}", string.Format("{0}", reqSts));

                            emailBody = emailBody.Replace("{Action}", string.Format("{0}", action));

                            Onboarding.Common.Email.SendEmail(emailTo, emailCC, emailBCC, emailSubject, emailBody);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            catch (ArgumentException ex2)
            {
                Common.Logger.LogError(ex2);
            }

        }

        public string CopyForFacultyRequest(Entity.Request req, string emailCC)
        {
            string faEmail = null;
            if (req.EmploymentInfo.UserTypeCd == Entity.Constant.UserType.Faculty)
            {
                faEmail = ConfigurationManager.AppSettings["RequestNotification_Submitted_Faculty_CC"];
                if (!string.IsNullOrEmpty(faEmail))
                {
                    if (!string.IsNullOrEmpty(emailCC))
                    {
                        return string.Format("{0},{1}", emailCC, faEmail);
                    }
                    else
                    {
                        return faEmail;
                    }
                }
                else
                {
                    return emailCC;
                }
            }
            else
            {
                return emailCC;
            }
        }

        public void NotifyTaskToCustomList(Entity.Request req, List<Entity.Task> taskList)
        {
            if (taskList != null)
            {
                foreach (Entity.Task tsk in taskList)
                {
                    NotifyTaskToCustomList(req, tsk);
                }
            }
        }
        public void NotifyTaskToCustomList(Entity.Request req, Entity.Task tsk)
        {
            try
            {
                try
                {
                    TaskStatusProvider tskStsProv = new TaskStatusProvider();

                    List<Entity.TaskAlertConfig> cfgList = GetTaskAlertConfig();
                    if ((cfgList != null) && (req != null) && (tsk != null))
                    {
                        List<Entity.TaskAlertConfig> cfgList2 = cfgList.Where(x => (x.TaskCd == tsk.TaskCd) && (x.TaskStsCd == tsk.TaskStsCd)).ToList();
                        foreach (Entity.TaskAlertConfig cfg in cfgList2)
                        {
                            string emailTo = cfg.EmailList;
                            string emailSubject = cfg.EmailSubject;
                            emailSubject = emailSubject.Replace("{RequestId}", req.ReqId.ToString());
                            emailSubject = emailSubject.Replace("{FirstName}", string.Format("{0}", req.FirstName));
                            emailSubject = emailSubject.Replace("{LastName}", string.Format("{0}", req.LastName));

                            string templateFile = System.AppDomain.CurrentDomain.BaseDirectory + "\\config\\CustomTaskNotification_Template.html";
                            string emailBody = File.ReadAllText(templateFile);
                            string reqUrl = cfg.RequestUrl;
                            reqUrl = reqUrl.Replace("{RequestId}", req.ReqId.ToString());
                            emailBody = emailBody.Replace("{RequestId}", req.ReqId.ToString());
                            emailBody = emailBody.Replace("{RequestUrl}", reqUrl);
                            emailBody = emailBody.Replace("{RequestType}", string.Format("{0}", req.ReqTypeDesc));
                            emailBody = emailBody.Replace("{FirstName}", string.Format("{0}", req.FirstName));
                            emailBody = emailBody.Replace("{LastName}", string.Format("{0}", req.LastName));
                            emailBody = emailBody.Replace("{StaffType}", string.Format("{0}", req.EmploymentInfo.UserTypeDesc));
                            emailBody = emailBody.Replace("{StaffSubType}", string.Format("{0}", req.EmploymentInfo.UserSubTypeDesc));
                            emailBody = emailBody.Replace("{StartDate}", string.Format("{0:dd MMM yyyy}", req.EmploymentInfo.EmplStartDt));
                            emailBody = emailBody.Replace("{EndDate}", string.Format("{0:dd MMM yyyy}", req.EmploymentInfo.EmplEndDt));
                            emailBody = emailBody.Replace("{CostCentre}", string.Format("{0}", req.EmploymentInfo.CostCtrDesc));
                            emailBody = emailBody.Replace("{RequestedBy}", string.Format("{0}", req.SubmittedByName));
                            emailBody = emailBody.Replace("{RequestedDate}", string.Format("{0:dd-MMM-yyyy}", req.SubmittedDt));
                            string reqSts = req.ReqStsDesc;
                            if ((req.IsCancelled != true) && (req.IsAmended == true))
                            {
                                reqSts += " (with amendment)";
                            }
                            emailBody = emailBody.Replace("{RequestStatus}", string.Format("{0}", reqSts));
                            emailBody = emailBody.Replace("{TaskDescription}", string.Format("{0}", tsk.TaskDesc));
                            emailBody = emailBody.Replace("{TaskStatus}", tskStsProv.GetTaskStatusDesc(tsk.TaskStsCd));

                            Onboarding.Common.Email.SendEmail(emailTo, null, null, emailSubject, emailBody);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            catch (ArgumentException ex2)
            {
                Common.Logger.LogError(ex2);
            }

        }

        private List<Entity.TaskAlertConfig> GetTaskAlertConfig()
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<Entity.TaskAlertConfig>));
            string configFile = System.AppDomain.CurrentDomain.BaseDirectory + "\\config\\TaskAlertConfig.xml";
            List<Entity.TaskAlertConfig> cfg = null;
            using (StreamReader sr = new StreamReader(configFile))
            {
                cfg = (List<Entity.TaskAlertConfig>)ser.Deserialize(sr);
            }
            return cfg;
        }

    }
}

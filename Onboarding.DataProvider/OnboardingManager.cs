using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onboarding.DataProvider;
using Onboarding.Entity;
using Onboarding.Common;
using System.Collections;
using log4net;

namespace Onboarding.DataProvider
{
    public class OnboardingManager
    {
        static ICollection _log4netConfiguration = null;
        static ILog _logger = null;
        public OnboardingManager()
        {
            //workaround to auto-copy EntityFramework.SqlServer.dll to bin folder
            bool instanceExists = System.Data.Entity.SqlServer.SqlProviderServices.Instance != null;

            if (_log4netConfiguration == null)
            {
                log4net.Config.XmlConfigurator.Configure();
                _logger = LogManager.GetLogger("Logger");
            }

        }


        public List<Patron> GetPatronAll()
        {
            
            List<Patron> patronList = new List<Patron>();
            /*
            using (Model.PATRONEntities patEntities = new Model.PATRONEntities())
            {
                List<Model.OnboardingPatDtl> vPatDtlList = patEntities.Model.OnboardingPatDtls.ToList<Model.OnboardingPatDtl>();
                List<Model.OnboardingPatDept> vPatDeptList = patEntities.Model.OnboardingPatDepts.ToList<Model.OnboardingPatDept>();
                List<Model.OnboardingPatPosn> vPatPosnList = patEntities.Model.OnboardingPatPosns.ToList<Model.OnboardingPatPosn>();
                if (vPatDtlList != null)
                {
                    foreach (Model.OnboardingPatDtl vPatDtl in vPatDtlList)
                    {
                        Patron pat = new Patron();
                        pat.ActiveNum = vPatDtl.ActiveNum;
                        pat.PrefName = vPatDtl.PrefName;
                        pat.NtLoginId = vPatDtl.NtLoginId;
                        pat.SmuEmail = vPatDtl.SmuEmail;
                        pat.IsActive = (vPatDtl.IsActive == true) ? true : false;

                        List<Model.OnboardingPatDept> vDeptList = vPatDeptList.Where(x => x.ActiveNum == vPatDtl.ActiveNum).ToList<Model.OnboardingPatDept>();
                        if (vDeptList != null)
                        {
                            pat.Departments = new List<PatronDept>();
                            foreach (Model.OnboardingPatDept vDept in vDeptList)
                            {
                                PatronDept patDept = new PatronDept();
                                patDept.ActiveNum = vDept.ActiveNum;
                                patDept.NtLoginId = vDept.NtLoginId;
                                patDept.CostCentre = vDept.CostCentre;
                                patDept.JobTitle = vDept.JobTitle;
                                patDept.IsPrimary = (vDept.IsPrimary == true) ? true : false;
                                pat.Departments.Add(patDept);
                            }
                        }


                        List<Model.OnboardingPatPosn> vPosnList = vPatPosnList.Where(x => x.ActiveNum == vPatDtl.ActiveNum).ToList<Model.OnboardingPatPosn>();
                        if (vPosnList != null)
                        {
                            pat.Positions = new List<PatronPosition>();
                            foreach (Model.OnboardingPatPosn vPosn in vPosnList)
                            {
                                PatronPosition patPosn = new PatronPosition();
                                patPosn.ActiveNum = vPosn.ActiveNum;
                                patPosn.NtLoginId = vPosn.NtLoginId;
                                patPosn.PosnId = vPosn.PosnId;
                                patPosn.OrgId = vPosn.OrgId;
                                patPosn.BusinessTitle = vPosn.BusinessTitle;
                                patPosn.IsPrimary = (vPosn.IsPrimary == true) ? true : false;
                                pat.Positions.Add(patPosn);
                            }
                        }

                        patronList.Add(pat);

                    }
                }
            }
            */
            return patronList;
        }

        public List<Agent> GetAgentAll()
        {
            List<Agent> agentList = new List<Agent>();
            using (Model.OnboardingEntities obEntities = new Model.OnboardingEntities())
            {
                List<Model.V_Agent> vAgentList = obEntities.V_Agent.ToList<Model.V_Agent>();
                if (vAgentList != null)
                {
                    foreach (Model.V_Agent vag in vAgentList)
                    {
                        Agent ag = new Agent();
                        ag.TaskCd = vag.TaskCd;
                        ag.AgentId = vag.AgentId;
                        ag.IsPrimary = vag.IsPrimary;
                        ag.IsActive = vag.IsActive;
                        ag.CreatedBy = vag.CreatedBy;
                        ag.CreatedDt = vag.CreatedDt;
                        ag.ModifiedBy = vag.ModifiedBy;
                        ag.ModifiedDt = vag.ModifiedDt;

                        ag.Email = vag.AgentId.ToLower().Replace("smustf\\","") + "@smu.edu.sg";

                        agentList.Add(ag);
                    }
                }
            }
            return agentList;
        }

        public string GetDefaultAgent(string taskCd)
        {
            List<Agent> agentList = GetAgentAll();
            Agent ag = agentList.Where(x => (x.TaskCd.Trim().ToUpper() == taskCd.Trim().ToUpper()) &&
                                            (x.IsActive == true) && (x.IsPrimary == true)).FirstOrDefault();
            if (ag == null)
            {
                ag = agentList.Where(x => (x.TaskCd.Trim().ToUpper() == taskCd.Trim().ToUpper()) &&
                                            (x.IsActive == true)).FirstOrDefault();
                if (ag != null)
                {
                    return ag.AgentId;
                }
            }
            else
            {
                return ag.AgentId;
            }
            return null;
        }

        //Return true if there is changes, else false
        private bool SaveDbChanges(DbContext context)
        {
            bool hasChanges = false;
            try
            {
                hasChanges = context.ChangeTracker.HasChanges();
                context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                throw new DbEntityValidationException(
                    "Entity Validation Failed - errors follow:\n" +
                    sb.ToString(), ex
                ); // Add the original exception as the innerException
            }

            return hasChanges;
        }

        public List<Model.V_Task> UpdateWorkflowTasks(Model.OnboardingEntities obEntities, Model.V_Task completedTask, int reqId)
        {
            bool hasChanges = false;

            List<Model.V_Task> nextTasks = null;
            if (completedTask != null)
            {
                var vts = from a in obEntities.V_Task
                          join b in obEntities.V_TaskFlow
                              on a.TaskCd equals b.TaskCd
                          where (a.ReqId == reqId) &&
                                 (a.TaskStsCd == "PEN") && //NEW
                                 (b.PreTaskCd == completedTask.TaskCd)
                          select a;
                nextTasks = vts.ToList<Model.V_Task>();
            }
            else
            {
                var vts = from a in obEntities.V_Task
                          join b in obEntities.V_TaskFlow
                              on a.TaskCd equals b.TaskCd
                          where (a.ReqId == reqId) &&
                                 (a.TaskStsCd == "PEN") &&
                                 (string.IsNullOrEmpty(b.PreTaskCd))
                          select a;
                nextTasks = vts.ToList<Model.V_Task>();
            }
            if (nextTasks != null)
            {
                DateTime sqlDt = Common.GetSqlDateTime();
                foreach (Model.V_Task vt in nextTasks)
                {
                    vt.TaskStsCd = "PEN"; //PEN
                    vt.ModifiedBy = "SYSTEM";
                    vt.ModifiedDt = sqlDt;
                }
                hasChanges = SaveDbChanges(obEntities) || hasChanges;
            }
            return nextTasks;
        }

        public void NotifyAgents(Model.V_Req vr, List<Model.V_Task> vtl)
        {
            try
            {
                List<Agent> allAgents = GetAgentAll();
                if (vtl != null)
                {
                    foreach (Model.V_Task vt in vtl)
                    {
                        List<Agent> agents = allAgents.Where(x => x.TaskCd == vt.TaskCd).ToList<Agent>();
                        string emls = string.Empty;
                        if (agents != null)
                        {
                            for (int i = 0; i < agents.Count(); i++)
                            {
                                if (!string.IsNullOrEmpty(agents[i].Email))
                                {
                                    if (i > 0)
                                        emls += ",";
                                    emls += agents[i].Email;
                                }
                            }
                            if (!string.IsNullOrEmpty(emls))
                            {
                                string subject = ConfigurationManager.AppSettings["emailSubjectForTask"];
                                subject = subject.Replace("{id}", vr.ReqId.ToString());
                                subject = subject.Replace("{name}", string.Format("{0} {1}", vr.FirstName, vr.LastName));
                                string body = File.ReadAllText(ConfigurationManager.AppSettings["emailBodyForTask"]);
                                body = body.Replace("{id}", vr.ReqId.ToString());
                                body = body.Replace("{name}", string.Format("{0} {1}", vr.FirstName, vr.LastName));
                                Email.SendEmail(emls, subject, body);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {

                _logger.Error("Error in Web API - NotifyAgents", ex);
            }
        }

        public List<string> GetAgentTaskCd(string agentId)
        {
            List<string> taskCdList = new List<string>();
            using (Model.OnboardingEntities obEntities = new Model.OnboardingEntities())
            {
                List<Model.V_Agent> agtList = obEntities.V_Agent.Where(x => x.AgentId.Trim().ToLower() == agentId.Trim().ToLower() && x.IsActive == true).ToList<Model.V_Agent>();
                if (agtList != null)
                {
                    foreach (Model.V_Agent agt in agtList)
                    {
                        taskCdList.Add(agt.TaskCd.Trim().ToUpper());
                    }
                }
            }
            return taskCdList;
        }

        public List<TaskCount> GetTaskCount(string agentId)
        {
            List<TaskCount> tcList = new List<TaskCount>();
            List<string> taskCdList = GetAgentTaskCd(agentId);

            using (Model.OnboardingEntities obEntities = new Model.OnboardingEntities())
            {
                var query = from a in obEntities.V_Task
                            join b in obEntities.V_Req
                                on a.ReqId equals b.ReqId
                            where taskCdList.Contains(a.TaskCd) &&
                            (a.TaskStsCd == "PEN") && (b.ReqStsCd == "SBM") //TaskStsCd = PEN
                            group a by a.TaskCd into g
                            select new
                            {
                                g.Key,
                                TaskCount = g.Count()
                            };
                foreach (var item in query)
                {
                    TaskCount tc = new TaskCount();
                    tc.TaskCd = item.Key;
                    //tc.TaskDesc = GetTaskCodeDesc(item.Key);
                    tc.PendingCount = item.TaskCount;
                    tcList.Add(tc);
                }
                return tcList;
            }

        }
        
        
    }
}

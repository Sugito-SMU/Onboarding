using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding;
using Onboarding.Entity;

namespace Onboarding.DataProvider
{
    public class EmailAlertProvider
    {

        public Entity.OutstandingTaskAlert GetOutstandingTaskAlert()
        {
            DataProvider.Model.OnboardingEntities obCtx = new Model.OnboardingEntities();
            CostCentreProvider costCtrProv = new CostCentreProvider();
            RequestorProvider requestorProv = new RequestorProvider();
            TaskCodeProvider taskCdProv = new TaskCodeProvider();
            AgentProvider agentProv = new AgentProvider();
            UserProfileProvider usrProfileProv = new UserProfileProvider();


            double urgentRequestCutoff = 7;
            if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["UrgentRequestCutOff"]))
            {
                double.TryParse(ConfigurationManager.AppSettings["UrgentRequestCutOff"], out urgentRequestCutoff);
            }
            

            Entity.OutstandingTaskAlert outTaskAlert = null;

            DateTime emplStartCutOff = DateTime.Now.Date.AddDays(urgentRequestCutoff);

            var dbOffSearchResult = from a in obCtx.V_Req
                                 join b in obCtx.V_ReqEmp
                                    on a.ReqId equals b.ReqId
                                 join c in obCtx.V_OffTask
                                    on a.ReqId equals c.ReqId
                                 where (a.ReqStsCd != Entity.Constant.RequestStatusCode.Draft)
                                        && (c.TaskStsCd == Entity.Constant.TaskStatusCode.Pending)
                                 select new
                                 {
                                     a.ReqId,
                                     a.FirstNm,
                                     a.LastNm,
                                     b.CostCtrCd,
                                     b.EmpStartDt,
                                     a.SubmittedBy,
                                     a.SubmittedDt,
                                     c.TaskCd,
                                     a.ReqStsCd
                                 };

            var dbSysAccSearchResult = from a in obCtx.V_Req
                                    join b in obCtx.V_ReqEmp
                                       on a.ReqId equals b.ReqId
                                    join c in obCtx.V_SysAccTask
                                       on a.ReqId equals c.ReqId
                                    where (a.ReqStsCd != Entity.Constant.RequestStatusCode.Draft)
                                        && (c.TaskStsCd == Entity.Constant.TaskStatusCode.Pending)
                                    select new
                                    {
                                        a.ReqId,
                                        a.FirstNm,
                                        a.LastNm,
                                        b.CostCtrCd,
                                        b.EmpStartDt,
                                        a.SubmittedBy,
                                        a.SubmittedDt,
                                        c.TaskCd,
                                        a.ReqStsCd
                                    };

            if ((dbOffSearchResult.Count() > 0) || (dbSysAccSearchResult.Count() > 0))
            {
                outTaskAlert = new Entity.OutstandingTaskAlert();
                outTaskAlert.Rows = new List<Entity.OutstandingTaskAlertRow>();
                foreach (var dbRow in dbOffSearchResult)
                {
                    List<Entity.Agent> agentList = agentProv.GetAgentsForTask(dbRow.TaskCd);
                    if (agentList != null)
                    {
                        foreach (Entity.Agent agent in agentList)
                        {

                            Entity.OutstandingTaskAlertRow row = new Entity.OutstandingTaskAlertRow();
                            row.ReqId = dbRow.ReqId;
                            row.FirstName = dbRow.FirstNm;
                            row.LastName = dbRow.LastNm;
                            row.EmplStartDt = dbRow.EmpStartDt;
                            row.CostCtrDesc = costCtrProv.GetCostCentreDesc(dbRow.CostCtrCd);
                            row.RequestorName = requestorProv.GetRequestorName(dbRow.SubmittedBy);
                            row.RequestDt = dbRow.SubmittedDt;
                            row.TaskDesc = taskCdProv.GetTaskCodeDesc(dbRow.TaskCd);
                            row.AgentName = agent.AgentName;
                            row.AgentEmail = agent.Email;
                            row.IsUrgent = Onboarding.Common.Validation.IsUrgentRequest(dbRow.ReqStsCd, dbRow.EmpStartDt);
                            outTaskAlert.Rows.Add(row);
                        }
                    }
                }
                foreach (var dbRow in dbSysAccSearchResult)
                {
                    List<Entity.Agent> agentList = agentProv.GetAgentsForTask(dbRow.TaskCd);
                    if (agentList != null)
                    {
                        foreach (Entity.Agent agent in agentList)
                        {

                            Entity.OutstandingTaskAlertRow row = new Entity.OutstandingTaskAlertRow();
                            row.ReqId = dbRow.ReqId;
                            row.FirstName = dbRow.FirstNm;
                            row.LastName = dbRow.LastNm;
                            row.EmplStartDt = dbRow.EmpStartDt;
                            row.CostCtrDesc = costCtrProv.GetCostCentreDesc(dbRow.CostCtrCd);
                            row.RequestorName = requestorProv.GetRequestorName(dbRow.SubmittedBy);
                            row.RequestDt = dbRow.SubmittedDt;
                            row.TaskDesc = taskCdProv.GetTaskCodeDesc(dbRow.TaskCd);
                            row.AgentName = agent.AgentName;
                            row.AgentEmail = agent.Email;
                            row.IsUrgent = Onboarding.Common.Validation.IsUrgentRequest(dbRow.ReqStsCd, dbRow.EmpStartDt);
                            outTaskAlert.Rows.Add(row);
                        }
                    }
                }
            }
            return outTaskAlert;
        }

        public Entity.OutstandingRequestAlert GetOutstandingRequestAlert()
        {
            DataProvider.Model.OnboardingEntities obCtx = new Model.OnboardingEntities();
            CostCentreProvider costCtrProv = new CostCentreProvider();
            RequestorProvider requestorProv = new RequestorProvider();
            TaskCodeProvider taskCdProv = new TaskCodeProvider();
            AgentProvider agentProv = new AgentProvider();
            UserProfileProvider usrProfileProv = new UserProfileProvider();


            Entity.OutstandingRequestAlert outRequestAlert = null;

            var dbOffSearchResult = from a in obCtx.V_Req
                                 join b in obCtx.V_ReqEmp
                                    on a.ReqId equals b.ReqId
                                 join c in obCtx.V_OffTask
                                    on a.ReqId equals c.ReqId
                                 where (a.ReqStsCd != Entity.Constant.RequestStatusCode.Draft) &&
                                        (c.TaskStsCd != Entity.Constant.TaskStatusCode.Completed)
                                 select new
                                 {
                                     a.ReqId,
                                     a.FirstNm,
                                     a.LastNm,
                                     b.CostCtrCd,
                                     b.EmpStartDt,
                                     a.SubmittedBy,
                                     a.SubmittedDt,
                                     c.TaskCd,
                                     a.ReqStsCd
                                 };

            var dbSysAccSearchResult = from a in obCtx.V_Req
                                   join b in obCtx.V_ReqEmp
                                      on a.ReqId equals b.ReqId
                                   join c in obCtx.V_SysAccTask
                                      on a.ReqId equals c.ReqId
                                   where (a.ReqStsCd != Entity.Constant.RequestStatusCode.Draft) &&
                                          (c.TaskStsCd != Entity.Constant.TaskStatusCode.Completed)
                                   select new
                                   {
                                       a.ReqId,
                                       a.FirstNm,
                                       a.LastNm,
                                       b.CostCtrCd,
                                       b.EmpStartDt,
                                       a.SubmittedBy,
                                       a.SubmittedDt,
                                       c.TaskCd,
                                       a.ReqStsCd
                                   };

            if ((dbOffSearchResult.Count() > 0) || (dbSysAccSearchResult.Count() > 0))
            {
                outRequestAlert = new Entity.OutstandingRequestAlert();
                outRequestAlert.Rows = new List<Entity.OutstandingRequestAlertRow>();
                foreach (var dbRow in dbOffSearchResult)
                {
                    List<Entity.Agent> agentList = agentProv.GetAgentsForTask(dbRow.TaskCd);
                    string agentNameList = string.Empty;
                    if (agentList != null)
                    {
                        for (int i = 0; i < agentList.Count; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(agentList[i].AgentName))
                            {
                                if (!string.IsNullOrWhiteSpace(agentNameList))
                                {
                                    agentNameList += "; ";
                                }
                                agentNameList += agentList[i].AgentName;
                            }
                        }
                    }
                    Entity.OutstandingRequestAlertRow row = new Entity.OutstandingRequestAlertRow();
                    row.ReqId = dbRow.ReqId;
                    row.FirstName = dbRow.FirstNm;
                    row.LastName = dbRow.LastNm;
                    row.EmplStartDt = dbRow.EmpStartDt;
                    row.CostCtrDesc = costCtrProv.GetCostCentreDesc(dbRow.CostCtrCd);
                    row.RequestorName = requestorProv.GetRequestorName(dbRow.SubmittedBy);
                    row.RequestDt = dbRow.SubmittedDt;
                    row.TaskDesc = taskCdProv.GetTaskCodeDesc(dbRow.TaskCd);
                    row.AgentNameList = agentNameList;
                    row.IsUrgent = Onboarding.Common.Validation.IsUrgentRequest(dbRow.ReqStsCd, dbRow.EmpStartDt);
                    outRequestAlert.Rows.Add(row);
                }
                foreach (var dbRow in dbSysAccSearchResult)
                {
                    List<Entity.Agent> agentList = agentProv.GetAgentsForTask(dbRow.TaskCd);
                    string agentNameList = string.Empty;
                    if (agentList != null)
                    {
                        for (int i = 0; i < agentList.Count; i++)
                        {

                            if (!string.IsNullOrWhiteSpace(agentList[i].AgentName))
                            {
                                if (!string.IsNullOrWhiteSpace(agentNameList))
                                {
                                    agentNameList += "; ";
                                }
                                agentNameList += agentList[i].AgentName;
                            }
                        }
                    }
                    Entity.OutstandingRequestAlertRow row = new Entity.OutstandingRequestAlertRow();
                    row.ReqId = dbRow.ReqId;
                    row.FirstName = dbRow.FirstNm;
                    row.LastName = dbRow.LastNm;
                    row.EmplStartDt = dbRow.EmpStartDt;
                    row.CostCtrDesc = costCtrProv.GetCostCentreDesc(dbRow.CostCtrCd);
                    row.RequestorName = requestorProv.GetRequestorName(dbRow.SubmittedBy);
                    row.RequestDt = dbRow.SubmittedDt;
                    row.TaskDesc = taskCdProv.GetTaskCodeDesc(dbRow.TaskCd);
                    row.AgentNameList = agentNameList;
                    row.IsUrgent = Onboarding.Common.Validation.IsUrgentRequest(dbRow.ReqStsCd, dbRow.EmpStartDt);
                    outRequestAlert.Rows.Add(row);
                }
            }
            return outRequestAlert;
        }
        
    }
}

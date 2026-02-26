using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding;

namespace Onboarding.DataProvider
{
    public class RequestListForAgentProvider
    {

        public Entity.RequestListForAgent GetRequestListForAgent(string taskStatusCd, string userId)
        {
            DataProvider.Model.OnboardingEntities obCtx = new Model.OnboardingEntities();
            RequestTypeProvider reqTypeProv = new RequestTypeProvider();
            UserTypeProvider userTypeProv = new UserTypeProvider();
            UserSubTypeProvider userSubTypeProv = new UserSubTypeProvider();
            CostCentreProvider costCtrProv = new CostCentreProvider();
            RequestStatusProvider reqStsProv = new RequestStatusProvider();
            RequestorProvider requestorProv = new RequestorProvider();
            UserProfileProvider usrProfileProv = new UserProfileProvider();

            Entity.RequestListForAgent reqList = null;

            var dbOffQuery = from a in obCtx.V_Req
                                join b in obCtx.V_ReqEmp
                                    on a.ReqId equals b.ReqId
                                join c in obCtx.V_OffTask
                                    on a.ReqId equals c.ReqId
                                join d in obCtx.V_TaskAgent
                                    on c.TaskCd equals d.TaskCd
                          where (c.TaskStsCd != Entity.Constant.TaskStatusCode.New) && (d.AgentId == userId)
                          orderby b.EmpStartDt ascending, a.ReqId ascending
                          select new
                          {
                              a.ReqId,
                              a.ReqTypeCd,
                              a.FirstNm,
                              a.LastNm,
                              a.ReqStsCd,
                              a.SubmittedBy,
                              a.SubmittedDt,
                              b.UserTypeCd,
                              b.UserSubTypeCd,
                              b.EmpStartDt,
                              b.CostCtrCd,
                              c.TaskStsCd
                          };

            var dbSysAccQuery = from a in obCtx.V_Req
                             join b in obCtx.V_ReqEmp
                                 on a.ReqId equals b.ReqId
                             join c in obCtx.V_SysAccTask
                                 on a.ReqId equals c.ReqId
                             join d in obCtx.V_TaskAgent
                                 on c.TaskCd equals d.TaskCd
                             where (c.TaskStsCd != Entity.Constant.TaskStatusCode.New) && (d.AgentId == userId)
                             select new
                             {
                                 a.ReqId,
                                 a.ReqTypeCd,
                                 a.FirstNm,
                                 a.LastNm,
                                 a.ReqStsCd,
                                 a.SubmittedBy,
                                 a.SubmittedDt,
                                 b.UserTypeCd,
                                 b.UserSubTypeCd,
                                 b.EmpStartDt,
                                 b.CostCtrCd,
                                 c.TaskStsCd
                             };

            var pendingOffQuery = dbOffQuery.Where(x => x.TaskStsCd == Entity.Constant.TaskStatusCode.Pending);
            var pendingSysAccQuery = dbSysAccQuery.Where(x => x.TaskStsCd == Entity.Constant.TaskStatusCode.Pending);
            List<int> pendingList = new List<int>();
            foreach (var item in pendingOffQuery)
            {
                if (!pendingList.Contains(item.ReqId))
                {
                    pendingList.Add(item.ReqId);
                }
            }
            foreach (var item in pendingSysAccQuery)
            {
                if (!pendingList.Contains(item.ReqId))
                {
                    pendingList.Add(item.ReqId);
                }
            }

            var completedOffQuery = dbOffQuery.Where(x => x.TaskStsCd == Entity.Constant.TaskStatusCode.Completed);
            var completedSysAccQuery = dbSysAccQuery.Where(x => x.TaskStsCd == Entity.Constant.TaskStatusCode.Completed);
            List<int> completedList = new List<int>();
            foreach (var item in completedOffQuery)
            {
                if (!pendingList.Contains(item.ReqId))
                {
                    if (!completedList.Contains(item.ReqId))
                    {
                        completedList.Add(item.ReqId);
                    }
                }
            }
            foreach (var item in completedSysAccQuery)
            {
                if (!pendingList.Contains(item.ReqId))
                {
                    if (!completedList.Contains(item.ReqId))
                    {
                        completedList.Add(item.ReqId);
                    }
                }
            }

            List<int> reqIdList = null;
            switch (taskStatusCd)
            {
                case Entity.Constant.TaskStatusCode.Pending:
                    reqIdList = pendingList;
                    break;
                case Entity.Constant.TaskStatusCode.Completed:
                    reqIdList = completedList;
                    break;
                default:
                    break;
            }

            if ((reqIdList != null) && (reqIdList.Count > 0))
            {
                reqList = new Entity.RequestListForAgent();
                reqList.Rows = new List<Entity.RequestListForAgentRow>();
                foreach (int reqId in reqIdList)
                {
                    var dbRow = dbOffQuery.Where(x => x.ReqId == reqId).FirstOrDefault();
                    if (dbRow == null)
                    {
                        dbRow = dbSysAccQuery.Where(x => x.ReqId == reqId).FirstOrDefault();
                    }
                    if (dbRow != null)
                    {
                        Entity.RequestListForAgentRow row = new Entity.RequestListForAgentRow();
                        row.ReqId = dbRow.ReqId;
                        row.ReqTypeDesc = reqTypeProv.GetRequestTypeDesc(dbRow.ReqTypeCd);
                        row.FirstName = dbRow.FirstNm;
                        row.LastName = dbRow.LastNm;
                        row.UserTypeDesc = userTypeProv.GetUserTypeDesc(dbRow.UserTypeCd);
                        row.UserSubTypeDesc = userSubTypeProv.GetUserSubTypeDesc(dbRow.UserSubTypeCd);
                        row.EmplStartDt = dbRow.EmpStartDt;
                        row.CostCtrDesc = costCtrProv.GetCostCentreDesc(dbRow.CostCtrCd);
                        row.ReqStsDesc = reqStsProv.GetRequestStatusDesc(dbRow.ReqStsCd);
                        row.RequestorName = requestorProv.GetRequestorName(dbRow.SubmittedBy);
                        row.RequestDt = dbRow.SubmittedDt;
                        row.TaskStsCd = taskStatusCd;
                        if (taskStatusCd != Entity.Constant.TaskStatusCode.Completed)
                        {
                            row.IsUrgent = Onboarding.Common.Validation.IsUrgentRequest(dbRow.ReqStsCd, dbRow.EmpStartDt);
                        }
                        else
                        {
                            row.IsUrgent = false;
                        }
                        reqList.Rows.Add(row);
                    }
                }
            }
            return reqList;
        }

        public Entity.RequestListForAgent GetRequestListForHRAdmin(string taskStatusCd, string userId)
        {
            DataProvider.Model.OnboardingEntities obCtx = new Model.OnboardingEntities();
            RequestTypeProvider reqTypeProv = new RequestTypeProvider();
            UserTypeProvider userTypeProv = new UserTypeProvider();
            UserSubTypeProvider userSubTypeProv = new UserSubTypeProvider();
            CostCentreProvider costCtrProv = new CostCentreProvider();
            RequestStatusProvider reqStsProv = new RequestStatusProvider();
            RequestorProvider requestorProv = new RequestorProvider();
            UserProfileProvider usrProfileProv = new UserProfileProvider();

            Entity.RequestListForAgent reqList = null;

            var dbOffQuery = from a in obCtx.V_Req
                             join b in obCtx.V_ReqEmp
                                 on a.ReqId equals b.ReqId
                             join c in obCtx.V_OffTask
                                 on a.ReqId equals c.ReqId
                             join d in obCtx.V_TaskAgent
                                 on c.TaskCd equals d.TaskCd
                             where (c.TaskStsCd != Entity.Constant.TaskStatusCode.New) && (d.AgentId == userId)
                             orderby b.EmpStartDt ascending, a.ReqId ascending
                             select new
                             {
                                 a.ReqId,
                                 a.ReqTypeCd,
                                 a.FirstNm,
                                 a.LastNm,
                                 a.ReqStsCd,
                                 a.SubmittedBy,
                                 a.SubmittedDt,
                                 b.UserTypeCd,
                                 b.UserSubTypeCd,
                                 b.EmpStartDt,
                                 b.CostCtrCd,
                                 c.TaskStsCd
                             };

            var dbSysAccQuery = from a in obCtx.V_Req
                                join b in obCtx.V_ReqEmp
                                    on a.ReqId equals b.ReqId
                                join c in obCtx.V_SysAccTask
                                    on a.ReqId equals c.ReqId
                                join d in obCtx.V_TaskAgent
                                    on c.TaskCd equals d.TaskCd
                                where (c.TaskStsCd != Entity.Constant.TaskStatusCode.New) && (d.AgentId == userId)
                                select new
                                {
                                    a.ReqId,
                                    a.ReqTypeCd,
                                    a.FirstNm,
                                    a.LastNm,
                                    a.ReqStsCd,
                                    a.SubmittedBy,
                                    a.SubmittedDt,
                                    b.UserTypeCd,
                                    b.UserSubTypeCd,
                                    b.EmpStartDt,
                                    b.CostCtrCd,
                                    c.TaskStsCd
                                };

            var pendingOffQuery = dbOffQuery.Where(x => x.TaskStsCd == Entity.Constant.TaskStatusCode.Pending);
            var pendingSysAccQuery = dbSysAccQuery.Where(x => x.TaskStsCd == Entity.Constant.TaskStatusCode.Pending);
            List<int> pendingList = new List<int>();
            foreach (var item in pendingOffQuery)
            {
                if (!pendingList.Contains(item.ReqId) && Common.IsSchoolAdmin(item.SubmittedBy))
                {
                    pendingList.Add(item.ReqId);
                }
            }
            foreach (var item in pendingSysAccQuery)
            {
                if (!pendingList.Contains(item.ReqId) && Common.IsSchoolAdmin(item.SubmittedBy))
                {
                    pendingList.Add(item.ReqId);
                }
            }

            var completedOffQuery = dbOffQuery.Where(x => x.TaskStsCd == Entity.Constant.TaskStatusCode.Completed);
            var completedSysAccQuery = dbSysAccQuery.Where(x => x.TaskStsCd == Entity.Constant.TaskStatusCode.Completed);
            List<int> completedList = new List<int>();
            foreach (var item in completedOffQuery)
            {
                if (!pendingList.Contains(item.ReqId) && Common.IsSchoolAdmin(item.SubmittedBy))
                {
                    if (!completedList.Contains(item.ReqId))
                    {
                        completedList.Add(item.ReqId);
                    }
                }
            }
            foreach (var item in completedSysAccQuery)
            {
                if (!pendingList.Contains(item.ReqId) && Common.IsSchoolAdmin(item.SubmittedBy))
                {
                    if (!completedList.Contains(item.ReqId))
                    {
                        completedList.Add(item.ReqId);
                    }
                }
            }

            List<int> reqIdList = null;
            switch (taskStatusCd)
            {
                case Entity.Constant.TaskStatusCode.Pending:
                    reqIdList = pendingList;
                    break;
                case Entity.Constant.TaskStatusCode.Completed:
                    reqIdList = completedList;
                    break;
                default:
                    break;
            }

            if ((reqIdList != null) && (reqIdList.Count > 0))
            {
                reqList = new Entity.RequestListForAgent();
                reqList.Rows = new List<Entity.RequestListForAgentRow>();
                foreach (int reqId in reqIdList)
                {
                    var dbRow = dbOffQuery.Where(x => x.ReqId == reqId).FirstOrDefault();
                    if (dbRow == null)
                    {
                        dbRow = dbSysAccQuery.Where(x => x.ReqId == reqId).FirstOrDefault();
                    }
                    if (dbRow != null)
                    {
                        Entity.RequestListForAgentRow row = new Entity.RequestListForAgentRow();
                        row.ReqId = dbRow.ReqId;
                        row.ReqTypeDesc = reqTypeProv.GetRequestTypeDesc(dbRow.ReqTypeCd);
                        row.FirstName = dbRow.FirstNm;
                        row.LastName = dbRow.LastNm;
                        row.UserTypeDesc = userTypeProv.GetUserTypeDesc(dbRow.UserTypeCd);
                        row.UserSubTypeDesc = userSubTypeProv.GetUserSubTypeDesc(dbRow.UserSubTypeCd);
                        row.EmplStartDt = dbRow.EmpStartDt;
                        row.CostCtrDesc = costCtrProv.GetCostCentreDesc(dbRow.CostCtrCd);
                        row.ReqStsDesc = reqStsProv.GetRequestStatusDesc(dbRow.ReqStsCd);
                        row.RequestorName = requestorProv.GetRequestorName(dbRow.SubmittedBy);
                        row.RequestDt = dbRow.SubmittedDt;
                        row.TaskStsCd = taskStatusCd;
                        if (taskStatusCd != Entity.Constant.TaskStatusCode.Completed)
                        {
                            row.IsUrgent = Onboarding.Common.Validation.IsUrgentRequest(dbRow.ReqStsCd, dbRow.EmpStartDt);
                        }
                        else
                        {
                            row.IsUrgent = false;
                        }
                        reqList.Rows.Add(row);
                    }
                }
            }
            return reqList;
        }

    }
}

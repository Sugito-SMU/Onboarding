using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding;
using Onboarding.Entity;

namespace Onboarding.DataProvider
{
    public class RequestCountForAgentProvider
    {
        public List<Entity.RequestCountForAgent> GetRequestCountForAgent(string userId)
        {
            TaskStatusProvider taskStatusProv = new TaskStatusProvider();
            List<Entity.RequestCountForAgent> reqCountList = new List<RequestCountForAgent>();
            using (Model.OnboardingEntities obCtx = new Model.OnboardingEntities())
            {
                var dbOffQuery = from a in obCtx.V_OffTask
                            join b in obCtx.V_TaskAgent
                                on a.TaskCd equals b.TaskCd
                            where (a.TaskStsCd != Entity.Constant.TaskStatusCode.New) && (b.AgentId == userId)
                            select new
                            {
                                a.ReqId,
                                a.TaskStsCd
                            };

                var dbSysAccQuery = from a in obCtx.V_SysAccTask
                                 join b in obCtx.V_TaskAgent
                                     on a.TaskCd equals b.TaskCd
                                 where (a.TaskStsCd != Entity.Constant.TaskStatusCode.New) && (b.AgentId == userId)
                                 select new
                                 {
                                     a.ReqId,
                                     a.TaskStsCd
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

                RequestCountForAgent pendingCount = new RequestCountForAgent();
                pendingCount.TaskStsCd = Entity.Constant.TaskStatusCode.Pending;
                pendingCount.TaskStsDesc = taskStatusProv.GetTaskStatusDesc(pendingCount.TaskStsCd);
                pendingCount.ReqCount = pendingList.Count;
                reqCountList.Add(pendingCount);

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
                RequestCountForAgent completedCount = new RequestCountForAgent();
                completedCount.TaskStsCd = Entity.Constant.TaskStatusCode.Completed;
                completedCount.TaskStsDesc = taskStatusProv.GetTaskStatusDesc(completedCount.TaskStsCd);
                completedCount.ReqCount = completedList.Count;
                reqCountList.Add(completedCount);

            }
            return reqCountList;
        }

    }
}

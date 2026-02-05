using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding;
using Onboarding.DataProvider.Model;

namespace Onboarding.DataProvider
{
    public class TaskProvider
    {
        DataProvider.Model.OnboardingEntities _obCtx = null;
        public TaskProvider(DataProvider.Model.OnboardingEntities obCtx)
        {
            _obCtx = obCtx;
        }

        public Entity.Task GetTask(int taskId, string taskCd, string userId)
        {
            Entity.Task task = null;
            if ((taskId > 0) && (!string.IsNullOrWhiteSpace(taskCd)))
            {
                if (Onboarding.Common.Validation.IsOfficeTask(taskCd))
                {
                    Model.V_OffTask dbOffTask = _obCtx.V_OffTask.FirstOrDefault(x => x.OffTaskId == taskId);
                    if (dbOffTask != null)
                    {
                        task = new Entity.Task();
                        Mapper.MapToEntity.Map(dbOffTask, task, userId, _obCtx);
                    }
                }
                else
                {
                    Model.V_SysAccTask dbSysAccTask = _obCtx.V_SysAccTask.FirstOrDefault(x => x.SysAccTaskId == taskId);
                    if (dbSysAccTask != null)
                    {
                        task = new Entity.Task();
                        Mapper.MapToEntity.Map(dbSysAccTask, task, userId, _obCtx);
                    }
                }

                   
            }
            return task;
        }



        public Entity.Task GetSysAccTask(int taskId, string userId)
        {
            Entity.Task task = null;
            if (taskId > 0)
            {
                Model.V_SysAccTask dbSysAccTask = _obCtx.V_SysAccTask.FirstOrDefault(x => x.SysAccTaskId == taskId);
                if (dbSysAccTask != null)
                {
                    task = new Entity.Task();
                    Mapper.MapToEntity.Map(dbSysAccTask, task, userId, _obCtx);
                }
            }
            return task;
        }

        public void SaveActivatedTaskStatus(List<Entity.Task> taskList)
        {
            if (taskList != null)
            {
                foreach (Entity.Task task in taskList)
                {
                    if (Onboarding.Common.Validation.IsOfficeTask(task.TaskCd))
                    {
                        Model.V_OffTask dbOffTask = _obCtx.V_OffTask.FirstOrDefault(x => (x.OffTaskId == task.TaskId) && (x.TaskCd == task.TaskCd));
                        dbOffTask.TaskStsCd = task.TaskStsCd;
                    }
                    else
                    {
                        Model.V_SysAccTask dbSysAccTask = _obCtx.V_SysAccTask.FirstOrDefault(x => (x.SysAccTaskId == task.TaskId) && (x.TaskCd == task.TaskCd));
                        dbSysAccTask.TaskStsCd = task.TaskStsCd;
                    }
                }
                Common.SaveDbChanges(_obCtx);
            }
        }


        public int SaveTask(Entity.Task task, string userId)
        {
            if ((task != null) && (task.TaskId > 0))
            {
                DateTime sqlDateTime = Common.GetSqlDateTime();
                Model.V_Req dbReq = null;

                if (Onboarding.Common.Validation.IsOfficeTask(task.TaskCd))
                {
                    Model.V_OffTask dbOffTask = _obCtx.V_OffTask.FirstOrDefault(x => (x.OffTaskId == task.TaskId) && (x.TaskCd == task.TaskCd));
                    if (dbOffTask != null)
                    {
                        task.ReqId = dbOffTask.ReqId;
                        dbReq = _obCtx.V_Req.FirstOrDefault(x => x.ReqId == dbOffTask.ReqId);
                        Mapper.MapToDb.Map(task, userId, sqlDateTime, dbReq, dbOffTask, _obCtx);
                        Common.SaveDbChanges(_obCtx);
                    }
                }
                else
                {
                    Model.V_SysAccTask dbSysAccTask = _obCtx.V_SysAccTask.FirstOrDefault(x => (x.SysAccTaskId == task.TaskId) && (x.TaskCd == task.TaskCd));
                    if (dbSysAccTask != null)
                    {
                        task.ReqId = dbSysAccTask.ReqId;
                        dbReq = _obCtx.V_Req.FirstOrDefault(x => x.ReqId == dbSysAccTask.ReqId);
                        Mapper.MapToDb.Map(task, userId, sqlDateTime, dbReq, dbSysAccTask, _obCtx);
                        Common.SaveDbChanges(_obCtx);
                    }

                }

                int offTaskCount = _obCtx.V_OffTask.Where(x => (x.ReqId == dbReq.ReqId) && (x.TaskStsCd != Entity.Constant.TaskStatusCode.Completed)).Count();
                int sysAccTaskCount = _obCtx.V_SysAccTask.Where(x => (x.ReqId == dbReq.ReqId) && (x.TaskStsCd != Entity.Constant.TaskStatusCode.Completed)).Count();
                if ((offTaskCount == 0) && (sysAccTaskCount == 0))
                {
                    if (dbReq.ReqStsCd == Entity.Constant.RequestStatusCode.Submitted)
                    {
                        dbReq.ReqStsCd = Entity.Constant.RequestStatusCode.Closed;
                        dbReq.ModifiedBy = userId;
                        dbReq.ModifiedDt = sqlDateTime;
                    }
                }
                Common.SaveDbChanges(_obCtx);

            }
            return task.TaskId;
        }

        public bool IsTaskChanged(Entity.Task updatedTask, string userId)
        {
            bool isTaskChanged = false;
            Entity.Task currentTask = GetTask(updatedTask.TaskId, updatedTask.TaskCd, userId);
            if ((currentTask == null) || (currentTask.TaskStsCd != Entity.Constant.TaskStatusCode.Completed))
            {
                isTaskChanged = true;
            }
            else if (currentTask.TaskStsCd == Entity.Constant.TaskStatusCode.Completed)
            {
                if (currentTask.Attributes != null)
                {
                    foreach (var currentAttr in currentTask.Attributes)
                    {
                        var newAttr = updatedTask.GetTaskAttribute(currentAttr.TaskAttrbCd);
                        if ((newAttr != null) && (newAttr.TaskAttrbVal != currentAttr.TaskAttrbVal))
                        {
                            isTaskChanged = true;
                        }
                    }
                }
                if (updatedTask.Remark != currentTask.Remark)
                {
                    isTaskChanged = true;
                }
            }
            return isTaskChanged;
        }
    }
}

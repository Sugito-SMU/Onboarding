using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onboarding;

namespace Onboarding.DataProvider.Mapper
{
    public static partial class MapToDb
    {

        public static void MapTasks(List<Entity.Task> tasks, string userId, DateTime sqlDateTime, Model.V_Req dbReq, Model.OnboardingEntities obCtx)
        {
            if ((tasks != null) && (tasks.Count > 0) && dbReq != null)
            {
                List<Model.V_OffTask> dbOffTaskList = obCtx.V_OffTask.Where(x => x.ReqId == dbReq.ReqId).ToList();
                List<Model.V_SysAccTask> dbSysAccTaskList = obCtx.V_SysAccTask.Where(x => x.ReqId == dbReq.ReqId).ToList();

                //Insert new or update existing task
                foreach (Entity.Task tsk in tasks)
                {
                    if (Onboarding.Common.Validation.IsOfficeTask(tsk.TaskCd))
                    {
                        Model.V_OffTask dbOffTask = dbOffTaskList.Where(x => x.TaskCd == tsk.TaskCd).FirstOrDefault();
                        if (dbOffTask == null)
                        {
                            dbOffTask = new Model.V_OffTask();
                            dbOffTask.ReqId = dbReq.ReqId;
                            dbOffTask.TaskCd = tsk.TaskCd;
                            dbOffTask.TaskStsCd = tsk.TaskStsCd;
                            dbOffTask.Remark = tsk.Remark;
                            dbOffTask.IsActive = tsk.IsActive;
                            dbOffTask.CreatedBy = userId;
                            dbOffTask.CreatedDt = sqlDateTime;
                            obCtx.V_OffTask.Add(dbOffTask);
                            Common.SaveDbChanges(obCtx);
                        }
                        Map(tsk, userId, sqlDateTime, dbReq, dbOffTask, obCtx);
                    }
                    else
                    {
                        Model.V_SysAccTask dbSysAccTask = dbSysAccTaskList.Where(x => x.TaskCd == tsk.TaskCd).FirstOrDefault();
                        if (dbSysAccTask == null)
                        {
                            dbSysAccTask = new Model.V_SysAccTask();
                            dbSysAccTask.ReqId = dbReq.ReqId;
                            dbSysAccTask.TaskCd = tsk.TaskCd;
                            dbSysAccTask.TaskStsCd = tsk.TaskStsCd;
                            dbSysAccTask.Remark = tsk.Remark;
                            dbSysAccTask.IsActive = tsk.IsActive;
                            dbSysAccTask.CreatedBy = userId;
                            dbSysAccTask.CreatedDt = sqlDateTime;
                            obCtx.V_SysAccTask.Add(dbSysAccTask);
                            Common.SaveDbChanges(obCtx);
                        }
                        Map(tsk, userId, sqlDateTime, dbReq, dbSysAccTask, obCtx);

                    }
                }

                //remove deleted tasks
                foreach (Model.V_OffTask dbOffTask in dbOffTaskList)
                {
                    if (tasks.Where(x => x.TaskCd == dbOffTask.TaskCd).FirstOrDefault() == null)
                    {
                        Remove(dbOffTask, obCtx);
                    }
                }
                foreach (Model.V_SysAccTask dbSysAccTask in dbSysAccTaskList)
                {
                    if (tasks.Where(x => x.TaskCd == dbSysAccTask.TaskCd).FirstOrDefault() == null)
                    {
                        Remove(dbSysAccTask, obCtx);
                    }
                }
            }
        }

        public static void Remove(Model.V_OffTask dbOffTask, Model.OnboardingEntities obCtx)
        {
            List<Model.V_OffTaskAttrb> dbOffTaskAttrbList = obCtx.V_OffTaskAttrb.Where(x => x.OffTaskId == dbOffTask.OffTaskId).ToList();
            if (dbOffTaskAttrbList != null)
            {
                foreach (Model.V_OffTaskAttrb dbOffTaskAttrb in dbOffTaskAttrbList)
                {
                    obCtx.V_OffTaskAttrb.Remove(dbOffTaskAttrb);
                }
            }
            Common.SaveDbChanges(obCtx);
            obCtx.V_OffTask.Remove(dbOffTask);
            Common.SaveDbChanges(obCtx);
        }
        
        public static void Remove(Model.V_SysAccTask dbSysAccTask, Model.OnboardingEntities obCtx)
        {
            List<Model.V_SysAccTaskAttrb> dbSysAccTaskAttrbList = obCtx.V_SysAccTaskAttrb.Where(x => x.SysAccTaskId == dbSysAccTask.SysAccTaskId).ToList();
            if (dbSysAccTaskAttrbList != null)
            {
                foreach (Model.V_SysAccTaskAttrb dbTaskAttrb in dbSysAccTaskAttrbList)
                {
                    obCtx.V_SysAccTaskAttrb.Remove(dbTaskAttrb);
                }
            }
            Common.SaveDbChanges(obCtx);
            obCtx.V_SysAccTask.Remove(dbSysAccTask);
            Common.SaveDbChanges(obCtx);
        }


        public static void Map(Entity.Task task, string userId, DateTime sqlDateTime, Model.V_Req dbReq, Model.V_OffTask dbOffTask, Model.OnboardingEntities obCtx)
        {
            if ((dbOffTask != null) && (task != null))
            {
                LogOffTaskChange("Remarks", dbOffTask.Remark, task.Remark, null, null, userId, sqlDateTime, dbReq, dbOffTask, obCtx);
                dbOffTask.Remark = task.Remark;

                string prevCancelStatus = dbOffTask.IsActive.HasValue ? (!dbOffTask.IsActive.Value).ToString() : string.Empty;
                string newCancelStatus = task.IsActive.HasValue ? (!task.IsActive.Value).ToString() : string.Empty;
                LogOffTaskChange("Cancellation Status", prevCancelStatus, newCancelStatus, null, null, userId, sqlDateTime, dbReq, dbOffTask, obCtx);
                dbOffTask.IsActive = task.IsActive;

                dbOffTask.TaskStsCd = task.TaskStsCd;

                if (task.TaskStsCd == Entity.Constant.TaskStatusCode.Completed)
                {
                    dbOffTask.CompletedBy = userId;
                    dbOffTask.CompletedDt = sqlDateTime;
                }

                if (task.Attributes != null)
                {
                    foreach (Entity.TaskAttribute taskAttrb in task.Attributes)
                    {
                        Model.V_OffTaskAttrb dbOffTaskAttrb = obCtx.V_OffTaskAttrb.Where(x => (x.OffTaskId == dbOffTask.OffTaskId) && (x.TaskAttrbCd == taskAttrb.TaskAttrbCd)).FirstOrDefault();
                        if (dbOffTaskAttrb == null)
                        {
                            dbOffTaskAttrb = new Model.V_OffTaskAttrb();
                            dbOffTaskAttrb.OffTaskId = dbOffTask.OffTaskId;
                            dbOffTaskAttrb.TaskAttrbCd = taskAttrb.TaskAttrbCd;
                            dbOffTaskAttrb.CreatedBy = userId;
                            dbOffTaskAttrb.CreatedDt = sqlDateTime;
                            obCtx.V_OffTaskAttrb.Add(dbOffTaskAttrb);
                        }
                        else
                        {
                            LogOffTaskAttrbChange(dbOffTaskAttrb.TaskAttrbVal, taskAttrb.TaskAttrbVal, null, null, userId, sqlDateTime, dbReq, dbOffTask, dbOffTaskAttrb, obCtx);
                        }
                        dbOffTaskAttrb.TaskAttrbVal = taskAttrb.TaskAttrbVal;

                    }
                }
            }
        }

        public static void Map(Entity.Task task, string userId, DateTime sqlDateTime, Model.V_Req dbReq, Model.V_SysAccTask dbSysAccTask, Model.OnboardingEntities obCtx)
        {
            if ((dbSysAccTask != null) && (task != null))
            {
                LogSysAccTaskChange("Remarks", dbSysAccTask.Remark, task.Remark, null, null, userId, sqlDateTime, dbReq, dbSysAccTask, obCtx);
                dbSysAccTask.Remark = task.Remark;

                string prevCancelStatus = dbSysAccTask.IsActive.HasValue ? (!dbSysAccTask.IsActive.Value).ToString() : string.Empty;
                string newCancelStatus = task.IsActive.HasValue ? (!task.IsActive.Value).ToString() : string.Empty;
                LogSysAccTaskChange("Cancellation Status", prevCancelStatus, newCancelStatus, null, null, userId, sqlDateTime, dbReq, dbSysAccTask, obCtx);
                dbSysAccTask.IsActive = task.IsActive;

                dbSysAccTask.TaskStsCd = task.TaskStsCd;

                if (task.TaskStsCd == Entity.Constant.TaskStatusCode.Completed)
                {
                    dbSysAccTask.CompletedBy = userId;
                    dbSysAccTask.CompletedDt = sqlDateTime;
                }

                if (task.Attributes != null)
                {
                    foreach (Entity.TaskAttribute taskAttrb in task.Attributes)
                    {
                        Model.V_SysAccTaskAttrb dbSysAccTaskAttrb = obCtx.V_SysAccTaskAttrb.Where(x => (x.SysAccTaskId == dbSysAccTask.SysAccTaskId) && (x.TaskAttrbCd == taskAttrb.TaskAttrbCd)).FirstOrDefault();
                        if (dbSysAccTaskAttrb == null)
                        {
                            dbSysAccTaskAttrb = new Model.V_SysAccTaskAttrb();
                            dbSysAccTaskAttrb.SysAccTaskId = dbSysAccTask.SysAccTaskId;
                            dbSysAccTaskAttrb.TaskAttrbCd = taskAttrb.TaskAttrbCd;
                            dbSysAccTaskAttrb.CreatedBy = userId;
                            dbSysAccTaskAttrb.CreatedDt = sqlDateTime;
                            obCtx.V_SysAccTaskAttrb.Add(dbSysAccTaskAttrb);
                        }
                        else
                        {
                            LogSysAccTaskAttrbChange(dbSysAccTaskAttrb.TaskAttrbVal, taskAttrb.TaskAttrbVal, null, null, userId, sqlDateTime, dbReq, dbSysAccTask, dbSysAccTaskAttrb, obCtx);
                        }
                        dbSysAccTaskAttrb.TaskAttrbVal = taskAttrb.TaskAttrbVal;

                    }
                }
            }
        }




    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onboarding;

namespace Onboarding.DataProvider.Mapper
{
    public static partial class MapToEntity
    {

        public static void MapTasks(Model.V_Req dbReq, List<Entity.Task> tasks, string userId, Model.OnboardingEntities obCtx)
        {
            if ((tasks != null) && (dbReq != null))
            {
                List<Model.V_OffTask> dbOffTaskList = obCtx.V_OffTask.Where(x => x.ReqId == dbReq.ReqId).ToList();
                if (dbOffTaskList != null)
                {
                    foreach (Model.V_OffTask dbTask in dbOffTaskList)
                    {
                        Entity.Task tsk = tasks.Where(x => x.TaskCd == dbTask.TaskCd).FirstOrDefault();
                        if (tsk == null)
                        {
                            tsk = new Entity.Task();
                            tasks.Add(tsk);
                        }
                        Map(dbTask, tsk, userId, obCtx);
                    }
                }

                List<Model.V_SysAccTask> dbSysAccTaskList = obCtx.V_SysAccTask.Where(x => x.ReqId == dbReq.ReqId).ToList();
                if (dbSysAccTaskList != null)
                {
                    foreach (Model.V_SysAccTask dbTask in dbSysAccTaskList)
                    {
                        Entity.Task tsk = tasks.Where(x => x.TaskCd == dbTask.TaskCd).FirstOrDefault();
                        if (tsk == null)
                        {
                            tsk = new Entity.Task();
                            tasks.Add(tsk);
                        }
                        Map(dbTask, tsk, userId, obCtx);
                    }
                }

            }
        }

        public static void Map(Model.V_OffTask dbOffTask, Entity.Task task, string userId, Model.OnboardingEntities obCtx)
        {
            TaskCodeProvider taskCodeProv = new TaskCodeProvider();
            TaskStatusProvider taskStatusProv = new TaskStatusProvider();
            TaskAttributeCodeProvider taskAttrbProv = new TaskAttributeCodeProvider();
            UserProfileProvider usrProv = new UserProfileProvider();
            AgentProvider agentProv = new AgentProvider();

            if ((dbOffTask != null) && (task != null))
            {
                task.TaskId = dbOffTask.OffTaskId;
                task.ReqId = dbOffTask.ReqId;
                task.TaskCd = dbOffTask.TaskCd;
                task.TaskDesc = taskCodeProv.GetTaskCodeDesc(dbOffTask.TaskCd);
                task.TaskStsCd = dbOffTask.TaskStsCd;
                task.TaskStsDesc = taskStatusProv.GetTaskStatusDesc(dbOffTask.TaskStsCd);
                task.Remark = dbOffTask.Remark;
                Entity.UserProfile p = usrProv.GetUserProfile(dbOffTask.CompletedBy);
                if (p != null)
                {
                    task.CompletedByName = p.Name;
                }
                task.CompletedDt = dbOffTask.CompletedDt;
                task.IsActive = dbOffTask.IsActive;
                task.CreatedBy = dbOffTask.CreatedBy;
                task.CreatedDt = dbOffTask.CreatedDt;
                task.ModifiedBy = dbOffTask.ModifiedBy;
                task.ModifiedDt = dbOffTask.ModifiedDt;

                List<Entity.Agent> agentList = agentProv.GetAgentsForTask(dbOffTask.TaskCd);
                if (agentList.Where(x => x.AgentId.ToLower().Trim() == userId.ToLower().Trim()).Count() > 0)
                {
                    task.IsMyTask = true;
                }
                else
                {
                    task.IsMyTask = false;
                }

                List<Model.V_OffTaskAttrb> dbTaskAttrbList = obCtx.V_OffTaskAttrb.Where(x => x.OffTaskId == dbOffTask.OffTaskId).ToList();
                if ((dbTaskAttrbList != null) && (dbTaskAttrbList.Count > 0))
                {
                    if (task.Attributes == null)
                    {
                        task.Attributes = new List<Entity.TaskAttribute>();
                    }
                    foreach (Model.V_OffTaskAttrb dbTaskAttrb in dbTaskAttrbList)
                    {
                        Entity.TaskAttribute taskAttrb = task.GetTaskAttribute(dbTaskAttrb.TaskAttrbCd);
                        if (taskAttrb == null)
                        {
                            taskAttrb = new Entity.TaskAttribute();
                            task.Attributes.Add(taskAttrb);
                        }
                        taskAttrb.TaskAttrbId = dbTaskAttrb.OffTaskAttrbId;
                        taskAttrb.TaskId = dbTaskAttrb.OffTaskId;
                        taskAttrb.TaskAttrbCd = dbTaskAttrb.TaskAttrbCd;
                        taskAttrb.TaskAttrbDesc = taskAttrbProv.GetTaskAttributeCodeDesc(dbTaskAttrb.TaskAttrbCd);
                        taskAttrb.TaskAttrbVal = dbTaskAttrb.TaskAttrbVal;
                        taskAttrb.CreatedBy = dbTaskAttrb.CreatedBy;
                        taskAttrb.CreatedDt = dbTaskAttrb.CreatedDt;
                        taskAttrb.ModifiedBy = dbTaskAttrb.ModifiedBy;
                        taskAttrb.ModifiedDt = dbTaskAttrb.ModifiedDt;
                    }
                }
            }
        }


        public static void Map(Model.V_SysAccTask dbSysAccTask, Entity.Task task, string userId, Model.OnboardingEntities obCtx)
        {
            TaskCodeProvider taskCodeProv = new TaskCodeProvider();
            TaskStatusProvider taskStatusProv = new TaskStatusProvider();
            TaskAttributeCodeProvider taskAttrbProv = new TaskAttributeCodeProvider();
            UserProfileProvider usrProv = new UserProfileProvider();
            AgentProvider agentProv = new AgentProvider();

            if ((dbSysAccTask != null) && (task != null))
            {
                task.TaskId = dbSysAccTask.SysAccTaskId;
                task.ReqId = dbSysAccTask.ReqId;
                task.TaskCd = dbSysAccTask.TaskCd;
                task.TaskDesc = taskCodeProv.GetTaskCodeDesc(dbSysAccTask.TaskCd);
                task.TaskStsCd = dbSysAccTask.TaskStsCd;
                task.TaskStsDesc = taskStatusProv.GetTaskStatusDesc(dbSysAccTask.TaskStsCd);
                task.Remark = dbSysAccTask.Remark;
                Entity.UserProfile p = usrProv.GetUserProfile(dbSysAccTask.CompletedBy);
                if (p != null)
                {
                    task.CompletedByName = p.Name;
                }
                task.CompletedDt = dbSysAccTask.CompletedDt;
                task.IsActive = dbSysAccTask.IsActive;
                task.CreatedBy = dbSysAccTask.CreatedBy;
                task.CreatedDt = dbSysAccTask.CreatedDt;
                task.ModifiedBy = dbSysAccTask.ModifiedBy;
                task.ModifiedDt = dbSysAccTask.ModifiedDt;

                List<Entity.Agent> agentList = agentProv.GetAgentsForTask(dbSysAccTask.TaskCd);
                if (agentList.Where(x => x.AgentId.ToLower().Trim() == userId.ToLower().Trim()).Count() > 0)
                {
                    task.IsMyTask = true;
                }
                else
                {
                    task.IsMyTask = false;
                }

                List<Model.V_SysAccTaskAttrb> dbTaskAttrbList = obCtx.V_SysAccTaskAttrb.Where(x => x.SysAccTaskId == dbSysAccTask.SysAccTaskId).ToList();
                if ((dbTaskAttrbList != null) && (dbTaskAttrbList.Count > 0))
                {
                    if (task.Attributes == null)
                    {
                        task.Attributes = new List<Entity.TaskAttribute>();
                    }
                    foreach (Model.V_SysAccTaskAttrb dbTaskAttrb in dbTaskAttrbList)
                    {
                        Entity.TaskAttribute taskAttrb = task.GetTaskAttribute(dbTaskAttrb.TaskAttrbCd);
                        if (taskAttrb == null)
                        {
                            taskAttrb = new Entity.TaskAttribute();
                            task.Attributes.Add(taskAttrb);
                        }
                        taskAttrb.TaskAttrbId = dbTaskAttrb.SysAccTaskAttrbId;
                        taskAttrb.TaskId = dbTaskAttrb.SysAccTaskId;
                        taskAttrb.TaskAttrbCd = dbTaskAttrb.TaskAttrbCd;
                        taskAttrb.TaskAttrbDesc = taskAttrbProv.GetTaskAttributeCodeDesc(dbTaskAttrb.TaskAttrbCd);
                        taskAttrb.TaskAttrbVal = dbTaskAttrb.TaskAttrbVal;
                        taskAttrb.CreatedBy = dbTaskAttrb.CreatedBy;
                        taskAttrb.CreatedDt = dbTaskAttrb.CreatedDt;
                        taskAttrb.ModifiedBy = dbTaskAttrb.ModifiedBy;
                        taskAttrb.ModifiedDt = dbTaskAttrb.ModifiedDt;
                    }
                }
            }
        }


    }
}

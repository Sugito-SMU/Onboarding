using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding;
using Onboarding.Entity;

namespace Onboarding.DataProvider
{
    public class TaskStatusProvider
    {
        public List<Entity.TaskStatus> GetTaskStatusAll()
        {
            List<Entity.TaskStatus> taskStatusList = (List<Entity.TaskStatus>)MCache.Get("TaskStatus");
            if (taskStatusList == null)
            {
                taskStatusList = new List<Entity.TaskStatus>();
                using (Model.OnboardingEntities obCtx = new Model.OnboardingEntities())
                {
                    List<Model.V_TaskStsCd> dbTaskStatusList = obCtx.V_TaskStsCd.ToList<Model.V_TaskStsCd>();
                    if (dbTaskStatusList != null)
                    {
                        foreach (Model.V_TaskStsCd dbTaskStsCd in dbTaskStatusList)
                        {
                            Entity.TaskStatus taskSts = new Entity.TaskStatus();
                            Mapper.MapToEntity.Map(dbTaskStsCd, taskSts);
                            taskStatusList.Add(taskSts);
                        }
                    }
                }
                MCache.Set("TaskStatus", taskStatusList);
            }
            return taskStatusList;
        }

        public string GetTaskStatusDesc(string taskStsCd)
        {
            if (!string.IsNullOrWhiteSpace(taskStsCd))
            {
                List<Entity.TaskStatus> taskStatusList = GetTaskStatusAll();
                Entity.TaskStatus taskStatus = taskStatusList.FirstOrDefault(x => x.TaskStsCd.Trim().ToLower() == taskStsCd.Trim().ToLower());
                if (taskStatus != null)
                {
                    return taskStatus.TaskStsDesc;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }


    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding;
using Onboarding.Entity;

namespace Onboarding.DataProvider
{
    public class TaskFlowProvider
    {
        public List<TaskFlow> GetTaskFlowAll()
        {
            List<TaskFlow> taskFlowList = (List<TaskFlow>)MCache.Get("TaskFlow");
            if (taskFlowList == null)
            {
                taskFlowList = new List<TaskFlow>();
                using (Model.OnboardingEntities obCtx = new Model.OnboardingEntities())
                {
                    List<Model.V_TaskDep> dbTaskFlowList = obCtx.V_TaskDep.ToList<Model.V_TaskDep>();
                    if (dbTaskFlowList != null)
                    {
                        foreach (Model.V_TaskDep dbTaskFlow in dbTaskFlowList)
                        {
                            TaskFlow taskFlow = new TaskFlow();
                            Mapper.MapToEntity.Map(dbTaskFlow, taskFlow);
                            taskFlowList.Add(taskFlow);
                        }
                    }
                }
                MCache.Set("TaskFlow", taskFlowList);
            }
            return taskFlowList;
        }
        


    }
}

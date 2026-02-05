using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding;
using Onboarding.Entity;

namespace Onboarding.DataProvider
{
    public class TaskCodeProvider
    {
        public List<TaskCode> GetTaskCodeAll()
        {
            List<TaskCode> taskCodeList = (List<TaskCode>)MCache.Get("TaskCode");
            if (taskCodeList == null)
            {
                taskCodeList = new List<TaskCode>();
                using (Model.OnboardingEntities obCtx = new Model.OnboardingEntities())
                {
                    List<Model.V_TaskCd> dbTaskCodeList = obCtx.V_TaskCd.ToList<Model.V_TaskCd>();
                    if (dbTaskCodeList != null)
                    {
                        foreach (Model.V_TaskCd dbTaskCd in dbTaskCodeList)
                        {
                            TaskCode taskCd = new TaskCode();
                            Mapper.MapToEntity.Map(dbTaskCd, taskCd);
                            taskCodeList.Add(taskCd);
                        }
                    }
                }
                MCache.Set("TaskCode", taskCodeList);
            }
            return taskCodeList;
        }

        public string GetTaskCodeDesc(string taskCd)
        {
            if (!string.IsNullOrWhiteSpace(taskCd))
            {
                List<TaskCode> taskCodeList = GetTaskCodeAll();
                TaskCode taskCode = taskCodeList.FirstOrDefault(x => x.TaskCd.Trim().ToLower() == taskCd.Trim().ToLower());
                if (taskCode != null)
                {
                    return taskCode.TaskDesc;
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

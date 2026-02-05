using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding;
using Onboarding.Entity;

namespace Onboarding.DataProvider
{
    public class TaskAttributeCodeProvider
    {
        public List<TaskAttributeCode> GetTaskAttributeCodeAll()
        {
            List<TaskAttributeCode> taskAttributeCodeList = (List<TaskAttributeCode>)MCache.Get("TaskAttributeCode");
            if (taskAttributeCodeList == null)
            {
                taskAttributeCodeList = new List<TaskAttributeCode>();
                using (Model.OnboardingEntities obCtx = new Model.OnboardingEntities())
                {
                    List<Model.V_TaskAttrbCd> dbTaskAttributeCodeList = obCtx.V_TaskAttrbCd.ToList<Model.V_TaskAttrbCd>();
                    if (dbTaskAttributeCodeList != null)
                    {
                        foreach (Model.V_TaskAttrbCd dbTaskAttrbCd in dbTaskAttributeCodeList)
                        {
                            TaskAttributeCode TaskAttrbCd = new TaskAttributeCode();
                            Mapper.MapToEntity.Map(dbTaskAttrbCd, TaskAttrbCd);
                            taskAttributeCodeList.Add(TaskAttrbCd);
                        }
                    }
                }
                MCache.Set("TaskAttributeCode", taskAttributeCodeList);
            }
            return taskAttributeCodeList;
        }

        public string GetTaskAttributeCodeDesc(string TaskAttrbCd)
        {
            if (!string.IsNullOrWhiteSpace(TaskAttrbCd))
            {
                List<TaskAttributeCode> TaskAttributeCodeList = GetTaskAttributeCodeAll();
                TaskAttributeCode TaskAttributeCode = TaskAttributeCodeList.FirstOrDefault(x => x.TaskAttrbCd.Trim().ToLower() == TaskAttrbCd.Trim().ToLower());
                if (TaskAttributeCode != null)
                {
                    return TaskAttributeCode.TaskAttrbDesc;
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

using Onboarding.DataProvider.Model;
using Onboarding.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.DataProvider
{
    public class TaskAgentProvider
    {

        public List<TaskAgent> GetTaskAgentAll()
        {
            List<TaskAgent> taskAgentList = (List<TaskAgent>)MCache.Get("TaskAgent");
            if (taskAgentList == null)
            {

                taskAgentList = new List<TaskAgent>();
                using (Model.OnboardingEntities obCtx = new Model.OnboardingEntities())
                {
                    List<V_TaskAgent> dbTaskAgentList = obCtx.V_TaskAgent.ToList<V_TaskAgent>();
                    if (dbTaskAgentList != null)
                    {
                        foreach (V_TaskAgent dbTaskAgent in dbTaskAgentList)
                        {
                            TaskAgent taskAgent = new TaskAgent();
                            Mapper.MapToEntity.Map(dbTaskAgent, taskAgent);
                            taskAgentList.Add(taskAgent);
                        }
                    }
                }
            }
            MCache.Set("TaskAgent", taskAgentList);
            return taskAgentList;
        }
    }
}

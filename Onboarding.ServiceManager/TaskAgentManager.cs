using Onboarding.DataProvider;
using Onboarding.Entity;
using Onboarding.ServiceManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.ServiceManager
{
    public class TaskAgentManager
    {
        public List<Entity.TaskAgent> GetTaskAgent(string taskCd)
        {
            TaskAgentProvider taskAgentProvider = new TaskAgentProvider();
            List<Entity.TaskAgent> taskAgentList = taskAgentProvider.GetTaskAgentAll();
            return taskAgentList.Where(x => x.IsActive == true && x.TaskCd == taskCd).ToList();
        }
    }
}
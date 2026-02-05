using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding.DataProvider;
using Onboarding.DataProvider.Model;

namespace Onboarding.ServiceManager
{
    public class TaskStatusManager
    {
        public List<Entity.TaskStatus> GetTaskStatusAll()
        {
            TaskStatusProvider taskStatusProvider = new TaskStatusProvider();
            List<Entity.TaskStatus> taskStatusList = taskStatusProvider.GetTaskStatusAll();
            return taskStatusList.Where(x => x.IsActive == true).ToList();
        }

        public string GetTaskStatusDesc(string taskStatusCd)
        {
            TaskStatusProvider taskStatusProvider = new TaskStatusProvider();
            string taskStatusDesc = taskStatusProvider.GetTaskStatusDesc(taskStatusCd);
            return taskStatusDesc;
        }

    }
}

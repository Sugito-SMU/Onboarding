using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding.DataProvider;
using Onboarding.DataProvider.Model;

namespace Onboarding.ServiceManager
{
    public class TaskCodeManager
    {
        public List<Entity.TaskCode> GetTaskCodeAll()
        {
            TaskCodeProvider taskCodeProvider = new TaskCodeProvider();
            List<Entity.TaskCode> taskCodeList = taskCodeProvider.GetTaskCodeAll();
            return taskCodeList.Where(x => x.IsActive == true).ToList();
        }

        public string GetTaskCodeDesc(string taskCd)
        {
            TaskCodeProvider taskCodeProvider = new TaskCodeProvider();
            string taskCodeDesc = taskCodeProvider.GetTaskCodeDesc(taskCd);
            return taskCodeDesc;
        }

    }
}

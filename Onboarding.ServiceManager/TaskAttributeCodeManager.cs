using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding.DataProvider;
using Onboarding.DataProvider.Model;

namespace Onboarding.ServiceManager
{
    public class TaskAttributeCodeManager
    {
        public List<Entity.TaskAttributeCode> GetTaskAttributeCodeAll()
        {
            TaskAttributeCodeProvider taskAttributeCodeProvider = new TaskAttributeCodeProvider();
            List<Entity.TaskAttributeCode> taskAttributeCodeList = taskAttributeCodeProvider.GetTaskAttributeCodeAll();
            return taskAttributeCodeList.Where(x => x.IsActive == true).ToList();
        }

        public string GetTaskAttributeCodeDesc(string taskAttrbCd)
        {
            TaskAttributeCodeProvider taskAttributeCodeProvider = new TaskAttributeCodeProvider();
            string taskAttributeCodeDesc = taskAttributeCodeProvider.GetTaskAttributeCodeDesc(taskAttrbCd);
            return taskAttributeCodeDesc;
        }

    }
}

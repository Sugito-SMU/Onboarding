using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding.DataProvider;
using Onboarding.DataProvider.Model;

namespace Onboarding.ServiceManager
{
    public class TaskWorkflowManager
    {
        public List<Entity.Task> ActivateNextTasks(List<Entity.Task> taskList)
        {
            TaskFlowProvider taskFlowProv = new TaskFlowProvider();
            List<Entity.TaskFlow> taskFlowList = taskFlowProv.GetTaskFlowAll();
            List<Entity.Task> activatedTaskList = new List<Entity.Task>();

            if (taskList != null)
            {
                for (int i = 0; i < taskList.Count; i++)
                {
                    if (taskList[i].TaskStsCd == Entity.Constant.TaskStatusCode.New)
                    {
                        bool depTaskCompleted = true;
                        if ((taskFlowList != null) && (taskFlowList.Count > 0))
                        {
                            List<Entity.TaskFlow> depTaskFlowList = taskFlowList.Where(x => x.TaskCd == taskList[i].TaskCd).ToList();
                            if (depTaskFlowList != null)
                            {
                                foreach (Entity.TaskFlow depTaskFlow in depTaskFlowList)
                                {
                                    Entity.Task depTask = taskList.Where(x => x.TaskCd == depTaskFlow.PreTaskCd).FirstOrDefault();
                                    if ((depTask != null) && (depTask.TaskStsCd != Entity.Constant.TaskStatusCode.Completed))
                                    {
                                        depTaskCompleted = false;
                                        break;
                                    }
                                }
                            }
                        }
                        if (depTaskCompleted)
                        {
                            taskList[i].TaskStsCd = Entity.Constant.TaskStatusCode.Pending;
                            activatedTaskList.Add(taskList[i]);
                        }
                    }
                }
            }
            return activatedTaskList;
        }

        public List<Entity.Task> RevertTasksToPending(Entity.Task currentTask, List<Entity.Task> taskList, List<Entity.Task> nextTaskList)
        {
            TaskFlowProvider taskFlowProv = new TaskFlowProvider();
            List<Entity.TaskFlow> taskFlowList = taskFlowProv.GetTaskFlowAll();

            if ((currentTask != null) && (taskList != null))
            {
                List<Entity.TaskFlow> taskList2 = taskFlowList.Where(x => x.PreTaskCd == currentTask.TaskCd).ToList();
                for (int i = 0; i < taskList2.Count; i++)
                {
                    Entity.Task nextTask = taskList.Where(x => x.TaskCd == taskList2[i].TaskCd).FirstOrDefault();
                    if (nextTask != null)
                    {
                        if (nextTask.TaskStsCd == Entity.Constant.TaskStatusCode.Completed)
                        {
                            nextTask.TaskStsCd = Entity.Constant.TaskStatusCode.Pending;
                            if ((nextTaskList != null) &&
                                (nextTaskList.Where(x => x.TaskCd == nextTask.TaskCd).FirstOrDefault() == null))
                            {
                                nextTaskList.Add(nextTask);
                            }
                            RevertTasksToPending(nextTask, taskList, nextTaskList);
                        }
                    }
                }
            }
            return nextTaskList;
        }

    }
}

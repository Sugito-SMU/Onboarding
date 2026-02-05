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
    public class TaskManager
    {
        public Entity.Task SaveTask(Entity.Task task, string userId)
        {
            bool txCommitted = false;

            if ((task != null) && (task.TaskId > 0))
            {
                using (OnboardingEntities obCtx = new OnboardingEntities())
                {
                    DbContextTransaction tx = null;
                    try
                    {
                        tx = obCtx.Database.BeginTransaction();
                        TaskProvider taskProv = new TaskProvider(obCtx);
                        bool isTaskChanged = taskProv.IsTaskChanged(task, userId);
                        taskProv.SaveTask(task, userId);
                        if (isTaskChanged)
                        {
                            RequestProvider reqProv = new RequestProvider(obCtx);
                            Entity.Request req = reqProv.GetRequest(task.ReqId, userId);
                            List<Entity.Task> nextTaskList = null;
                            if ((req.ReqStsCd == Entity.Constant.RequestStatusCode.Submitted) || (req.ReqStsCd == Entity.Constant.RequestStatusCode.Cancelled))
                            {
                                TaskWorkflowManager taskWf = new TaskWorkflowManager();
                                nextTaskList = taskWf.ActivateNextTasks(req.Tasks);
                                nextTaskList = taskWf.RevertTasksToPending(task, req.Tasks, nextTaskList);
                                taskProv.SaveActivatedTaskStatus(nextTaskList);
                            }
                            tx.Commit();
                            txCommitted = true;
                            NotificationManager notifMgr = new NotificationManager();
                            notifMgr.NotifyNextTask(req, nextTaskList);
                            notifMgr.NotifyTaskToCustomList(req, nextTaskList);
                            notifMgr.NotifyTaskToCustomList(req, req.GetTask(task.TaskCd));
                            string reqStsCd = reqProv.GetRequestStatus(req.ReqId);
                            if (reqStsCd == Entity.Constant.RequestStatusCode.Closed)
                            {
                                req = reqProv.GetRequest(req.ReqId, userId);
                                notifMgr.NotifyClosedRequest(req);
                            }
                        }
                        else
                        {
                            tx.Commit();
                            txCommitted = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!txCommitted)
                        {
                            tx.Rollback();
                        }
                        throw ex;
                    }
                }
                Entity.Task task2 = GetTask(task.TaskId, task.TaskCd, userId);
                return task2;
            }
            return null;
        }


        public Entity.Task GetTask(int taskId, string taskCd, string userId)
        {
            Entity.Task task = null;
            using (OnboardingEntities obCtx = new OnboardingEntities())
            {
                TaskProvider taskProvider = new TaskProvider(obCtx);
                task = taskProvider.GetTask(taskId, taskCd, userId);
            }
            return task;
        }
    }
}

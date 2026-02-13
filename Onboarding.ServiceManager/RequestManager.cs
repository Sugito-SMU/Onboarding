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
    public class RequestManager
    {
        List<Entity.TaskAttributeCode> _taskAttrbCdList = null;

        public RequestManager()
        {
            TaskAttributeCodeManager mgr = new TaskAttributeCodeManager();
            _taskAttrbCdList = mgr.GetTaskAttributeCodeAll();
        }

        public Entity.Request SaveRequest(Entity.Request req, string userId)
        {
            int reqId = 0;
            Entity.Request req2 = null;
            bool txCommitted = false;

            using (OnboardingEntities obCtx = new OnboardingEntities())
            {
                DbContextTransaction tx = null;
                try
                {
                    tx = obCtx.Database.BeginTransaction();
                    RequestProvider reqProvider = new RequestProvider(obCtx);
                    List<Entity.Task> nextTaskList = null;
                    if ((req.ReqStsCd == Entity.Constant.RequestStatusCode.Submitted) || (req.ReqStsCd == Entity.Constant.RequestStatusCode.Cancelled))
                    {
                        TaskWorkflowManager taskWf = new TaskWorkflowManager();
                        SetupAllTask(req);
                        ResetAllTasks(req); // re-submitting a submitted request causes all tasks to reset
                        nextTaskList = taskWf.ActivateNextTasks(req.Tasks);
                    }
                    reqId = reqProvider.SaveRequest(req, userId);
                    tx.Commit();
                    txCommitted = true;
                    if (reqId != 0)
                    {
                        req2 = GetRequest(reqId, userId);
                    }
                    NotificationManager notifMgr = new NotificationManager();
                    notifMgr.NotifyNextTask(req2, nextTaskList);
                    notifMgr.NotifyTaskToCustomList(req2, nextTaskList);
                    notifMgr.NotifySubmittedRequest(req2);
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
            return req2;
        }


        public Entity.RequestAttachment SaveRequestAttachment(Entity.RequestAttachment reqAtt, string userId)
        {
            int reqAttId = 0;
            bool txCommitted = false;

            using (OnboardingEntities obCtx = new OnboardingEntities())
            {
                DbContextTransaction tx = null;
                try
                {
                    tx = obCtx.Database.BeginTransaction();
                    RequestProvider reqProvider = new RequestProvider(obCtx);
                    reqAttId = reqProvider.SaveRequestAttachment(reqAtt, userId);
                    tx.Commit();
                    txCommitted = true;
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
            Entity.RequestAttachment reqAtt2 = null;
            if (reqAttId != 0)
            {
                reqAtt2 = GetRequestAttachment(reqAttId, userId);
            }
            return reqAtt2;
        }

        public bool DeleteRequest(int reqId, string userId)
        {
            bool txCommitted = false;
            bool success = false;

            using (OnboardingEntities obCtx = new OnboardingEntities())
            {
                DbContextTransaction tx = null;
                try
                {
                    tx = obCtx.Database.BeginTransaction();
                    RequestProvider reqProvider = new RequestProvider(obCtx);
                    success = reqProvider.DeleteRequest(reqId, userId);
                    tx.Commit();
                    txCommitted = true;
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
            return success;
        }

        public bool DeleteRequestAttachment(int reqAttId, string userId)
        {
            bool txCommitted = false;
            bool success = false;

            using (OnboardingEntities obCtx = new OnboardingEntities())
            {
                DbContextTransaction tx = null;
                try
                {
                    tx = obCtx.Database.BeginTransaction();
                    RequestProvider reqProvider = new RequestProvider(obCtx);
                    success = reqProvider.DeleteRequestAttachment(reqAttId, userId);
                    tx.Commit();
                    txCommitted = true;
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
            return success;
        }

        public Entity.Request GetRequest(int reqId, string userId)
        {
            Entity.Request req = null;
            using (OnboardingEntities obCtx = new OnboardingEntities())
            {
                RequestProvider reqProvider = new RequestProvider(obCtx);               
                req = reqProvider.GetRequest(reqId, userId);                
            }

            return req;
        }

        public Entity.RequestAttachment GetRequestAttachment(int reqAttId, string userId)
        {
            Entity.RequestAttachment reqAtt = null;
            using (OnboardingEntities obCtx = new OnboardingEntities())
            {
                RequestProvider reqProvider = new RequestProvider(obCtx);
                reqAtt = reqProvider.GetRequestAttachment(reqAttId);
            }

            return reqAtt;
        }

        public void SetupTask(Entity.Request req, bool? taskRequired, string taskCd)
        {
            Entity.Task tsk = req.GetTask(taskCd);
            if (taskRequired == true)
            {
                if (req.ReqStsCd != Entity.Constant.RequestStatusCode.Draft)
                {
                    if (req.Tasks == null)
                    {
                        req.Tasks = new List<Entity.Task>();
                    }
                    if (tsk == null)
                    {
                        tsk = new Entity.Task();
                        tsk.TaskCd = taskCd;
                        tsk.TaskStsCd = Entity.Constant.TaskStatusCode.New;
                        tsk.IsActive = true;
                        tsk.CreatedBy = req.CreatedBy;
                        tsk.CreatedDt = req.CreatedDt;
                        req.Tasks.Add(tsk);
                    }
                    tsk.IsActive = (taskRequired == true) ? true : false;
                    List<Entity.TaskAttributeCode> tacdList = _taskAttrbCdList.Where(x => x.TaskCd == tsk.TaskCd).ToList();
                    if ((tacdList != null) && (tacdList.Count > 0))
                    {
                        if (tsk.Attributes == null)
                        {
                            tsk.Attributes = new List<Entity.TaskAttribute>();
                        }
                        foreach (Entity.TaskAttributeCode tacd in tacdList)
                        {
                            Entity.TaskAttribute tattrb = tsk.GetTaskAttribute(tacd.TaskAttrbCd);
                            if (tattrb == null)
                            {
                                tattrb = new Entity.TaskAttribute();
                                tattrb.TaskCd = tacd.TaskCd;
                                tattrb.TaskAttrbCd = tacd.TaskAttrbCd;
                                tattrb.CreatedBy = req.CreatedBy;
                                tattrb.CreatedDt = req.CreatedDt;
                                if (taskCd == Entity.Constant.TaskCode.CreateNTID && !string.IsNullOrEmpty(req.Resources.NetworkID))
                                {
                                    tattrb.TaskAttrbVal = req.Resources.NetworkID;
                                }
                                tsk.Attributes.Add(tattrb);
                            }
                        }
                    }
                }
            }
            else
            {
                if (tsk != null)
                {
                    tsk.IsActive = false;
                }
            }
        }

        public void SetupAllTask(Entity.Request req)
        {
            if (req.Resources == null)
            {
                req.Resources = new Entity.RequestResources();
            }
            SetupTask(req, req.Resources.IsPCSelected, Entity.Constant.TaskCode.AssignPCNB);
            SetupTask(req, req.Resources.IsIPPhoneSelected, Entity.Constant.TaskCode.AssignPhone);
            SetupTask(req, req.Resources.IsIPPhoneSelected, Entity.Constant.TaskCode.ConfigurePhone);
            SetupTask(req, req.Resources.IsIPPhoneSelected, Entity.Constant.TaskCode.DeployIPPhone);
            SetupTask(req, req.Resources.IsIPPhoneSelected, Entity.Constant.TaskCode.UpdatePhoneOutlook);
            SetupTask(req, true, Entity.Constant.TaskCode.CreateNTID); //Network ID is always required
            SetupTask(req, req.Resources.IsEmailSelected, Entity.Constant.TaskCode.CreateEmail);
            SetupTask(req, IsAMSRequired(req), Entity.Constant.TaskCode.CreateAMS);
            SetupTask(req, req.Resources.IsSAPSelected, Entity.Constant.TaskCode.CreateSAP);
            SetupTask(req, req.Resources.IsSAPSelected, Entity.Constant.TaskCode.AssignSAPAuthz);
            SetupTask(req, req.Resources.IsISISSelected, Entity.Constant.TaskCode.CreateISIS);
            SetupTask(req, req.Resources.IsELearnSelected, Entity.Constant.TaskCode.CreateELearn);
            SetupTask(req, req.Resources.IsMtgRoomSelected, Entity.Constant.TaskCode.GrantMtgRoom);
            SetupTask(req, req.Resources.IsINetSelected, Entity.Constant.TaskCode.GrantCMSiNet);
            SetupTask(req, req.Resources.IsOasisSelected, Entity.Constant.TaskCode.GrantCMSOasis);
            SetupTask(req, req.Resources.IsNextwebSelected, Entity.Constant.TaskCode.GrantCMSNextweb);
            SetupTask(req, req.Resources.IsEmailDLSelected, Entity.Constant.TaskCode.GrantAddInfra);
            SetupTask(req, true, Entity.Constant.TaskCode.VerifyAllTasksCmpl);
        }

        private bool IsAMSRequired(Entity.Request req)
        {
            bool required = false;

            if (req.EmploymentInfo != null)
            {
                switch (req.EmploymentInfo.UserSubTypeCd)
                {
                    case Entity.Constant.UserSubType.AdminTemporary:
                        required = true;
                        break;
                    case Entity.Constant.UserSubType.FacultyAssociatedAdjunctsContractOfService:
                        required = true;
                        break;
                    case Entity.Constant.UserSubType.FacultyAssociatedAdjunctsContractForService:
                        required = true;
                        break;
                    case Entity.Constant.UserSubType.FacultyAssociatedAffiliated:
                        required = true;
                        break;
                    case Entity.Constant.UserSubType.FacultyAssociatedDistinguishedFellow:
                        required = true;
                        break;
                    case Entity.Constant.UserSubType.FacultyAssociatedPostDoc:
                        required = true;
                        break;
                    case Entity.Constant.UserSubType.FacultyAssociatedResearchFellow:
                        required = true;
                        break;
                    case Entity.Constant.UserSubType.FacultyAssociatedTeachingFellow:
                        required = true;
                        break;
                    case Entity.Constant.UserSubType.FacultyAssociatedVisitingLessThan12:
                        required = true;
                        break;
                    case Entity.Constant.UserSubType.FacultyAssociatedEmeritus:
                        required = true;
                        break;
                    case Entity.Constant.UserSubType.FacultyGraduateInstructor:
                        required = true;
                        break;
                    case Entity.Constant.UserSubType.ExternalContract:
                        required = true;
                        break;
                    case Entity.Constant.UserSubType.ExternalVendor:
                        required = true;
                        break;
                    case Entity.Constant.UserSubType.ExternalVIP:
                        required = true;
                        break;
                    case Entity.Constant.UserSubType.ExternalVisitors:
                        required = true;
                        break;
                    default:
                        required = false;
                        break;
                }
            }
            return required;
        }

        private void ResetAllTasks(Entity.Request req)
        {
            if ((req != null) && (req.Tasks != null))
            {
                for (int i = 0; i < req.Tasks.Count; i++)
                {
                    req.Tasks[i].TaskStsCd = Entity.Constant.TaskStatusCode.New;
                    if (req.ReqStsCd == Entity.Constant.RequestStatusCode.Cancelled)
                    {
                        req.Tasks[i].IsActive = false;
                    }
                }
            }
        }

    }
}

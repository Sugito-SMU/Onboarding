using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding.Common;
using Onboarding.DataProvider;
using Onboarding.DataProvider.Model;

namespace Onboarding.ServiceManager
{
    public class BulkRequestManager
    {

        public Entity.BulkRequestSubmitResult SubmitBulkRequest(Entity.BulkRequest bulkReq, string userId)
        {
            int reqId = 0;
            bool txCommitted = false;

            RequestTypeProvider reqProv = new RequestTypeProvider();
            UserTypeProvider usrTypeProv = new UserTypeProvider();

            Entity.BulkRequestSubmitResult bulkReqSbmRes = null;
            if ((bulkReq != null) && (bulkReq.Requests.Count > 0))
            {
                for (int i = 0; i < bulkReq.Requests.Count; i++)
                {
                    bulkReq.Requests[i].ReqTypeDesc = reqProv.GetRequestTypeDesc(bulkReq.Requests[i].ReqTypeCd);
                    bulkReq.Requests[i].EmploymentInfo.UserTypeDesc = usrTypeProv.GetUserTypeDesc(bulkReq.Requests[i].EmploymentInfo.UserTypeCd);
                }

                RequestValidation reqVal = new RequestValidation();
                bulkReqSbmRes = reqVal.ValidateBulkRequest(bulkReq);
                if (bulkReqSbmRes.IsAllResultPass)
                {
                    using (OnboardingEntities obCtx = new OnboardingEntities())
                    {
                        DbContextTransaction tx = null;
                        try
                        {
                            RequestManager reqMgr = new RequestManager();
                            bulkReqSbmRes.Rows = new List<Entity.BulkRequestSubmitResultRow>();
                            tx = obCtx.Database.BeginTransaction();
                            RequestProvider reqProvider = new RequestProvider(obCtx);
                            TaskWorkflowManager taskWf = new TaskWorkflowManager();
                            Dictionary<int, List<Entity.Task>> bulkActivatedTaskList = new Dictionary<int, List<Entity.Task>>();
                            for (int i = 0; i < bulkReq.Requests.Count; i++)
                            {
                                //By default, set the Request Status to "SUBMITTED"
                                bulkReq.Requests[i].ReqStsCd = Entity.Constant.RequestStatusCode.Submitted;
                                reqMgr.SetupAllTask(bulkReq.Requests[i]);
                                List<Entity.Task> activatedTaskList = taskWf.ActivateNextTasks(bulkReq.Requests[i].Tasks);
                                reqId = reqProvider.SaveRequest(bulkReq.Requests[i], userId);
                                bulkActivatedTaskList.Add(reqId, activatedTaskList);
                                Entity.BulkRequestSubmitResultRow sbmResRow = new Entity.BulkRequestSubmitResultRow();
                                sbmResRow.RequestId = reqId;
                                sbmResRow.FirstName = bulkReq.Requests[i].FirstName;
                                sbmResRow.LastName = bulkReq.Requests[i].LastName;
                                sbmResRow.ReqTypeDesc = bulkReq.Requests[i].ReqTypeDesc;
                                sbmResRow.UserTypeDesc = bulkReq.Requests[i].EmploymentInfo.UserTypeDesc;
                                sbmResRow.EmplStartDt = bulkReq.Requests[i].EmploymentInfo.EmplStartDt;
                                sbmResRow.IsResultPass = true;
                                bulkReqSbmRes.Rows.Add(sbmResRow);
                            }
                            tx.Commit();
                            txCommitted = true;

                            NotificationManager notifMgr = new NotificationManager();
                            for (int i = 0; i < bulkReqSbmRes.Rows.Count; i++)
                            {
                                Entity.Request req2 = reqProvider.GetRequest(bulkReqSbmRes.Rows[i].RequestId, userId);
                                if (bulkActivatedTaskList.ContainsKey(bulkReqSbmRes.Rows[i].RequestId))
                                {
                                    notifMgr.NotifyNextTask(req2, bulkActivatedTaskList[bulkReqSbmRes.Rows[i].RequestId]);
                                    notifMgr.NotifyTaskToCustomList(req2, bulkActivatedTaskList[bulkReqSbmRes.Rows[i].RequestId]);
                                }
                                notifMgr.NotifySubmittedRequest(req2);
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
                }
            }
            return bulkReqSbmRes;
        }



    }
}

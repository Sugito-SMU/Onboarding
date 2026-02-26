using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Onboarding.Models;
using Onboarding.Entity;

namespace Onboarding.Controllers
{
    public class HRAdminController : Controller
    {

        [Authorize]
        public ActionResult Index()
        {
            if (!Helper.Utility.HasPermission(Entity.Constant.AppResource.AppHRAdminDashboard))
            {
                return View("Unauthorized");
            }

            return View();
        }

        [Authorize]
        public ActionResult Dashboard()
        {
            if (!Helper.Utility.HasPermission(Entity.Constant.AppResource.AppHRAdminDashboard))
            {
                return View("Unauthorized");
            }

            Onboarding.WebAPIManager.WebAPIManager api = new Onboarding.WebAPIManager.WebAPIManager();
            string userid = Helper.Utility.GetCurrentUserId();
            if (string.IsNullOrEmpty(userid))
            {
                return View("Unauthorized");
            }

            List<Onboarding.Entity.RequestCountForHRAdmin> olstAgent = api.GetHRAdminTaskCount(userid);
            return View(olstAgent);
        }

        [HttpGet]
        public ActionResult HRAdminRequest(int id = 0, int ViewOnly = 0)
        {
            if (!Helper.Utility.HasPermission(Entity.Constant.AppResource.AppHRAdminDashboard))
            {
                return View("Unauthorized");
            }

            if (id == 0)
            {
                return RedirectToAction("Dashboard", "HRAdmin");
            }
            Onboarding.WebAPIManager.WebAPIManager api = new Onboarding.WebAPIManager.WebAPIManager();
            if (string.IsNullOrEmpty(Helper.Utility.GetCurrentUserId()))
            {
                return View("Unauthorized");
            }

            Onboarding.Entity.Request oRequest = api.GetRequest(id, Helper.Utility.GetCurrentUserId());
            Onboarding.Models.AgentRequestModel agentRequest = new Models.AgentRequestModel();
            agentRequest.Request = oRequest;
            agentRequest.ReqId = oRequest.ReqId;
            if (agentRequest.Request != null)
            {
                foreach (Entity.Task task in agentRequest.Request.Tasks)
                {
                    switch (task.TaskCd)
                    {
                        case Entity.Constant.TaskCode.AssignPhone:
                            Entity.TaskAttribute tPhModel = task.GetTaskAttribute(Entity.Constant.TaskAttributeCode.PhoneModel);
                            agentRequest.PhoneModel = tPhModel.TaskAttrbVal;
                            Entity.TaskAttribute tPhMac = task.GetTaskAttribute(Entity.Constant.TaskAttributeCode.PhoneMACAddr);
                            agentRequest.PhoneMACAddr = tPhMac.TaskAttrbVal;
                            Entity.TaskAttribute tPhAddInf = task.GetTaskAttribute(Entity.Constant.TaskAttributeCode.PhoneAddInfo);
                            agentRequest.PhoneAddInfo = tPhAddInf.TaskAttrbVal;
                            break;
                        case Entity.Constant.TaskCode.ConfigurePhone:
                            Entity.TaskAttribute tPhExt = task.GetTaskAttribute(Entity.Constant.TaskAttributeCode.AssignedPhoneExt);
                            agentRequest.AssignedPhoneExt = tPhExt.TaskAttrbVal;
                            break;
                        case Entity.Constant.TaskCode.CreateNTID:
                            Entity.TaskAttribute tAtt = task.GetTaskAttribute(Entity.Constant.TaskAttributeCode.AssignedNTID);
                            agentRequest.AssignedNTID = tAtt.TaskAttrbVal;
                            break;
                        case Entity.Constant.TaskCode.CreateEmail:
                            Entity.TaskAttribute tEmailAddress = task.GetTaskAttribute(Entity.Constant.TaskAttributeCode.AssignedEmailAddress);
                            agentRequest.AssignedEmailAddress = tEmailAddress.TaskAttrbVal;
                            Entity.TaskAttribute tEmailDispNm = task.GetTaskAttribute(Entity.Constant.TaskAttributeCode.AssignedEmailDispNm);
                            agentRequest.AssignedEmailDispNm = tEmailDispNm.TaskAttrbVal;
                            break;
                        default:
                            break;
                    }
                }
            }
            if (ViewOnly == 1)
            {
                agentRequest.ViewOnly = true;
            }
            else
            {
                agentRequest.ViewOnly = false;
            }
            Session["AgentRequestModel"] = agentRequest;
            return View(agentRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult HRAdminRequest(AgentRequestModel agentReqParam)
        {
            if (!Helper.Utility.HasPermission(Entity.Constant.AppResource.AppAgentForm))
            {
                return View("Unauthorized");
            }

            string oldnetworkid = null;
            string userId = Helper.Utility.GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return View("Unauthorized");
            }
            Onboarding.WebAPIManager.WebAPIManager api = new Onboarding.WebAPIManager.WebAPIManager();
            Onboarding.Common.AccessControl access = new Common.AccessControl();
            Onboarding.Models.AgentRequestModel agentReq = (Onboarding.Models.AgentRequestModel)Session["AgentRequestModel"];
            if ((!string.IsNullOrWhiteSpace(agentReqParam.SubmitCommand)) && (agentReq != null) && 
                (agentReq.Request != null) && (agentReq.Request.Tasks != null))
            {
                List<Entity.Task> taskList = new List<Entity.Task>();
                if ((agentReqParam.SubmitCommand == "CompleteAllTask") || (agentReqParam.SubmitCommand == "CompleteAllTaskNextRequest"))
                {
                    foreach (Entity.Task tsk in agentReq.Request.Tasks)
                    {
                        if ((tsk.IsMyTask == true) && (tsk.TaskStsCd != Entity.Constant.TaskStatusCode.New))
                        {
                            taskList.Add(tsk);
                        }
                    }
                }
                else if (agentReqParam.SubmitCommand == "CompleteTask")
                {
                    taskList = agentReq.Request.Tasks.Where(x => (x.TaskCd == agentReqParam.TaskCd) && (x.IsMyTask == true)).ToList();
                }                

                Dictionary<string, Entity.TaskValidationError> taskValErrorList = new Dictionary<string, Entity.TaskValidationError>(); 
                foreach (Entity.Task task in taskList)
                {
                    task.Remark = Common.Security.Sanitize(agentReqParam.Request.GetTask(task.TaskCd).Remark);
                    switch (task.TaskCd)
                    {
                        case Entity.Constant.TaskCode.AssignPhone:
                            Entity.TaskAttribute tPhModel = task.GetTaskAttribute(Entity.Constant.TaskAttributeCode.PhoneModel);
                            tPhModel.TaskAttrbVal = Common.Security.Sanitize(agentReqParam.PhoneModel);
                            Entity.TaskAttribute tPhMac = task.GetTaskAttribute(Entity.Constant.TaskAttributeCode.PhoneMACAddr);
                            tPhMac.TaskAttrbVal = Common.Security.Sanitize(agentReqParam.PhoneMACAddr);
                            Entity.TaskAttribute tPhAddInf = task.GetTaskAttribute(Entity.Constant.TaskAttributeCode.PhoneAddInfo);
                            tPhAddInf.TaskAttrbVal = Common.Security.Sanitize(agentReqParam.PhoneAddInfo);
                            break;
                        case Entity.Constant.TaskCode.ConfigurePhone:
                            Entity.TaskAttribute tPhExt = task.GetTaskAttribute(Entity.Constant.TaskAttributeCode.AssignedPhoneExt);
                            tPhExt.TaskAttrbVal = Common.Security.Sanitize(agentReqParam.AssignedPhoneExt);
                            break;
                        case Entity.Constant.TaskCode.CreateNTID:
                            Entity.TaskAttribute tAtt = task.GetTaskAttribute(Entity.Constant.TaskAttributeCode.AssignedNTID);
                            oldnetworkid = tAtt.TaskAttrbVal;
                            tAtt.TaskAttrbVal = Common.Security.Sanitize(agentReqParam.AssignedNTID);
                            break;
                        case Entity.Constant.TaskCode.CreateEmail:
                            Entity.TaskAttribute tEmailAddress = task.GetTaskAttribute(Entity.Constant.TaskAttributeCode.AssignedEmailAddress);
                            tEmailAddress.TaskAttrbVal = Common.Security.Sanitize(agentReqParam.AssignedEmailAddress);
                            Entity.TaskAttribute tEmailDispNm = task.GetTaskAttribute(Entity.Constant.TaskAttributeCode.AssignedEmailDispNm);
                            tEmailDispNm.TaskAttrbVal = Common.Security.Sanitize(agentReqParam.AssignedEmailDispNm);
                            break;
                        default:
                            break;
                    }
                    task.ValidationError = null;
                    Entity.TaskValidationError taskValErr = Common.TaskValidation.ValidateTask(task, agentReq.Request);
                    if (taskValErr.HasError() && !taskValErrorList.ContainsKey(task.TaskCd))
                    {
                        task.ValidationError = taskValErr;
                        taskValErrorList.Add(task.TaskCd, taskValErr);
                    }
                }

                if (taskValErrorList.Count() > 0)
                {
                    return View(agentReq);
                }
                else
                {
                    foreach (Entity.Task task in taskList)
                    {
                        task.TaskStsCd = Entity.Constant.TaskStatusCode.Completed;
                        Entity.Task savedTask = null;
                        if ((task.TaskCd == Entity.Constant.TaskCode.CreateNTID) && 
                            (ConfigurationManager.AppSettings["InterfaceToNaas"].Trim().ToLower() == "true"))
                        {
                            string loginId = task.Attributes.Where(x => x.TaskId == task.TaskId).FirstOrDefault().TaskAttrbVal;
                            if (!string.IsNullOrEmpty(loginId))
                            {
                                CASEntityADAccount adAcct = api.GetNetworkIdFromNaas(loginId);
                                if ((adAcct != null) && (adAcct.LoginID != null) && (loginId.Trim().ToLower() == adAcct.LoginID.Trim().ToLower()))
                                {
                                    savedTask = api.SaveTask(task.TaskId, task, userId);
                                    Onboarding.Entity.Request oRequest = api.GetRequest(task.ReqId, Helper.Utility.GetCurrentUserId());
                                    ViewBag.TNTIDMessage = string.Empty;
                                    ViewBag.TNTIDErrorMessage = string.Empty;
                                    if ((oRequest.ReqTypeCd == Entity.Constant.RequestTypeCode.NewHire) || (oRequest.ReqTypeCd == Entity.Constant.RequestTypeCode.ReEntry))
                                    {
                                        if (!string.IsNullOrEmpty(oRequest.PrsnEmail) && !string.IsNullOrEmpty(oRequest.MobileNo))
                                        {
                                            bool success = api.PostNetworkIdToNaas(loginId, oRequest, userId, oldnetworkid);
                                            if (!success)
                                            {
                                                ViewBag.TNTIDErrorMessage = "Unable to interface the account information to Network Account Activation System, account activation notification needs to be handled manually.";
                                            }
                                        }
                                        else
                                        {
                                            ViewBag.TNTIDMessage = "Personal Email or Mobile No is not available, account activation notification needs to be handled manually.";
                                        }
                                    }
                                }
                                else
                                {
                                    ViewBag.TNTIDErrorMessage = "Assigned Network ID is not found in Active Directory";
                                }
                            }
                            else
                            {
                                savedTask = api.SaveTask(task.TaskId, task, userId);
                            }
                        }
                        else
                        {
                            savedTask = api.SaveTask(task.TaskId, task, userId);
                        }
                    }
                }

                Entity.Request req = api.GetRequest(agentReq.Request.ReqId, userId);
                agentReq.Request = req;
                agentReq.ReqId = agentReq.ReqId;
                Session["AgentRequestModel"] = agentReq;

                if ((agentReqParam.SubmitCommand == "CompleteAllTaskNextRequest") || (agentReqParam.SubmitCommand == "NextRequest"))
                {
                    Entity.RequestListForAgent reqList = api.GetRequestListForAgent(Entity.Constant.TaskStatusCode.Pending, userId);
                    if ((reqList != null) && (reqList.Rows != null) && (reqList.Rows.Count > 0))
                    {
                        int nextReqId = 0;
                        var sortedReqList = from a in reqList.Rows
                                            orderby a.EmplStartDt ascending, a.ReqId ascending
                                            select a;
                        bool reqIdFound = false;
                        foreach (Entity.RequestListForAgentRow row in sortedReqList)
                        {
                            if (reqIdFound == true)
                            {
                                nextReqId = row.ReqId;
                                break;
                            }
                            if (row.ReqId == agentReq.ReqId)
                            {
                                reqIdFound = true;
                            }
                        }
                        if (nextReqId != 0)
                        {
                            return RedirectToAction("AgentRequest", "Agent", new { id = nextReqId });
                        }
                        else
                        {
                            return RedirectToAction("Dashboard", "Agent");
                        }
                    }
                    else
                    {
                        return RedirectToAction("Dashboard", "Agent");
                    }
                }
            }

            return View(agentReq);
        }


        public ActionResult RequestListForHRAdmin(string id)
        {
            if ((!Helper.Utility.HasPermission(Entity.Constant.AppResource.AppHRAdminDashboard)) ||
                (!Common.Security.IsValidAlphaNum(id)))
            {
                return View("Unauthorized");
            }

            Onboarding.WebAPIManager.WebAPIManager api = new Onboarding.WebAPIManager.WebAPIManager();
            List<Entity.TaskStatus> taskStatusList = api.GetAllTaskStatus();
            if ((taskStatusList != null) && (!string.IsNullOrEmpty(id)))
            {
                Entity.TaskStatus taskSts = taskStatusList.Where(x => x.TaskStsCd.ToUpper().Trim() == id.ToUpper().Trim()).FirstOrDefault();
                if (taskSts != null)
                {
                    ViewBag.reqStatusName = taskSts.TaskStsDesc;
                }
            }
            ViewBag.ID = id;
            return View();
        }

        public ActionResult DownloadFile(int id)
        {
            if (!Helper.Utility.HasPermission(Entity.Constant.AppResource.AppAgentForm))
            {
                return View("Unauthorized");
            }

            Entity.RequestAttachment reqAttach = null;
            if (Session["AgentRequestModel"] != null)
            {
                AgentRequestModel agentReq = (AgentRequestModel)Session["AgentRequestModel"];
                if ((agentReq != null) && (agentReq.Request != null))
                {
                    reqAttach = agentReq.Request.Attachment;
                }
            }
            if (reqAttach == null)
            {
                WebAPIManager.WebAPIManager api = new WebAPIManager.WebAPIManager();
                reqAttach = api.GetRequestAttachment(id, Helper.Utility.GetCurrentUserId());
            }
            if (reqAttach != null)
            {

                string folderPath = ConfigurationManager.AppSettings["AttachmentBaseFolder"].ToString() + "\\" + reqAttach.ReqId.ToString();
                string filePath = folderPath + "\\" + reqAttach.ReqAttachId.ToString();
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
                return File(fs, "application/octet-stream", reqAttach.Filename);
            }
            return HttpNotFound();
        }
    }
}
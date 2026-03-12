using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Configuration;
using System.Threading.Tasks;
using Onboarding.Models;
using Onboarding.WebAPIManager;
using Onboarding.Entity;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.IO;

namespace Onboarding.Controllers
{
    public partial class RequestorController : Controller
    {
        [HttpGet]
        [Authorize]
        public ActionResult SystemAccess()
        {
            if (!Helper.Utility.HasPermission(Entity.Constant.AppResource.AppRequestForm))
            {
                return View("Unauthorized");
            }

            SystemAccess sysAccess = new SystemAccess();
            Request request = (Request)Session["Request"];
            if (request == null)
            {
                return RedirectToAction("Dashboard", "Requestor");
            }
            BindEntityToModel(request, sysAccess);
            ViewBag.ShowSaveSuccessful = false;
            sysAccess.IsHRAdmin = Helper.Utility.HasRole(Entity.Constant.SystemRole.HRAdmin) || Helper.Utility.HasRole(Entity.Constant.SystemRole.SuperUser);
            return View("SystemAccess", sysAccess);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SystemAccess(SystemAccess sysAccess, string FormCommand, string RedirectTo)
        {
            if (!Helper.Utility.HasPermission(Entity.Constant.AppResource.AppRequestForm))
            {
                return View("Unauthorized");
            }
            ViewBag.ShowSaveSuccessful = false;
            WebAPIManager.WebAPIManager api = new WebAPIManager.WebAPIManager();
            Request request = (Request)Session["Request"];
            if ((Session["RequestViewMode"] != null) && ((bool)Session["RequestViewMode"] == false))
            {
                BindModelToEntity(sysAccess, request);
                Session["Request"] = request;
            }
            if ((FormCommand == "Save") || (FormCommand == "Submit"))
            {
                bool hasAttachment = false;
                bool deleteAttachment = false;
                int reqAttId = 0;
                if ((sysAccess.AttachmentFile != null) && (ValidateAttachmentFile(sysAccess.AttachmentFile)))
                {
                    if (request.Attachment == null)
                    {
                        request.Attachment = new RequestAttachment();
                    }
                    request.Attachment.Filename = Path.GetFileName(sysAccess.AttachmentFile.FileName);
                    hasAttachment = true;
                }
                else
                {
                    if ((request.Attachment != null) && (sysAccess.IsAttachmentDeleted == true))
                    {
                        reqAttId = request.Attachment.ReqAttachId;
                        request.Attachment = null;
                        deleteAttachment = true;
                    }
                }
                if(Helper.Utility.HasRole(Entity.Constant.SystemRole.HRAdmin))
                {
                    request.Resources.IsNetworkIdMandatory = true;
                    if (!string.IsNullOrEmpty(sysAccess.NetworkID))
                    {
                        request.Resources.NetworkID = sysAccess.NetworkID;
                    }
                }

                if (FormCommand == "Submit")
                {

                    request.Validation = Common.RequestValidation.ValidateRequest(request, false);
                    Session["Request"] = request;
                    if (request.HasPersonalInfoError())
                    {
                        return RedirectToAction("PersonalInfo", "Requestor", new { id = request.ReqId });
                    }
                    else if (request.HasEmploymentInfoError())
                    {
                        return RedirectToAction("EmployeeInfo", "Requestor");
                    }
                    else if (request.HasOfficeLogisticsError())
                    {
                        return RedirectToAction("OfficeLog", "Requestor");
                    }
                    else if (request.HasSystemAccessError())
                    {
                        return RedirectToAction("SystemAccess", "Requestor");
                    }

                    request.ReqStsCd = Entity.Constant.RequestStatusCode.Submitted;
                }

                Request req = api.SaveRequest(request, Helper.Utility.GetCurrentUserId());
                Session["Request"] = req;
                BindEntityToModel(req, sysAccess);
                if (string.IsNullOrEmpty(request.Resources.NetworkID) && Helper.Utility.HasRole(Entity.Constant.SystemRole.SchoolAmin))
                {
                    string url = ConfigurationManager.AppSettings["RedirectURL"] + "HRAdmin/HRAdminRequest/" + req.ReqId;
                    Common.Email.SendEmailForNetworkIdUpdate(req.FirstName, req.ReqId, request.CreatedBy, url);
                }
                
                if (hasAttachment)
                {
                    SaveAttachmentFile(sysAccess.AttachmentFile, req.Attachment);
                }
                if (deleteAttachment)
                {
                    DeleteAttachmentFile(reqAttId, req.ReqId);
                }

                if (FormCommand == "Submit")
                {
                    Session.Clear();
                    return RedirectToAction("Dashboard", "Requestor");
                }
                else
                {
                    BindEntityToModel(req, sysAccess);
                    ViewBag.ShowSaveSuccessful = true;
                    return View("SystemAccess", sysAccess);
                } 
            }
            else if (FormCommand == "Amend")
            {
                sysAccess.IsHRAdmin = Helper.Utility.HasRole(Entity.Constant.SystemRole.HRAdmin) || Helper.Utility.HasRole(Entity.Constant.SystemRole.SuperUser);
                Session["RequestViewMode"] = false;
                ModelState.Clear();
                BindEntityToModel(request, sysAccess);
                return View("SystemAccess", sysAccess);
            }
            else if (FormCommand == "Cancel Request")
            {
                request.ReqStsCd = Entity.Constant.RequestStatusCode.Cancelled;
                Request req = api.SaveRequest(request, Helper.Utility.GetCurrentUserId());
                Session.Clear();
                return RedirectToAction("Dashboard", "Requestor");
            }
            else if (FormCommand == "Delete Request")
            {
                bool res = api.DeleteRequest(request.ReqId, Helper.Utility.GetCurrentUserId());
                Session.Clear();
                return RedirectToAction("Dashboard", "Requestor");
            }
            else if (FormCommand == "Previous")
            {
                return RedirectToAction("OfficeLog", "Requestor");
            }
            if (!string.IsNullOrEmpty(RedirectTo))
            {
                switch (RedirectTo)
                {
                    case "PersonalInfo":
                        return RedirectToAction("PersonalInfo", "Requestor", new { id = request.ReqId });
                    case "EmployeeInfo":
                        return RedirectToAction("EmployeeInfo", "Requestor");
                    case "OfficeLog":
                        return RedirectToAction("OfficeLog", "Requestor");
                    case "SystemAccess":
                        return RedirectToAction("SystemAccess", "Requestor");
                    default:
                        break;
                }
            }
            return RedirectToAction("SystemAccess", "Requestor");
        }

        private bool ValidateAttachmentFile(HttpPostedFileBase file)
        {

            if (file != null && file.ContentLength > 0)
            {
                int maxFileSize = 3145728;
                int.TryParse(ConfigurationManager.AppSettings["AttachmentMaxSize"], out maxFileSize);
                if (file.ContentLength > maxFileSize)
                {
                    Common.Logger.LogInfo(string.Format("{0}: {1}", file.FileName, "file exceeds 3MB"));
                    ModelState.AddModelError("AttFileSize", "The size of the file should not exceed 3MB");
                    return false;
                }

                var supportedTypes = new[] { "pdf", "msg", "zip" };

                string fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);

                if (!supportedTypes.Contains(fileExt))
                {
                    ModelState.AddModelError("AttFileExt", "Only MSG, PDF or ZIP file is supported.");
                    Common.Logger.LogInfo(string.Format("{0}: {1}", file.FileName, "file type is not supported"));
                    return false;
                }

                string fname = System.IO.Path.GetFileName(file.FileName);
                if (!Common.Security.IsValidFilename(fname))
                {
                    Common.Logger.LogInfo(string.Format("{0}: {1}, {2}", file.FileName, "invalid filename", Path.GetInvalidFileNameChars().ToString()));
                    ModelState.AddModelError("AttFilename", "Invalid filename");
                    return false;
                }

                return true;
            }
            return false;
        }
        
        private void BindModelToEntity(SystemAccess sysAccess, Request request)
        {
            if (request.Resources == null)
            {
                request.Resources = new RequestResources();
            }

            request.Resources.IsEmailSelected = sysAccess.IsEmailSelected;

            request.Resources.IsSAPSelected = sysAccess.IsSAPSelected;
            request.Resources.DARTFund = Common.Security.Sanitize(sysAccess.DARTFund);

            request.Resources.IsISISSelected = sysAccess.IsISISSelected;
            request.Resources.ISISRemarks = Common.Security.Sanitize(sysAccess.ISISRemarks);

            request.Resources.IsELearnSelected = sysAccess.IsELearnSelected;
            request.Resources.ELearnStartDate = sysAccess.ELearnStartDate;
            request.Resources.ELearnRemarks = Common.Security.Sanitize(sysAccess.ELearnRemarks);

            request.Resources.IsMtgRoomSelected = sysAccess.IsMtgRoomSelected;
            request.Resources.MtgRoomDetails = Common.Security.Sanitize(sysAccess.MtgRoomDetails);

            request.Resources.IsINetSelected = sysAccess.IsINetSelected;
            request.Resources.IsOasisSelected = sysAccess.IsOasisSelected;
            request.Resources.IsNextwebSelected = sysAccess.IsNextwebSelected;

            request.Resources.IsEmailDLSelected = sysAccess.IsEmailDLSelected;
            request.Resources.EmailDLDetails = Common.Security.Sanitize(sysAccess.EmailDLDetails);
            request.Resources.NetworkID = sysAccess.NetworkID;

            //Disabled check-boxes will assign false value to the model, 
            // hence the mandatory default value that is previously set is reset to false.
            // Need to revert the mandatory default value
            Helper.Utility.SetMandatoryValues(request, false);
        }

        private void BindEntityToModel(Request request, SystemAccess sysAccess)
        {
            ModelState.Clear();
            sysAccess.Request = request;

            Helper.Utility.SetMandatoryValues(request, false);
            if (request.Resources != null)
            {
                if (request.Resources.NetworkID != null)
                    sysAccess.NetworkID = request.Resources.NetworkID;
                if (request.Resources.IsEmailSelected != null)
                    sysAccess.IsEmailSelected = request.Resources.IsEmailSelected.Value;

                if (request.Resources.IsSAPSelected != null)
                    sysAccess.IsSAPSelected = request.Resources.IsSAPSelected.Value;
                sysAccess.DARTFund = request.Resources.DARTFund;

                if (request.Resources.IsISISSelected != null)
                    sysAccess.IsISISSelected = request.Resources.IsISISSelected.Value;
                sysAccess.ISISRemarks = request.Resources.ISISRemarks;

                if (request.Resources.IsELearnSelected != null)
                    sysAccess.IsELearnSelected = request.Resources.IsELearnSelected.Value;
                if (request.Resources.ELearnStartDate != null)
                    sysAccess.ELearnStartDate = request.Resources.ELearnStartDate.Value;
                sysAccess.ELearnRemarks = request.Resources.ELearnRemarks;

                if (request.Resources.IsMtgRoomSelected != null)
                    sysAccess.IsMtgRoomSelected = request.Resources.IsMtgRoomSelected.Value;
                sysAccess.MtgRoomDetails = request.Resources.MtgRoomDetails;

                if (request.Resources.IsINetSelected != null)
                    sysAccess.IsINetSelected = request.Resources.IsINetSelected.Value;

                if (request.Resources.IsOasisSelected != null)
                    sysAccess.IsOasisSelected = request.Resources.IsOasisSelected.Value;

                if (request.Resources.IsNextwebSelected != null)
                    sysAccess.IsNextwebSelected = request.Resources.IsNextwebSelected.Value;

                if (request.Resources.IsEmailDLSelected != null)
                    sysAccess.IsEmailDLSelected = request.Resources.IsEmailDLSelected.Value;
                sysAccess.EmailDLDetails = request.Resources.EmailDLDetails;
                if (request.Resources.NetworkID != null)
                {
                    sysAccess.NetworkID = request.Resources.NetworkID;
                }
            }

            if (request.Attachment != null)
            {
                if (sysAccess.Attachment == null)
                {
                    sysAccess.Attachment = new RequestAttachmentModel();
                }
                sysAccess.Attachment.Filename = request.Attachment.Filename;
                sysAccess.Attachment.ReqAttachId = request.Attachment.ReqAttachId;
                sysAccess.Attachment.ReqId = request.ReqId;
                sysAccess.HasAttachment = true;
            }

            sysAccess.IsAmended = request.IsAmended.HasValue ? request.IsAmended.Value : false;
            ViewBag.IsAmended = request.IsAmended;
            ViewBag.IsCancelled = request.IsCancelled;
            ViewBag.RequestStatus = request.ReqStsDesc;

            if (request.ReqId > 0)
            {
                ViewBag.Title = string.Format("Request #{0} ({1})", request.ReqId, request.ReqTypeDesc);
            }
            else
            {
                ViewBag.Title = string.Format("New Request ({0})", Session["RequestTypeDesc"]);
            }

            Helper.Utility.SetActionButtons(request, ViewBag, true, false);

            if (Session["RequestViewMode"] != null)
            {
                sysAccess.IsViewMode = (bool)Session["RequestViewMode"];
            }
            else
            {
                if (request.ReqStsCd != Entity.Constant.RequestStatusCode.Draft)
                {
                    Session["RequestViewMode"] = sysAccess.IsViewMode = true;
                }
            }
        }

        public void SaveAttachmentFile(HttpPostedFileBase file, Entity.RequestAttachment reqAtt)
        {
            if ((file != null) && (reqAtt != null) && (reqAtt.ReqAttachId > 0))
            {

                string folderPath = ConfigurationManager.AppSettings["AttachmentBaseFolder"].ToString() + "\\" + reqAtt.ReqId.ToString();
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                string filePath = folderPath + "\\" + reqAtt.ReqAttachId.ToString();
                file.SaveAs(filePath);
            }
        }
                
        public ActionResult DownloadFile(int id)
        {
            if (!Helper.Utility.HasPermission(Entity.Constant.AppResource.AppRequestForm))
            {
                return View("Unauthorized");
            }

            Entity.RequestAttachment reqAttach = null;
            if (Session["Request"] != null)
            {
                Entity.Request request = (Request)Session["Request"];
                if (request != null)
                {
                    reqAttach = request.Attachment;
                }
            }
            else
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

        private void DeleteAttachmentFile(int reqAttId, int reqId)
        {
            if ((reqAttId > 0) && (reqId > 0))
            {
                string folderPath = ConfigurationManager.AppSettings["AttachmentBaseFolder"].ToString() + "\\" + reqId.ToString();
                string filePath = folderPath + "\\" + reqAttId.ToString();
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
        }
    }
}
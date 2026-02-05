using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Onboarding.Entity.Constant;
using Onboarding.Models;

namespace Onboarding.Controllers
{
    public partial class RequestorController : Controller
    {
        public ActionResult BulkRequest()
        {
            if (!Helper.Utility.HasPermission(Entity.Constant.AppResource.AppBulkRequest))
            {
                return View("Unauthorized");
            }

            Session["BulkRequest"] = null;
            Session["BulkRequestSubmitResult"] = null;
            ViewBag.LblStatus = "";
            BulkRequestModel mdl = new BulkRequestModel();
            return View("BulkRequest", mdl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitBulkRequest(BulkRequestModel bulkRequestParam, string Command)
        {
            if (!Helper.Utility.HasPermission(Entity.Constant.AppResource.AppBulkRequest))
            {
                return View("Unauthorized");
            }

            BulkRequestModel bulkRequestModel = new BulkRequestModel();
            if (Command == "Upload")
            {
                Session["BulkRequest"] = null;
                Session["BulkRequestSubmitResult"] = null;

                bulkRequestModel.IsUploadSuccessful = true;
                if (bulkRequestParam.BulkRequestFile != null)
                {
                    bulkRequestModel.UploadedFilename = bulkRequestParam.BulkRequestFile.FileName;
                    DataTable dt = ExcelReader.ExcelReader.ReadExcelContent(bulkRequestParam.BulkRequestFile.InputStream);
                    if (dt != null)
                    {
                        BulkRequestRemoveEmptyRows(dt);

                        int maxRows = int.Parse(System.Configuration.ConfigurationManager.AppSettings["BulkRequestMaxRows"]);
                        if (dt.Rows.Count > maxRows)
                        {
                            bulkRequestModel.IsUploadSuccessful = false;
                            bulkRequestModel.ErrorMessage = string.Format("You have exceeded {0} rows", maxRows);
                            return View("BulkRequest", bulkRequestModel);
                        }

                        if (BulkRequestCheckFirstDataRow(dt) == false)
                        {
                            bulkRequestModel.IsUploadSuccessful = false;
                            bulkRequestModel.ErrorMessage = "Data cannot be found on the first row";
                            return View("BulkRequest", bulkRequestModel);
                        }

                        if (ValidateExcelContent(dt) == false)
                        {
                            bulkRequestModel.IsUploadSuccessful = false;
                            bulkRequestModel.ErrorMessage = "Header row has invalid column. If you copied from sample bulk request, please use bulk request template instead.";
                            return View("BulkRequest", bulkRequestModel);
                        }

                        Onboarding.Entity.BulkRequest bulkReq = new Entity.BulkRequest();
                        Entity.BulkRequestSubmitResult bulkReqRes = BulkRequestMapToEntity(dt, bulkReq);
                        // Onboarding.Entity.BulkRequestSubmitResult oBulkRequestSubmitResult = (new Onboarding.Common.RequestValidation()).ValidateBulkRequest(oBulkRequest);

                        if (bulkReqRes.IsAllResultPass)
                        {
                            bulkRequestModel.IsUploadSuccessful = true;
                            bulkRequestModel.RowCount = bulkReq.Requests.Count;
                            Session["BulkRequest"] = bulkReq;
                        }
                        else
                        {
                            bulkRequestModel.IsUploadSuccessful = false;
                            bulkRequestModel.IsRowValidationError = true;
                        }

                        Session["BulkRequestSubmitResult"] = bulkReqRes;
                    }
                }
                else
                {
                    bulkRequestModel.IsUploadSuccessful = false;
                    bulkRequestModel.ErrorMessage = "No file is uploaded";
                }
            }
            else if (Command == "Submit")
            {
                Entity.BulkRequest bulkReq = (Entity.BulkRequest)Session["BulkRequest"];
                if (bulkReq != null)
                {
                    WebAPIManager.WebAPIManager api = new WebAPIManager.WebAPIManager();
                    Entity.BulkRequestSubmitResult bulkReqRes = api.SaveBulkRequest(bulkReq, Helper.Utility.GetCurrentUserId());

                    if (bulkReqRes.IsAllResultPass)
                    {
                        bulkRequestModel.IsUploadSuccessful = false;
                        bulkRequestModel.IsSubmitSuccessful = true;
                        bulkRequestModel.RowCount = bulkReq.Requests.Count;
                        Session["BulkRequest"] = bulkReq;
                    }
                    else
                    {
                        bulkRequestModel.IsUploadSuccessful = false;
                        bulkRequestModel.IsSubmitSuccessful = false;
                        bulkRequestModel.IsRowValidationError = true;
                    }
                    Session["BulkRequestSubmitResult"] = bulkReqRes;
                }
            }

            return View("BulkRequest", bulkRequestModel);
        }

        public ActionResult BulkRequestError()
        {
            if (!Helper.Utility.HasPermission(Entity.Constant.AppResource.AppBulkRequest))
            {
                return View("Unauthorized");
            }

            Onboarding.Entity.BulkRequestSubmitResult oBulkRequestSubmitResult = new Entity.BulkRequestSubmitResult();
            if (Session["BulkRequestSubmitResult"] != null)
            {
                oBulkRequestSubmitResult = (Onboarding.Entity.BulkRequestSubmitResult)Session["BulkRequestSubmitResult"];
            }
            return View(oBulkRequestSubmitResult);
        }

        private DataTable BulkRequestRemoveEmptyRows(DataTable dt)
        {
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (dt.Rows[i][0] == DBNull.Value)
                    dt.Rows[i].Delete();
            }
            dt.AcceptChanges();
            return dt;
        }

        private bool BulkRequestCheckFirstDataRow(DataTable dt)
        {
            if (dt.Rows.Count > 1)
            {
                if ((dt.Rows[1][0] == DBNull.Value) || (string.IsNullOrEmpty((string)dt.Rows[1][0])))
                {
                    return false;
                }
                return true;
            }
            return false;
        }
        
        private Entity.BulkRequestSubmitResult BulkRequestMapToEntity(DataTable dt, Entity.BulkRequest bulkRequest)
        {
            Entity.BulkRequestSubmitResult bulkReqRes = new Entity.BulkRequestSubmitResult();
            bulkReqRes.Rows = new List<Entity.BulkRequestSubmitResultRow>();
            Onboarding.WebAPIManager.WebAPIManager api = new WebAPIManager.WebAPIManager();
            bulkRequest.Requests = new List<Entity.Request>();
            for (int rowNum = 1; rowNum < dt.Rows.Count; rowNum++)
            {
                Entity.BulkRequestSubmitResultRow resRow = new Entity.BulkRequestSubmitResultRow();
                resRow.RowNum = rowNum;
                resRow.Errors = new List<string>();
                Entity.Request requestItem = new Entity.Request();
                requestItem.EmploymentInfo = new Entity.RequestEmployment();
                requestItem.Resources = new Entity.RequestResources();
                if ((dt.Rows[rowNum][ExcelColumnNameVal.RequestType] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.RequestType]))
                {
                    var reqType = api.GetRequestTypeList().Where(m => m.ReqTypeDesc.ToLower().Trim() == dt.Rows[rowNum][ExcelColumnNameVal.RequestType].ToString().ToLower().Trim()).FirstOrDefault();
                    if (reqType != null)
                    {
                        requestItem.ReqTypeCd = Common.Security.Sanitize(reqType.ReqTypeCd);
                    }
                    else
                    {
                        resRow.Errors.Add(Entity.Constant.RequestFieldValueError.ReqTypeCd);
                    }
                    resRow.ReqTypeDesc = Common.Security.Sanitize((string)dt.Rows[rowNum][ExcelColumnNameVal.RequestType]);
                }
                if ((dt.Rows[rowNum][ExcelColumnNameVal.FirstName] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.FirstName]))
                {
                    requestItem.FirstName = Common.Security.Sanitize(dt.Rows[rowNum][ExcelColumnNameVal.FirstName].ToString().Trim());
                    resRow.FirstName = requestItem.FirstName;
                }
                if ((dt.Rows[rowNum][ExcelColumnNameVal.LastName] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.LastName]))
                {
                    requestItem.LastName = Common.Security.Sanitize(dt.Rows[rowNum][ExcelColumnNameVal.LastName].ToString().Trim());
                    resRow.LastName = requestItem.LastName;
                }
                if ((dt.Rows[rowNum][ExcelColumnNameVal.PrefNm] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.PrefNm]))
                {
                    requestItem.PrefNm = Common.Security.Sanitize(dt.Rows[rowNum][ExcelColumnNameVal.PrefNm].ToString().Trim());
                }
                if ((dt.Rows[rowNum][ExcelColumnNameVal.Salutation] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.Salutation]))
                {
                    var strSalutation = api.GetSalutationList().Where(m => m.SalutDesc.ToLower().Trim() == dt.Rows[rowNum][ExcelColumnNameVal.Salutation].ToString().ToLower().Trim()).FirstOrDefault();
                    if (strSalutation != null)
                    {
                        requestItem.SalutCd = Common.Security.Sanitize(strSalutation.SalutCd);
                    }
                    else
                    {
                        resRow.Errors.Add(Entity.Constant.RequestFieldValueError.SalutCd);
                    }
                }
                if ((dt.Rows[rowNum][ExcelColumnNameVal.Gender] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.Gender]))
                {
                    var Gender = api.GetGenderList().Where(m => m.GenderDesc.ToLower().Trim() == dt.Rows[rowNum][ExcelColumnNameVal.Gender].ToString().ToLower().Trim()).FirstOrDefault();
                    if (Gender != null)
                    {
                        requestItem.GenderCd = Common.Security.Sanitize(Gender.GenderCd);
                    }
                    else
                    {
                        resRow.Errors.Add(Entity.Constant.RequestFieldValueError.GenderCd);
                    }
                }
                if ((dt.Rows[rowNum][ExcelColumnNameVal.EthnicOrigin] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.EthnicOrigin]))
                {
                    var raceList = api.GetRaceList().Where(m => m.RaceDesc.ToLower().Trim() == dt.Rows[rowNum][ExcelColumnNameVal.EthnicOrigin].ToString().ToLower().Trim()).FirstOrDefault();
                    if (raceList != null)
                    {
                        requestItem.RaceCd = Common.Security.Sanitize(raceList.RaceCd);
                    }
                    else
                    {
                        resRow.Errors.Add(Entity.Constant.RequestFieldValueError.RaceCd);
                    }
                }
                if ((dt.Rows[rowNum][ExcelColumnNameVal.PersonalEmail] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.PersonalEmail]))
                {
                    if (Common.Security.IsValidEmail(dt.Rows[rowNum][ExcelColumnNameVal.PersonalEmail].ToString().Trim()))
                    {
                        requestItem.PrsnEmail = dt.Rows[rowNum][ExcelColumnNameVal.PersonalEmail].ToString().Trim();
                    }
                    else
                    {
                        resRow.Errors.Add(Entity.Constant.RequestFieldValueError.PrsnEmail);
                    }
                }
                if ((dt.Rows[rowNum][ExcelColumnNameVal.MobileNo] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.MobileNo]))
                {
                    if (Common.Security.IsValidMobileNum(dt.Rows[rowNum][ExcelColumnNameVal.MobileNo].ToString().Trim()))
                    {
                        requestItem.MobileNo = dt.Rows[rowNum][ExcelColumnNameVal.MobileNo].ToString().Trim();
                    }
                    else
                    {
                        resRow.Errors.Add(Entity.Constant.RequestFieldValueError.MobileNo);
                    }
                }

                requestItem.IsExistingStaff = false;
                if ((dt.Rows[rowNum][ExcelColumnNameVal.IsExistingStaff] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.IsExistingStaff]))
                {
                    if (dt.Rows[rowNum][ExcelColumnNameVal.IsExistingStaff] != null)
                    {
                        requestItem.IsExistingStaff = dt.Rows[rowNum][ExcelColumnNameVal.IsExistingStaff].ToString() == "Y" ? true : false;
                    }
                }
                requestItem.IsExSmuStd = false;
                if ((dt.Rows[rowNum][ExcelColumnNameVal.IsExSMUStudent] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.IsExSMUStudent]))
                {
                    if (dt.Rows[rowNum][ExcelColumnNameVal.IsExSMUStudent] != null)
                    {
                        requestItem.IsExSmuStd = dt.Rows[rowNum][ExcelColumnNameVal.IsExSMUStudent].ToString() == "Y" ? true : false;
                    }
                }
                if ((dt.Rows[rowNum][ExcelColumnNameVal.UserType] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.UserType]))
                {
                    var UserTypeList = api.GetUserTypeList().Where(m => m.UserTypeDesc.ToLower().Trim() == dt.Rows[rowNum][ExcelColumnNameVal.UserType].ToString().ToLower().Trim()).FirstOrDefault();
                    if (UserTypeList != null)
                    {
                        requestItem.EmploymentInfo.UserTypeCd = Common.Security.Sanitize(UserTypeList.UserTypeCd);
                    }
                    else
                    {
                        resRow.Errors.Add(Entity.Constant.RequestFieldValueError.UserTypeCd);
                    }
                    resRow.UserTypeDesc = Common.Security.Sanitize((string)dt.Rows[rowNum][ExcelColumnNameVal.UserType]);
                }
                if ((dt.Rows[rowNum][ExcelColumnNameVal.UserSubType] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.UserSubType]))
                {
                    //var UserSubType = api.GetUserSubTypeList().Where(m => m.UserSubTypeDesc.ToLower().Trim() == dt.Rows[rowNum][ExcelColumnNameVal.UserSubType].ToString().ToLower().Trim()).FirstOrDefault();
                    var UserSubType = api.GetUserSubTypeList().Where(m => (m.UserTypeCd == requestItem.EmploymentInfo.UserTypeCd) && (m.UserSubTypeDesc.ToLower().Trim() == dt.Rows[rowNum][ExcelColumnNameVal.UserSubType].ToString().ToLower().Trim())).FirstOrDefault();
                    if (UserSubType != null)
                    {
                        requestItem.EmploymentInfo.UserSubTypeCd = Common.Security.Sanitize(UserSubType.UserSubTypeCd);
                    }
                    else
                    {
                        resRow.Errors.Add(Entity.Constant.RequestFieldValueError.UserSubTypeCd);
                    }
                }
                if ((dt.Rows[rowNum][ExcelColumnNameVal.BusinessTitle] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.BusinessTitle]))
                {
                    requestItem.EmploymentInfo.JobTitle = Common.Security.Sanitize(dt.Rows[rowNum][ExcelColumnNameVal.BusinessTitle].ToString().Trim());
                }
                if ((dt.Rows[rowNum][ExcelColumnNameVal.CostCentre] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.CostCentre]))
                {
                    var costCtr = api.GetCostCentreList().Where(m => m.CostCtrCd.ToUpper().Trim() == dt.Rows[rowNum][ExcelColumnNameVal.CostCentre].ToString().ToUpper().Trim()).FirstOrDefault();
                    if (costCtr != null)
                    {
                        requestItem.EmploymentInfo.CostCtrCd = Common.Security.Sanitize(dt.Rows[rowNum][ExcelColumnNameVal.CostCentre].ToString());
                    }
                    else
                    {
                        resRow.Errors.Add(Entity.Constant.RequestFieldValueError.CostCtrCd);
                    }
                }

                if ((dt.Rows[rowNum][ExcelColumnNameVal.Remarks] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.Remarks]))
                {
                    requestItem.Remark = Common.Security.Sanitize(dt.Rows[rowNum][ExcelColumnNameVal.Remarks].ToString().Trim());
                }
               

                if ((dt.Rows[rowNum][ExcelColumnNameVal.EmploymentStartDate] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.EmploymentStartDate]))
                {
                    requestItem.EmploymentInfo.EmplStartDt = Onboarding.ExcelReader.ExcelReader.ToDateTime(dt.Rows[rowNum][ExcelColumnNameVal.EmploymentStartDate].ToString());
                    resRow.EmplStartDt = requestItem.EmploymentInfo.EmplStartDt;
                }
                else
                {
                    resRow.Errors.Add(Entity.Constant.RequestFieldValueError.EmplStartDt);
                }
                if ((dt.Rows[rowNum][ExcelColumnNameVal.EmploymentEndDate] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.EmploymentEndDate]))
                {
                    requestItem.EmploymentInfo.EmplEndDt = Onboarding.ExcelReader.ExcelReader.ToDateTime(dt.Rows[rowNum][ExcelColumnNameVal.EmploymentEndDate].ToString());
                }
                if ((dt.Rows[rowNum][ExcelColumnNameVal.IsPCNotebookRequired] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.IsPCNotebookRequired]))
                {
                    if (dt.Rows[rowNum][ExcelColumnNameVal.IsPCNotebookRequired] != null)
                    {
                        requestItem.Resources.IsPCSelected = dt.Rows[rowNum][ExcelColumnNameVal.IsPCNotebookRequired].ToString() == "Y" ? true : false;
                    }
                }
                if ((dt.Rows[rowNum][ExcelColumnNameVal.CorpTitleLevel] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.CorpTitleLevel]))
                {
                    requestItem.Resources.CorpTitleLevelCd = Entity.Constant.CorpTitle.GetCode(Common.Security.Sanitize(dt.Rows[rowNum][ExcelColumnNameVal.CorpTitleLevel].ToString().Trim()));
                    if (!string.IsNullOrEmpty(requestItem.Resources.CorpTitleLevelCd))
                    {
                        requestItem.Resources.CorpTitleLevelDesc = Entity.Constant.CorpTitle.GetDescription(requestItem.Resources.CorpTitleLevelCd);
                    }
                    else
                    {
                        resRow.Errors.Add(Entity.Constant.RequestFieldValueError.CorpTitleLevel);
                    }
                }
                if ((dt.Rows[rowNum][ExcelColumnNameVal.IsIPPhoneRequired] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.IsIPPhoneRequired]))
                {
                    if (dt.Rows[rowNum][ExcelColumnNameVal.IsIPPhoneRequired] != null)
                    {
                        requestItem.Resources.IsIPPhoneSelected = dt.Rows[rowNum][ExcelColumnNameVal.IsIPPhoneRequired].ToString() == "Y" ? true : false;
                    }
                }
                if ((dt.Rows[rowNum][ExcelColumnNameVal.Building] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.Building]))
                {
                    requestItem.Resources.Building = Common.Security.Sanitize(dt.Rows[rowNum][ExcelColumnNameVal.Building].ToString().Trim());
                }
                if ((dt.Rows[rowNum][ExcelColumnNameVal.Floor] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.Floor]))
                {
                    requestItem.Resources.Floor = Common.Security.Sanitize(dt.Rows[rowNum][ExcelColumnNameVal.Floor].ToString().Trim());
                }
                if ((dt.Rows[rowNum][ExcelColumnNameVal.Room] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.Room]))
                {
                    requestItem.Resources.Room = Common.Security.Sanitize(dt.Rows[rowNum][ExcelColumnNameVal.Room].ToString().Trim());
                }
                if ((dt.Rows[rowNum][ExcelColumnNameVal.IsEmailRequired] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.IsEmailRequired]))
                {
                    if (dt.Rows[rowNum][ExcelColumnNameVal.IsEmailRequired] != null)
                    {
                        requestItem.Resources.IsEmailSelected = dt.Rows[rowNum][ExcelColumnNameVal.IsEmailRequired].ToString() == "Y" ? true : false;
                    }
                }
                if ((dt.Rows[rowNum][ExcelColumnNameVal.IsSAPRequired] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.IsSAPRequired]))
                {
                    if (dt.Rows[rowNum][ExcelColumnNameVal.IsSAPRequired] != null)
                    {
                        requestItem.Resources.IsSAPSelected = dt.Rows[rowNum][ExcelColumnNameVal.IsSAPRequired].ToString() == "Y" ? true : false;
                    }
                }
                if (Common.RequestValidation.IsSAPRequired(requestItem.EmploymentInfo.UserSubTypeCd) == true)
                {
                    requestItem.Resources.IsSAPSelected = true;
                }
                if ((dt.Rows[rowNum][ExcelColumnNameVal.DARTFund] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.DARTFund]))
                {
                    requestItem.Resources.DARTFund = Common.Security.Sanitize(dt.Rows[rowNum][ExcelColumnNameVal.DARTFund].ToString().Trim());
                }
                if ((dt.Rows[rowNum][ExcelColumnNameVal.IsISISRequired] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.IsISISRequired]))
                {
                    if (dt.Rows[rowNum][ExcelColumnNameVal.IsISISRequired] != null)
                    {
                        requestItem.Resources.IsISISSelected = dt.Rows[rowNum][ExcelColumnNameVal.IsISISRequired].ToString() == "Y" ? true : false;
                    }
                }
                if (Common.RequestValidation.IsISISRequired(requestItem.EmploymentInfo.UserSubTypeCd) == true)
                {
                    requestItem.Resources.IsISISSelected = true;
                }
                if ((dt.Rows[rowNum][ExcelColumnNameVal.ISISRemarks] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.ISISRemarks]))
                {
                    requestItem.Resources.ISISRemarks = Common.Security.Sanitize(dt.Rows[rowNum][ExcelColumnNameVal.ISISRemarks].ToString().Trim());
                }
                if ((dt.Rows[rowNum][ExcelColumnNameVal.IseLearnEarlyAccessRequired] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.IseLearnEarlyAccessRequired]))
                {
                    if (dt.Rows[rowNum][ExcelColumnNameVal.IseLearnEarlyAccessRequired] != null)
                    {
                        requestItem.Resources.IsELearnSelected = dt.Rows[rowNum][ExcelColumnNameVal.IseLearnEarlyAccessRequired].ToString() == "Y" ? true : false;
                    }
                }
                if ((dt.Rows[rowNum][ExcelColumnNameVal.eLearnStartDate] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.eLearnStartDate]))
                {
                    requestItem.Resources.ELearnStartDate = Onboarding.ExcelReader.ExcelReader.ToDateTime(dt.Rows[rowNum][ExcelColumnNameVal.eLearnStartDate].ToString());
                }
                if ((dt.Rows[rowNum][ExcelColumnNameVal.eLearnRemarks] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.eLearnRemarks]))
                {
                    requestItem.Resources.ELearnRemarks = Common.Security.Sanitize(dt.Rows[rowNum][ExcelColumnNameVal.eLearnRemarks].ToString().Trim());
                }
                if ((dt.Rows[rowNum][ExcelColumnNameVal.IsAdditionalMeetingRoomBookingAccessRequired] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.IsAdditionalMeetingRoomBookingAccessRequired]))
                {
                    if (dt.Rows[rowNum][ExcelColumnNameVal.IsAdditionalMeetingRoomBookingAccessRequired] != null)
                    {
                        requestItem.Resources.IsMtgRoomSelected = dt.Rows[rowNum][ExcelColumnNameVal.IsAdditionalMeetingRoomBookingAccessRequired].ToString() == "Y" ? true : false;
                    }
                }
                if ((dt.Rows[rowNum][ExcelColumnNameVal.AdditionalMeetingRoomBookingDetails] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.AdditionalMeetingRoomBookingDetails]))
                {
                    requestItem.Resources.MtgRoomDetails = Common.Security.Sanitize(dt.Rows[rowNum][ExcelColumnNameVal.AdditionalMeetingRoomBookingDetails].ToString().Trim());
                }
                if ((dt.Rows[rowNum][ExcelColumnNameVal.IsCMSiNetRequired] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.IsCMSiNetRequired]))
                {
                    if (dt.Rows[rowNum][ExcelColumnNameVal.IsCMSiNetRequired] != null)
                    {
                        requestItem.Resources.IsINetSelected = dt.Rows[rowNum][ExcelColumnNameVal.IsCMSiNetRequired].ToString() == "Y" ? true : false;
                    }
                }
                if ((dt.Rows[rowNum][ExcelColumnNameVal.IsCMSOasisRequired] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.IsCMSOasisRequired]))
                {
                    if (dt.Rows[rowNum][ExcelColumnNameVal.IsCMSOasisRequired] != null)
                    {
                        requestItem.Resources.IsOasisSelected = dt.Rows[rowNum][ExcelColumnNameVal.IsCMSOasisRequired].ToString() == "Y" ? true : false;
                    }
                }
                if ((dt.Rows[rowNum][ExcelColumnNameVal.IsCMSNextwebRequired] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.IsCMSNextwebRequired]))
                {
                    if (dt.Rows[rowNum][ExcelColumnNameVal.IsCMSNextwebRequired] != null)
                    {
                        requestItem.Resources.IsNextwebSelected = dt.Rows[rowNum][ExcelColumnNameVal.IsCMSNextwebRequired].ToString() == "Y" ? true : false;
                    }
                }
                if ((dt.Rows[rowNum][ExcelColumnNameVal.IsEmailDistributionListRequired] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.IsEmailDistributionListRequired]))
                {
                    if (dt.Rows[rowNum][ExcelColumnNameVal.IsEmailDistributionListRequired] != null)
                    {
                        requestItem.Resources.IsEmailDLSelected = dt.Rows[rowNum][ExcelColumnNameVal.IsEmailDistributionListRequired].ToString() == "Y" ? true : false;
                    }
                }
                if ((dt.Rows[rowNum][ExcelColumnNameVal.EmailDistributionListDetails] != System.DBNull.Value) && !string.IsNullOrEmpty((string)dt.Rows[rowNum][ExcelColumnNameVal.EmailDistributionListDetails]))
                {
                    requestItem.Resources.EmailDLDetails = Common.Security.Sanitize(dt.Rows[rowNum][ExcelColumnNameVal.EmailDistributionListDetails].ToString().Trim());
                }

                Entity.RequestValidation reqVal = Common.RequestValidation.ValidateRequest(requestItem, true);
                List<string> valErrList = reqVal.GetAllErrorMessage();
                if (valErrList != null)
                {
                    foreach (string err in valErrList)
                    {
                        resRow.Errors.Add(err);
                    }
                }
                bulkRequest.Requests.Add(requestItem);
                if (resRow.Errors.Count > 0)
                {
                    resRow.IsResultPass = false;
                    bulkReqRes.Rows.Add(resRow);
                }
                else
                {
                    resRow.IsResultPass = true;
                }
            }
            return bulkReqRes;
        }

        private bool ValidateExcelContent(DataTable excelDT)
        {
            if ((excelDT == null) || (excelDT.Rows.Count == 0) || (excelDT.Columns.Count < Entity.Constant.ExcelColumnName.ColumnCount))
            {
                return false;
            }
            for (int i = 0; i < Entity.Constant.ExcelColumnName.ColumnCount; i++)
            {
                if ((string)(excelDT.Rows[0][i]) != Entity.Constant.ExcelColumnName.ColumnNames[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
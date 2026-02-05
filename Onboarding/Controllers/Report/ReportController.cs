using Onboarding.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Models;

namespace Onboarding.Controllers
{
    public partial class ReportController : Controller
    {
        [HttpGet]
        [Authorize]
        public ActionResult GeneralReport()
        {
            if (!Helper.Utility.HasPermission(Entity.Constant.AppResource.AppGeneralReport))
            {
                return View("Unauthorized");
            }

            Onboarding.Entity.RptSearchCriteria oData = new Entity.RptSearchCriteria();
            AssignValues();
            BindValuesGeneralReport();
            return View(oData);
        }

        [HttpGet]
        [Authorize]
        public void GeneralReportExport()
        {
            if (!Helper.Utility.HasPermission(Entity.Constant.AppResource.AppGeneralReport))
            {
                RedirectToAction("Unauthorized");
            }

            string firstname1 = SanitizeSearchOption(Request.QueryString["firstname1"]);
            string lastname = SanitizeSearchOption(Request.QueryString["lastname1"]);
            string UserTypeCd = SanitizeSearchOption(Request.QueryString["UserTypeCd1"]);
            DateTime? EmplStartDtFrom = TryConvertToDateTime(Request.QueryString["EmplStartDtFrom1"]);
            DateTime? EmplStartDtTo = TryConvertToDateTime(Request.QueryString["EmplStartDtTo1"]);
            DateTime? ReqDtFrom = TryConvertToDateTime(Request.QueryString["ReqDtFrom1"]);
            DateTime? ReqDtTo = TryConvertToDateTime(Request.QueryString["ReqDtTo1"]);
            int? ReqId = TryConvertToInt(Request.QueryString["ReqId1"]);
            string ReqStsCd = SanitizeSearchOption(Request.QueryString["ReqStsCd1"]);
            string UserSubTypeCd = SanitizeSearchOption(Request.QueryString["UserSubTypeCd1"]);
            string CostCtrCd = SanitizeSearchOption(Request.QueryString["CostCtrCd1"]);
            string RequestorUserId = SanitizeSearchOption(Request.QueryString["RequestorUserId1"]);
            string ReqTypeCd = SanitizeSearchOption(Request.QueryString["ReqTypeCd1"]);

            WebAPIManager.WebAPIManager api = new WebAPIManager.WebAPIManager();
            Onboarding.Entity.GenRptSearchResult oGenRptSearchResult = api.GetGeneralReport(Helper.Utility.GetCurrentUserId(), new RptSearchCriteria() { FirstName = firstname1, LastName = lastname, UserTypeCd = UserTypeCd, EmplStartDtFrom = EmplStartDtFrom, EmplStartDtTo = EmplStartDtTo, ReqDtFrom = ReqDtFrom, ReqDtTo = ReqDtTo, ReqId = ReqId, ReqStsCd = ReqStsCd, UserSubTypeCd = UserSubTypeCd, CostCtrCd = CostCtrCd, RequestorUserId = RequestorUserId, ReqTypeCd = ReqTypeCd });

            DataTable dt = ToDataTable(oGenRptSearchResult.Rows);
            using (MemoryStream ms = ExcelWriter.ExcelWriter.WriteExcelContent(dt))
            {
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "Onboarding_Report.xlsx"));
                ms.WriteTo(Response.OutputStream);
                Response.End();
            }
        }

        public ActionResult SearchRequest()
        {
            if (!Helper.Utility.HasPermission(Entity.Constant.AppResource.AppRequestSearch))
            {
                return View("Unauthorized");
            }

            Onboarding.Entity.RptSearchCriteria oData = new Entity.RptSearchCriteria();
            AssignValues();
            BindValuesSearchRequest();
            return View(oData);
        }

        private void AssignValues()
        {
            WebAPIManager.WebAPIManager api = new WebAPIManager.WebAPIManager();
            List<Entity.CostCentre> CostCentreList = api.GetCostCentreList();
            List<Entity.UserType> UserTypeList = api.GetUserTypeList();
            List<Entity.UserSubType> UserSubTypeList = api.GetUserSubTypeList();
            List<Entity.Requestor> oRequestor = api.GetRequestorList(Helper.Utility.GetCurrentUserId());
            List<Entity.RequestStatus> oRequestStatus = api.GetRequestStatusList();
            List<RequestType> oRequestType = api.GetRequestTypeList();
        }

        private void BindValuesGeneralReport()
        {
            WebAPIManager.WebAPIManager api = new WebAPIManager.WebAPIManager();
            List<CostCentre> costCtrList = api.GetCostCentreList();
            ViewBag.CostCentreList = costCtrList;
            ViewBag.UserTypeList = api.GetUserTypeList();
            ViewBag.UserSubTypeList = api.GetUserSubTypeList();
            ViewBag.RequestorList = api.GetRequestorList(Helper.Utility.GetCurrentUserId());
            ViewBag.RequestStatusList = api.GetRequestStatusList();
            ViewBag.RequestType = api.GetRequestTypeList();
        }

        private void BindValuesSearchRequest()
        {
            WebAPIManager.WebAPIManager api = new WebAPIManager.WebAPIManager();
            ViewBag.CostCentreList = api.GetAllCostCentreList();
        }

        private Entity.RptSearchCriteria SanitizeSearchCriteria(Entity.RptSearchCriteria searchCriteria)
        {
            searchCriteria.CostCtrCd = (searchCriteria.CostCtrCd == "null") ? null : searchCriteria.CostCtrCd;
            searchCriteria.FirstName = (searchCriteria.FirstName == "null") ? null : searchCriteria.FirstName;
            searchCriteria.LastName = (searchCriteria.LastName == "null") ? null : searchCriteria.LastName;
            searchCriteria.ReqStsCd = (searchCriteria.ReqStsCd == "null") ? null : searchCriteria.ReqStsCd;
            searchCriteria.ReqTypeCd = (searchCriteria.ReqTypeCd == "null") ? null : searchCriteria.ReqTypeCd;
            searchCriteria.RequestorUserId = (searchCriteria.RequestorUserId == "null") ? null : searchCriteria.RequestorUserId;
            searchCriteria.UserTypeCd = (searchCriteria.UserTypeCd == "null") ? null : searchCriteria.UserTypeCd;
            searchCriteria.UserSubTypeCd = (searchCriteria.UserSubTypeCd == "null") ? null : searchCriteria.UserSubTypeCd;
            return searchCriteria;
        }

        private string SanitizeSearchOption(string opt)
        {
            return (opt == "null") ? null : Common.Security.Sanitize(opt);
        }

        private DateTime? SanitizeSearchOption(DateTime opt)
        {
            DateTime? dt = null;
            if (opt != DateTime.MinValue)
            {
                dt = opt;
            }
            return dt;
        }

        private int? SanitizeSearchOption(int opt)
        {
            int? i = null;
            if (opt != 0)
            {
                i = opt;
            }
            return i;
        }

        DateTime? TryConvertToDateTime(string input)
        {
            DateTime dt = DateTime.MinValue;
            if (DateTime.TryParse(input, out dt))
            {
                return dt;
            }
            else
            {
                return null;
            }
        }

        int? TryConvertToInt(string input)
        {
            int i = 0;
            if (int.TryParse(input, out i))
            {
                return i;
            }
            else
            {
                return null;
            }
        }

        private DataTable ToDataTable(List<GenRptSearchResultRow> entityList)
        {
            var properties = typeof(GenRptSearchResultRow).GetProperties();
            var table = new DataTable();

            foreach (var property in properties)
            {
                var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                table.Columns.Add(MapColumnName(property.Name), type);
            }
            foreach (var entity in entityList)
            {
                table.Rows.Add(properties.Select(p => p.GetValue(entity, null)).ToArray());
            }
            return table;
        }

        private string MapColumnName(string colName)
        {
            switch (colName)
            {
                case "ReqId":
                    return "Request ID";
                case "ReqTypeDesc":
                    return "Request Type";
                case "FirstName":
                    return "First Name";
                case "LastName":
                    return "Last Name";
                case "UserTypeDesc":
                    return "Staff Type";
                case "UserSubTypeDesc":
                    return "Staff Subtype";               
                case "EmplStartDt":
                    return "Start Date";
                case "CostCtrDesc":
                    return "Cost Centre";
                case "ReqStsDesc":
                    return "Request Status";
                case "RequestorName":
                    return "Requestor";
                case "RequestDt":
                    return "Request Date";
                case "IsUrgent":
                    return "Is Urgent?";
                default:
                    break;
            }
            return colName;
        }
    }
}
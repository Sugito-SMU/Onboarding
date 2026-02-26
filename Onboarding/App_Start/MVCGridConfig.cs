using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using MVCGrid;
using MVCGrid.Web;
using MVCGrid.Models;
using MVCGrid.Web.Models;
using Onboarding.Entity;

namespace Onboarding.App_Start
{

    public class MVCGridConfig
    {
        public static void RegisterGeneralReportGrid()
        {
            // add your Grid definitions here, using the MVCGridDefinitionTable.Add method
            MVCGridDefinitionTable.Add("GeneralReportGrid", new MVCGridBuilder<GenRptSearchResultRow>()
                .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                .WithAdditionalQueryOptionNames("firstname1")
                .WithAdditionalQueryOptionNames("lastname1")
                .WithAdditionalQueryOptionNames("UserTypeCd1")
                .WithAdditionalQueryOptionNames("EmplStartDtFrom1")
                .WithAdditionalQueryOptionNames("EmplStartDtTo1")
                .WithAdditionalQueryOptionNames("EmplEndDtFrom1")
                .WithAdditionalQueryOptionNames("EmplEndDtTo1")
                .WithAdditionalQueryOptionNames("ReqDtFrom1")
                .WithAdditionalQueryOptionNames("ReqDtTo1")
                .WithAdditionalQueryOptionNames("ReqId1")
                .WithAdditionalQueryOptionNames("ReqStsCd1")
                .WithAdditionalQueryOptionNames("UserSubTypeCd1")
                .WithAdditionalQueryOptionNames("CostCtrDesc1")
                .WithAdditionalQueryOptionNames("RequestorUserId1")
                .WithAdditionalQueryOptionNames("ReqTypeCd1")
                .AddColumns(cols =>
                {
                    cols.Add("ReqId").WithHeaderText("Req Id")
                        .WithSorting(true)
                        .WithValueTemplate("", false)
                        .WithValueExpression(p => string.Format("<a href='../Agent/AgentRequest/{0}?ViewOnly=1'>{0}</a>", p.ReqId));
                    cols.Add("ReqTypeDesc").WithHeaderText("Request Type")
                        .WithSorting(true)
                        .WithValueExpression(p => p.ReqTypeDesc);
                    cols.Add("FirstName").WithHeaderText("First Name")
                        .WithSorting(true)
                        .WithValueExpression(p => p.FirstName);
                    cols.Add("LastName").WithHeaderText("Last Name")
                        .WithSorting(true)
                        .WithValueExpression(p => p.LastName);
                    cols.Add("UserTypeDesc").WithHeaderText("Staff Type")
                        .WithSorting(true)
                        .WithValueExpression(p => p.UserTypeDesc);
                    cols.Add("UserSubTypeDesc").WithHeaderText("Staff Subtype")
                        .WithSorting(true)
                        .WithValueExpression(p => p.UserSubTypeDesc);
                    cols.Add("EmplStartDt").WithHeaderText("Start Date")
                        .WithSorting(true)
                        .WithValueExpression(p => p.EmplStartDt.HasValue ? p.EmplStartDt.Value.ToString("d MMM yyyy") : string.Empty);
                    cols.Add("EmplEndDt").WithHeaderText("End Date")
                        .WithSorting(true)
                        .WithValueExpression(p => p.EmplEndDt.HasValue ? p.EmplEndDt.Value.ToString("d MMM yyyy") : string.Empty);
                    cols.Add("CostCtrDesc").WithHeaderText("Cost Centre")
                        .WithSorting(true)
                        .WithValueExpression(p => p.CostCtrDesc);
                    cols.Add("ReqStsDesc").WithHeaderText("Request Status")
                        .WithSorting(true)
                        .WithValueExpression(p => p.ReqStsDesc);
                    cols.Add("RequestorName").WithHeaderText("Requested By")
                        .WithSorting(true)
                        .WithValueExpression(p => p.RequestorName);
                    cols.Add("RequestDt").WithHeaderText("Request Date")
                        .WithSorting(true)
                        .WithValueExpression(p => p.RequestDt.HasValue ? p.RequestDt.Value.ToString("d MMM yyyy") : string.Empty);
                    cols.Add("IsUrgent").WithHeaderText("Urgency")
                        .WithSorting(true)
                        .WithValueExpression(p => p.IsUrgent.Value ? "Urgent" : "");
                })
                .WithSorting(true, "ReqId")
                .WithRetrieveDataMethod((context) =>
                {
                    // Query your data here. Obey Ordering, paging and filtering paramters given in the context.QueryOptions.        
                    // Use Entity Framwork, a module from your IoC Container, or any other method.         
                    // Return QueryResult object containing                             IEnumerable<YouModelItem>  
                    var options = context.QueryOptions;
                    string firstname1 = SanitizeSearchOption(options.GetAdditionalQueryOptionString("firstname1"));
                    string lastname = SanitizeSearchOption(options.GetAdditionalQueryOptionString("lastname1"));
                    string UserTypeCd = SanitizeSearchOption(options.GetAdditionalQueryOptionString("UserTypeCd1"));
                    DateTime tmpDt = DateTime.MinValue;
                    DateTime? EmplStartDtFrom = DateTime.TryParse(options.GetAdditionalQueryOptionString("EmplStartDtFrom1"), out tmpDt) ? SanitizeSearchOption(Convert.ToDateTime(options.GetAdditionalQueryOptionString("EmplStartDtFrom1"))) : null;
                    DateTime? EmplStartDtTo = DateTime.TryParse(options.GetAdditionalQueryOptionString("EmplStartDtTo1"), out tmpDt) ? SanitizeSearchOption(Convert.ToDateTime(options.GetAdditionalQueryOptionString("EmplStartDtTo1"))) : null; ;
                    DateTime? EmplEndDtFrom = DateTime.TryParse(options.GetAdditionalQueryOptionString("EmplEndDtFrom1"), out tmpDt) ? SanitizeSearchOption(Convert.ToDateTime(options.GetAdditionalQueryOptionString("EmplEndDtFrom1"))) : null; ;
                    DateTime? EmplEndDtTo = DateTime.TryParse(options.GetAdditionalQueryOptionString("EmplEndDtTo1"), out tmpDt) ? SanitizeSearchOption(Convert.ToDateTime(options.GetAdditionalQueryOptionString("EmplEndDtTo1"))) : null; ;
                    DateTime? ReqDtFrom = DateTime.TryParse(options.GetAdditionalQueryOptionString("ReqDtFrom1"), out tmpDt) ? SanitizeSearchOption(Convert.ToDateTime(options.GetAdditionalQueryOptionString("ReqDtFrom1"))) : null; ;
                    DateTime? ReqDtTo = DateTime.TryParse(options.GetAdditionalQueryOptionString("ReqDtTo1"), out tmpDt) ? SanitizeSearchOption(Convert.ToDateTime(options.GetAdditionalQueryOptionString("ReqDtTo1"))) : null; ;
                    int tmpInt = 0;
                    int? ReqId = int.TryParse(options.GetAdditionalQueryOptionString("ReqId1"), out tmpInt) ? SanitizeSearchOption(Convert.ToInt32(options.GetAdditionalQueryOptionString("ReqId1"))) : null;
                    string ReqStsCd = SanitizeSearchOption(options.GetAdditionalQueryOptionString("ReqStsCd1"));
                    string UserSubTypeCd = SanitizeSearchOption(options.GetAdditionalQueryOptionString("UserSubTypeCd1"));
                    string CostCtrDesc = SanitizeSearchOption(options.GetAdditionalQueryOptionString("CostCtrDesc1"));
                    CostCentre cc = Helper.Utility.GetCostCentreFromTextInput(CostCtrDesc);
                    string CostCtrCd = (cc != null) ? cc.CostCtrCd : null;
                    string RequestorUserId = SanitizeSearchOption(options.GetAdditionalQueryOptionString("RequestorUserId1"));
                    string ReqTypeCd = SanitizeSearchOption(options.GetAdditionalQueryOptionString("ReqTypeCd1"));

                    WebAPIManager.WebAPIManager api = new WebAPIManager.WebAPIManager();
                    Onboarding.Entity.GenRptSearchResult oGenRptSearchResult = api.GetGeneralReport(Helper.Utility.GetCurrentUserId(), new RptSearchCriteria() { FirstName = firstname1, LastName = lastname, UserTypeCd = UserTypeCd, EmplStartDtFrom = EmplStartDtFrom, EmplStartDtTo = EmplStartDtTo, EmplEndDtFrom = EmplEndDtFrom, EmplEndDtTo = EmplEndDtTo, ReqDtFrom = ReqDtFrom, ReqDtTo = ReqDtTo, ReqId = ReqId, ReqStsCd = ReqStsCd, UserSubTypeCd = UserSubTypeCd, CostCtrCd = CostCtrCd, RequestorUserId = RequestorUserId, ReqTypeCd = ReqTypeCd });

                    if (oGenRptSearchResult == null)
                    {
                        oGenRptSearchResult = new GenRptSearchResult();
                        oGenRptSearchResult.Rows = new List<GenRptSearchResultRow>();
                    }
                    else
                    {
                        if (!String.IsNullOrWhiteSpace(options.SortColumnName))
                        {
                            switch (options.SortColumnName.ToLower())
                            {

                                case "reqid":
                                    if (options.SortDirection == SortDirection.Asc)
                                        oGenRptSearchResult.Rows = oGenRptSearchResult.Rows.OrderBy(p => p.ReqId).ToList();
                                    else
                                        oGenRptSearchResult.Rows = oGenRptSearchResult.Rows.OrderByDescending(p => p.ReqId).ToList();
                                    break;

                                case "firstname":
                                    if (options.SortDirection == SortDirection.Asc)
                                        oGenRptSearchResult.Rows = oGenRptSearchResult.Rows.OrderBy(p => p.FirstName).ToList();
                                    else
                                        oGenRptSearchResult.Rows = oGenRptSearchResult.Rows.OrderByDescending(p => p.FirstName).ToList();
                                    break;
                                case "lastname":
                                    if (options.SortDirection == SortDirection.Asc)
                                        oGenRptSearchResult.Rows = oGenRptSearchResult.Rows.OrderBy(p => p.LastName).ToList();
                                    else
                                        oGenRptSearchResult.Rows = oGenRptSearchResult.Rows.OrderByDescending(p => p.LastName).ToList();
                                    break;
                                case "usertypedesc":
                                    if (options.SortDirection == SortDirection.Asc)
                                        oGenRptSearchResult.Rows = oGenRptSearchResult.Rows.OrderBy(p => p.UserTypeDesc).ToList();
                                    else
                                        oGenRptSearchResult.Rows = oGenRptSearchResult.Rows.OrderByDescending(p => p.UserTypeDesc).ToList();
                                    break;
                                case "reqtypedesc":
                                    if (options.SortDirection == SortDirection.Asc)
                                        oGenRptSearchResult.Rows = oGenRptSearchResult.Rows.OrderBy(p => p.ReqTypeDesc).ToList();
                                    else
                                        oGenRptSearchResult.Rows = oGenRptSearchResult.Rows.OrderByDescending(p => p.ReqTypeDesc).ToList();
                                    break;

                                case "usersubtypedesc":
                                    if (options.SortDirection == SortDirection.Asc)
                                        oGenRptSearchResult.Rows = oGenRptSearchResult.Rows.OrderBy(p => p.UserSubTypeDesc).ToList();
                                    else
                                        oGenRptSearchResult.Rows = oGenRptSearchResult.Rows.OrderByDescending(p => p.UserSubTypeDesc).ToList();
                                    break;

                                case "emplstartdt":
                                    if (options.SortDirection == SortDirection.Asc)
                                        oGenRptSearchResult.Rows = oGenRptSearchResult.Rows.OrderBy(p => p.EmplStartDt).ToList();
                                    else
                                        oGenRptSearchResult.Rows = oGenRptSearchResult.Rows.OrderByDescending(p => p.EmplStartDt).ToList();
                                    break;

                                case "emplenddt":
                                    if (options.SortDirection == SortDirection.Asc)
                                        oGenRptSearchResult.Rows = oGenRptSearchResult.Rows.OrderBy(p => p.EmplEndDt).ToList();
                                    else
                                        oGenRptSearchResult.Rows = oGenRptSearchResult.Rows.OrderByDescending(p => p.EmplEndDt).ToList();
                                    break;

                                case "costctrdesc":
                                    if (options.SortDirection == SortDirection.Asc)
                                        oGenRptSearchResult.Rows = oGenRptSearchResult.Rows.OrderBy(p => p.CostCtrDesc).ToList();
                                    else
                                        oGenRptSearchResult.Rows = oGenRptSearchResult.Rows.OrderByDescending(p => p.CostCtrDesc).ToList();
                                    break;

                                case "reqstsdesc":
                                    if (options.SortDirection == SortDirection.Asc)
                                        oGenRptSearchResult.Rows = oGenRptSearchResult.Rows.OrderBy(p => p.ReqStsDesc).ToList();
                                    else
                                        oGenRptSearchResult.Rows = oGenRptSearchResult.Rows.OrderByDescending(p => p.ReqStsDesc).ToList();
                                    break;

                                case "requestorname":
                                    if (options.SortDirection == SortDirection.Asc)
                                        oGenRptSearchResult.Rows = oGenRptSearchResult.Rows.OrderBy(p => p.RequestorName).ToList();
                                    else
                                        oGenRptSearchResult.Rows = oGenRptSearchResult.Rows.OrderByDescending(p => p.RequestorName).ToList();
                                    break;

                                case "requestdt":
                                    if (options.SortDirection == SortDirection.Asc)
                                        oGenRptSearchResult.Rows = oGenRptSearchResult.Rows.OrderBy(p => p.RequestDt).ToList();
                                    else
                                        oGenRptSearchResult.Rows = oGenRptSearchResult.Rows.OrderByDescending(p => p.RequestDt).ToList();
                                    break;

                                case "isurgent":
                                    if (options.SortDirection == SortDirection.Asc)
                                        oGenRptSearchResult.Rows = oGenRptSearchResult.Rows.OrderBy(p => p.IsUrgent).ToList();
                                    else
                                        oGenRptSearchResult.Rows = oGenRptSearchResult.Rows.OrderByDescending(p => p.IsUrgent).ToList();
                                    break;

                            }
                        }
                    }

                    return new QueryResult<GenRptSearchResultRow>()
                    {
                        Items = oGenRptSearchResult.Rows.Skip(options.PageIndex.Value * options.ItemsPerPage.Value).Take(options.ItemsPerPage.Value),
                        TotalRecords = oGenRptSearchResult.Rows.Count // if paging is enabled, return the total number of records of all pages        
                    };
                })
            );
        }

         public static string SanitizeSearchOption(string opt)
        {
            return (opt == "null") ? null : Common.Security.Sanitize(opt);
        }

        public static DateTime? SanitizeSearchOption(DateTime opt)
        {
            DateTime? dt = null;
            if (opt != DateTime.MinValue)
            {
                dt = opt;
            }
            return dt;
        }

        public static int? SanitizeSearchOption(int opt)
        {
            int? i = null;
            if (opt != 0)
            {
                i = opt;
            }
            return i;
        }

        public static void RegisterSearchRequestGrid()
        {
            // add your Grid definitions here, using the MVCGridDefinitionTable.Add method
            MVCGridDefinitionTable.Add("SearchRequest", new MVCGridBuilder<RequestSearchResultRow>()
                .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                .WithAdditionalQueryOptionNames("firstname1")
                .WithAdditionalQueryOptionNames("lastname1")
                .WithAdditionalQueryOptionNames("EmplStartDtFrom1")
                .WithAdditionalQueryOptionNames("EmplStartDtTo1")
                .WithAdditionalQueryOptionNames("EmplEndDtFrom1")
                .WithAdditionalQueryOptionNames("EmplEndDtTo1")
                .WithAdditionalQueryOptionNames("CostCtrDesc1")
                .AddColumns(cols =>
                {
                    cols.Add("ReqId").WithHeaderText("Req Id")
                        .WithValueExpression(p => p.ReqId.ToString())
                        .WithSorting(true);
                    cols.Add("FirstName").WithHeaderText("First Name")
                        .WithSorting(true)
                        .WithValueExpression(p => p.FirstName)
                        .WithSorting(true);
                    cols.Add("LastName").WithHeaderText("Last Name")
                        .WithValueExpression(p => p.LastName)
                        .WithSorting(true);
                    cols.Add("EmplStartDt").WithHeaderText("Start Date")
                        .WithSorting(true)
                        .WithVisibility(true, true)
                        .WithValueExpression(p => p.EmplStartDt.HasValue ? p.EmplStartDt.Value.ToString("d MMM yyyy") : string.Empty)
                        .WithSorting(true);
                    cols.Add("EmplEndDt").WithHeaderText("End Date")
                        .WithSorting(true)
                        .WithVisibility(true, true)
                        .WithValueExpression(p => p.EmplEndDt.HasValue ? p.EmplEndDt.Value.ToString("d MMM yyyy") : string.Empty)
                        .WithSorting(true);
                    cols.Add("CostCtrDesc").WithHeaderText("Cost Centre").WithSorting(true)
                        .WithVisibility(true, true)
                        .WithValueExpression(p => p.CostCtrDesc).WithSorting(true);
                    cols.Add("RequestorName").WithHeaderText("Requested By")
                        .WithSorting(true)
                        .WithValueExpression(p => p.RequestorName);
                    cols.Add("RequestDt").WithHeaderText("Request Date")
                        .WithSorting(true)
                        .WithValueExpression(p => p.RequestDt.HasValue ? p.RequestDt.Value.ToString("d MMM yyyy") : string.Empty);
                })
                .WithSorting(true, "ReqId")
                .WithRetrieveDataMethod((context) =>
                    {
                        // Query your data here. Obey Ordering, paging and filtering paramters given in the context.QueryOptions.        
                        // Use Entity Framwork, a module from your IoC Container, or any other method.         
                        // Return QueryResult object containing                             IEnumerable<YouModelItem>  
                        var options = context.QueryOptions;
                        string firstname1 = SanitizeSearchOption(options.GetAdditionalQueryOptionString("firstname1"));
                        string lastname = SanitizeSearchOption(options.GetAdditionalQueryOptionString("lastname1"));
                        DateTime tmpDt = DateTime.MinValue;
                        DateTime? EmplStartDtFrom = DateTime.TryParse(options.GetAdditionalQueryOptionString("EmplStartDtFrom1"), out tmpDt) ? SanitizeSearchOption(Convert.ToDateTime(options.GetAdditionalQueryOptionString("EmplStartDtFrom1"))) : null;
                        DateTime? EmplStartDtTo = DateTime.TryParse(options.GetAdditionalQueryOptionString("EmplStartDtTo1"), out tmpDt) ? SanitizeSearchOption(Convert.ToDateTime(options.GetAdditionalQueryOptionString("EmplStartDtTo1"))) : null;
                        DateTime? EmplEndDtFrom = DateTime.TryParse(options.GetAdditionalQueryOptionString("EmplEndDtFrom1"), out tmpDt) ? SanitizeSearchOption(Convert.ToDateTime(options.GetAdditionalQueryOptionString("EmplEndDtFrom1"))) : null;
                        DateTime? EmplEndDtTo = DateTime.TryParse(options.GetAdditionalQueryOptionString("EmplEndDtTo1"), out tmpDt) ? SanitizeSearchOption(Convert.ToDateTime(options.GetAdditionalQueryOptionString("EmplEndDtTo1"))) : null;
                        string CostCtrDesc = SanitizeSearchOption(options.GetAdditionalQueryOptionString("CostCtrDesc1"));
                        CostCentre cc = Helper.Utility.GetCostCentreFromTextInput(CostCtrDesc);
                        string CostCtrCd = (cc != null) ? cc.CostCtrCd : null;

                        WebAPIManager.WebAPIManager api = new WebAPIManager.WebAPIManager();
                        Onboarding.Entity.RequestSearchResult oSearchResult = api.SearchRequest(Helper.Utility.GetCurrentUserId(), new RptSearchCriteria() { FirstName = firstname1, LastName = lastname, EmplStartDtFrom = EmplStartDtFrom, EmplStartDtTo = EmplStartDtTo, EmplEndDtFrom = EmplEndDtFrom, EmplEndDtTo = EmplEndDtTo, CostCtrCd = CostCtrCd });
                        if (oSearchResult == null)
                        {
                            oSearchResult = new RequestSearchResult();
                            oSearchResult.Rows = new List<RequestSearchResultRow>();
                        }
                        else
                        {
                            if (!String.IsNullOrWhiteSpace(options.SortColumnName))
                            {
                                switch (options.SortColumnName.ToLower())
                                {
                                    case "firstname":
                                        if (options.SortDirection == SortDirection.Asc)
                                            oSearchResult.Rows = oSearchResult.Rows.OrderBy(p => p.FirstName).ToList();
                                        else
                                            oSearchResult.Rows = oSearchResult.Rows.OrderByDescending(p => p.FirstName).ToList();
                                        break;
                                    case "lastname":
                                        if (options.SortDirection == SortDirection.Asc)
                                            oSearchResult.Rows = oSearchResult.Rows.OrderBy(p => p.LastName).ToList();
                                        else
                                            oSearchResult.Rows = oSearchResult.Rows.OrderByDescending(p => p.LastName).ToList();
                                        break;
                                    case "reqid":
                                        if (options.SortDirection == SortDirection.Asc)
                                            oSearchResult.Rows = oSearchResult.Rows.OrderBy(p => p.ReqId).ToList();
                                        else
                                            oSearchResult.Rows = oSearchResult.Rows.OrderByDescending(p => p.ReqId).ToList();
                                        break;
                                    case "emplstartdt":
                                        if (options.SortDirection == SortDirection.Asc)
                                            oSearchResult.Rows = oSearchResult.Rows.OrderBy(p => p.EmplStartDt).ToList();
                                        else
                                            oSearchResult.Rows = oSearchResult.Rows.OrderByDescending(p => p.EmplStartDt).ToList();
                                        break;
                                    case "emplenddt":
                                        if (options.SortDirection == SortDirection.Asc)
                                            oSearchResult.Rows = oSearchResult.Rows.OrderBy(p => p.EmplEndDt).ToList();
                                        else
                                            oSearchResult.Rows = oSearchResult.Rows.OrderByDescending(p => p.EmplEndDt).ToList();
                                        break;
                                    case "costctrdesc":
                                        if (options.SortDirection == SortDirection.Asc)
                                            oSearchResult.Rows = oSearchResult.Rows.OrderBy(p => p.CostCtrDesc).ToList();
                                        else
                                            oSearchResult.Rows = oSearchResult.Rows.OrderByDescending(p => p.CostCtrDesc).ToList();
                                        break;
                                    case "requestorname":
                                        if (options.SortDirection == SortDirection.Asc)
                                            oSearchResult.Rows = oSearchResult.Rows.OrderBy(p => p.RequestorName).ToList();
                                        else
                                            oSearchResult.Rows = oSearchResult.Rows.OrderByDescending(p => p.RequestorName).ToList();
                                        break;
                                    case "requestdt":
                                        if (options.SortDirection == SortDirection.Asc)
                                            oSearchResult.Rows = oSearchResult.Rows.OrderBy(p => p.RequestDt).ToList();
                                        else
                                            oSearchResult.Rows = oSearchResult.Rows.OrderByDescending(p => p.RequestDt).ToList();
                                        break;
                                }
                            }
                        }

                        return new QueryResult<RequestSearchResultRow>()
                        {
                            Items = oSearchResult.Rows.Skip(options.PageIndex.Value * options.ItemsPerPage.Value).Take(options.ItemsPerPage.Value),
                            TotalRecords = oSearchResult.Rows.Count // if paging is enabled, return the total number of records of all pages        
                        };
                    })
                );
        }

        public static void RegisterMyRequestGrid()
        {
            // add your Grid definitions here, using the MVCGridDefinitionTable.Add method
            MVCGridDefinitionTable.Add("MyRequest", new MVCGridBuilder<RequestListForRequestorRow>()
            .WithAuthorizationType(AuthorizationType.AllowAnonymous)
            .WithSorting(sorting: true, defaultSortColumn: "ReqId", defaultSortDirection: SortDirection.Dsc)
            .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)

            .WithPageParameterNames("StatusCode")

           .AddColumns(cols =>
           {

               cols.Add("ReqId").WithHeaderText("Req Id")
                 .WithVisibility(true, true).WithSortColumnData("ReqId")
                 .WithValueExpression(p => p.ReqId.ToString())
                 .WithValueTemplate("", false)
                 .WithValueExpression(p => string.Format("<a href='../../Requestor/PersonalInfo/{0}'>{0}</a>", p.ReqId))
                 .WithSortColumnData("ReqId").WithSorting(true);
               cols.Add("ReqTypeDesc").WithHeaderText("Request Type")
                   .WithVisibility(true, true)
                   .WithValueExpression(p => p.ReqTypeDesc)
                    .WithSortColumnData("ReqTypeDesc").WithSorting(true);
               cols.Add("FirstName").WithHeaderText("First Name")
                   .WithVisibility(true, true)
                   .WithValueExpression(p => p.FirstName).WithSorting(true);
               cols.Add("LastName").WithHeaderText("Last Name")
                   .WithVisibility(true, true)
                   .WithValueExpression(p => p.LastName).WithSorting(true);
               cols.Add("UserTypeDesc").WithHeaderText("Staff Type")
                  .WithVisibility(true, true)
                  .WithValueExpression(p => p.UserTypeDesc).WithSorting(true);
               cols.Add("UserSubTypeDesc").WithHeaderText("Staff Subtype")
                   .WithVisibility(true, true)
                   .WithValueExpression(p => p.UserSubTypeDesc).WithSorting(true);
               cols.Add("EmplStartDt").WithHeaderText("Start Date")
                   .WithVisibility(true, true)
                   .WithValueExpression(p => (p.EmplStartDt.HasValue ? p.EmplStartDt.Value.ToString("d MMM yyyy") : string.Empty)).WithSorting(true);
               cols.Add("CostCtrDesc").WithHeaderText("Cost Centre")
                   .WithVisibility(true, true)
                   .WithValueExpression(p => p.CostCtrDesc)
                   .WithSorting(true);
               cols.Add("ReqStsDesc").WithHeaderText("Request Status")
                   .WithVisibility(true, true)
                   .WithValueExpression(p => p.ReqStsDesc).WithSorting(true);
               cols.Add("RequestorName").WithHeaderText("Requested By")
                   .WithVisibility(true, true)
                   .WithValueExpression(p => p.RequestorName)
                   .WithSorting(true);
               cols.Add("RequestDt").WithHeaderText("Request Date")
                  .WithVisibility(true, true)
                  .WithValueExpression(p => (p.RequestDt.HasValue ? p.RequestDt.Value.ToString("d MMM yyyy") : string.Empty)).WithSorting(true);
               cols.Add("Url").WithVisibility(false)
                   .WithValueExpression((p, c) => c.UrlHelper.Action("detail", "demo", new { id = p.ReqId }));
           })
                        .WithRetrieveDataMethod((context) =>
                        {
                            // Query your data here. Obey Ordering, paging and filtering paramters given in the context.QueryOptions.        
                            // Use Entity Framwork, a module from your IoC Container, or any other method.         
                            // Return QueryResult object containing                             IEnumerable<YouModelItem>  
                            var options = context.QueryOptions;
                            var dd = Common.Security.Sanitize(options.GetPageParameterString("StatusCode"));

                            WebAPIManager.WebAPIManager api = new WebAPIManager.WebAPIManager();
                            Entity.RequestListForRequestor oRequestListForRequestor = api.GetRequestListForRequestor(dd, Helper.Utility.GetCurrentUserId());
                            if (oRequestListForRequestor == null)
                            {
                                oRequestListForRequestor = new RequestListForRequestor();
                                oRequestListForRequestor.Rows = new List<RequestListForRequestorRow>();
                            }
                            else
                            {
                                if (!String.IsNullOrWhiteSpace(options.SortColumnName))
                                {
                                    switch (options.SortColumnName.ToLower())
                                    {
                                        case "reqid":
                                            if (options.SortDirection == SortDirection.Asc)
                                                oRequestListForRequestor.Rows = oRequestListForRequestor.Rows.OrderBy(p => p.ReqId).ToList();
                                            else
                                                oRequestListForRequestor.Rows = oRequestListForRequestor.Rows.OrderByDescending(p => p.ReqId).ToList();
                                            break;
                                        case "reqtypedesc":
                                            if (options.SortDirection == SortDirection.Asc)
                                                oRequestListForRequestor.Rows = oRequestListForRequestor.Rows.OrderBy(p => p.ReqTypeDesc).ToList();
                                            else
                                                oRequestListForRequestor.Rows = oRequestListForRequestor.Rows.OrderByDescending(p => p.ReqTypeDesc).ToList();
                                            break;
                                        case "firstname":
                                            if (options.SortDirection == SortDirection.Asc)
                                                oRequestListForRequestor.Rows = oRequestListForRequestor.Rows.OrderBy(p => p.FirstName).ToList();
                                            else
                                                oRequestListForRequestor.Rows = oRequestListForRequestor.Rows.OrderByDescending(p => p.FirstName).ToList();
                                            break;
                                        case "lastname":
                                            if (options.SortDirection == SortDirection.Asc)
                                                oRequestListForRequestor.Rows = oRequestListForRequestor.Rows.OrderBy(p => p.LastName).ToList();
                                            else
                                                oRequestListForRequestor.Rows = oRequestListForRequestor.Rows.OrderByDescending(p => p.LastName).ToList();
                                            break;
                                        case "usertypedesc":
                                            if (options.SortDirection == SortDirection.Asc)
                                                oRequestListForRequestor.Rows = oRequestListForRequestor.Rows.OrderBy(p => p.UserTypeDesc).ToList();
                                            else
                                                oRequestListForRequestor.Rows = oRequestListForRequestor.Rows.OrderByDescending(p => p.UserTypeDesc).ToList();
                                            break;
                                        case "usersubtypedesc":
                                            if (options.SortDirection == SortDirection.Asc)
                                                oRequestListForRequestor.Rows = oRequestListForRequestor.Rows.OrderBy(p => p.UserSubTypeDesc).ToList();
                                            else
                                                oRequestListForRequestor.Rows = oRequestListForRequestor.Rows.OrderByDescending(p => p.UserSubTypeDesc).ToList();
                                            break;
                                        case "emplstartdt":
                                            if (options.SortDirection == SortDirection.Asc)
                                                oRequestListForRequestor.Rows = oRequestListForRequestor.Rows.OrderBy(p => p.EmplStartDt).ToList();
                                            else
                                                oRequestListForRequestor.Rows = oRequestListForRequestor.Rows.OrderByDescending(p => p.EmplStartDt).ToList();
                                            break;
                                        case "costctrdesc":
                                            if (options.SortDirection == SortDirection.Asc)
                                                oRequestListForRequestor.Rows = oRequestListForRequestor.Rows.OrderBy(p => p.CostCtrDesc).ToList();
                                            else
                                                oRequestListForRequestor.Rows = oRequestListForRequestor.Rows.OrderByDescending(p => p.CostCtrDesc).ToList();
                                            break;
                                        case "reqstsdesc":
                                            if (options.SortDirection == SortDirection.Asc)
                                                oRequestListForRequestor.Rows = oRequestListForRequestor.Rows.OrderBy(p => p.ReqStsDesc).ToList();
                                            else
                                                oRequestListForRequestor.Rows = oRequestListForRequestor.Rows.OrderByDescending(p => p.ReqStsDesc).ToList();
                                            break;
                                        case "requestorname":
                                            if (options.SortDirection == SortDirection.Asc)
                                                oRequestListForRequestor.Rows = oRequestListForRequestor.Rows.OrderBy(p => p.RequestorName).ToList();
                                            else
                                                oRequestListForRequestor.Rows = oRequestListForRequestor.Rows.OrderByDescending(p => p.RequestorName).ToList();
                                            break;
                                        case "requestdt":
                                            if (options.SortDirection == SortDirection.Asc)
                                                oRequestListForRequestor.Rows = oRequestListForRequestor.Rows.OrderBy(p => p.RequestDt).ToList();
                                            else
                                                oRequestListForRequestor.Rows = oRequestListForRequestor.Rows.OrderByDescending(p => p.RequestDt).ToList();
                                            break;


                                    }
                                }
                            }
                            return new QueryResult<RequestListForRequestorRow>()
                            {
                                Items = oRequestListForRequestor.Rows.Skip(options.PageIndex.Value * options.ItemsPerPage.Value).Take(options.ItemsPerPage.Value),
                                TotalRecords = oRequestListForRequestor.Rows.Count // if paging is enabled, return the total number of records of all pages        
                            };
                        })
                        );

        }

        public static void RegisterRequestListForAgentGrid()
        {
            // add your Grid definitions here, using the MVCGridDefinitionTable.Add method
            MVCGridDefinitionTable.Add("AgentGrid", new MVCGridBuilder<RequestListForAgentRow>()
               .WithAdditionalSetting(MVCGrid.Rendering.BootstrapRenderingEngine.SettingNameTableClass, "table table-bordered")
                .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                 .WithSorting(sorting: true, defaultSortColumn: "EmplStartDt", defaultSortDirection: SortDirection.Asc)
                 .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                 .WithPageParameterNames("StatusCode")
                 .WithRowCssClassExpression(p => p.IsUrgent.Value ? "urgent_row" : "")
                 .AddColumns(cols =>
                 {
                     cols.Add("ReqId").WithHeaderText("Req Id")
                         .WithVisibility(true, true).WithSortColumnData("ReqId")
                         .WithValueExpression(p => p.ReqId.ToString()).WithSorting(true)
                         .WithValueTemplate("", false)
                         .WithValueExpression(p => string.Format("<a href='../../Agent/AgentRequest/{0}'>{0}</a>", p.ReqId))
                         .WithSortColumnData("ReqId").WithSorting(true);
                     cols.Add("ReqTypeDesc").WithHeaderText("Request Type")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.ReqTypeDesc)
                         .WithSortColumnData("ReqTypeDesc")
                         .WithSorting(true);
                     cols.Add("FirstName").WithHeaderText("First Name")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.FirstName)
                         .WithSorting(true);
                     cols.Add("LastName").WithHeaderText("Last Name")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.LastName)
                         .WithSorting(true);
                     cols.Add("UserTypeDesc").WithHeaderText("Staff Type")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.UserTypeDesc)
                         .WithSorting(true);
                     cols.Add("UserSubTypeDesc").WithHeaderText("Staff Subtype")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.UserSubTypeDesc)
                         .WithSorting(true);
                     cols.Add("EmplStartDt").WithHeaderText("Start Date")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.EmplStartDt.Value.ToString("d MMM yyyy"))
                         .WithSorting(true);
                     cols.Add("CostCtrDesc").WithHeaderText("Cost Centre")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.CostCtrDesc)
                         .WithSorting(true);
                     cols.Add("ReqStsDesc").WithHeaderText("Request Status")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.ReqStsDesc)
                         .WithSorting(true);
                     cols.Add("RequestorName").WithHeaderText("Requested By")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.RequestorName)
                         .WithSorting(true);
                     cols.Add("RequestDt").WithHeaderText("Request Date")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.RequestDt.HasValue ? p.RequestDt.Value.ToString("d MMM yyyy") : string.Empty)
                         .WithSorting(true);
                     cols.Add("IsUrgent").WithHeaderText("Urgency")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.IsUrgent.Value ? "Urgent" : "")
                         .WithSorting(true);
                     cols.Add("Url").WithVisibility(false)
                         .WithValueExpression((p, c) => c.UrlHelper.Action("detail", "demo", new { id = p.ReqId }));
                 })
                 .WithRetrieveDataMethod((context) =>
                 {
                     // Query your data here. Obey Ordering, paging and filtering paramters given in the context.QueryOptions.        
                     // Use Entity Framwork, a module from your IoC Container, or any other method.         
                     // Return QueryResult object containing                             IEnumerable<YouModelItem>  
                     var options = context.QueryOptions;

                     var dd = Common.Security.Sanitize(options.GetPageParameterString("StatusCode"));

                     WebAPIManager.WebAPIManager api = new WebAPIManager.WebAPIManager();
                     Entity.RequestListForAgent oRequestListForAgent = api.GetRequestListForAgent(dd, Helper.Utility.GetCurrentUserId());
                     if (oRequestListForAgent == null)
                     {
                         oRequestListForAgent = new RequestListForAgent();
                         oRequestListForAgent.Rows = new List<RequestListForAgentRow>();
                     }
                     else
                     {
                         if (!String.IsNullOrWhiteSpace(options.SortColumnName))
                         {
                             switch (options.SortColumnName.ToLower())
                             {

                                 case "reqid":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.ReqId).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.ReqId).ToList();
                                     break;
                                 case "reqtypedesc":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.ReqTypeDesc).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.ReqTypeDesc).ToList();
                                     break;
                                 case "firstname":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.FirstName).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.FirstName).ToList();
                                     break;
                                 case "lastname":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.LastName).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.LastName).ToList();
                                     break;
                                 case "usertypedesc":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.UserTypeDesc).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.UserTypeDesc).ToList();
                                     break;
                                 case "usersubtypedesc":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.UserSubTypeDesc).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.UserSubTypeDesc).ToList();
                                     break;
                                 case "emplstartdt":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.EmplStartDt).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.EmplStartDt).ToList();
                                     break;
                                 case "costctrdesc":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.CostCtrDesc).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.CostCtrDesc).ToList();
                                     break;
                                 case "reqstsdesc":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.ReqStsDesc).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.ReqStsDesc).ToList();
                                     break;
                                 case "requestorname":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.RequestorName).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.RequestorName).ToList();
                                     break;
                                 case "requestdt":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.RequestDt).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.RequestDt).ToList();
                                     break;
                                 case "isurgent":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.IsUrgent).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.IsUrgent).ToList();
                                     break;
                             }
                         }
                     }
                     return new QueryResult<RequestListForAgentRow>()
                     {
                         Items = oRequestListForAgent.Rows.Skip(options.PageIndex.Value * options.ItemsPerPage.Value).Take(options.ItemsPerPage.Value),
                         TotalRecords = oRequestListForAgent.Rows.Count // if paging is enabled, return the total number of records of all pages        
                     };
                 })
             );

        }

        public static void RegisterRequestListForHRAdminGrid()
        {
            // add your Grid definitions here, using the MVCGridDefinitionTable.Add method
            MVCGridDefinitionTable.Add("HRAdminGrid", new MVCGridBuilder<RequestListForAgentRow>()
               .WithAdditionalSetting(MVCGrid.Rendering.BootstrapRenderingEngine.SettingNameTableClass, "table table-bordered")
                .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                 .WithSorting(sorting: true, defaultSortColumn: "EmplStartDt", defaultSortDirection: SortDirection.Asc)
                 .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                 .WithPageParameterNames("StatusCode")
                 .WithRowCssClassExpression(p => p.IsUrgent.Value ? "urgent_row" : "")
                 .AddColumns(cols =>
                 {
                     cols.Add("ReqId").WithHeaderText("Req Id")
                         .WithVisibility(true, true).WithSortColumnData("ReqId")
                         .WithValueExpression(p => p.ReqId.ToString()).WithSorting(true)
                         .WithValueTemplate("", false)
                         .WithValueExpression(p => string.Format("<a href='../../HRAdmin/HRAdminRequest/{0}'>{0}</a>", p.ReqId))
                         .WithSortColumnData("ReqId").WithSorting(true);
                     cols.Add("ReqTypeDesc").WithHeaderText("Request Type")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.ReqTypeDesc)
                         .WithSortColumnData("ReqTypeDesc")
                         .WithSorting(true);
                     cols.Add("FirstName").WithHeaderText("First Name")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.FirstName)
                         .WithSorting(true);
                     cols.Add("LastName").WithHeaderText("Last Name")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.LastName)
                         .WithSorting(true);
                     cols.Add("UserTypeDesc").WithHeaderText("Staff Type")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.UserTypeDesc)
                         .WithSorting(true);
                     cols.Add("UserSubTypeDesc").WithHeaderText("Staff Subtype")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.UserSubTypeDesc)
                         .WithSorting(true);
                     cols.Add("EmplStartDt").WithHeaderText("Start Date")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.EmplStartDt.Value.ToString("d MMM yyyy"))
                         .WithSorting(true);
                     cols.Add("CostCtrDesc").WithHeaderText("Cost Centre")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.CostCtrDesc)
                         .WithSorting(true);
                     cols.Add("ReqStsDesc").WithHeaderText("Request Status")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.ReqStsDesc)
                         .WithSorting(true);
                     cols.Add("RequestorName").WithHeaderText("Requested By")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.RequestorName)
                         .WithSorting(true);
                     cols.Add("RequestDt").WithHeaderText("Request Date")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.RequestDt.HasValue ? p.RequestDt.Value.ToString("d MMM yyyy") : string.Empty)
                         .WithSorting(true);
                     cols.Add("IsUrgent").WithHeaderText("Urgency")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.IsUrgent.Value ? "Urgent" : "")
                         .WithSorting(true);
                     cols.Add("Url").WithVisibility(false)
                         .WithValueExpression((p, c) => c.UrlHelper.Action("detail", "demo", new { id = p.ReqId }));
                 })
                 .WithRetrieveDataMethod((context) =>
                 {
                     // Query your data here. Obey Ordering, paging and filtering paramters given in the context.QueryOptions.        
                     // Use Entity Framwork, a module from your IoC Container, or any other method.         
                     // Return QueryResult object containing                             IEnumerable<YouModelItem>  
                     var options = context.QueryOptions;

                     var dd = Common.Security.Sanitize(options.GetPageParameterString("StatusCode"));

                     WebAPIManager.WebAPIManager api = new WebAPIManager.WebAPIManager();
                     Entity.RequestListForAgent oRequestListForAgent = api.GetRequestListForHRAdmin(dd, Helper.Utility.GetCurrentUserId());
                     if (oRequestListForAgent == null)
                     {
                         oRequestListForAgent = new RequestListForAgent();
                         oRequestListForAgent.Rows = new List<RequestListForAgentRow>();
                     }
                     else
                     {
                         if (!String.IsNullOrWhiteSpace(options.SortColumnName))
                         {
                             switch (options.SortColumnName.ToLower())
                             {

                                 case "reqid":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.ReqId).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.ReqId).ToList();
                                     break;
                                 case "reqtypedesc":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.ReqTypeDesc).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.ReqTypeDesc).ToList();
                                     break;
                                 case "firstname":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.FirstName).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.FirstName).ToList();
                                     break;
                                 case "lastname":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.LastName).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.LastName).ToList();
                                     break;
                                 case "usertypedesc":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.UserTypeDesc).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.UserTypeDesc).ToList();
                                     break;
                                 case "usersubtypedesc":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.UserSubTypeDesc).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.UserSubTypeDesc).ToList();
                                     break;
                                 case "emplstartdt":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.EmplStartDt).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.EmplStartDt).ToList();
                                     break;
                                 case "costctrdesc":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.CostCtrDesc).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.CostCtrDesc).ToList();
                                     break;
                                 case "reqstsdesc":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.ReqStsDesc).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.ReqStsDesc).ToList();
                                     break;
                                 case "requestorname":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.RequestorName).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.RequestorName).ToList();
                                     break;
                                 case "requestdt":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.RequestDt).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.RequestDt).ToList();
                                     break;
                                 case "isurgent":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.IsUrgent).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.IsUrgent).ToList();
                                     break;
                             }
                         }
                     }
                     return new QueryResult<RequestListForAgentRow>()
                     {
                         Items = oRequestListForAgent.Rows.Skip(options.PageIndex.Value * options.ItemsPerPage.Value).Take(options.ItemsPerPage.Value),
                         TotalRecords = oRequestListForAgent.Rows.Count // if paging is enabled, return the total number of records of all pages        
                     };
                 })
             );

        }

        public static void RegisterRequestListForAgentGridCompleted()
        {
            // add your Grid definitions here, using the MVCGridDefinitionTable.Add method
            MVCGridDefinitionTable.Add("AgentGridCompleted", new MVCGridBuilder<RequestListForAgentRow>()
               .WithAdditionalSetting(MVCGrid.Rendering.BootstrapRenderingEngine.SettingNameTableClass, "table table-bordered")
                .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                 .WithSorting(sorting: true, defaultSortColumn: "EmplStartDt", defaultSortDirection: SortDirection.Asc)
                 .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                 .WithPageParameterNames("StatusCode")
                 .WithRowCssClassExpression(p => p.IsUrgent.Value ? "urgent_row" : "")
                 .AddColumns(cols =>
                 {
                     cols.Add("ReqId").WithHeaderText("Req Id")
                         .WithVisibility(true, true).WithSortColumnData("ReqId")
                         .WithValueExpression(p => p.ReqId.ToString()).WithSorting(true)
                         .WithValueTemplate("", false)
                         .WithValueExpression(p => string.Format("<a href='../../Agent/AgentRequest/{0}'>{0}</a>", p.ReqId))
                         .WithSortColumnData("ReqId").WithSorting(true);
                     cols.Add("ReqTypeDesc").WithHeaderText("Request Type")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.ReqTypeDesc)
                         .WithSortColumnData("ReqTypeDesc")
                         .WithSorting(true);
                     cols.Add("FirstName").WithHeaderText("First Name")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.FirstName)
                         .WithSorting(true);
                     cols.Add("LastName").WithHeaderText("Last Name")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.LastName)
                         .WithSorting(true);
                     cols.Add("UserTypeDesc").WithHeaderText("Staff Type")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.UserTypeDesc)
                         .WithSorting(true);
                     cols.Add("UserSubTypeDesc").WithHeaderText("Staff Subtype")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.UserSubTypeDesc)
                         .WithSorting(true);
                     cols.Add("EmplStartDt").WithHeaderText("Start Date")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.EmplStartDt.Value.ToString("d MMM yyyy"))
                         .WithSorting(true);
                     cols.Add("CostCtrDesc").WithHeaderText("Cost Centre")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.CostCtrDesc)
                         .WithSorting(true);
                     cols.Add("ReqStsDesc").WithHeaderText("Request Status")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.ReqStsDesc)
                         .WithSorting(true);
                     cols.Add("RequestorName").WithHeaderText("Requested By")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.RequestorName)
                         .WithSorting(true);
                     cols.Add("RequestDt").WithHeaderText("Request Date")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.RequestDt.HasValue ? p.RequestDt.Value.ToString("d MMM yyyy") : string.Empty)
                         .WithSorting(true);
                     cols.Add("Url").WithVisibility(false)
                         .WithValueExpression((p, c) => c.UrlHelper.Action("detail", "demo", new { id = p.ReqId }));
                 })
                 .WithRetrieveDataMethod((context) =>
                 {
                     // Query your data here. Obey Ordering, paging and filtering paramters given in the context.QueryOptions.        
                     // Use Entity Framwork, a module from your IoC Container, or any other method.         
                     // Return QueryResult object containing                             IEnumerable<YouModelItem>  
                     var options = context.QueryOptions;

                     var dd = Common.Security.Sanitize(options.GetPageParameterString("StatusCode"));

                     WebAPIManager.WebAPIManager api = new WebAPIManager.WebAPIManager();
                     Entity.RequestListForAgent oRequestListForAgent = api.GetRequestListForAgent(dd, Helper.Utility.GetCurrentUserId());
                     if (oRequestListForAgent == null)
                     {
                         oRequestListForAgent = new RequestListForAgent();
                         oRequestListForAgent.Rows = new List<RequestListForAgentRow>();
                     }
                     else
                     {
                         if (!String.IsNullOrWhiteSpace(options.SortColumnName))
                         {
                             switch (options.SortColumnName.ToLower())
                             {

                                 case "reqid":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.ReqId).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.ReqId).ToList();
                                     break;
                                 case "reqtypedesc":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.ReqTypeDesc).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.ReqTypeDesc).ToList();
                                     break;
                                 case "firstname":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.FirstName).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.FirstName).ToList();
                                     break;
                                 case "lastname":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.LastName).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.LastName).ToList();
                                     break;
                                 case "usertypedesc":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.UserTypeDesc).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.UserTypeDesc).ToList();
                                     break;
                                 case "usersubtypedesc":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.UserSubTypeDesc).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.UserSubTypeDesc).ToList();
                                     break;
                                 case "emplstartdt":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.EmplStartDt).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.EmplStartDt).ToList();
                                     break;
                                 case "costctrdesc":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.CostCtrDesc).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.CostCtrDesc).ToList();
                                     break;
                                 case "reqstsdesc":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.ReqStsDesc).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.ReqStsDesc).ToList();
                                     break;
                                 case "requestorname":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.RequestorName).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.RequestorName).ToList();
                                     break;
                                 case "requestdt":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.RequestDt).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.RequestDt).ToList();
                                     break;
                             }
                         }
                     }
                     return new QueryResult<RequestListForAgentRow>()
                     {
                         Items = oRequestListForAgent.Rows.Skip(options.PageIndex.Value * options.ItemsPerPage.Value).Take(options.ItemsPerPage.Value),
                         TotalRecords = oRequestListForAgent.Rows.Count // if paging is enabled, return the total number of records of all pages        
                     };
                 })
             );

        }

        public static void RegisterRequestListForHRAdminGridCompleted()
        {
            // add your Grid definitions here, using the MVCGridDefinitionTable.Add method
            MVCGridDefinitionTable.Add("HRAdminGridCompleted", new MVCGridBuilder<RequestListForAgentRow>()
               .WithAdditionalSetting(MVCGrid.Rendering.BootstrapRenderingEngine.SettingNameTableClass, "table table-bordered")
                .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                 .WithSorting(sorting: true, defaultSortColumn: "EmplStartDt", defaultSortDirection: SortDirection.Asc)
                 .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                 .WithPageParameterNames("StatusCode")
                 .WithRowCssClassExpression(p => p.IsUrgent.Value ? "urgent_row" : "")
                 .AddColumns(cols =>
                 {
                     cols.Add("ReqId").WithHeaderText("Req Id")
                         .WithVisibility(true, true).WithSortColumnData("ReqId")
                         .WithValueExpression(p => p.ReqId.ToString()).WithSorting(true)
                         .WithValueTemplate("", false)
                         .WithValueExpression(p => string.Format("<a href='../../HRAdmin/HRAdminRequest/{0}'>{0}</a>", p.ReqId))
                         .WithSortColumnData("ReqId").WithSorting(true);
                     cols.Add("ReqTypeDesc").WithHeaderText("Request Type")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.ReqTypeDesc)
                         .WithSortColumnData("ReqTypeDesc")
                         .WithSorting(true);
                     cols.Add("FirstName").WithHeaderText("First Name")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.FirstName)
                         .WithSorting(true);
                     cols.Add("LastName").WithHeaderText("Last Name")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.LastName)
                         .WithSorting(true);
                     cols.Add("UserTypeDesc").WithHeaderText("Staff Type")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.UserTypeDesc)
                         .WithSorting(true);
                     cols.Add("UserSubTypeDesc").WithHeaderText("Staff Subtype")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.UserSubTypeDesc)
                         .WithSorting(true);
                     cols.Add("EmplStartDt").WithHeaderText("Start Date")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.EmplStartDt.Value.ToString("d MMM yyyy"))
                         .WithSorting(true);
                     cols.Add("CostCtrDesc").WithHeaderText("Cost Centre")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.CostCtrDesc)
                         .WithSorting(true);
                     cols.Add("ReqStsDesc").WithHeaderText("Request Status")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.ReqStsDesc)
                         .WithSorting(true);
                     cols.Add("RequestorName").WithHeaderText("Requested By")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.RequestorName)
                         .WithSorting(true);
                     cols.Add("RequestDt").WithHeaderText("Request Date")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.RequestDt.HasValue ? p.RequestDt.Value.ToString("d MMM yyyy") : string.Empty)
                         .WithSorting(true);
                     cols.Add("Url").WithVisibility(false)
                         .WithValueExpression((p, c) => c.UrlHelper.Action("detail", "demo", new { id = p.ReqId }));
                 })
                 .WithRetrieveDataMethod((context) =>
                 {
                     // Query your data here. Obey Ordering, paging and filtering paramters given in the context.QueryOptions.        
                     // Use Entity Framwork, a module from your IoC Container, or any other method.         
                     // Return QueryResult object containing                             IEnumerable<YouModelItem>  
                     var options = context.QueryOptions;

                     var dd = Common.Security.Sanitize(options.GetPageParameterString("StatusCode"));

                     WebAPIManager.WebAPIManager api = new WebAPIManager.WebAPIManager();
                     Entity.RequestListForAgent oRequestListForAgent = api.GetRequestListForHRAdmin(dd, Helper.Utility.GetCurrentUserId());
                     if (oRequestListForAgent == null)
                     {
                         oRequestListForAgent = new RequestListForAgent();
                         oRequestListForAgent.Rows = new List<RequestListForAgentRow>();
                     }
                     else
                     {
                         if (!String.IsNullOrWhiteSpace(options.SortColumnName))
                         {
                             switch (options.SortColumnName.ToLower())
                             {

                                 case "reqid":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.ReqId).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.ReqId).ToList();
                                     break;
                                 case "reqtypedesc":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.ReqTypeDesc).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.ReqTypeDesc).ToList();
                                     break;
                                 case "firstname":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.FirstName).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.FirstName).ToList();
                                     break;
                                 case "lastname":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.LastName).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.LastName).ToList();
                                     break;
                                 case "usertypedesc":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.UserTypeDesc).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.UserTypeDesc).ToList();
                                     break;
                                 case "usersubtypedesc":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.UserSubTypeDesc).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.UserSubTypeDesc).ToList();
                                     break;
                                 case "emplstartdt":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.EmplStartDt).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.EmplStartDt).ToList();
                                     break;
                                 case "costctrdesc":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.CostCtrDesc).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.CostCtrDesc).ToList();
                                     break;
                                 case "reqstsdesc":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.ReqStsDesc).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.ReqStsDesc).ToList();
                                     break;
                                 case "requestorname":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.RequestorName).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.RequestorName).ToList();
                                     break;
                                 case "requestdt":
                                     if (options.SortDirection == SortDirection.Asc)
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderBy(p => p.RequestDt).ToList();
                                     else
                                         oRequestListForAgent.Rows = oRequestListForAgent.Rows.OrderByDescending(p => p.RequestDt).ToList();
                                     break;
                             }
                         }
                     }
                     return new QueryResult<RequestListForAgentRow>()
                     {
                         Items = oRequestListForAgent.Rows.Skip(options.PageIndex.Value * options.ItemsPerPage.Value).Take(options.ItemsPerPage.Value),
                         TotalRecords = oRequestListForAgent.Rows.Count // if paging is enabled, return the total number of records of all pages        
                     };
                 })
             );

        }

    }

}
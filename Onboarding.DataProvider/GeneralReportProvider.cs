using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding;

namespace Onboarding.DataProvider
{
    public class GeneralReportProvider
    {

        public Entity.GenRptSearchResult GetReport(Entity.RptSearchCriteria searchCriteria, string userId)
        {
            DataProvider.Model.OnboardingEntities obCtx = new Model.OnboardingEntities();
            RequestTypeProvider reqTypeProv = new RequestTypeProvider();
            UserTypeProvider userTypeProv = new UserTypeProvider();
            UserSubTypeProvider userSubTypeProv = new UserSubTypeProvider();
            CostCentreProvider costCtrProv = new CostCentreProvider();
            RequestStatusProvider reqStsProv = new RequestStatusProvider();
            RequestorProvider requestorProv = new RequestorProvider();
            UserProfileProvider usrProfileProv = new UserProfileProvider();
            Entity.UserProfile usr = usrProfileProv.GetUserProfile(userId);

            bool accessAllCostCtr = Utility.HasPermission(userId, Entity.Constant.AppResource.AccessAllCostCentre);
            string usrCostCtrList = Utility.GetAllowedCostCentreString(userId);

            bool accessAllUserType = Utility.HasPermission(userId, Entity.Constant.AppResource.AccessAllUserType);
            string userTypeList = Utility.GetAllowedUserTypeString(userId);

            Entity.GenRptSearchResult searchResult = null;

            if (searchCriteria.EmplStartDtFrom == DateTime.MinValue)
                searchCriteria.EmplStartDtFrom = null;
            if (searchCriteria.EmplStartDtTo == DateTime.MinValue)
                searchCriteria.EmplStartDtTo = null;
            if (searchCriteria.EmplEndDtFrom == DateTime.MinValue)
                searchCriteria.EmplEndDtFrom = null;
            if (searchCriteria.EmplEndDtTo == DateTime.MinValue)
                searchCriteria.EmplEndDtTo = null;
            if (searchCriteria.ReqDtFrom == DateTime.MinValue)
                searchCriteria.ReqDtFrom = null;
            if (searchCriteria.ReqDtTo == DateTime.MinValue)
                searchCriteria.ReqDtTo = null;
            if (searchCriteria.ReqId == 0)
                searchCriteria.ReqId = null;

            DateTime? reqDtTo = null;
            if (searchCriteria.ReqDtTo != null)
            {
                reqDtTo = searchCriteria.ReqDtTo.Value.AddDays(1);
            }

            var dbSearchResult = from a in obCtx.V_Req
                                 join x in obCtx.V_ReqEmp
                                     on a.ReqId equals x.ReqId into y
                                 from b in y.DefaultIfEmpty()
                                 where
                                        (
                                            (
                                                ((accessAllCostCtr == true) || (usrCostCtrList.Contains("|" + b.CostCtrCd.Trim() + "|"))) &&
                                                ((accessAllUserType == true) || (userTypeList.Contains("|" + b.UserTypeCd.Trim() + "|")))
                                            ) 
                                        ) &&
                                        ((a.ReqStsCd != Entity.Constant.RequestStatusCode.Draft) || ((a.ReqStsCd == Entity.Constant.RequestStatusCode.Draft) && (a.CreatedBy == userId))) &&
                                        ((searchCriteria.FirstName == null) || (searchCriteria.FirstName.Trim() == "") || (a.FirstNm.Contains(searchCriteria.FirstName.Trim()))) &&
                                        ((searchCriteria.LastName == null) || (searchCriteria.LastName.Trim() == "") || (a.LastNm.Contains(searchCriteria.LastName.Trim()))) &&
                                        ((searchCriteria.UserTypeCd == null) || (searchCriteria.UserTypeCd.Trim() == "") || (b.UserTypeCd == searchCriteria.UserTypeCd)) &&
                                        ((searchCriteria.UserSubTypeCd == null) || (searchCriteria.UserSubTypeCd.Trim() == "") || (b.UserSubTypeCd == searchCriteria.UserSubTypeCd)) &&
                                        ((searchCriteria.EmplStartDtFrom == null) || (b.EmpStartDt >= searchCriteria.EmplStartDtFrom)) &&
                                        ((searchCriteria.EmplStartDtTo == null) || (b.EmpStartDt <= searchCriteria.EmplStartDtTo)) &&
                                        ((searchCriteria.EmplEndDtFrom == null) || (b.EmpEndDt >= searchCriteria.EmplEndDtFrom)) &&
                                        ((searchCriteria.EmplEndDtTo == null) || (b.EmpEndDt <= searchCriteria.EmplEndDtTo)) &&
                                        ((searchCriteria.CostCtrCd == null) || (searchCriteria.CostCtrCd.Trim() == "") || (b.CostCtrCd == searchCriteria.CostCtrCd)) &&
                                        ((searchCriteria.ReqDtFrom == null) || (a.CreatedDt >= searchCriteria.ReqDtFrom)) &&
                                        ((searchCriteria.ReqDtTo == null) || (a.CreatedDt < reqDtTo)) &&
                                        ((searchCriteria.RequestorUserId == null) || (searchCriteria.RequestorUserId.Trim() == "") || (a.SubmittedBy == searchCriteria.RequestorUserId.Trim())) &&
                                        ((!searchCriteria.ReqId.HasValue) || (a.ReqId == searchCriteria.ReqId)) &&
                                        ((searchCriteria.ReqStsCd == null) || (searchCriteria.ReqStsCd.Trim() == "") || (a.ReqStsCd == searchCriteria.ReqStsCd)) &&
                                        ((searchCriteria.ReqTypeCd == null) || (searchCriteria.ReqTypeCd.Trim() == "") || (a.ReqTypeCd == searchCriteria.ReqTypeCd))
                                 orderby b.EmpStartDt ascending, a.ReqId ascending
                                 select new
                                 {
                                     a.ReqId,
                                     a.ReqTypeCd,
                                     a.FirstNm,
                                     a.LastNm,
                                     a.ReqStsCd,
                                     a.SubmittedBy,
                                     a.SubmittedDt,
                                     b.UserTypeCd,
                                     b.UserSubTypeCd,
                                     b.EmpStartDt,
                                     b.EmpEndDt,
                                     b.CostCtrCd
                                 };
            
            if (dbSearchResult.Count() > 0)
            {
                searchResult = new Entity.GenRptSearchResult();
                searchResult.Rows = new List<Entity.GenRptSearchResultRow>();
                foreach (var dbRow in dbSearchResult)
                {
                    Entity.GenRptSearchResultRow row = new Entity.GenRptSearchResultRow();
                    row.ReqId = dbRow.ReqId;
                    row.ReqTypeDesc = reqTypeProv.GetRequestTypeDesc(dbRow.ReqTypeCd);
                    row.FirstName = dbRow.FirstNm;
                    row.LastName = dbRow.LastNm;
                    row.UserTypeDesc = userTypeProv.GetUserTypeDesc(dbRow.UserTypeCd);
                    row.UserSubTypeDesc = userSubTypeProv.GetUserSubTypeDesc(dbRow.UserSubTypeCd);
                    row.EmplStartDt = dbRow.EmpStartDt;
                    row.EmplEndDt = dbRow.EmpEndDt;
                    row.CostCtrDesc = costCtrProv.GetCostCentreDesc(dbRow.CostCtrCd);
                    row.ReqStsDesc = reqStsProv.GetRequestStatusDesc(dbRow.ReqStsCd);
                    row.RequestorName = requestorProv.GetRequestorName(dbRow.SubmittedBy);
                    row.RequestDt = dbRow.SubmittedDt;
                    row.IsUrgent = Onboarding.Common.Validation.IsUrgentRequest(dbRow.ReqStsCd, dbRow.EmpStartDt);
                    searchResult.Rows.Add(row);
                }
            }
            return searchResult;
        }
        
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding;

namespace Onboarding.DataProvider
{
    public class SearchRequestProvider
    {

        public Entity.RequestSearchResult SearchRequest(Entity.RptSearchCriteria searchCriteria, string userId)
        {
            DataProvider.Model.OnboardingEntities obCtx = new Model.OnboardingEntities();
            CostCentreProvider costCtrProv = new CostCentreProvider();
            RequestorProvider requestorProv = new RequestorProvider();

            bool accessAllUserType = Utility.HasPermission(userId, Entity.Constant.AppResource.AccessAllUserType);
            string userTypeList = Utility.GetAllowedUserTypeString(userId);

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

            Entity.RequestSearchResult searchResult = null;

            var dbSearchResult = from a in obCtx.V_Req
                                 join x in obCtx.V_ReqEmp
                                     on a.ReqId equals x.ReqId into y
                                 from b in y.DefaultIfEmpty()
                                 where (
                                            (
                                                ((accessAllUserType == true) || (userTypeList.Contains("|" + b.UserTypeCd.Trim() + "|")))
                                            )
                                        ) &&
                                        ((a.ReqStsCd != Entity.Constant.RequestStatusCode.Draft) || ((a.ReqStsCd == Entity.Constant.RequestStatusCode.Draft) && (a.CreatedBy == userId))) &&
                                        ((searchCriteria.FirstName == null) || (searchCriteria.FirstName.Trim() == "") || (a.FirstNm.Contains(searchCriteria.FirstName.Trim()))) &&
                                        ((searchCriteria.LastName == null) || (searchCriteria.LastName.Trim() == "") || (a.LastNm.Contains(searchCriteria.LastName.Trim()))) &&
                                        ((searchCriteria.EmplStartDtFrom == null) || (b.EmpStartDt >= searchCriteria.EmplStartDtFrom)) &&
                                        ((searchCriteria.EmplStartDtTo == null) || (b.EmpStartDt <= searchCriteria.EmplStartDtTo)) &&
                                        ((searchCriteria.EmplEndDtFrom == null) || (b.EmpEndDt >= searchCriteria.EmplEndDtFrom)) &&
                                        ((searchCriteria.EmplEndDtTo == null) || (b.EmpEndDt <= searchCriteria.EmplEndDtTo)) &&
                                        ((searchCriteria.CostCtrCd == null) || (searchCriteria.CostCtrCd.Trim() == "") || (b.CostCtrCd == searchCriteria.CostCtrCd))
                                 select new
                                 {
                                     a.ReqId,
                                     a.FirstNm,
                                     a.LastNm,
                                     b.EmpStartDt,
                                     b.EmpEndDt,
                                     b.CostCtrCd,
                                     a.SubmittedBy,
                                     a.SubmittedDt
                                 };
            
            if (dbSearchResult.Count() > 0)
            {
                searchResult = new Entity.RequestSearchResult();
                searchResult.Rows = new List<Entity.RequestSearchResultRow>();
                foreach (var dbRow in dbSearchResult)
                {
                    Entity.RequestSearchResultRow row = new Entity.RequestSearchResultRow();
                    row.ReqId = dbRow.ReqId;
                    row.FirstName = dbRow.FirstNm;
                    row.LastName = dbRow.LastNm;
                    row.EmplStartDt = dbRow.EmpStartDt;
                    row.EmplEndDt = dbRow.EmpEndDt;
                    row.CostCtrDesc = costCtrProv.GetCostCentreDesc(dbRow.CostCtrCd);
                    row.RequestorName = requestorProv.GetRequestorName(dbRow.SubmittedBy);
                    row.RequestDt = dbRow.SubmittedDt;
                    searchResult.Rows.Add(row);
                }
            }
            return searchResult;
        }
        
    }
}

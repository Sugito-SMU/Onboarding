using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding;

namespace Onboarding.DataProvider
{
    public class RequestListForRequestorProvider
    {

        public Entity.RequestListForRequestor GetRequestListForRequestor(string requestStatusCd, string userId)
        {
            DataProvider.Model.OnboardingEntities obCtx = new Model.OnboardingEntities();
            RequestTypeProvider reqTypeProv = new RequestTypeProvider();
            UserTypeProvider userTypeProv = new UserTypeProvider();
            UserSubTypeProvider userSubTypeProv = new UserSubTypeProvider();
            RequestStatusProvider reqStsProv = new RequestStatusProvider();
            UserProfileProvider usrProfileProv = new UserProfileProvider();
            CostCentreProvider costCtrProv = new CostCentreProvider();
            RequestorProvider requestorProv = new RequestorProvider();

            bool accessAllCostCtr = Utility.HasPermission(userId, Entity.Constant.AppResource.AccessAllCostCentre);
            string usrCostCtrList = Utility.GetAllowedCostCentreString(userId);

            bool accessAllUserType = Utility.HasPermission(userId, Entity.Constant.AppResource.AccessAllUserType);
            string userTypeList = Utility.GetAllowedUserTypeString(userId);

            Entity.RequestListForRequestor reqList = null;

            var dbQuery = from a in obCtx.V_Req
                          join b in obCtx.V_ReqEmp
                             on a.ReqId equals b.ReqId into c
                          from d in c.DefaultIfEmpty()
                          where (a.ReqStsCd == requestStatusCd) &&
                                (
                                  (
                                    (a.ReqStsCd == Entity.Constant.RequestStatusCode.Draft) &&
                                    (
                                      ((accessAllCostCtr == true) && (a.CreatedBy == userId)) ||
                                      ((accessAllCostCtr == false) && (d == null) && (a.CreatedBy == userId)) ||
                                      ((accessAllCostCtr == false) && (d != null) && (string.IsNullOrEmpty(d.CostCtrCd)) && (a.CreatedBy == userId)) ||
                                      ((accessAllCostCtr == false) && (d != null) && (!string.IsNullOrEmpty(d.CostCtrCd))) && (usrCostCtrList.Contains("|" + d.CostCtrCd.Trim() + "|"))
                                    )
                                  )
                                  ||
                                  (
                                    (a.ReqStsCd != Entity.Constant.RequestStatusCode.Draft) &&
                                    (d != null) &&
                                    ((accessAllCostCtr == true) || (usrCostCtrList.Contains("|" + d.CostCtrCd.Trim() + "|"))) &&
                                    ((accessAllUserType == true) || (userTypeList.Contains("|" + d.UserTypeCd.Trim() + "|")))
                                  )
                                )
                          select new
                          {
                              a.ReqId,
                              a.ReqTypeCd,
                              a.FirstNm,
                              a.LastNm,
                              a.ReqStsCd,
                              a.SubmittedBy,
                              a.SubmittedDt,
                              d.UserTypeCd,
                              d.UserSubTypeCd,
                              d.EmpStartDt,
                              d.CostCtrCd,
                          };

            if (dbQuery.Count() > 0)
            {
                reqList = new Entity.RequestListForRequestor();
                reqList.Rows = new List<Entity.RequestListForRequestorRow>();
                foreach (var dbRow in dbQuery)
                {
                    Entity.RequestListForRequestorRow row = new Entity.RequestListForRequestorRow();
                    row.ReqId = dbRow.ReqId;
                    row.ReqTypeDesc = reqTypeProv.GetRequestTypeDesc(dbRow.ReqTypeCd);
                    row.FirstName = dbRow.FirstNm;
                    row.LastName = dbRow.LastNm;
                    row.UserTypeDesc = userTypeProv.GetUserTypeDesc(dbRow.UserTypeCd);
                    row.UserSubTypeDesc = userSubTypeProv.GetUserSubTypeDesc(dbRow.UserSubTypeCd);
                    row.EmplStartDt = dbRow.EmpStartDt;
                    row.CostCtrDesc = costCtrProv.GetCostCentreDesc(dbRow.CostCtrCd);
                    row.ReqStsDesc = reqStsProv.GetRequestStatusDesc(dbRow.ReqStsCd);
                    row.RequestorName = requestorProv.GetRequestorName(dbRow.SubmittedBy);
                    row.RequestDt = dbRow.SubmittedDt;
                    reqList.Rows.Add(row);
                }
            }
            return reqList;
        }
        
    }
}

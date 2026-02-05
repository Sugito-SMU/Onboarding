using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding;
using Onboarding.Entity;

namespace Onboarding.DataProvider
{
    public class RequestCountForRequestorProvider
    {
        public List<Entity.RequestCountForRequestor> GetRequestCountForRequestor(string userId)
        {
            RequestStatusProvider reqStatusProv = new RequestStatusProvider();
            List<Entity.RequestCountForRequestor> reqCountList = new List<RequestCountForRequestor>();

            bool accessAllCostCtr = Utility.HasPermission(userId, Entity.Constant.AppResource.AccessAllCostCentre);
            string usrCostCtrList = Utility.GetAllowedCostCentreString(userId);

            bool accessAllUserType = Utility.HasPermission(userId, Entity.Constant.AppResource.AccessAllUserType);
            string userTypeList = Utility.GetAllowedUserTypeString(userId);

            using (Model.OnboardingEntities obCtx = new Model.OnboardingEntities())
            {
                var query = from a in obCtx.V_Req
                            join b in obCtx.V_ReqEmp
                                on a.ReqId equals b.ReqId into c
                            from d in c.DefaultIfEmpty()
                            where
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
                            group a by a.ReqStsCd into g
                            select new
                            {
                                g.Key,
                                ReqCount = g.Count()
                            };
                foreach (var item in query)
                {
                    RequestCountForRequestor rc = new RequestCountForRequestor();
                    rc.ReqStsCd = item.Key;
                    rc.ReqStsDesc = reqStatusProv.GetRequestStatusDesc(item.Key);
                    rc.ReqCount = item.ReqCount;
                    reqCountList.Add(rc);
                }
            }
            return reqCountList;
        }
    }
}

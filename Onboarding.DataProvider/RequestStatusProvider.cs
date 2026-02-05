using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding;
using Onboarding.Entity;

namespace Onboarding.DataProvider
{
    public class RequestStatusProvider
    {
        public List<Entity.RequestStatus> GetRequestStatusAll()
        {
            List<Entity.RequestStatus> reqStsList = (List<Entity.RequestStatus>)MCache.Get("RequestStatus");
            if (reqStsList == null)
            {
                reqStsList = new List<Entity.RequestStatus>();
                using (Model.OnboardingEntities obCtx = new Model.OnboardingEntities())
                {
                    List<Model.V_ReqStsCd> dbReqStsList = obCtx.V_ReqStsCd.ToList<Model.V_ReqStsCd>();
                    if (dbReqStsList != null)
                    {
                        foreach (Model.V_ReqStsCd dbReqStsCd in dbReqStsList)
                        {
                            Entity.RequestStatus reqSts = new Entity.RequestStatus();
                            Mapper.MapToEntity.Map(dbReqStsCd, reqSts);
                            reqStsList.Add(reqSts);
                        }
                    }
                }
                MCache.Set("RequestStatus", reqStsList);
            }
            return reqStsList;
        }

        public string GetRequestStatusDesc(string reqStsCd)
        {
            if (!string.IsNullOrWhiteSpace(reqStsCd))
            {
                List<Entity.RequestStatus> reqStatusList = GetRequestStatusAll();
                Entity.RequestStatus reqSts = reqStatusList.FirstOrDefault(x => x.ReqStsCd.Trim().ToLower() == reqStsCd.Trim().ToLower());
                if (reqSts != null)
                {
                    return reqSts.ReqStsDesc;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }


    }
}

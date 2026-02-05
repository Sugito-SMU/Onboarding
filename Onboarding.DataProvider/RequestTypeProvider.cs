using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding;
using Onboarding.Entity;

namespace Onboarding.DataProvider
{
    public class RequestTypeProvider
    {
        public List<Entity.RequestType> GetRequestTypeAll()
        {
            List<Entity.RequestType> requestTypeList = (List<Entity.RequestType>)MCache.Get("RequestType");
            if (requestTypeList == null)
            {
                requestTypeList = new List<Entity.RequestType>();
                using (Model.OnboardingEntities obCtx = new Model.OnboardingEntities())
                {
                    List<Model.V_ReqTypeCd> dbRequestTypeList = obCtx.V_ReqTypeCd.ToList<Model.V_ReqTypeCd>();
                    if (dbRequestTypeList != null)
                    {
                        foreach (Model.V_ReqTypeCd dbReqTypeCd in dbRequestTypeList)
                        {
                            Entity.RequestType reqType = new Entity.RequestType();
                            Mapper.MapToEntity.Map(dbReqTypeCd, reqType);
                            requestTypeList.Add(reqType);
                        }
                    }
                }
                MCache.Set("RequestType", requestTypeList);
            }
            return requestTypeList;
        }

        public string GetRequestTypeDesc(string reqTypeCd)
        {
            if (!string.IsNullOrWhiteSpace(reqTypeCd))
            {
                List<Entity.RequestType> requestTypeList = GetRequestTypeAll();
                Entity.RequestType requestType = requestTypeList.FirstOrDefault(x => x.ReqTypeCd.Trim().ToLower() == reqTypeCd.Trim().ToLower());
                if (requestType != null)
                {
                    return requestType.ReqTypeDesc;
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

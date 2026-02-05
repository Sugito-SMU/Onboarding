using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding;
using Onboarding.Entity;

namespace Onboarding.DataProvider
{
    public class RequestAttributeCodeProvider
    {
        public List<RequestAttributeCode> GetRequestAttributeCodeAll()
        {
            List<RequestAttributeCode> taskAttributeCodeList = (List<RequestAttributeCode>)MCache.Get("RequestAttributeCode");
            if (taskAttributeCodeList == null)
            {
                taskAttributeCodeList = new List<RequestAttributeCode>();
                using (Model.OnboardingEntities obCtx = new Model.OnboardingEntities())
                {
                    List<Model.V_ReqAttrbCd> dbRequestAttributeCodeList = obCtx.V_ReqAttrbCd.ToList<Model.V_ReqAttrbCd>();
                    if (dbRequestAttributeCodeList != null)
                    {
                        foreach (Model.V_ReqAttrbCd dbRequestAttrbCd in dbRequestAttributeCodeList)
                        {
                            RequestAttributeCode RequestAttrbCd = new RequestAttributeCode();
                            Mapper.MapToEntity.Map(dbRequestAttrbCd, RequestAttrbCd);
                            taskAttributeCodeList.Add(RequestAttrbCd);
                        }
                    }
                }
                MCache.Set("RequestAttributeCode", taskAttributeCodeList);
            }
            return taskAttributeCodeList;
        }

        public string GetRequestAttributeCodeDesc(string RequestAttrbCd)
        {
            if (!string.IsNullOrWhiteSpace(RequestAttrbCd))
            {
                List<RequestAttributeCode> RequestAttributeCodeList = GetRequestAttributeCodeAll();
                RequestAttributeCode RequestAttributeCode = RequestAttributeCodeList.FirstOrDefault(x => x.RequestAttrbCd.Trim().ToLower() == RequestAttrbCd.Trim().ToLower());
                if (RequestAttributeCode != null)
                {
                    return RequestAttributeCode.RequestAttrbDesc;
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

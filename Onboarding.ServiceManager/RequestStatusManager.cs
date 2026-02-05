using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding.DataProvider;
using Onboarding.DataProvider.Model;

namespace Onboarding.ServiceManager
{
    public class RequestStatusManager
    {
        public List<Entity.RequestStatus> GetRequestStatusAll()
        {
            RequestStatusProvider reqStatusProvider = new RequestStatusProvider();
            List<Entity.RequestStatus> reqStatusList = reqStatusProvider.GetRequestStatusAll();
            return reqStatusList.Where(x => x.IsActive == true).ToList();
        }

        public string GetRequestStatusDesc(string RequestCd)
        {
            RequestStatusProvider reqStatusProvider = new RequestStatusProvider();
            string reqStatusDesc = reqStatusProvider.GetRequestStatusDesc(RequestCd);
            return reqStatusDesc;
        }

    }
}

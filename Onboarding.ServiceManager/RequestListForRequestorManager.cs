using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding.DataProvider;
using Onboarding.DataProvider.Model;

namespace Onboarding.ServiceManager
{
    public class RequestListForRequestorManager
    {
        public Entity.RequestListForRequestor GetRequestListForRequestor(string requestStatusCd, string userId)
        {
            RequestListForRequestorProvider reqListProv = new RequestListForRequestorProvider();
            Entity.RequestListForRequestor reqList = reqListProv.GetRequestListForRequestor(requestStatusCd, userId);
            return reqList;
        }
        
    }
}

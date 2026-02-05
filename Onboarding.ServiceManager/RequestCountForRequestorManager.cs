using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding.DataProvider;
using Onboarding.DataProvider.Model;

namespace Onboarding.ServiceManager
{
    public class RequestCountForRequestorManager
    {
        public List<Entity.RequestCountForRequestor> GetRequestCountForRequestor(string userId)
        {
            RequestCountForRequestorProvider reqCountProvider = new RequestCountForRequestorProvider();
            List<Entity.RequestCountForRequestor> reqCountList = reqCountProvider.GetRequestCountForRequestor(userId);
            return reqCountList;
        }
    }
}

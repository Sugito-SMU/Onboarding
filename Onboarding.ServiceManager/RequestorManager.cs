using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding.DataProvider;
using Onboarding.DataProvider.Model;

namespace Onboarding.ServiceManager
{
    public class RequestorManager
    {
        public List<Entity.Requestor> GetRequestorList(string userId)
        {
            RequestorProvider requestorListProvider = new RequestorProvider();
            List<Entity.Requestor> requestorList = requestorListProvider.GetRequestorList(userId);
            return requestorList;
        }
    }
}

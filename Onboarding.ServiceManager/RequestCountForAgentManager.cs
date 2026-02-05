using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding.DataProvider;
using Onboarding.DataProvider.Model;

namespace Onboarding.ServiceManager
{
    public class RequestCountForAgentManager
    {
        public List<Entity.RequestCountForAgent> GetRequestCountForAgent(string userId)
        {
            RequestCountForAgentProvider reqCountProvider = new RequestCountForAgentProvider();
            List<Entity.RequestCountForAgent> reqCountList = reqCountProvider.GetRequestCountForAgent(userId);
            return reqCountList;
        }
    }
}

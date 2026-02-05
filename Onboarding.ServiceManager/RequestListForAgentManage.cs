using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding.DataProvider;
using Onboarding.DataProvider.Model;

namespace Onboarding.ServiceManager
{
    public class RequestListForAgentManager
    {
        public Entity.RequestListForAgent GetRequestListForAgent(string taskStatusCd, string userId)
        {
            RequestListForAgentProvider reqListProv = new RequestListForAgentProvider();
            Entity.RequestListForAgent reqList = reqListProv.GetRequestListForAgent(taskStatusCd, userId);
            return reqList;
        }
        
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding.DataProvider;
using Onboarding.DataProvider.Model;

namespace Onboarding.ServiceManager
{
    public class RequestTypeManager
    {
        public List<Entity.RequestType> GetRequestTypeAll()
        {
            RequestTypeProvider requestTypeProvider = new RequestTypeProvider();
            List<Entity.RequestType> requestTypeList = requestTypeProvider.GetRequestTypeAll();
            return requestTypeList.Where(x => x.IsActive == true).OrderBy(x => x.Sequence).ToList() ;
        }

        public string GetRequestTypeDesc(string requestTypeCd)
        {
            RequestTypeProvider requestTypeProvider = new RequestTypeProvider();
            string requestTypeDesc = requestTypeProvider.GetRequestTypeDesc(requestTypeCd);
            return requestTypeDesc;
        }

    }
}

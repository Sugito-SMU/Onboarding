using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding.DataProvider;
using Onboarding.DataProvider.Model;

namespace Onboarding.ServiceManager
{
    public class SearchRequestManager
    {
        public Entity.RequestSearchResult SearchRequest(Entity.RptSearchCriteria searchCriteria, string userId)
        {
            SearchRequestProvider requestSearchProv = new SearchRequestProvider();
            Entity.RequestSearchResult searchResult = requestSearchProv.SearchRequest(searchCriteria, userId);
            return searchResult;
        }
        
    }
}

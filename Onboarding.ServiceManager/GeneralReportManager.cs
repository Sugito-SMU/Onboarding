using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding.DataProvider;
using Onboarding.DataProvider.Model;

namespace Onboarding.ServiceManager
{
    public class GeneralReportManager
    {
        public Entity.GenRptSearchResult GetReport(Entity.RptSearchCriteria searchCriteria, string userId)
        {
            GeneralReportProvider genReportProv = new GeneralReportProvider();
            Entity.GenRptSearchResult genRpt = genReportProv.GetReport(searchCriteria, userId);
            return genRpt;
        }
        
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding.DataProvider;
using Onboarding.DataProvider.Model;

namespace Onboarding.ServiceManager
{
    public class CostCentreManager
    {
        public List<Entity.CostCentre> GetCostCentreAll()
        {
            CostCentreProvider costCentreProvider = new CostCentreProvider();
            List<Entity.CostCentre> costCentreList = costCentreProvider.GetCostCentreAll();
            return costCentreList.Where(x => x.IsActive == true).OrderBy(x => x.CostCtrDesc).ToList();
        }

        public string GetCostCentreDesc(string costCentreCd)
        {
            CostCentreProvider costCentreProvider = new CostCentreProvider();
            string costCentreDesc = costCentreProvider.GetCostCentreDesc(costCentreCd);
            return costCentreDesc;
        }

    }
}

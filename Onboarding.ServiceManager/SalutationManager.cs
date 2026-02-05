using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding.DataProvider;
using Onboarding.DataProvider.Model;

namespace Onboarding.ServiceManager
{
    public class SalutationManager
    {
        public List<Entity.Salutation> GetSalutationAll()
        {
            SalutationProvider salutationProvider = new SalutationProvider();
            List<Entity.Salutation> salutationList = salutationProvider.GetSalutationAll();
            return salutationList.Where(x => x.IsActive == true).ToList();
        }

        public string GetSalutationDesc(string salutationCd)
        {
            SalutationProvider salutationProvider = new SalutationProvider();
            string salutationDesc = salutationProvider.GetSalutationDesc(salutationCd);
            return salutationDesc;
        }

    }
}

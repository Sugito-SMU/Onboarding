using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding.DataProvider;
using Onboarding.DataProvider.Model;

namespace Onboarding.ServiceManager
{
    public class GenderManager
    {
        public List<Entity.Gender> GetGenderAll()
        {
            GenderProvider genderProvider = new GenderProvider();
            List<Entity.Gender> genderList = genderProvider.GetGenderAll();
            return genderList.Where(x => x.IsActive == true).ToList();
        }

        public string GetGenderDesc(string genderCd)
        {
            GenderProvider genderProvider = new GenderProvider();
            string genderDesc = genderProvider.GetGenderDesc(genderCd);
            return genderDesc;
        }

    }
}

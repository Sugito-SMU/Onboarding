using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding.DataProvider;
using Onboarding.DataProvider.Model;

namespace Onboarding.ServiceManager
{
    public class UserTypeManager
    {
        public List<Entity.UserType> GetUserTypeAll()
        {
            UserTypeProvider userTypeProvider = new UserTypeProvider();
            List<Entity.UserType> userTypeList = userTypeProvider.GetUserTypeAll();
            return userTypeList.Where(x => x.IsActive == true).OrderBy(x => x.Sequence).ToList();
        }

        public string GetUserTypeDesc(string userTypeCd)
        {
            UserTypeProvider userTypeProvider = new UserTypeProvider();
            string userTypeDesc = userTypeProvider.GetUserTypeDesc(userTypeCd);
            return userTypeDesc;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding.DataProvider;
using Onboarding.DataProvider.Model;

namespace Onboarding.ServiceManager
{
    public class UserSubTypeManager
    {
        public List<Entity.UserSubType> GetUserSubTypeAll()
        {
            UserSubTypeProvider userSubTypeProvider = new UserSubTypeProvider();
            List<Entity.UserSubType> userSubTypeList = userSubTypeProvider.GetUserSubTypeAll();
            return userSubTypeList.Where(x => x.IsActive == true).OrderBy(x => x.Sequence).ToList();
        }

        public string GetUserSubTypeDesc(string userSubTypeCd)
        {
            UserSubTypeProvider userSubTypeProvider = new UserSubTypeProvider();
            string userSubTypeDesc = userSubTypeProvider.GetUserSubTypeDesc(userSubTypeCd);
            return userSubTypeDesc;
        }

    }
}

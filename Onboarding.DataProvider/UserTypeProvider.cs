using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding;
using Onboarding.Entity;

namespace Onboarding.DataProvider
{
    public class UserTypeProvider
    {
        public List<Entity.UserType> GetUserTypeAll()
        {
            List<Entity.UserType> userTypeList = (List<Entity.UserType>)MCache.Get("UserType");
            if (userTypeList == null)
            {
                userTypeList = new List<Entity.UserType>();
                using (Model.OnboardingEntities obCtx = new Model.OnboardingEntities())
                {
                    List<Model.V_UserTypeCd> dbUserTypeList = obCtx.V_UserTypeCd.ToList<Model.V_UserTypeCd>();
                    if (dbUserTypeList != null)
                    {
                        foreach (Model.V_UserTypeCd dbUserTypeCd in dbUserTypeList)
                        {
                            Entity.UserType userType = new Entity.UserType();
                            Mapper.MapToEntity.Map(dbUserTypeCd, userType);
                            userTypeList.Add(userType);
                        }
                    }
                }
                MCache.Set("UserType", userTypeList);
            }
            return userTypeList;
        }

        public string GetUserTypeDesc(string userTypeCd)
        {
            if (!string.IsNullOrWhiteSpace(userTypeCd))
            {
                List<Entity.UserType> userTypeList = GetUserTypeAll();
                Entity.UserType userType = userTypeList.FirstOrDefault(x => x.UserTypeCd.Trim().ToLower() == userTypeCd.Trim().ToLower());
                if (userType != null)
                {
                    return userType.UserTypeDesc;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }


    }
}

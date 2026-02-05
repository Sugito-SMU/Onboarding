using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding;
using Onboarding.Entity;

namespace Onboarding.DataProvider
{
    public class UserSubTypeProvider
    {
        public List<Entity.UserSubType> GetUserSubTypeAll()
        {
            List<Entity.UserSubType> userSubTypeList = (List<Entity.UserSubType>)MCache.Get("UserSubType");
            if (userSubTypeList == null)
            {
                userSubTypeList = new List<Entity.UserSubType>();
                using (Model.OnboardingEntities obCtx = new Model.OnboardingEntities())
                {
                    List<Model.V_UserSubTypeCd> dbUserSubTypeList = obCtx.V_UserSubTypeCd.ToList<Model.V_UserSubTypeCd>();
                    if (dbUserSubTypeList != null)
                    {
                        foreach (Model.V_UserSubTypeCd dbUserSubTypeCd in dbUserSubTypeList)
                        {
                            Entity.UserSubType userSubType = new Entity.UserSubType();
                            Mapper.MapToEntity.Map(dbUserSubTypeCd, userSubType);
                            userSubTypeList.Add(userSubType);
                        }
                    }
                }
                MCache.Set("UserSubType", userSubTypeList);
            }
            return userSubTypeList;
        }

        public string GetUserSubTypeDesc(string userSubTypeCd)
        {
            if (!string.IsNullOrWhiteSpace(userSubTypeCd))
            {
                List<Entity.UserSubType> userSubTypeList = GetUserSubTypeAll();
                Entity.UserSubType userSubType = userSubTypeList.FirstOrDefault(x => x.UserSubTypeCd.Trim().ToLower() == userSubTypeCd.Trim().ToLower());
                if (userSubType != null)
                {
                    return userSubType.UserSubTypeDesc;
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

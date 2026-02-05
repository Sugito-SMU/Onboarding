using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Onboarding.Common;
using Onboarding.Entity;

namespace Onboarding.DataProvider
{
    public class Utility
    {
        public static bool HasPermission(string userId, string resourceCd)
        {
            UserProfileProvider userProfProvider = new UserProfileProvider();
            UserProfile userProf = userProfProvider.GetUserProfile(userId);
            return AccessControl.HasPermission(userProf, resourceCd);
        }

        public static string GetAllowedCostCentreString(string userId)
        {
            UserProfileProvider userProfProvider = new UserProfileProvider();
            Entity.UserProfile usrProf = userProfProvider.GetUserProfile(userId);
            string usrCostCtrStr = AccessControl.GetAllowedCostCentreString(usrProf);
            return usrCostCtrStr;
        }

        public static string GetAllowedUserTypeString(string userId)
        {
            UserProfileProvider userProfProvider = new UserProfileProvider();
            Entity.UserProfile usrProf = userProfProvider.GetUserProfile(userId);
            string usrTypeStr = AccessControl.GetAllowedUserTypeString(usrProf);
            return usrTypeStr;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Onboarding.Common;
using Onboarding.ServiceManager;
using Onboarding.Entity;

namespace Onboarding.WebAPI
{
    public class Utility
    {
        public static bool HasPermission(string userId, string resourceCd)
        {
            UserProfileManager userProfMgr = new UserProfileManager();
            UserProfile userProf = userProfMgr.GetUserProfile(userId);
            return AccessControl.HasPermission(userProf, resourceCd);
        }

        public static bool HasPermission(string userId, Entity.Request req)
        {
            UserProfileManager userProfMgr = new UserProfileManager();
            UserProfile userProf = userProfMgr.GetUserProfile(userId);
            return AccessControl.HasPermission(userProf, req);
        }
    }
}
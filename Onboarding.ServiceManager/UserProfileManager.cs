using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding.DataProvider;
using Onboarding.DataProvider.Model;

namespace Onboarding.ServiceManager
{
    public class UserProfileManager
    {
        public List<Entity.UserProfile> GetUserProfileAll()
        {
            UserProfileProvider usrProfileProv = new UserProfileProvider();
            List<Entity.UserProfile> UserProfileList = usrProfileProv.GetUserProfileAll();
            return UserProfileList.Where(x => x.IsActive == true).ToList();
        }

        public Entity.UserProfile GetUserProfile(string userId)
        {
            UserProfileProvider usrProfileProv = new UserProfileProvider();
            Entity.UserProfile usrProfile = usrProfileProv.GetUserProfile(userId);
            return usrProfile;
        }

    }
}

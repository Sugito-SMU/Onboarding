using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding;
using Onboarding.Entity;

namespace Onboarding.DataProvider
{
    public class RequestorProvider
    {

        public List<Entity.Requestor> GetRequestorListAll()
        {
            UserProfileProvider usrProfileProv = new UserProfileProvider();

            List<Entity.Requestor> requestorList = (List<Entity.Requestor>)MCache.Get("RequestorList");
            if (requestorList == null)
            {
                requestorList = new List<Entity.Requestor>();
                using (Model.OnboardingEntities obCtx = new Model.OnboardingEntities())
                {
                    var dbRequestorList = from a in obCtx.V_Req
                                          select new { a.SubmittedBy };

                    foreach (var dbRequestor in dbRequestorList)
                    {
                        if (!string.IsNullOrWhiteSpace(dbRequestor.SubmittedBy))
                        {
                            if (requestorList.Where(x => x.RequestorUserId == dbRequestor.SubmittedBy.Trim().ToLower()).FirstOrDefault() == null)
                            {
                                Entity.UserProfile usrProfile = usrProfileProv.GetUserProfile(dbRequestor.SubmittedBy.Trim().ToLower());

                                Entity.Requestor requestor = new Requestor();
                                requestor.RequestorUserId = dbRequestor.SubmittedBy.Trim().ToLower();
                                requestor.RequestorName = usrProfile != null ? usrProfile.Name : null;
                                requestorList.Add(requestor);
                            }
                        }
                    }
                }
                MCache.Set("RequestorList", requestorList);
            }
            return requestorList;
        }

        public List<Entity.Requestor> GetRequestorList(string userId)
        {
            List<Entity.Requestor> requestorList = GetRequestorListAll();
            return requestorList;
        }

        public string GetRequestorName(string requestorUserId)
        {
            if (!string.IsNullOrWhiteSpace(requestorUserId))
            {
                List<Entity.Requestor> requestorList = GetRequestorListAll();
                Entity.Requestor requestor = requestorList.FirstOrDefault(x => x.RequestorUserId.Trim().ToLower() == requestorUserId.Trim().ToLower());
                if (requestor != null)
                {
                    return requestor.RequestorName;
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

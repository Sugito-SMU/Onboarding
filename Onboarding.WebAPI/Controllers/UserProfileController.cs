using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Onboarding.Entity;
using Onboarding.ServiceManager;

namespace Onboarding.WebAPI.Controllers
{
    public class UserProfileController : ApiController
    {

        [HttpGet]
        [Route("api/UserProfile/{id}")]
        [ResponseType(typeof(IEnumerable<TaskStatus>))]
        public IHttpActionResult Get(string id)
        {
            UserProfileManager mgr = new UserProfileManager();
            return Ok(mgr.GetUserProfile(id));
        }
    }
}

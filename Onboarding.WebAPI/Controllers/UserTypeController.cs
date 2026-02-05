using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Onboarding.ServiceManager;
using Onboarding.Entity;


namespace Onboarding.WebAPI.Controllers
{
    public class UserTypeController : ApiController
    {

        [HttpGet]
        [Route("api/UserType")]
        [ResponseType(typeof(IEnumerable<UserType>))]
        public IHttpActionResult Get()
        {
            UserTypeManager mgr = new UserTypeManager();
            List<UserType> list = mgr.GetUserTypeAll();
            return Ok(list.Where(x => x.IsActive == true));
        }

        [HttpGet]
        [Route("api/UserType/{id}")]
        [ResponseType(typeof(string))]
        public IHttpActionResult Get(string id)
        {
            UserTypeManager mgr = new UserTypeManager();
            return Ok(mgr.GetUserTypeDesc(id));
        }

    }
}

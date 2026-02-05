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
    public class UserSubTypeController : ApiController
    {

        [HttpGet]
        [Route("api/UserSubType")]
        [ResponseType(typeof(IEnumerable<UserSubType>))]
        public IHttpActionResult Get()
        {
            UserSubTypeManager mgr = new UserSubTypeManager();
            List<UserSubType> list = mgr.GetUserSubTypeAll();
            return Ok(list.Where(x => x.IsActive == true));
        }

        [HttpGet]
        [Route("api/UserSubType/{id}")]
        [ResponseType(typeof(string))]
        public IHttpActionResult Get(string id)
        {
            UserSubTypeManager mgr = new UserSubTypeManager();
            return Ok(mgr.GetUserSubTypeDesc(id));
        }

    }
}

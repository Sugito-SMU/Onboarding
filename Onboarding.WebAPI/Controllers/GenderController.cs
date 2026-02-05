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
    public class GenderController : ApiController
    {
        [HttpGet]
        [Route("api/Gender")]
        [ResponseType(typeof(IEnumerable<Gender>))]
        public IHttpActionResult Get()
        {
            GenderManager mgr = new GenderManager();
            List<Gender> list = mgr.GetGenderAll();
            return Ok(list.Where(x => x.IsActive == true));
        }

        [HttpGet]
        [Route("api/Gender/{id}")]
        [ResponseType(typeof(string))]
        public IHttpActionResult Get(string id)
        {
            GenderManager mgr = new GenderManager();
            return Ok(mgr.GetGenderDesc(id));
        }
    }
}

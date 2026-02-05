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
    public class SalutationController : ApiController
    {

        [HttpGet]
        [Route("api/Salutation")]
        [ResponseType(typeof(IEnumerable<Salutation>))]
        public IHttpActionResult Get()
        {
            SalutationManager mgr = new SalutationManager();
            List<Salutation> list = mgr.GetSalutationAll();
            return Ok(list.Where(x => x.IsActive == true));
        }

        [HttpGet]
        [Route("api/Salutation/{id}")]
        [ResponseType(typeof(string))]
        public IHttpActionResult Get(string id)
        {
            SalutationManager mgr = new SalutationManager();
            return Ok(mgr.GetSalutationDesc(id));
        }

    }
}

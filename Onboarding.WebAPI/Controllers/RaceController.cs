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
    public class RaceController : ApiController
    {

        [HttpGet]
        [Route("api/Race")]
        [ResponseType(typeof(IEnumerable<Race>))]
        public IHttpActionResult Get()
        {
            RaceManager mgr = new RaceManager();
            List<Race> list = mgr.GetRaceAll();
            return Ok(list.Where(x => x.IsActive == true));
        }

        [HttpGet]
        [Route("api/Race/{id}")]
        [ResponseType(typeof(string))]
        public IHttpActionResult Get(string id)
        {
            RaceManager mgr = new RaceManager();
            return Ok(mgr.GetRaceDesc(id));
        }

    }
}

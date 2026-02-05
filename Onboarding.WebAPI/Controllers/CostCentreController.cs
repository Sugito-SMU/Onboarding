using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Onboarding.ServiceManager;
using Onboarding.Entity;
using System.Web.Http.Description;

namespace Onboarding.WebAPI.Controllers
{
    public class CostCentreController : ApiController
    {
        [HttpGet]
        [Route("api/CostCentre")]
        [ResponseType(typeof(IEnumerable<CostCentre>))]
        public IHttpActionResult Get()
        {
            CostCentreManager mgr = new CostCentreManager();
            List<CostCentre> list = mgr.GetCostCentreAll();
            return Ok(list.Where(x => x.IsActive == true));
        }

        [HttpGet]
        [Route("api/CostCentre/{id}")]
        [ResponseType(typeof(string))]
        public IHttpActionResult Get(string id)
        {
            CostCentreManager mgr = new CostCentreManager();
            return Ok(mgr.GetCostCentreDesc(id));
        }
    }
}

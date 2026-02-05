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
    public class RequestStatusController : ApiController
    {

        [HttpGet]
        [Route("api/RequestStatus")]
        [ResponseType(typeof(IEnumerable<RequestStatus>))]
        public IHttpActionResult Get()
        {
            RequestStatusManager mgr = new RequestStatusManager();
            List<RequestStatus> list = mgr.GetRequestStatusAll();
            return Ok(list.Where(x => x.IsActive == true));
        }

        [HttpGet]
        [Route("api/RequestStatus/{id}")]
        [ResponseType(typeof(string))]
        public IHttpActionResult Get(string id)
        {
            RequestStatusManager mgr = new RequestStatusManager();
            return Ok(mgr.GetRequestStatusDesc(id));
        }
    }
}

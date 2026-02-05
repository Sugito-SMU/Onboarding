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
    public class RequestTypeController : ApiController
    {

        [HttpGet]
        [Route("api/RequestType")]
        [ResponseType(typeof(IEnumerable<RequestType>))]
        public IHttpActionResult Get()
        {
            RequestTypeManager mgr = new RequestTypeManager();
            List<RequestType> list = mgr.GetRequestTypeAll();
            return Ok(list.Where(x => x.IsActive == true));
        }

        [HttpGet]
        [Route("api/RequestType/{id}")]
        [ResponseType(typeof(string))]
        public IHttpActionResult Get(string id)
        {
            RequestTypeManager mgr = new RequestTypeManager();
            return Ok(mgr.GetRequestTypeDesc(id));
        }
    }
}

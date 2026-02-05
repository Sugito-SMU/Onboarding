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
    public class RequestorListController : ApiController
    {
        [HttpGet]
        [Route("api/RequestorList/{userId}")]
        [ResponseType(typeof(Request))]
        public IHttpActionResult Get(string userId)
        {
            if (Utility.HasPermission(userId, Entity.Constant.AppResource.ApiRequestorList))
            {
                RequestorManager mgr = new RequestorManager();
                List<Requestor> list = mgr.GetRequestorList(userId);
                return Ok(list);
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}

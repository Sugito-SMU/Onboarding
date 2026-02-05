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
    public class BulkRequestController : ApiController
    {
        [HttpPost]
        [Route("api/BulkRequest/{userId}")]
        [ResponseType(typeof(BulkRequestSubmitResult))]
        public IHttpActionResult Post([FromBody]BulkRequest bulkReq, string userId)
        {
            if (Utility.HasPermission(userId, Entity.Constant.AppResource.ApiBulkRequest))
            {
                BulkRequestManager om = new BulkRequestManager();
                return Ok(om.SubmitBulkRequest(bulkReq, userId));
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}

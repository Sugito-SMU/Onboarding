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
    public class RequestAttachmentController : ApiController
    {

        [HttpGet]
        [Route("api/RequestAttachment/{reqAttachmentId}/{userId}")]
        [ResponseType(typeof(RequestAttachment))]
        public IHttpActionResult Get(int reqAttachmentId, string userId)
        {
            if (Utility.HasPermission(userId, Entity.Constant.AppResource.ApiRequest))
            {
                RequestManager om = new RequestManager();
                Entity.RequestAttachment reqAtt = om.GetRequestAttachment(reqAttachmentId, userId);
                if (reqAtt == null)
                {
                    return NotFound();
                }
                Entity.Request req = om.GetRequest(reqAtt.ReqId, userId);
                if (req == null)
                {
                    return NotFound();
                }
                if (Utility.HasPermission(userId, req))
                {
                    return Ok(reqAtt);
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        [Route("api/RequestAttachment/{userId}")]
        [ResponseType(typeof(RequestAttachment))]
        public IHttpActionResult Post([FromBody]RequestAttachment reqAttachment, string userId)
        {
            RequestManager om = new RequestManager();

            if (reqAttachment == null)
            {
                return NotFound();
            }
            Entity.Request req = om.GetRequest(reqAttachment.ReqId, userId);
            if (req == null)
            {
                return NotFound();
            }
            if (Utility.HasPermission(userId, req))
            {
                Entity.RequestAttachment reqAtt = om.SaveRequestAttachment(reqAttachment, userId);
                return Ok(reqAtt);
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPut]
        [Route("api/RequestAttachment/{userid}")]
        [ResponseType(typeof(RequestAttachment))]
        public IHttpActionResult Put([FromBody]RequestAttachment reqAttachment, string userid)
        {
            RequestManager om = new RequestManager();
            return Ok(om.SaveRequestAttachment(reqAttachment, userid));
        }

        [HttpDelete]
        [Route("api/RequestAttachment/{reqAttachmentId}/{userid}")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult Delete(int reqAttachmentId, string userid)
        {
            RequestManager om = new RequestManager();
            bool success = om.DeleteRequestAttachment(reqAttachmentId, userid);
            if (success)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
    }
}

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
    public class RequestController : ApiController
    {

        [HttpGet]
        [Route("api/Request/{requestId}/{userId}")]
        [ResponseType(typeof(Request))]
        public IHttpActionResult Get(int requestId, string userId)
        {
            if (Utility.HasPermission(userId, Entity.Constant.AppResource.ApiRequest))
            {
                RequestManager om = new RequestManager();
                Entity.Request req = om.GetRequest(requestId, userId);
                if (req == null)
                {
                    return NotFound();
                }
                if (Utility.HasPermission(userId, req))
                {
                    return Ok(req);
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
        [Route("api/Request/{userId}")]
        [ResponseType(typeof(Request))]
        public IHttpActionResult Post([FromBody]Request req, string userId)
        {
            if (Utility.HasPermission(userId, Entity.Constant.AppResource.ApiRequest))
            {
                RequestManager om = new RequestManager();
                if (Utility.HasPermission(userId, req))
                {
                    Entity.Request req2 = om.SaveRequest(req, userId);
                    return Ok(req2);
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

        [HttpPut]
        [Route("api/Request/{userId}")]
        [ResponseType(typeof(Request))]
        public IHttpActionResult Put([FromBody]Request req, string userId)
        {
            if (Utility.HasPermission(userId, Entity.Constant.AppResource.ApiRequest))
            {
                RequestManager om = new RequestManager();
                if (Utility.HasPermission(userId, req))
                {
                    Entity.Request req2 = om.SaveRequest(req, userId);
                    return Ok(req2);
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

        [HttpDelete]
        [Route("api/Request/{requestId}/{userId}")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult Delete(int requestId, string userId)
        {
            if (Utility.HasPermission(userId, Entity.Constant.AppResource.ApiRequest))
            {
                RequestManager om = new RequestManager();
                Entity.Request req = om.GetRequest(requestId, userId);
                if (req == null)
                {
                    return NotFound();
                }
                if (Utility.HasPermission(userId, req))
                {
                    bool success = om.DeleteRequest(requestId, userId);
                    if (success)
                    {
                        return Ok();
                    }
                    else
                    {
                        return NotFound();
                    }
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
    }
}

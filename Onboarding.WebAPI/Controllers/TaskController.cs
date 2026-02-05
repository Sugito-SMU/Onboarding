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
    public class TaskController : ApiController
    {

        [HttpGet]
        [Route("api/Task/{taskId}/{taskCd}/{userId}")]
        [ResponseType(typeof(Request))]
        public IHttpActionResult Get(int taskId, string taskCd, string userId)
        {
            if (Utility.HasPermission(userId, Entity.Constant.AppResource.ApiTask))
            {
                TaskManager mgr = new TaskManager();
                return Ok(mgr.GetTask(taskId, taskCd, userId));
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPut]
        [Route("api/Task/{userId}")]
        [ResponseType(typeof(Task))]
        public IHttpActionResult Put([FromBody]Task task, string userId)
        {
            if (Utility.HasPermission(userId, Entity.Constant.AppResource.ApiTask))
            {
                TaskManager mgr = new TaskManager();
                return Ok(mgr.SaveTask(task, userId));
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}

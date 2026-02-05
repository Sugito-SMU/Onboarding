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
    public class TaskStatusController : ApiController
    {

        [HttpGet]
        [Route("api/TaskStatus")]
        [ResponseType(typeof(IEnumerable<TaskStatus>))]
        public IHttpActionResult Get()
        {
            TaskStatusManager mgr = new TaskStatusManager();
            List<TaskStatus> list = mgr.GetTaskStatusAll();
            return Ok(list.Where(x => x.IsActive == true));
        }

        [HttpGet]
        [Route("api/TaskStatus/{id}")]
        [ResponseType(typeof(string))]
        public IHttpActionResult Get(string id)
        {
            TaskStatusManager mgr = new TaskStatusManager();
            return Ok(mgr.GetTaskStatusDesc(id));
        }
    }
}

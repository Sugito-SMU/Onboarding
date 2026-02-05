using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Onboarding.Entity;
using Onboarding.ServiceManager;


namespace Onboarding.WebAPI.Controllers
{
    public class TaskCodeController : ApiController
    {

        [HttpGet]
        [Route("api/TaskCode")]
        [ResponseType(typeof(IEnumerable<TaskCode>))]
        public IHttpActionResult Get()
        {
            TaskCodeManager mgr = new TaskCodeManager();
            List<TaskCode> list = mgr.GetTaskCodeAll();
            return Ok(list);
        }

        [HttpGet]
        [Route("api/TaskCode/{id}")]
        [ResponseType(typeof(string))]
        public IHttpActionResult Get(string id)
        {
            TaskCodeManager mgr = new TaskCodeManager();
            return Ok(mgr.GetTaskCodeDesc(id));
        }
    }
}

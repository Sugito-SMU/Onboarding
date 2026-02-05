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
    public class TaskAttributeCodeController : ApiController
    {

        [HttpGet]
        [Route("api/TaskAttributeCode")]
        [ResponseType(typeof(IEnumerable<TaskAttributeCode>))]
        public IHttpActionResult Get()
        {
            TaskAttributeCodeManager mgr = new TaskAttributeCodeManager();
            List<TaskAttributeCode> list = mgr.GetTaskAttributeCodeAll();
            return Ok(list.Where(x => x.IsActive == true));
        }

        [HttpGet]
        [Route("api/TaskAttributeCode/{id}")]
        [ResponseType(typeof(string))]
        public IHttpActionResult Get(string id)
        {
            TaskAttributeCodeManager mgr = new TaskAttributeCodeManager();
            return Ok(mgr.GetTaskAttributeCodeDesc(id));
        }
    }
}

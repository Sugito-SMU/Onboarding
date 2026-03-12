using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Onboarding.ServiceManager;
using Onboarding.Entity;
using System.Web.Http.Description;
using System.Web.Http.Cors;


namespace Onboarding.WebAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class HRAdminEmailController : ApiController
    {
        [HttpGet]
        [Route("api/HRAdminEmail/{taskCd}")]
        [ResponseType(typeof(string))]
        public IHttpActionResult Get(string taskCd)
        {
            TaskAgentManager taskAgentMgr = new TaskAgentManager();
            List<TaskAgent> taskAgents = taskAgentMgr.GetTaskAgent(taskCd);
            UserProfileManager userProfileMgr = new UserProfileManager();
            List<UserProfile> hrAdminUsers = userProfileMgr.GetUserProfileAll().Where(x => x.HasSystemRole(Entity.Constant.SystemRole.HRAdmin)).ToList();
            var hrAdminTaskAgents = taskAgents
          .Join(
              hrAdminUsers,
              agent => agent.AgentId,
              user => user.NtLoginId,
              (agent, user) => user
          )
          .ToList();
            string emails = string.Join(",", hrAdminTaskAgents.Select(u => u.Email));
            return Ok(new { emails });
        }
    }
}

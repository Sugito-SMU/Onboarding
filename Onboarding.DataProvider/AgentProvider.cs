using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding;
using Onboarding.Entity;

namespace Onboarding.DataProvider
{
    public class AgentProvider
    {
        public List<Entity.Agent> GetAgentAll()
        {
            UserProfileProvider userProv = new UserProfileProvider();

            List<Entity.Agent> agentList = (List<Entity.Agent>)MCache.Get("Agent");
            if (agentList == null)
            {
                using (Model.OnboardingEntities obCtx = new Model.OnboardingEntities())
                {
                    List<Model.V_TaskAgent> dbAgentList = obCtx.V_TaskAgent.ToList<Model.V_TaskAgent>();
                    if (dbAgentList != null)
                    {
                        agentList = new List<Agent>();
                        foreach (Model.V_TaskAgent dbAgent in dbAgentList)
                        {
                            Entity.Agent agent = new Agent();
                            agent.AgentId = dbAgent.AgentId;
                            agent.TaskCd = dbAgent.TaskCd;
                            Entity.UserProfile usrProf = userProv.GetUserProfile(dbAgent.AgentId);
                            if (usrProf != null)
                            {
                                agent.AgentName = usrProf.Name;
                                agent.Email = usrProf.Email;
                            }
                            agent.IsActive = dbAgent.IsActive;
                            agent.CreatedBy = dbAgent.CreatedBy;
                            agent.CreatedDt = dbAgent.CreatedDt;
                            agent.ModifiedBy = dbAgent.ModifiedBy;
                            agent.ModifiedDt = dbAgent.ModifiedDt;
                            agentList.Add(agent);
                        }
                    }   
                }
                MCache.Set("Agent", agentList);
            }
            return agentList;
        }

        public List<Entity.Agent> GetAgentsForTask(string taskCd)
        {
            List<Entity.Agent> agentList = GetAgentAll();
            List<Entity.Agent> agentTaskList = agentList.Where(x => (x.TaskCd.Trim().ToUpper() == taskCd.Trim().ToUpper())).ToList();
            return agentTaskList;
        }

        public Entity.Agent GetAgent(string agentUserId)
        {
            Entity.Agent agent = null;
            if (!string.IsNullOrWhiteSpace(agentUserId))
            {
                List<Entity.Agent> agentList = GetAgentAll();
                agent = agentList.FirstOrDefault(x => x.AgentId.Trim().ToLower() == agentUserId.Trim().ToLower());
            }
            return agent;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.DataProvider.Mapper
{
    public static partial class MapToEntity
    {
        public static void Map(Model.V_TaskAgent dbTaskAgent, Entity.TaskAgent taskAgent)
        {
            if ((dbTaskAgent != null) && (taskAgent != null))
            {
                taskAgent.AgentId = dbTaskAgent.AgentId;
                taskAgent.TaskCd = dbTaskAgent.TaskCd;
                taskAgent.IsActive = dbTaskAgent.IsActive;
                taskAgent.IsPrimary = dbTaskAgent.IsPrimary;
                taskAgent.CreatedBy = dbTaskAgent.CreatedBy;
                taskAgent.CreatedDt = dbTaskAgent.CreatedDt;
                taskAgent.ModifiedBy = dbTaskAgent.ModifiedBy;
                taskAgent.ModifiedDt = dbTaskAgent.ModifiedDt;
            }
        }
    }
}

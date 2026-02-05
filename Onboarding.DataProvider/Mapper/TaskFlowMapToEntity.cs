using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.DataProvider.Mapper
{
    public static partial class MapToEntity
    {
        public static void Map(Model.V_TaskDep dbTaskFlow, Entity.TaskFlow taskFlow)
        {
            if ((dbTaskFlow != null) && (taskFlow != null))
            {
                taskFlow.TaskCd = dbTaskFlow.TaskCd;
                taskFlow.PreTaskCd = dbTaskFlow.DepTaskCd;
                taskFlow.IsActive = dbTaskFlow.IsActive;
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.DataProvider.Mapper
{
    public static partial class MapToEntity
    {
        public static void Map(Model.V_TaskCd dbTaskCd, Entity.TaskCode taskCd)
        {
            if ((dbTaskCd != null) && (taskCd != null))
            {
                taskCd.TaskCd = dbTaskCd.TaskCd;
                taskCd.TaskDesc = dbTaskCd.TaskDesc;
                taskCd.IsActive = dbTaskCd.IsActive;
            }
        }

    }
}

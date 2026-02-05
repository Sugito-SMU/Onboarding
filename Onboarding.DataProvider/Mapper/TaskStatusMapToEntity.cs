using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.DataProvider.Mapper
{
    public static partial class MapToEntity
    {
        public static void Map(Model.V_TaskStsCd dbTaskStsCd, Entity.TaskStatus taskSts)
        {
            if ((dbTaskStsCd != null) && (taskSts != null))
            {
                taskSts.TaskStsCd = dbTaskStsCd.TaskStsCd;
                taskSts.TaskStsDesc = dbTaskStsCd.TaskStsDesc;
                taskSts.IsActive = dbTaskStsCd.IsActive;
            }
        }

    }
}

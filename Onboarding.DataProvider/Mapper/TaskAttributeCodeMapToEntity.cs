using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.DataProvider.Mapper
{
    public static partial class MapToEntity
    {
        public static void Map(Model.V_TaskAttrbCd dbTaskAttrbCd, Entity.TaskAttributeCode taskAttrbCd)
        {
            if ((dbTaskAttrbCd != null) && (taskAttrbCd != null))
            {
                taskAttrbCd.TaskCd = dbTaskAttrbCd.TaskCd;
                taskAttrbCd.TaskAttrbCd = dbTaskAttrbCd.TaskAttrbCd;
                taskAttrbCd.TaskAttrbDesc = dbTaskAttrbCd.TaskAttrbDesc;
                taskAttrbCd.IsActive = dbTaskAttrbCd.IsActive;
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onboarding;

namespace Onboarding.DataProvider.Mapper
{
    public static partial class MapToEntity
    {

        public static void Map(Model.V_OffTaskLog dbOffTaskChangeLog, Entity.TaskChangeLog taskChangeLog, Model.OnboardingEntities obCtx)
        {
            if ((dbOffTaskChangeLog != null) && (taskChangeLog != null))
            {
                taskChangeLog.TaskChangeLogId = dbOffTaskChangeLog.OffTaskLogId;
                taskChangeLog.TaskId = dbOffTaskChangeLog.OffTaskId;
                taskChangeLog.FieldNm = dbOffTaskChangeLog.FieldNm;
                taskChangeLog.FromVal = dbOffTaskChangeLog.FromVal;
                taskChangeLog.ToVal = dbOffTaskChangeLog.ToVal;
                taskChangeLog.ChangedBy = dbOffTaskChangeLog.ChangeBy;
                taskChangeLog.ChangedDt = dbOffTaskChangeLog.ChangeDt;
            }
        }

        public static void Map(Model.V_SysAccTaskLog dbSysAccTaskChangeLog, Entity.TaskChangeLog taskChangeLog, Model.OnboardingEntities obCtx)
        {
            if ((dbSysAccTaskChangeLog != null) && (taskChangeLog != null))
            {
                taskChangeLog.TaskChangeLogId = dbSysAccTaskChangeLog.SysAccTaskLogId;
                taskChangeLog.TaskId = dbSysAccTaskChangeLog.SysAccTaskId;
                taskChangeLog.FieldNm = dbSysAccTaskChangeLog.FieldNm;
                taskChangeLog.FromVal = dbSysAccTaskChangeLog.FromVal;
                taskChangeLog.ToVal = dbSysAccTaskChangeLog.ToVal;
                taskChangeLog.ChangedBy = dbSysAccTaskChangeLog.ChangeBy;
                taskChangeLog.ChangedDt = dbSysAccTaskChangeLog.ChangeDt;
            }
        }

    }
}

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

        public static void Map(Model.V_ReqLog dbReqChangeLog, Entity.RequestChangeLog reqChangeLog, Model.OnboardingEntities obCtx)
        {
            if ((dbReqChangeLog != null) && (reqChangeLog != null))
            {
                reqChangeLog.ReqChangeLogId = dbReqChangeLog.ReqLogId;
                reqChangeLog.ReqId = dbReqChangeLog.ReqId;
                reqChangeLog.FieldNm = dbReqChangeLog.FieldNm;
                reqChangeLog.FromVal = dbReqChangeLog.FromVal;
                reqChangeLog.ToVal = dbReqChangeLog.ToVal;
                reqChangeLog.ChangedBy = dbReqChangeLog.ChangeBy;
                reqChangeLog.ChangedDt = dbReqChangeLog.ChangeDt;
            }
        }

    }
}

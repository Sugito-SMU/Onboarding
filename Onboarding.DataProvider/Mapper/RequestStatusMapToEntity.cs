using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.DataProvider.Mapper
{
    public static partial class MapToEntity
    {
        public static void Map(Model.V_ReqStsCd dbReqStsCd, Entity.RequestStatus reqSts)
        {
            if ((dbReqStsCd != null) && (reqSts != null))
            {
                reqSts.ReqStsCd = dbReqStsCd.ReqStsCd;
                reqSts.ReqStsDesc = dbReqStsCd.ReqStsDesc;
                reqSts.IsActive = dbReqStsCd.IsActive;
            }
        }

    }
}

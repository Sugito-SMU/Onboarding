using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.DataProvider.Mapper
{
    public static partial class MapToEntity
    {
        public static void Map(Model.V_ReqTypeCd dbReqTypeCd, Entity.RequestType reqType)
        {
            if ((dbReqTypeCd != null) && (reqType != null))
            {
                reqType.ReqTypeCd = dbReqTypeCd.ReqTypeCd;
                reqType.ReqTypeDesc = dbReqTypeCd.ReqTypeDesc;
                reqType.Sequence = dbReqTypeCd.Seq;
                reqType.IsActive = dbReqTypeCd.IsActive;
            }
        }

    }
}

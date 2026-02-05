using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.DataProvider.Mapper
{
    public static partial class MapToEntity
    {
        public static void Map(Model.V_ReqAttrbCd dbReqAttrbCd, Entity.RequestAttributeCode reqAttrbCd)
        {
            if ((dbReqAttrbCd != null) && (reqAttrbCd != null))
            {
                reqAttrbCd.RequestAttrbCd = dbReqAttrbCd.ReqAttrbCd;
                reqAttrbCd.RequestAttrbDesc = dbReqAttrbCd.ReqAttrbDesc;
                reqAttrbCd.IsActive = dbReqAttrbCd.IsActive;
            }
        }

    }
}

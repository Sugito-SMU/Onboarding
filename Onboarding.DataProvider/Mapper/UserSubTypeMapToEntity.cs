using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.DataProvider.Mapper
{
    public static partial class MapToEntity
    {
        public static void Map(Model.V_UserSubTypeCd dbUserSubType, Entity.UserSubType userSubType)
        {
            if ((dbUserSubType != null) && (userSubType != null))
            {
                userSubType.UserTypeCd = dbUserSubType.UserTypeCd;
                userSubType.UserSubTypeCd = dbUserSubType.UserSubTypeCd;
                userSubType.UserSubTypeDesc = dbUserSubType.UserSubTypeDesc;
                userSubType.Sequence = dbUserSubType.Seq;
                userSubType.IsActive = dbUserSubType.IsActive;
            }
        }

    }
}

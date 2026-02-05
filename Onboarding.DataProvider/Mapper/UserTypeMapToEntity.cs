using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.DataProvider.Mapper
{
    public static partial class MapToEntity
    {
        public static void Map(Model.V_UserTypeCd dbUserType, Entity.UserType userType)
        {
            if ((dbUserType != null) && (userType != null))
            {
                userType.UserTypeCd = dbUserType.UserTypeCd;
                userType.UserTypeDesc = dbUserType.UserTypeDesc;
                userType.Sequence = dbUserType.Seq;
                userType.IsActive = dbUserType.IsActive;
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.DataProvider.Mapper
{
    public static partial class MapToEntity
    {
        public static void Map(Model.V_GenderCd dbGender, Entity.Gender gender)
        {
            if ((dbGender != null) && (gender != null))
            {
                gender.GenderCd = dbGender.GenderCd;
                gender.GenderDesc = dbGender.GenderDesc;
                gender.IsActive = dbGender.IsActive;
            }
        }

    }
}

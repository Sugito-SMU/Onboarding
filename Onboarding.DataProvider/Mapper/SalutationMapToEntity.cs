using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.DataProvider.Mapper
{
    public static partial class MapToEntity
    {
        public static void Map(Model.V_SalutCd dbSalut, Entity.Salutation salut)
        {
            if ((dbSalut != null) && (salut != null))
            {
                salut.SalutCd = dbSalut.SalutCd;
                salut.SalutDesc = dbSalut.SalutDesc;
                salut.IsActive = dbSalut.IsActive;
            }
        }

    }
}

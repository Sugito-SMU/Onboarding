using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.DataProvider.Mapper
{
    public static partial class MapToEntity
    {
        public static void Map(Model.V_CostCtrCd dbCostCentre, Entity.CostCentre costCentre)
        {
            if ((dbCostCentre != null) && (costCentre != null))
            {
                costCentre.CostCtrCd = dbCostCentre.CostCtrCd;
                costCentre.CostCtrDesc = string.Format("{0} ({1})", dbCostCentre.CostCtrDesc, dbCostCentre.CostCtrCd);
                costCentre.IsActive = dbCostCentre.IsActive;
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.DataProvider.Mapper
{
    public static partial class MapToEntity
    {
        public static void Map(Model.V_RaceCd dbRace, Entity.Race race)
        {
            if ((dbRace != null) && (race != null))
            {
                race.RaceCd = dbRace.RaceCd;
                race.RaceDesc = dbRace.RaceDesc;
                race.IsActive = dbRace.IsActive;
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding.DataProvider;
using Onboarding.DataProvider.Model;

namespace Onboarding.ServiceManager
{
    public class RaceManager
    {
        public List<Entity.Race> GetRaceAll()
        {
            RaceProvider raceProvider = new RaceProvider();
            List<Entity.Race> raceList = raceProvider.GetRaceAll();
            return raceList.Where(x => x.IsActive == true).OrderBy(x => x.RaceDesc).ToList();
        }

        public string GetRaceDesc(string raceCd)
        {
            RaceProvider raceProvider = new RaceProvider();
            string raceDesc = raceProvider.GetRaceDesc(raceCd);
            return raceDesc;
        }

    }
}

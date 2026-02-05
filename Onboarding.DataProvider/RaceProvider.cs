using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding;
using Onboarding.Entity;

namespace Onboarding.DataProvider
{
    public class RaceProvider
    {
        public List<Entity.Race> GetRaceAll()
        {
            List<Entity.Race> raceList = (List<Entity.Race>)MCache.Get("Race");
            if (raceList == null)
            {
                raceList = new List<Entity.Race>();
                using (Model.OnboardingEntities obCtx = new Model.OnboardingEntities())
                {
                    List<Model.V_RaceCd> dbRaceList = obCtx.V_RaceCd.ToList<Model.V_RaceCd>();
                    if (dbRaceList != null)
                    {
                        foreach (Model.V_RaceCd dbRaceCd in dbRaceList)
                        {
                            Entity.Race race = new Entity.Race();
                            Mapper.MapToEntity.Map(dbRaceCd, race);
                            raceList.Add(race);
                        }
                    }
                }
                MCache.Set("Race", raceList);
            }
            return raceList;
        }

        public string GetRaceDesc(string raceCd)
        {
            if (!string.IsNullOrWhiteSpace(raceCd))
            {
                List<Entity.Race> raceList = GetRaceAll();
                Entity.Race race = raceList.FirstOrDefault(x => x.RaceCd.Trim().ToLower() == raceCd.Trim().ToLower());
                if (race != null)
                {
                    return race.RaceDesc;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public Entity.PatronOnbdIntfRpt SynchronizeWithPatron(DataProvider.Model.OnboardingEntities obCtx)
        {
            Entity.PatronOnbdIntfRpt rpt = new PatronOnbdIntfRpt();
            List<Model.V_RaceCd> dbRaceList = obCtx.V_RaceCd.ToList();
            using (Model.PATRONEntities patronCtx = new Model.PATRONEntities())
            {
                List<Model.V_OnboardingRaceCd> patronRaceList = patronCtx.V_OnboardingRaceCd.ToList();
                foreach (Model.V_OnboardingRaceCd patronRace in patronRaceList)
                {
                    Model.V_RaceCd dbRace = dbRaceList.Where(x => x.RaceCd.ToUpper().Trim() == patronRace.RaceCd.ToUpper().Trim()).FirstOrDefault();
                    if (dbRace == null)
                    {
                        dbRace = new Model.V_RaceCd();
                        dbRace.RaceCd = patronRace.RaceCd;
                        dbRace.RaceDesc = patronRace.RaceDesc;
                        dbRace.IsActive = true;
                        dbRace.CreatedBy = Entity.Constant.PatronOnboardingInterface.CreateUpdateBy;
                        dbRace.CreatedDt = Common.GetSqlDateTime();
                        obCtx.V_RaceCd.Add(dbRace);
                        rpt.Inserted++;
                    }
                    else
                    {
                        if (!Common.IsSameDescription(dbRace.RaceDesc, patronRace.RaceDesc))
                        {
                            dbRace.RaceDesc = patronRace.RaceDesc;
                            dbRace.IsActive = true;
                            dbRace.ModifiedBy = Entity.Constant.PatronOnboardingInterface.CreateUpdateBy;
                            dbRace.ModifiedDt = Common.GetSqlDateTime();
                            rpt.Updated++;
                        }
                    }
                }

                foreach (Model.V_RaceCd dbRace in dbRaceList)
                {
                    Model.V_OnboardingRaceCd patronRace = patronCtx.V_OnboardingRaceCd.Where(x => x.RaceCd.ToUpper().Trim() == dbRace.RaceCd.ToUpper().Trim()).FirstOrDefault();
                    if (patronRace == null)
                    {
                        dbRace.IsActive = false;
                        rpt.Removed++;
                    }
                }
            }
            obCtx.SaveChanges();
            return rpt;
        }

    }
}

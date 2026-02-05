using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding;
using Onboarding.Entity;

namespace Onboarding.DataProvider
{
    public class GenderProvider
    {
        public List<Entity.Gender> GetGenderAll()
        {
            List<Entity.Gender> genderList = (List<Entity.Gender>)MCache.Get("Gender");
            if (genderList == null)
            {
                genderList = new List<Entity.Gender>();
                using (Model.OnboardingEntities obCtx = new Model.OnboardingEntities())
                {
                    List<Model.V_GenderCd> dbGenderList = obCtx.V_GenderCd.ToList<Model.V_GenderCd>();
                    if (dbGenderList != null)
                    {
                        foreach (Model.V_GenderCd dbGenderCd in dbGenderList)
                        {
                            Entity.Gender gender = new Entity.Gender();
                            Mapper.MapToEntity.Map(dbGenderCd, gender);
                            genderList.Add(gender);
                        }
                    }
                }
                MCache.Set("Gender", genderList);
            }
            return genderList;
        }

        public string GetGenderDesc(string genderCd)
        {
            if (!string.IsNullOrWhiteSpace(genderCd))
            {
                List<Entity.Gender> genderList = GetGenderAll();
                Entity.Gender gender = genderList.FirstOrDefault(x => x.GenderCd.Trim().ToLower() == genderCd.Trim().ToLower());
                if (gender != null)
                {
                    return gender.GenderDesc;
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
            List<Model.V_GenderCd> dbGenderList = obCtx.V_GenderCd.ToList();
            using (Model.PATRONEntities patronCtx = new Model.PATRONEntities())
            {
                List<Model.V_OnboardingGenderCd> patronGenderList = patronCtx.V_OnboardingGenderCd.ToList();
                foreach (Model.V_OnboardingGenderCd patronGender in patronGenderList)
                {
                    Model.V_GenderCd dbGender = dbGenderList.Where(x => x.GenderCd.ToUpper().Trim() == patronGender.GenderCd.ToUpper().Trim()).FirstOrDefault();
                    if (dbGender == null)
                    {
                        dbGender = new Model.V_GenderCd();
                        dbGender.GenderCd = patronGender.GenderCd;
                        dbGender.GenderDesc = patronGender.GenderDesc;
                        dbGender.IsActive = true;
                        dbGender.CreatedBy = Entity.Constant.PatronOnboardingInterface.CreateUpdateBy;
                        dbGender.CreatedDt = Common.GetSqlDateTime();
                        obCtx.V_GenderCd.Add(dbGender);
                        rpt.Inserted++;
                    }
                    else
                    {
                        if (!Common.IsSameDescription(dbGender.GenderDesc, patronGender.GenderDesc))
                        {
                            dbGender.GenderDesc = patronGender.GenderDesc;
                            dbGender.IsActive = true;
                            dbGender.ModifiedBy = Entity.Constant.PatronOnboardingInterface.CreateUpdateBy;
                            dbGender.ModifiedDt = Common.GetSqlDateTime();
                            rpt.Updated++;
                        }
                    }
                }

                foreach (Model.V_GenderCd dbGender in dbGenderList)
                {
                    Model.V_OnboardingGenderCd patronGender = patronCtx.V_OnboardingGenderCd.Where(x => x.GenderCd.ToUpper().Trim() == dbGender.GenderCd.ToUpper().Trim()).FirstOrDefault();
                    if (patronGender == null)
                    {
                        dbGender.IsActive = false;
                        rpt.Removed++;
                    }
                }
            }
            obCtx.SaveChanges();
            return rpt;
        }


    }
}

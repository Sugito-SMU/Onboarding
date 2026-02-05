using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding;
using Onboarding.Entity;

namespace Onboarding.DataProvider
{
    public class SalutationProvider
    {
        public List<Entity.Salutation> GetSalutationAll()
        {
            List<Entity.Salutation> salutList = (List<Entity.Salutation>)MCache.Get("Salutation");
            if (salutList == null)
            {
                salutList = new List<Entity.Salutation>();
                using (Model.OnboardingEntities obCtx = new Model.OnboardingEntities())
                {
                    List<Model.V_SalutCd> dbSalutationList = obCtx.V_SalutCd.ToList<Model.V_SalutCd>();
                    if (dbSalutationList != null)
                    {
                        foreach (Model.V_SalutCd dbSalutCd in dbSalutationList)
                        {
                            Entity.Salutation salut = new Entity.Salutation();
                            Mapper.MapToEntity.Map(dbSalutCd, salut);
                            salutList.Add(salut);
                        }
                    }
                }
                MCache.Set("Salutation", salutList);
            }
            return salutList;
        }

        public string GetSalutationDesc(string salutCd)
        {
            if (!string.IsNullOrWhiteSpace(salutCd))
            {
                List<Entity.Salutation> salutList = GetSalutationAll();
                Entity.Salutation salut = salutList.FirstOrDefault(x => x.SalutCd.Trim().ToLower() == salutCd.Trim().ToLower());
                if (salut != null)
                {
                    return salut.SalutDesc;
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
            List<Model.V_SalutCd> dbSalutList = obCtx.V_SalutCd.ToList();
            using (Model.PATRONEntities patronCtx = new Model.PATRONEntities())
            {
                List<Model.V_OnboardingSalutCd> patronSalutList = patronCtx.V_OnboardingSalutCd.ToList();
                foreach (Model.V_OnboardingSalutCd patronSalut in patronSalutList)
                {
                    Model.V_SalutCd dbSalut = dbSalutList.Where(x => x.SalutCd.ToUpper().Trim() == patronSalut.SalutCd.ToUpper().Trim()).FirstOrDefault();
                    if (dbSalut == null)
                    {
                        dbSalut = new Model.V_SalutCd();
                        dbSalut.SalutCd = patronSalut.SalutCd;
                        dbSalut.SalutDesc = patronSalut.SalutDesc;
                        dbSalut.IsActive = true;
                        dbSalut.CreatedBy = Entity.Constant.PatronOnboardingInterface.CreateUpdateBy;
                        dbSalut.CreatedDt = Common.GetSqlDateTime();
                        obCtx.V_SalutCd.Add(dbSalut);
                        rpt.Inserted++;
                    }
                    else
                    {
                        if (!Common.IsSameDescription(dbSalut.SalutDesc, patronSalut.SalutDesc))
                        {
                            dbSalut.SalutDesc = patronSalut.SalutDesc;
                            dbSalut.IsActive = true;
                            dbSalut.ModifiedBy = Entity.Constant.PatronOnboardingInterface.CreateUpdateBy;
                            dbSalut.ModifiedDt = Common.GetSqlDateTime();
                            rpt.Updated++;
                        }
                    }
                }

                foreach (Model.V_SalutCd dbSalut in dbSalutList)
                {
                    Model.V_OnboardingSalutCd patronSalut = patronCtx.V_OnboardingSalutCd.Where(x => x.SalutCd.ToUpper().Trim() == dbSalut.SalutCd.ToUpper().Trim()).FirstOrDefault();
                    if (patronSalut == null)
                    {
                        dbSalut.IsActive = false;
                        rpt.Removed++;
                    }
                }
            }
            obCtx.SaveChanges();
            return rpt;
        }

    }
}

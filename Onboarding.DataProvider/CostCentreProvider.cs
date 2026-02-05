using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding;
using Onboarding.Entity;

namespace Onboarding.DataProvider
{
    public class CostCentreProvider
    {
        public List<Entity.CostCentre> GetCostCentreAll()
        {
            List<Entity.CostCentre> costCtrList = (List<Entity.CostCentre>)MCache.Get("CostCentre");
            if (costCtrList == null)
            {
                costCtrList = new List<Entity.CostCentre>();
                using (Model.OnboardingEntities obCtx = new Model.OnboardingEntities())
                {
                    List<Model.V_CostCtrCd> dbCostCentreList = obCtx.V_CostCtrCd.ToList<Model.V_CostCtrCd>();
                    if (dbCostCentreList != null)
                    {
                        foreach (Model.V_CostCtrCd dbCostCtrCd in dbCostCentreList)
                        {
                            Entity.CostCentre costCtr = new Entity.CostCentre();
                            Mapper.MapToEntity.Map(dbCostCtrCd, costCtr);
                            costCtrList.Add(costCtr);
                        }
                    }
                }
                MCache.Set("CostCentre", costCtrList);
            }
            return costCtrList;
        }

        public string GetCostCentreDesc(string costCtrCd)
        {
            if (!string.IsNullOrWhiteSpace(costCtrCd))
            {
                List<Entity.CostCentre> costCtrList = GetCostCentreAll();
                Entity.CostCentre costCtr = costCtrList.FirstOrDefault(x => x.CostCtrCd.Trim().ToLower() == costCtrCd.Trim().ToLower());
                if (costCtr != null)
                {
                    return costCtr.CostCtrDesc;
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
            List<Model.V_CostCtrCd> dbCostCtrList = obCtx.V_CostCtrCd.ToList();
            using (Model.PATRONEntities patronCtx = new Model.PATRONEntities())
            {
                List<Model.V_OnboardingCostCtrCd> patronCostCtrList = patronCtx.V_OnboardingCostCtrCd.ToList();
                foreach (Model.V_OnboardingCostCtrCd patronCostCtr in patronCostCtrList)
                {
                    Model.V_CostCtrCd dbCostCtr = dbCostCtrList.Where(x => x.CostCtrCd.ToUpper().Trim() == patronCostCtr.CostCtrCd.ToUpper().Trim()).FirstOrDefault();
                    if (dbCostCtr == null)
                    {
                        dbCostCtr = new Model.V_CostCtrCd();
                        dbCostCtr.CostCtrCd = patronCostCtr.CostCtrCd;
                        dbCostCtr.CostCtrDesc = patronCostCtr.CostCtrDesc;
                        dbCostCtr.IsActive = true;
                        dbCostCtr.CreatedBy = Entity.Constant.PatronOnboardingInterface.CreateUpdateBy;
                        dbCostCtr.CreatedDt = Common.GetSqlDateTime();
                        obCtx.V_CostCtrCd.Add(dbCostCtr);
                        rpt.Inserted++;
                    }
                    else
                    {
                        if (!Common.IsSameDescription(dbCostCtr.CostCtrDesc, patronCostCtr.CostCtrDesc))
                        {
                            dbCostCtr.CostCtrDesc = patronCostCtr.CostCtrDesc;
                            dbCostCtr.IsActive = true;
                            dbCostCtr.ModifiedBy = Entity.Constant.PatronOnboardingInterface.CreateUpdateBy;
                            dbCostCtr.ModifiedDt = Common.GetSqlDateTime();
                            rpt.Updated++;
                        }
                    }
                }

                foreach (Model.V_CostCtrCd dbCostCtr in dbCostCtrList)
                {
                    Model.V_OnboardingCostCtrCd patronCostCtr = patronCtx.V_OnboardingCostCtrCd.Where(x => x.CostCtrCd.ToUpper().Trim() == dbCostCtr.CostCtrCd.ToUpper().Trim()).FirstOrDefault();
                    if (patronCostCtr == null)
                    {
                        dbCostCtr.IsActive = false;
                        rpt.Removed++;
                    }
                }
            }
            obCtx.SaveChanges();
            return rpt;
        }

    }
}

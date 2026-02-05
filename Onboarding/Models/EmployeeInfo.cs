using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Onboarding.Entity;
namespace Onboarding.Models
{
    public class EmployeeInfo
    {
        public Entity.Request Request { get; set; }

        public int ReqEmplId { get; set; }

        public int ReqId { get; set; }

        public string CostCtrCd { get; set; }

        public string CostCtrDesc { get; set; }

        public string UserTypeCd { get; set; }

        public string UserTypeDesc { get; set; }

        public string UserSubTypeCd { get; set; }

        public string UserSubTypeDesc { get; set; }

        public string JobTitle { get; set; }

        public string OrgCd { get; set; }

        public string SelectedOrgCd { get; set; }

        public string OrgName { get; set; }

        public DateTime? EmplStartDt { get; set; }

        public string StrEmplStartDt { get; set; }
      
        public DateTime? EmplEndDt { get; set; }

        public string StrEmplEndDt { get; set; }

        public bool IsPrimary { get; set; }

        public bool IsAmended { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDt { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDt { get; set; }

        public List<Entity.UserType> UserTypeList { get; set; }

        public List<Entity.UserSubType> UserSubTypeList { get; set; }

        public List<Entity.CostCentre> CostCentreList { get; set; }
        
        public Boolean IsViewMode { get; set; }

        public Boolean IsSaveAllowed { get; set; }

        public Boolean IsAmendAllowed { get; set; }
                

        [DataType(DataType.MultilineText)]
        public string Remarks { get; set; }

    }
}
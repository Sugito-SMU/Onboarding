using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Onboarding.Entity;
namespace Onboarding.Models
{
    public class PersonalInfo
    {
        public Entity.Request Request { get; set; }

        public int ReqId { get; set; }

        public string ReqTypeCd { get; set; }

        public string ReqTypeDesc { get; set; }
       
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PrefNm { get; set; }

        public string SalutCd { get; set; }

        public string SalutDesc { get; set; }
        
        public string GenderCd { get; set; }
    
        public string GenderDesc { get; set; }
             
        public string RaceCd { get; set; }

        public string RaceDesc { get; set; }
                
        public string PrsnEmail { get; set; }

        public string MobileNo { get; set; }

        public bool? ExSmuStd { get; set; }

        public bool? IsExistingStaff { get; set; }

        public string Remark { get; set; }

        public string ReqStsCd { get; set; }

        public string ReqStsDesc { get; set; }

        public bool IsActive { get; set; }

        public bool IsAmended { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDt { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDt { get; set; }

        public List<Salutation> SalutationList { get; set; }

        public List<Gender> GenderList { get; set; }

        public List<Race> RaceList { get; set; }

        public Boolean IsViewMode { get; set; }

        public Boolean IsSaveAllowed { get; set; }

        public Boolean IsAmendAllowed { get; set; }

    }
}
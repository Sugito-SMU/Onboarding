using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Entity
{
    public class RequestEmployment
    {
        public int ReqEmplId { get; set; }

        public string CostCtrCd { get; set; }

        public string CostCtrDesc { get; set; }

        public string UserTypeCd { get; set; }

        public string UserTypeDesc { get; set; }

        public string UserSubTypeCd { get; set; }

        public string UserSubTypeDesc { get; set; }

        public string JobTitle { get; set; }
        
        public DateTime? EmplStartDt { get; set; }

        public DateTime? EmplEndDt { get; set; }

        public bool IsPrimary { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDt { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDt { get; set; }


    }
}

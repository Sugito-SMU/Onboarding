using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Onboarding.Entity
{
    public class RptSearchCriteria
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserTypeCd { get; set; }
        public string UserSubTypeCd { get; set; }
        public DateTime? EmplStartDtFrom { get; set; }
        public DateTime? EmplStartDtTo { get; set; }
        public DateTime? EmplEndDtFrom { get; set; }
        public DateTime? EmplEndDtTo { get; set; }
        public string CostCtrCd { get; set; }
        public DateTime? ReqDtFrom { get; set; }
        public DateTime? ReqDtTo { get; set; }
        public string RequestorUserId { get; set; }
        public int? ReqId { get; set; }
        public string ReqStsCd { get; set; }
        public string ReqTypeCd { get; set; }

    }
}

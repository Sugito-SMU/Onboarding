using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Onboarding.Entity
{
    public class RequestListForRequestorRow
    {
        public int ReqId { get; set; }
        public string ReqTypeDesc { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserTypeDesc { get; set; }
        public string UserSubTypeDesc { get; set; }
        public DateTime? EmplStartDt { get; set; }
        public string CostCtrDesc { get; set; }
        public string ReqStsDesc { get; set; }
        public string RequestorName { get; set; }
        public DateTime? RequestDt { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Onboarding.Entity
{
    public class OutstandingTaskAlertRow
    {
        public int ReqId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CostCtrDesc { get; set; }
        public DateTime? EmplStartDt { get; set; }
        public string RequestorName { get; set; }
        public DateTime? RequestDt { get; set; }
        public string TaskDesc { get; set; }
        public string AgentName { get; set; }
        public string AgentEmail { get; set; }
        public bool IsUrgent { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Entity
{
    public class Agent
    {
        public string TaskCd { get; set; }

        public string AgentId { get; set; }

        public string AgentName { get; set; }

        public string Email { get; set; }

        public string CreatedBy { get; set; }

        public bool IsActive { get; set; }

        public bool IsPrimary { get; set; }

        public DateTime CreatedDt { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDt { get; set; }

    }
}

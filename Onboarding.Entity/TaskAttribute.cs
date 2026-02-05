using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Entity
{
    public class TaskAttribute
    {
        public int TaskAttrbId { get; set; }

        public int TaskId { get; set; }

        public string TaskCd { get; set; }

        public string TaskDesc { get; set; }

        public string TaskAttrbCd { get; set; }

        public string TaskAttrbDesc { get; set; }

        public string TaskAttrbVal { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDt { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDt { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Entity
{
    public class TaskStatus
    {
        public string TaskStsCd { get; set; }

        public string TaskStsDesc { get; set; }

        public bool IsActive { get; set; }
    }
}

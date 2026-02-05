using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Entity
{
    public class TaskFlow
    {
        public string TaskCd { get; set; }

        public string PreTaskCd { get; set; }

        public bool IsActive { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Entity
{
    public class PatronDept
    {
        public int ActiveNum { get; set; }

        public string NtLoginId { get; set; }

        public string CostCentre { get; set; }

        public string JobTitle { get; set; }

        public bool IsPrimary { get; set; }

    }
}

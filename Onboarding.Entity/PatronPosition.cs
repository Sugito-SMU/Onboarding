using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Entity
{
    public class PatronPosition
    {
        public int ActiveNum { get; set; }

        public string NtLoginId { get; set; }

        public string PosnId { get; set; }

        public string OrgId { get; set; }

        public string BusinessTitle { get; set; }

        public bool IsPrimary { get; set; }

    }
}

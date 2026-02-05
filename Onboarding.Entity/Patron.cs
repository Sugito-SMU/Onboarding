using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Entity
{
    public class Patron
    {
        public int ActiveNum { get; set; }

        public string PrefName { get; set; }

        public string NtLoginId { get; set; }

        public string SmuEmail { get; set; }

        public bool IsActive { get; set; }

        public List<PatronDept> Departments { get; set; }

        public List<PatronPosition> Positions { get; set; }


    }
}

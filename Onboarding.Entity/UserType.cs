using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Entity
{
    public class UserType
    {
        public string UserTypeCd { get; set; }

        public string UserTypeDesc { get; set; }

        public int Sequence { get; set; }

        public bool IsActive { get; set; }

    }
}

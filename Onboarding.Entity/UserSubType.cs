using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Entity
{
    public class UserSubType
    {
        public string UserTypeCd { get; set; }

        public string UserSubTypeCd { get; set; }

        public string UserSubTypeDesc { get; set; }

        public int Sequence { get; set; }

        public bool IsActive { get; set; }
    }
}

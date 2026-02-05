using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Entity
{
    public class RequestType
    {
        public string ReqTypeCd { get; set; }

        public string ReqTypeDesc { get; set; }

        public int Sequence { get; set; }

        public bool IsActive { get; set; }

    }
}

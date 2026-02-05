using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Entity
{
    public class RequestAttributeValueMap
    {
        public RequestAttributeValueMap(string cd, string desc)
        {
            RequestAttrbValueCd = cd;
            RequestAttrbValueDesc = desc;
        }
        public string RequestAttrbValueCd { get; set; }

        public string RequestAttrbValueDesc { get; set; }



    }
}

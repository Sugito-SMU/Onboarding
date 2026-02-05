using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Entity
{
    public class RequestChangeLog
    {
        public int ReqChangeLogId { get; set; }

        public int ReqId { get; set; }

        public string ChangedBy { get; set; }

        public DateTime ChangedDt { get; set; }

        public string FieldNm { get; set; }

        public string FromVal { get; set; }

        public string ToVal { get; set; }

    }
}

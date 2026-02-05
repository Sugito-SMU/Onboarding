using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Onboarding.Entity
{
    public class BulkRequestSubmitResultRow
    {
        public int RowNum { get; set; }

        public int RequestId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ReqTypeDesc { get; set; }

        public string UserTypeDesc { get; set; }

        public DateTime? EmplStartDt { get; set; }

        public bool IsResultPass { get; set; }

        public List<string> Errors { get; set; }
    }
}

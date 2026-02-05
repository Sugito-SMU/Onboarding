using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Entity
{
    public class TaskSummary
    {
        public int ReqId { get; set; }
        public string ReqTypeCd { get; set; }

        public string ReqTypeDesc { get; set; }
        public string TaskCd { get; set; }

        public string TaskDesc { get; set; }

        public string PrefName { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDt { get; set; }
        public string UserTypeCd { get; set; }

        public string UserTypeDesc { get; set; }

        public string UserSubTypeCd { get; set; }
        public string UserSubTypeDesc { get; set; }
        public string TaskStsCd { get; set; }

        public string TaskStsDesc { get; set; }

        public DateTime EmplStartDt { get; set; }


    }
}

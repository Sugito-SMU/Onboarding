using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Onboarding.Entity;

namespace Onboarding.Models
{
    public class AgentRequestModel
    {
        public string SubmitCommand { get; set; }
        public Request Request { get; set; }
        public int ReqId { get; set; }
        public string TaskCd { get; set; }
        public int TaskId { get; set; }
        public string PhoneModel { get; set; }
        public string PhoneMACAddr { get; set; }
        public string PhoneAddInfo { get; set; }
        public string AssignedPhoneExt { get; set; }
        public string AssignedNTID { get; set; }
        public bool CanModifyAssignedNTID { get; set; }
        public string AssignedEmailDispNm { get; set; }
        public string AssignedEmailAddress { get; set; }
        public bool ViewOnly { get; set; }

    }
}
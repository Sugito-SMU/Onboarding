using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Entity
{
    public class RequestResources
    {
        public bool? IsPCSelected { get; set; }

        public string CorpTitleLevelCd { get; set; }

        public string CorpTitleLevelDesc { get; set; }

        public bool? IsIPPhoneSelected { get; set; }
        
        public string Building { get; set; }

        public string Floor { get; set; }

        public string Room { get; set; }

        public bool? IsNetworkIDSelected { get; set; }
        public string NetworkID { get; set; }
        public bool IsNetworkIdMandatory { get; set; } = false;

        public string NetworkIDRemarks { get; set; }

        public bool? IsEmailSelected { get; set; }

        public bool? IsSAPSelected { get; set; }

        public string DARTFund { get; set; }

        public bool? IsISISSelected { get; set; }

        public string ISISRemarks { get; set; }

        public bool? IsELearnSelected { get; set; }

        public DateTime? ELearnStartDate { get; set; }

        public string ELearnRemarks { get; set; }
        
        public bool? IsMtgRoomSelected { get; set; }

        public string MtgRoomDetails { get; set; }

        public bool? IsINetSelected { get; set; }

        public bool? IsOasisSelected { get; set; }

        public bool? IsNextwebSelected { get; set; }

        public bool? IsEmailDLSelected { get; set; }

        public string EmailDLDetails { get; set; }
        
        public string CreatedBy { get; set; }

        public DateTime CreatedDt { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDt { get; set; }
    }
}

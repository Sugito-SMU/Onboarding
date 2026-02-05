using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Entity
{
    public class CASEntityAUPUser
    {
        public string Domain { get; set; }
        public string PrefNm { get; set; }
        public string NtLoginId { get; set; }
        public DateTime StartDt { get; set; }
        public string PrsnEmail { get; set; }
        public string MobileNo { get; set; }
        public string Otp { get; set; }
        public DateTime? OtpExpiryDt { get; set; }
        public string Pin { get; set; }
        public DateTime? PinExpiryDt { get; set; }
        public DateTime? NotifDt { get; set; }
        public DateTime? AccpDt { get; set; }
        public DateTime? ActivationDt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }
        public bool? IsOtpSent { get; set; }
        public bool IsValidMobileNo { get; set; }
        public bool IsActivated
        {
            get
            {
                return ActivationDt.HasValue;
            }
        }
    }
}

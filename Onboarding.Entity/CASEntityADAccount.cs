using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Onboarding.Entity
{
    public class CASEntityADAccount
    {
        public string Domain { get; set; }
        public string LoginID { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }

        public bool IsExpired { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsLocked { get; set; }
        public string Passwd { get; set; }

        public bool IsSecured()
        {
            return IsValidDomain(Domain) && IsValidLoginID(LoginID);
        }

        private bool IsValidDomain(string input)
        {
            Regex reDomain = new Regex(@"^[a-zA-Z]+$");
            if (!string.IsNullOrWhiteSpace(input))
            {
                return reDomain.IsMatch(input);
            }
            else
            {
                return false;
            }
        }

        private bool IsValidLoginID(string input)
        {
            Regex reLogin = new Regex(@"^[a-zA-Z0-9_\.]+$");
            if (!string.IsNullOrWhiteSpace(input))
            {
                return reLogin.IsMatch(input);
            }
            else
            {
                return false;
            }
        }
    }
}

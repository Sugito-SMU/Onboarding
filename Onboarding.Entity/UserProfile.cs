using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Entity
{
    public class UserProfile
    {
        public string NtLoginId { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public List<SystemRoleUser> SystemRoles { get; set; }

        public bool HasSystemRole(string sysRoleCd)
        {
            if (SystemRoles != null)
            {
                if (SystemRoles.Where(x => x.SysRoleCd.Trim().ToUpper() == sysRoleCd.Trim().ToUpper()).Count() > 0)
                {
                    return true;
                }
            }
            return false;
        }

    }
}

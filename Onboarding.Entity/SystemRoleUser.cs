using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Entity
{
    public class SystemRoleUser
    {
        public string SysRoleCd { get; set; }

        public string UserId { get; set; }

        public List<string> CostCtrCdList { get; set; }

        public string CostCtrString
        {
            get {
                string costCtrStr = string.Empty;
                if (CostCtrCdList != null)
                {
                    foreach (string costCtrCd in CostCtrCdList)
                    {
                        costCtrStr += "|" + costCtrCd.Trim().ToUpper() + "|";
                    }
                }
                return costCtrStr;
            }
        }

        public string UserTypeString
        {
            get
            {
                string usrTypeList = string.Empty;
                if (SysRoleCd.Trim().ToUpper() == Entity.Constant.SystemRole.SchoolAmin)
                {
                    usrTypeList += "|" + Entity.Constant.UserType.Faculty + "|" + Entity.Constant.UserType.SeniorAdmin + "|";
                }
                else if (SysRoleCd.Trim().ToUpper() == Entity.Constant.SystemRole.FAAdmin)
                {
                    usrTypeList += "|" + Entity.Constant.UserType.Faculty + "|";
                }
                return usrTypeList;
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onboarding.Entity;

namespace Onboarding.Common
{
    public class AccessControl
    {

        public static bool HasPermission(Entity.UserProfile userProf, Entity.Request req)
        {
            bool hasPermission = false;
            if (userProf != null)
            {
                if ((userProf.HasSystemRole(Entity.Constant.SystemRole.SuperUser)) ||
                    (userProf.HasSystemRole(Entity.Constant.SystemRole.HRAdmin)) ||
                    (userProf.HasSystemRole(Entity.Constant.SystemRole.SysAdmin)) ||
                    (userProf.HasSystemRole(Entity.Constant.SystemRole.Agent))||
                    (userProf.HasSystemRole(Entity.Constant.SystemRole.NonIITSAgent)))
                {
                    hasPermission = true;
                }
                else if ((userProf.HasSystemRole(Entity.Constant.SystemRole.FAAdmin)))
                {
                    if ((req.EmploymentInfo != null) && (req.EmploymentInfo.UserTypeCd == Entity.Constant.UserType.Faculty))
                    {
                        hasPermission = true;
                    }
                }
                else if ((userProf.HasSystemRole(Entity.Constant.SystemRole.SchoolAmin)))
                {
                    if ((req.EmploymentInfo != null) && (req.EmploymentInfo.CostCtrCd != null))
                    {
                        SystemRoleUser roleUsr = userProf.SystemRoles.Where(x => x.SysRoleCd.Trim().ToUpper() == Entity.Constant.SystemRole.SchoolAmin).FirstOrDefault();
                        if (roleUsr != null)
                        {
                            if (roleUsr.CostCtrCdList.Contains(req.EmploymentInfo.CostCtrCd.Trim().ToUpper()))
                            {
                                hasPermission = true;
                            }
                        }
                    }
                    else
                    {

                        if ((string.IsNullOrEmpty(req.CreatedBy)) || 
                            (req.CreatedBy.Trim().ToLower() == userProf.NtLoginId.Trim().ToLower()))
                        {
                            hasPermission = true;
                        }

                    }
                }
            }
            return hasPermission;
        }

        public static bool HasPermission(Entity.UserProfile userProf, string resourceCd)
        {
            bool hasPermission = false;
            if (userProf != null)
            {
                switch (resourceCd)
                {
                    case Entity.Constant.AppResource.AccessAllCostCentre:
                        hasPermission = (userProf.HasSystemRole(Entity.Constant.SystemRole.SuperUser)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.FAAdmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.HRAdmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.SysAdmin))||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.Agent)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.NonIITSAgent));
                        break;
                    case Entity.Constant.AppResource.AccessAllUserType:
                        hasPermission = (userProf.HasSystemRole(Entity.Constant.SystemRole.SuperUser)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.HRAdmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.SysAdmin))||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.Agent)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.NonIITSAgent));
                        break;
                    case Entity.Constant.AppResource.AppRequestorDashboard:
                        hasPermission = (userProf.HasSystemRole(Entity.Constant.SystemRole.SuperUser)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.SchoolAmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.HRAdmin));
                        break;
                    case Entity.Constant.AppResource.AppAgentDashboard:
                        hasPermission = (userProf.HasSystemRole(Entity.Constant.SystemRole.SuperUser)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.Agent));
                        break;
                    case Entity.Constant.AppResource.AppRequestListForRequestor:
                        hasPermission = (userProf.HasSystemRole(Entity.Constant.SystemRole.SuperUser)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.SchoolAmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.HRAdmin));
                        break;
                    case Entity.Constant.AppResource.AppRequestListForAgent:
                        hasPermission = (userProf.HasSystemRole(Entity.Constant.SystemRole.SuperUser)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.Agent));
                        break;
                    case Entity.Constant.AppResource.AppCreateRequest:
                        hasPermission = (userProf.HasSystemRole(Entity.Constant.SystemRole.SuperUser)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.SchoolAmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.HRAdmin));
                        break;
                    case Entity.Constant.AppResource.AppRequestForm:
                        hasPermission = (userProf.HasSystemRole(Entity.Constant.SystemRole.SuperUser)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.SchoolAmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.FAAdmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.HRAdmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.SysAdmin))||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.NonIITSAgent));
                        break;
                    case Entity.Constant.AppResource.AppAgentForm:
                        hasPermission = (userProf.HasSystemRole(Entity.Constant.SystemRole.SuperUser)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.SchoolAmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.FAAdmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.HRAdmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.SysAdmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.Agent)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.NonIITSAgent));
                        break;
                    case Entity.Constant.AppResource.AppBulkRequest:
                        hasPermission = (userProf.HasSystemRole(Entity.Constant.SystemRole.SuperUser)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.SchoolAmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.HRAdmin));
                        break;
                    case Entity.Constant.AppResource.AppGeneralReport:
                        hasPermission = (userProf.HasSystemRole(Entity.Constant.SystemRole.SuperUser)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.SchoolAmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.FAAdmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.HRAdmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.SysAdmin))||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.Agent)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.NonIITSAgent));
                        break;
                    case Entity.Constant.AppResource.AppRequestSearch:
                        hasPermission = (userProf.HasSystemRole(Entity.Constant.SystemRole.SuperUser)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.SchoolAmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.FAAdmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.HRAdmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.SysAdmin));
                        break;
                    case Entity.Constant.AppResource.ApiBulkRequest:
                        hasPermission = (userProf.HasSystemRole(Entity.Constant.SystemRole.SuperUser)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.SchoolAmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.HRAdmin));
                        break;
                    case Entity.Constant.AppResource.ApiGeneralReport:
                        hasPermission = (userProf.HasSystemRole(Entity.Constant.SystemRole.SuperUser)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.SchoolAmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.FAAdmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.HRAdmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.SysAdmin))||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.Agent)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.NonIITSAgent));
                        break;
                    case Entity.Constant.AppResource.ApiRequest:
                        hasPermission = (userProf.HasSystemRole(Entity.Constant.SystemRole.SuperUser)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.SchoolAmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.FAAdmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.HRAdmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.SysAdmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.Agent))||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.NonIITSAgent));
                        ;
                        break;
                    case Entity.Constant.AppResource.ApiRequestListForRequestor:
                        hasPermission = (userProf.HasSystemRole(Entity.Constant.SystemRole.SuperUser)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.SchoolAmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.HRAdmin));
                        break;
                    case Entity.Constant.AppResource.ApiRequestListForAgent:
                        hasPermission = (userProf.HasSystemRole(Entity.Constant.SystemRole.SuperUser)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.Agent));
                        break;
                    case Entity.Constant.AppResource.ApiRequestorList:
                        hasPermission = (userProf.HasSystemRole(Entity.Constant.SystemRole.SuperUser)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.SchoolAmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.HRAdmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.FAAdmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.SysAdmin))||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.Agent)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.NonIITSAgent));
                        break;
                    case Entity.Constant.AppResource.ApiRequestSearch:
                        hasPermission = (userProf.HasSystemRole(Entity.Constant.SystemRole.SuperUser)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.SchoolAmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.FAAdmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.HRAdmin)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.SysAdmin));
                        break;
                    case Entity.Constant.AppResource.ApiTask:
                        hasPermission = (userProf.HasSystemRole(Entity.Constant.SystemRole.SuperUser)) ||
                                        (userProf.HasSystemRole(Entity.Constant.SystemRole.Agent));
                        break;

                    default:
                        hasPermission = false;
                        break;
                }
            }
            return hasPermission;
        }

        public static string GetAllowedCostCentreString(Entity.UserProfile userProf)
        {
            string costCtrStr = string.Empty;
            foreach (SystemRoleUser role in userProf.SystemRoles)
            {
                if (!string.IsNullOrEmpty(role.CostCtrString))
                {
                    costCtrStr = role.CostCtrString;
                }
            }
            return costCtrStr;
        }

        public static string GetAllowedUserTypeString(Entity.UserProfile userProf)
        {
            string usrTypeStr = string.Empty;
            foreach (SystemRoleUser role in userProf.SystemRoles)
            {
                if (!string.IsNullOrEmpty(role.UserTypeString))
                {
                    usrTypeStr = role.UserTypeString;
                }
            }
            return usrTypeStr;
        }


    }
}

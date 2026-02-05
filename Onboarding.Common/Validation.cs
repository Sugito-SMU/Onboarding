using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Onboarding.Entity;

namespace Onboarding.Common
{
    public class Validation
    {

        public static bool IsHiddenField(string fieldCode, Request request)
        {
            bool isHidden = false;

            return isHidden;
        }

        public static bool IsUrgentRequest(string requestStatusCd, DateTime? employmentStartDt)
        {
            double urgentRequestCutoff = 7;
            if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["UrgentRequestCutOff"]))
            {
                double.TryParse(ConfigurationManager.AppSettings["UrgentRequestCutOff"], out urgentRequestCutoff);
            }

            if ((requestStatusCd != Entity.Constant.RequestStatusCode.Draft) && (employmentStartDt != null))
            {
                if (employmentStartDt.Value.Date <= DateTime.Now.Date.AddDays(urgentRequestCutoff))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsOfficeTask(string taskCd)
        {
            if ((taskCd == Entity.Constant.TaskCode.AssignPCNB) ||
                    (taskCd == Entity.Constant.TaskCode.AssignPhone) ||
                    (taskCd == Entity.Constant.TaskCode.ConfigurePhone) ||                    
                    (taskCd == Entity.Constant.TaskCode.UpdatePhoneOutlook) ||                                    
                    (taskCd == Entity.Constant.TaskCode.VerifyAllTasksCmpl)||
                    (taskCd == Entity.Constant.TaskCode.DeployIPPhone))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsPermStaff(string userSubTypeCd)
        {
            bool permStf = false;
            if (!string.IsNullOrWhiteSpace(userSubTypeCd))
            {
                switch (userSubTypeCd.Trim().ToUpper())
                {
                    case Entity.Constant.UserSubType.SeniorFacultyAdmin:
                        permStf = true;
                        break;
                    case Entity.Constant.UserSubType.AdminPermanent:
                        permStf = true;
                        break;
                    case Entity.Constant.UserSubType.AdminInstructorPermanent:
                        permStf = true;
                        break;
                    case Entity.Constant.UserSubType.FacultyFullTimeStanding:
                        permStf = true;
                        break;
                    case Entity.Constant.UserSubType.FacultyFullTimeSupport:
                        permStf = true;
                        break;
                    case Entity.Constant.UserSubType.FacultyFullTimeVisitingMoreThanEqual12:
                        permStf = true;
                        break;
                    case Entity.Constant.UserSubType.FacultyFullTimePractice:
                        permStf = true;
                        break;
                    case Entity.Constant.UserSubType.FacultyFullTimeEducation:
                        permStf = true;
                        break;
                    case Entity.Constant.UserSubType.FacultyFullTimeDistinguishedTerm:
                        permStf = true;
                        break;
                    case Entity.Constant.UserSubType.FacultyFullTimeResearch:
                        permStf = true;
                        break;
                    case Entity.Constant.UserSubType.FacultyFullTimeAdministrator:
                        permStf = true;
                        break;
                    case Entity.Constant.UserSubType.FacultyFullTimePostRetirement:
                        permStf = true;
                        break;
                    case Entity.Constant.UserSubType.FacultyFullTimeFDS:
                        permStf = true;
                        break;
                    case Entity.Constant.UserSubType.FacultyFullTimePostDoc:
                        permStf = true;
                        break;
                    case Entity.Constant.UserSubType.FacultyFullTimeTeaching:
                        permStf = true;
                        break;
                    case Entity.Constant.UserSubType.FacultyFullTimeEmeritus:
                        permStf = true;
                        break;
                    default:
                        break;
                }
            }
            return permStf;
        }


    }
}

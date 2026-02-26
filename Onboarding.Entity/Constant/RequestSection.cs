using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Entity.Constant
{
    public static class RequestSection
    {
        public const string PersonalInfo = "PersonalInfo";
        public const string EmploymentInfo = "EmploymentInfo";
        public const string OfficeLogistics = "OfficeLogistics";
        public const string SystemAccess = "SystemAccess";

        public static Dictionary<string, string> Fields = new Dictionary<string, string>()
        {
            { "RReqTypeCd", "PersonalInfo" },
            { "RFirstName", "PersonalInfo" },
            { "RLastName", "PersonalInfo" },
            { "RPrefNm", "PersonalInfo" },
            { "RSalutCd", "PersonalInfo" },
            { "RGenderCd", "PersonalInfo" },
            { "RRaceCd", "PersonalInfo" },
            { "RPrsnEmail", "PersonalInfo" },
            { "RMobileNo", "PersonalInfo" },
            { "RIsExistingStaff", "PersonalInfo" },
            { "RIsExSmuStd", "PersonalInfo" },

            { "RUserTypeCd", "EmploymentInfo" },
            { "RUserSubTypeCd", "EmploymentInfo" },
            { "RJobTitle", "EmploymentInfo" },
            { "RCostCtrCd", "EmploymentInfo" },
            { "ROrgCd", "EmploymentInfo" },
            { "REmplStartDt", "EmploymentInfo" },
            { "REmplEndDt", "EmploymentInfo" },
            { "RRemark", "EmploymentInfo" },
            { "RIsPCSelected", "OfficeLogistics" },
            { "RCorpTitleLevel", "OfficeLogistics" },
            { "RIsIPPhoneSelected", "OfficeLogistics" },            
            { "RBuilding", "OfficeLogistics" },
            { "RFloor", "OfficeLogistics" },
            { "RRoom", "OfficeLogistics" },

            { "RIsNetworkIDSelected", "SystemAccess" },
            { "RIsEmailSelected", "SystemAccess" },
            { "RIsSAPSelected", "SystemAccess" },
            { "RDARTFund", "SystemAccess" },
            { "RIsISISSelected", "SystemAccess" },
            { "RISISRemarks", "SystemAccess" },
            { "RIsELearnSelected", "SystemAccess" },
            { "RELearnStartDate", "SystemAccess" },
            { "RELearnRemarks", "SystemAccess" },
            { "RIsMtgRoomSelected", "SystemAccess" },
            { "RMtgRoomDetails", "SystemAccess" },
            { "RIsINetSelected", "SystemAccess" },
            { "RIsOasisSelected", "SystemAccess" },
            { "RIsNextwebSelected", "SystemAccess" },
            { "RIsEmailDLSelected", "SystemAccess" },
            { "REmailDLDetails", "SystemAccess" },
            { "RSupportingDocument", "SystemAccess" },
            { "RNetworkId", "SystemAccess" }
        };

        public static string GetSection(string field)
        {
            return Fields[field];
        }

    }
}

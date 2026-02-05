using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Entity.Constant
{

    public class ExcelColumnName
    {
        public static int ColumnCount
        {
            get
            {
                return ColumnNames.Keys.Count();
            }
        }

        public static Dictionary<int, string> ColumnNames = new Dictionary<int, string>()
        {
            {ExcelColumnNameVal.RequestType, "Request Type" },
            {ExcelColumnNameVal.FirstName, "First Name" },
            {ExcelColumnNameVal.LastName, "Last Name" },
            {ExcelColumnNameVal.PrefNm, "Known As" },
            {ExcelColumnNameVal.Salutation, "Salutation" },
            {ExcelColumnNameVal.Gender, "Gender" },
            {ExcelColumnNameVal.EthnicOrigin, "Ethnic Origin" },
            {ExcelColumnNameVal.PersonalEmail, "Personal Email" },
            {ExcelColumnNameVal.MobileNo, "Personal Mobile No" },
            {ExcelColumnNameVal.IsExistingStaff, "Is Existing Staff" },
            {ExcelColumnNameVal.IsExSMUStudent, "Is Existing SMU/Ex-SMU Student" },
            {ExcelColumnNameVal.UserType, "Staff Type" },
            {ExcelColumnNameVal.UserSubType, "Staff Subtype" },
            {ExcelColumnNameVal.BusinessTitle, "Business Title" },
            {ExcelColumnNameVal.Remarks, "Remarks" },
            {ExcelColumnNameVal.CostCentre, "Cost Centre" },
            {ExcelColumnNameVal.EmploymentStartDate, "Employment Start Date" },
            {ExcelColumnNameVal.EmploymentEndDate, "Employment End Date" },
            {ExcelColumnNameVal.IsPCNotebookRequired, "Is PC / Notebook Required" },
            {ExcelColumnNameVal.CorpTitleLevel, "Job Level Indicator" },
            {ExcelColumnNameVal.IsIPPhoneRequired, "Is IP Phone Required" },            
            {ExcelColumnNameVal.Building, "Building" },
            {ExcelColumnNameVal.Floor, "Floor" },
            {ExcelColumnNameVal.Room, "Room" },
            {ExcelColumnNameVal.IsEmailRequired, "Is Email Required" },
            {ExcelColumnNameVal.IsSAPRequired, "Is SAP Required" },
            {ExcelColumnNameVal.DARTFund, "DART Fund" },
            {ExcelColumnNameVal.IsISISRequired, "Is ISIS Required" },
            {ExcelColumnNameVal.ISISRemarks, "ISIS Remarks" },
            {ExcelColumnNameVal.IseLearnEarlyAccessRequired, "Is eLearn Early Access Required" },
            {ExcelColumnNameVal.eLearnStartDate, "eLearn Start Date" },
            {ExcelColumnNameVal.eLearnRemarks, "eLearn Remarks" },
            {ExcelColumnNameVal.IsAdditionalMeetingRoomBookingAccessRequired, "Is Additional Meeting Room Booking Access Required" },
            {ExcelColumnNameVal.AdditionalMeetingRoomBookingDetails, "Additional Meeting Room Booking Details" },
            {ExcelColumnNameVal.IsCMSiNetRequired, "Is CMS – iNet Required" },
            {ExcelColumnNameVal.IsCMSOasisRequired, "Is CMS – Oasis Required" },
            {ExcelColumnNameVal.IsCMSNextwebRequired, "Is CMS – Nextweb Required" },
            {ExcelColumnNameVal.IsEmailDistributionListRequired, "Is Email Distribution List Required" },
            {ExcelColumnNameVal.EmailDistributionListDetails, "Email Distribution List Details" }
        };
    }

    public class ExcelColumnNameVal
    {
        public const int RequestType = 0;
        public const int FirstName = 1;
        public const int LastName = 2;
        public const int PrefNm = 3;
        public const int Salutation = 4;
        public const int Gender = 5;
        public const int EthnicOrigin = 6;
        public const int PersonalEmail = 7;
        public const int MobileNo = 8;
        public const int IsExistingStaff = 9;
        public const int IsExSMUStudent = 10;
        public const int UserType = 11;
        public const int UserSubType = 12;
        public const int BusinessTitle = 13;
        public const int Remarks = 14;
        public const int CostCentre = 15;
        public const int EmploymentStartDate = 16;
        public const int EmploymentEndDate = 17;
        public const int IsPCNotebookRequired = 18;
        public const int CorpTitleLevel = 19;
        public const int IsIPPhoneRequired = 20;
        public const int Building = 21;
        public const int Floor = 22;
        public const int Room = 23;
        public const int IsEmailRequired = 24;
        public const int IsSAPRequired = 25;
        public const int DARTFund = 26;
        public const int IsISISRequired = 27;
        public const int ISISRemarks = 28;
        public const int IseLearnEarlyAccessRequired = 29;
        public const int eLearnStartDate = 30;
        public const int eLearnRemarks = 31;
        public const int IsAdditionalMeetingRoomBookingAccessRequired = 32;
        public const int AdditionalMeetingRoomBookingDetails = 33;
        public const int IsCMSiNetRequired = 34;
        public const int IsCMSOasisRequired = 35;
        public const int IsCMSNextwebRequired = 36;
        public const int IsEmailDistributionListRequired = 37;
        public const int EmailDistributionListDetails = 38;
    }
}

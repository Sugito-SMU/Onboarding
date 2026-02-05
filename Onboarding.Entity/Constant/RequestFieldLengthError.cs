using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Entity.Constant
{
    public static class RequestFieldLengthError
    {

        public static string FirstName = string.Format("First Name cannot be more than {0} characters", RequestFieldLength.GetLength(RequestField.FirstName));
        public static string LastName = string.Format("Last Name cannot be more than {0} characters", RequestFieldLength.GetLength(RequestField.LastName));
        public static string PrefNm = string.Format("Known As cannot be more than {0} characters", RequestFieldLength.GetLength(RequestField.PrefNm));
        public static string PrsnEmail = string.Format("Personal Email cannot be more than {0} characters", RequestFieldLength.GetLength(RequestField.PrsnEmail));
        public static string MobileNo = string.Format("Personal Mobile cannot be more than {0} characters", RequestFieldLength.GetLength(RequestField.MobileNo));
        public static string JobTitle = string.Format("Business Title cannot be more than {0} characters", RequestFieldLength.GetLength(RequestField.JobTitle));
        public static string Remarks = string.Format("Remarks cannot be more than {0} characters", RequestFieldLength.GetLength(RequestField.Remarks));
        public static string Building = string.Format("Building cannot be more than {0} characters", RequestFieldLength.GetLength(RequestField.Building));
        public static string Floor = string.Format("Floor cannot be more than {0} characters", RequestFieldLength.GetLength(RequestField.Floor));
        public static string Room = string.Format("Room cannot be more than {0} characters", RequestFieldLength.GetLength(RequestField.Room));
        public static string DARTFund = string.Format("DART Fund Code cannot be more than {0} characters", RequestFieldLength.GetLength(RequestField.DARTFund));
        public static string ISISRemarks = string.Format("ISIS Remarks cannot be more than {0} characters", RequestFieldLength.GetLength(RequestField.ISISRemarks));
        public static string ELearnRemarks = string.Format("eLearn Remarks cannot be more than {0} characters", RequestFieldLength.GetLength(RequestField.ELearnRemarks));
        public static string MtgRoomDetails = string.Format("Additional Meeting Room Booking Details cannot be more than {0} characters", RequestFieldLength.GetLength(RequestField.MtgRoomDetails));
        public static string EmailDLDetails = string.Format("Email Distribution List Details cannot be more than {0} characters", RequestFieldLength.GetLength(RequestField.EmailDLDetails));

    }
}

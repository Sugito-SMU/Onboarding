using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onboarding;

namespace Onboarding.DataProvider.Mapper
{
    public static partial class MapToDb
    {
        public static bool IsLogValueDifferent(string fromValue, string toValue)
        {
            string fromVal = null;
            if (!string.IsNullOrWhiteSpace(fromValue))
            {
                fromVal = fromValue.Trim();
            }
            string toVal = null;
            if (!string.IsNullOrWhiteSpace(toValue))
            {
                toVal = toValue.Trim();
            }
            if (fromVal != toVal)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string FormatLogDesc(string value, string description)
        {
            string formattedDesc = null;
            if (!string.IsNullOrWhiteSpace(description))
            {
                formattedDesc = string.Format("{0} ({1})", description, value);
            }
            else
            {
                formattedDesc = string.Format("{0}", value);
            }
            return formattedDesc;
        }

        public static string FormatLogValue(bool? value)
        {
            string formattedValue = string.Empty;
            if (value == true)
            {
                formattedValue = "Y";
            }
            else if (value == false)
            {
                formattedValue = "N";
            }
            return formattedValue;
        }

        public static string FormatLogValue(DateTime? value)
        {
            string formattedValue = string.Empty;
            if (value != null)
            {
                formattedValue = ((DateTime)value).ToString("dd MMM yyyy HH:mm:ss");
            }
            return formattedValue;
        }

    }
}

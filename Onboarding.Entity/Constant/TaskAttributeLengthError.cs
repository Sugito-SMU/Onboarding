using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Entity.Constant
{
    public static class TaskAttributeLengthError
    {
        public static string GetErrorMessage(string taskAttrCode)
        {
            return string.Format("The above entry cannot be more than {0} characters", TaskAttributeLength.GetLength(taskAttrCode));
        }

        public static string Remark = string.Format("The above entry cannot be more than {0} characters", TaskAttributeLength.GetRemarkLength());

    }
}

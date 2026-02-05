using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Entity.Constant
{
    public static class TaskAttributeLength
    {
        public static int GetLength(string taskAttrCode)
        {
            switch (taskAttrCode)
            {
                case TaskAttributeCode.AssignedNTID:
                    return 12;
                default:
                    return 600;
            }
        }

        public static int GetRemarkLength()
        {
            return 600;
        }
    }
}

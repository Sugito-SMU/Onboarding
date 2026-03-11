using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Entity.Constant
{
    public static class RequestFieldLength
    {

        public static int GetLength(string requestFieldCode)
        {
            switch (requestFieldCode)
            {
                case RequestField.FirstName:
                    return 80;
                case RequestField.LastName:
                    return 40;
                case RequestField.PrefNm:
                    return 40;
                case RequestField.JobTitle:
                    return 64;
                case RequestField.MobileNo:
                    return 20;
                case RequestField.NetworkId:
                    return 12;
                default:
                    return 600;
            }
        }



    }
}

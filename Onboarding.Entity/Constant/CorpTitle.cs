using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Entity.Constant
{
    public static class CorpTitle
    {

        public const string AsstDirAbvFacInstr = "ASST_DIR_ABV_FAC_INSTR";
        public const string SrMgrBlo = "SR_MGR_BLO";

        public static List<RequestAttributeValueMap> MapList = new List<RequestAttributeValueMap>()
        {
            new RequestAttributeValueMap(AsstDirAbvFacInstr, "Assistant Director and above/Faculty/Instructors"),
            new RequestAttributeValueMap(SrMgrBlo, "Senior Manager and below")
        };

        public static string GetCode(string description)
        {
            string cd = null;
            if (!string.IsNullOrWhiteSpace(description))
            {
                RequestAttributeValueMap map = MapList.Where(x => x.RequestAttrbValueDesc.Trim().ToLower() == description.Trim().ToLower()).FirstOrDefault();
                if (map != null)
                {
                    cd = map.RequestAttrbValueCd;
                }
            }
            return cd;
        }

        public static string GetDescription(string code)
        {
            string desc = null;
            if (!string.IsNullOrWhiteSpace(code))
            {
                RequestAttributeValueMap map = MapList.Where(x => x.RequestAttrbValueCd.Trim().ToLower() == code.Trim().ToLower()).FirstOrDefault();
                if (map != null)
                {
                    desc = map.RequestAttrbValueDesc;
                }
            }
            return desc;
        }

    }
}

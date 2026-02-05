using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Entity
{
    public class RequestAttachment
    {
        public int ReqAttachId { get; set; }

        public int ReqId { get; set; }

        public string Filename { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDt { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDt { get; set; }


    }
}

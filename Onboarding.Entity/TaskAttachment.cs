using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Entity
{
    public class TaskAttachment
    {
        public int TaskAttachId { get; set; }

        public int TaskId { get; set; }

        public string Filename { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDt { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDt { get; set; }


    }
}

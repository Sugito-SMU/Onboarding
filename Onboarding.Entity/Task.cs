using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Entity
{
    public class Task
    {
        public int TaskId { get; set; }

        public int ReqId { get; set; }

        public string TaskStsCd { get; set; }

        public string TaskStsDesc { get; set; }

        public string TaskCd { get; set; }

        public string TaskDesc { get; set; }

        public string AssignedAgent { get; set; }

        public DateTime? AssignedDt { get; set; }

        public DateTime? CompletedDt { get; set; }

        public string CompletedByName { get; set; }

        public string Remark { get; set; }

        public bool? IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDt { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDt { get; set; }

        public List<TaskAttribute> Attributes { get; set; }

        public List<TaskChangeLog> ChangeList { get; set; }

        public TaskAttachment Attachment { get; set; }

        public TaskValidationError ValidationError { get; set; }

        public TaskAttribute GetTaskAttribute(string taskAttrbCd)
        {
            if (Attributes != null)
            {
                return Attributes.Where(x => x.TaskAttrbCd == taskAttrbCd).FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        public bool IsNew
        {
            get
            {
                return (TaskStsCd == Constant.TaskStatusCode.New) ? true : false;
            }
        }

        public bool IsPending
        {
            get
            {
                return (TaskStsCd == Constant.TaskStatusCode.Pending) ? true : false;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return (TaskStsCd == Constant.TaskStatusCode.Completed) ? true : false;
            }
        }

        public bool IsMyTask { get; set; }


    }
}

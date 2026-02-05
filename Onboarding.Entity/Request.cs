using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Entity
{
    public class Request
    {
        public int ReqId { get; set; }

        public string ReqTypeCd { get; set; }

        public string ReqTypeDesc { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PrefNm { get; set; }

        public string SalutCd { get; set; }

        public string SalutDesc { get; set; }

        public string GenderCd { get; set; }

        public string GenderDesc { get; set; }

        public string RaceCd { get; set; }

        public string RaceDesc { get; set; }

        public string PrsnEmail { get; set; }
        public string MobileNo { get; set; }
        public bool? IsExistingStaff { get; set; }

        public bool? IsExSmuStd { get; set; }

        public string Remark { get; set; }

        public string ReqStsCd { get; set; }

        public string ReqStsDesc { get; set; }

        public DateTime? SubmittedDt { get; set; }

        public String SubmittedBy { get; set; }

        public String SubmittedByName { get; set; }

        public bool? IsAmended { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDt { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDt { get; set; }

        public RequestEmployment EmploymentInfo { get; set; }

        public RequestResources Resources { get; set; }

        public List<Task> Tasks { get; set; }

        public RequestAttachment Attachment { get; set; }

        public List<RequestChangeLog> ReqChangeList { get; set; }

        public List<TaskChangeLog> TaskChangeList { get; set; }

        public RequestValidation Validation { get; set; }

        public Task GetTask(string taskCd)
        {
            if (Tasks != null)
            {
                return Tasks.Where(x => x.TaskCd == taskCd).FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        public bool IsDraft
        {
            get
            {
                return (ReqStsCd == Constant.RequestStatusCode.Draft) ? true : false;
            }
        }

        public bool IsSubmitted
        {
            get
            {
                return (ReqStsCd == Constant.RequestStatusCode.Submitted) ? true : false;
            }
        }

        public bool IsCancelled
        {
            get
            {
                return (ReqStsCd == Constant.RequestStatusCode.Cancelled) ? true : false;
            }
        }


        public bool IsClosed
        {
            get
            {
                return (ReqStsCd == Constant.RequestStatusCode.Closed) ? true : false;
            }
        }

        public bool HasError()
        {
            if (this.Validation != null)
            {
                return this.Validation.HasError();
            }
            else
            {
                return false;
            }
        }

        public bool HasError(string requestField)
        {
            if (this.Validation != null)
            {
                return this.Validation.HasError(requestField);
            }
            else
            {
                return false;
            }
        }

        public bool HasPersonalInfoError()
        {
            if ((this.Validation != null) && 
                (this.Validation.HasSectionError(Entity.Constant.RequestSection.PersonalInfo)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool HasEmploymentInfoError()
        {
            if ((this.Validation != null) &&
                (this.Validation.HasSectionError(Entity.Constant.RequestSection.EmploymentInfo)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool HasOfficeLogisticsError()
        {
            if ((this.Validation != null) &&
                (this.Validation.HasSectionError(Entity.Constant.RequestSection.OfficeLogistics)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool HasSystemAccessError()
        {
            if ((this.Validation != null) &&
                (this.Validation.HasSectionError(Entity.Constant.RequestSection.SystemAccess)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetErrorMessage(string requestField)
        {
            if (this.Validation != null)
            {
                return this.Validation.GetErrorMessage(requestField);
            }
            else
            {
                return null;
            }
        }
    }
}

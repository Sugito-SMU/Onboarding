using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Onboarding.Entity;
namespace Onboarding.Models
{
    public class SystemAccess
    {
        public Entity.Request Request { get; set; }
        public bool IsNetworkIDSelected { get; set; }
        public string NetworkID { get; set; }
        public string NetworkIDRemarks { get; set; }
        public bool IsEmailSelected { get; set; }
        public bool IsSAPSelected { get; set; }
        public string DARTFund { get; set; }
        public bool IsISISSelected { get; set; }
        public string ISISRemarks { get; set; }
        public bool IsELearnSelected { get; set; }
        public DateTime? ELearnStartDate { get; set; }
        public string ELearnRemarks { get; set; }
        public bool IsMtgRoomSelected { get; set; }
        public string MtgRoomDetails { get; set; }
        public bool IsINetSelected { get; set; }
        public bool IsOasisSelected { get; set; }
        public bool IsNextwebSelected { get; set; }
        public bool IsEmailDLSelected { get; set; }
        public string EmailDLDetails { get; set; }
        public bool HasAttachment { get; set; }

        public RequestAttachmentModel Attachment { get; set; }
        public HttpPostedFileBase AttachmentFile { get; set; }
        public bool IsAttachmentDeleted { get; set; }

        public bool IsAmended { get; set; }
        public bool IsViewMode { get; set; }
        public bool IsSaveAllowed { get; set; }
        public bool IsAmendAllowed { get; set; }
        public bool IsCancelAllowed { get; set; }
        public bool IsSubmitAllowed { get; set; }
        public bool IsDeleteAllowed { get; set; }
        public bool IsHRAdmin { get; set; }

    }
    public class RequestAttachmentModel
    {
        public int ReqId { get; set; }
        public int ReqAttachId { get; set; }
        public string Filename { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Onboarding.Entity;

namespace Onboarding.Models
{
    public class BulkRequestModel
    {
        public int RowCount { get; set; }
        public string SubmitCommand { get; set; }

        public string UploadedFilename { get; set; }

        public bool IsUploadSuccessful { get; set; }

        public bool IsSubmitSuccessful { get; set; }

        public bool IsRowValidationError { get; set; }

        public string ErrorMessage { get; set; }

        public HttpPostedFileBase BulkRequestFile { get; set; }
    }
}
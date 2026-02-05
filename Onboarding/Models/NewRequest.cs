using System;
using System.Collections.Generic;
using Onboarding.Entity;
using System.ComponentModel.DataAnnotations;

namespace Onboarding.Models
{
    public class NewRequest
    {
        
        public string ReqTypeCd { get; set; }
        [Required(ErrorMessage = "Request Type is required.")]
        public string SelectedReqTypeCd { get; set; }
        public List<RequestType> RequestTypes { get; set; }

    }
}
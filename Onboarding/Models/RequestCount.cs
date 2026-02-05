using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Onboarding.Entity;
namespace Onboarding.Models
{
    public class RequestCount
    {
        public string ReqStsCd { get; set; }

        public string ReqStsDesc { get; set; }

        public int ReqCount { get; set; }

        public List<Entity.RequestCountForRequestor> RequestCounts { get; set; }


        public int ReqDraftCount { get; set; }
        public int ReqSubACount { get; set; }
        public int ReqClsCount { get; set; }
        public int ReqCanCount { get; set; }
    }
}
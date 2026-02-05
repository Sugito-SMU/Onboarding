using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Onboarding.Models
{
    public class OfficeLog
    {
        public Entity.Request Request { get; set; }

        public Boolean IsPCSelected { get; set; }

        public string CorpTitleLevelCd { get; set; }

        public string CorpTitleLevelDesc { get; set; }

        public Boolean IsIPPhoneSelected { get; set; }
        
        public string Building { get; set; }

        public string Floor { get; set; }

        public string Room { get; set; }

        public bool IsAmended { get; set; }

        public Boolean IsViewMode { get; set; }

        public Boolean IsSaveAllowed { get; set; }

        public Boolean IsAmendAllowed { get; set; }

    }


}
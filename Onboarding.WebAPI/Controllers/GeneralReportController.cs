using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Onboarding.ServiceManager;
using Onboarding.Entity;
using Onboarding.Common;


namespace Onboarding.WebAPI.Controllers
{
    public class GeneralReportController : ApiController
    {
        
        [HttpPost]
        [Route("api/GeneralReport/{userId}")]
        [ResponseType(typeof(GenRptSearchResult))]
        public IHttpActionResult Post([FromBody]RptSearchCriteria searchCriteria, string userId)
        {
            if (Utility.HasPermission(userId, Entity.Constant.AppResource.ApiGeneralReport))
            {
                GeneralReportManager rptMgr = new GeneralReportManager();
                GenRptSearchResult rpt = rptMgr.GetReport(searchCriteria, userId);
                return Ok(rpt);
            }
            else
            {
                return Unauthorized();
            }
        }

    }
}

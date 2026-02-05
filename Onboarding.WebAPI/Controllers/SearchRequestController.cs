using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Onboarding.ServiceManager;
using Onboarding.Entity;


namespace Onboarding.WebAPI.Controllers
{
    public class SearchRequestController : ApiController
    {
        
        [HttpPost]
        [Route("api/SearchRequest/{userId}")]
        [ResponseType(typeof(RequestSearchResult))]
        public IHttpActionResult Get([FromBody]RptSearchCriteria searchCriteria, string userId)
        {
            if (Utility.HasPermission(userId, Entity.Constant.AppResource.ApiRequestSearch))
            {
                SearchRequestManager mgr = new SearchRequestManager();
                RequestSearchResult searchResult = mgr.SearchRequest(searchCriteria, userId);
                return Ok(searchResult);
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}

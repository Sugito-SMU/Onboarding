using Onboarding.App_Start;
using Owin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Onboarding
{
    public class MvcApplication : System.Web.HttpApplication
    {
        string username = string.Empty;
        protected void Application_Start()
        {                  
                
                AreaRegistration.RegisterAllAreas();
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                MVCGridConfig.RegisterGeneralReportGrid();
                MVCGridConfig.RegisterMyRequestGrid();
                MVCGridConfig.RegisterRequestListForAgentGrid();
                MVCGridConfig.RegisterRequestListForAgentGridCompleted();
                MVCGridConfig.RegisterSearchRequestGrid();

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                //AntiForgeryConfig.UniqueClaimTypeIdentifier = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";// "name";

                AntiForgeryConfig.UniqueClaimTypeIdentifier = System.Security.Claims.ClaimTypes.NameIdentifier;
            

        }
        public void Configuration(IAppBuilder app)
		{           
            new Startup().Configuration(app);
		}
		protected void Application_Error(object sender, EventArgs e)
        {          
                
                Exception ex = Server.GetLastError();
                username = User.Identity.Name;
                if(!string.IsNullOrWhiteSpace(username))
                    Common.Email.SendEmailForError(ex, Request.Url.AbsoluteUri, username);
                Common.Logger.LogError(ex);
                Server.ClearError();
                Response.Redirect("~/error.htm");            
            
        }
    }
}

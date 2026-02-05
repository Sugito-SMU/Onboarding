using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using System.Configuration;
using System.Net.Mail;

using System.Collections;

namespace Onboarding.WebAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            Common.Email.SendEmailForError(ex, null, null);
            Common.Logger.LogError(ex);
            Server.ClearError();
        }

    }

    
}

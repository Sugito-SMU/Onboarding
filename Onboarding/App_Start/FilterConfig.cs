using System.Web;
using System.Web.Mvc;

namespace Onboarding
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
            //filters.Add(new CustomErrorAttribute());
        }
    }

    public class CustomErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            Common.Email.SendEmailForError(context.Exception, context.HttpContext.Request.Url.AbsoluteUri, context.HttpContext.User.Identity.Name);
            Common.Logger.LogError(context.Exception);
            HttpContext.Current.Response.Redirect("~/error.htm");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Onboarding;

public static class HtmlHelpers
{
    public static MvcHtmlString DateFormat(this HtmlHelper helper, DateTime? src)
    {
        if (src == null)
            return MvcHtmlString.Create("");
        string Date = src.Value.ToString("dd-MMM-yyyy");
        return MvcHtmlString.Create(Date);
    }
    public static MvcHtmlString DateFormatSpace(this HtmlHelper helper, DateTime? src)
    {
        if (src == null)
            return MvcHtmlString.Create("");
        string Date = src.Value.ToString("dd MMM yyyy");
        return MvcHtmlString.Create(Date);
    }
    public static MvcHtmlString IsNullOrEmpty(this HtmlHelper helper, object Obj)
    {
        if (Obj == null)
            return MvcHtmlString.Create("");
        string Date = Obj.ToString();
        return MvcHtmlString.Create(Date);
    }

    public static MvcHtmlString boolToStirng(this HtmlHelper helper, bool? src, string TrueString, String FalseString)
    {
        if (src == null)
            return MvcHtmlString.Create(FalseString);
        if (src == true)
            return MvcHtmlString.Create(TrueString);
        else
            return MvcHtmlString.Create(FalseString);
    }

    public static void HandleApiResponse(System.Net.Http.HttpResponseMessage respMsg)
    {
        if (respMsg.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            System.Web.HttpContext.Current.Response.Redirect("Unauthorized");
        }
        else
        {
            string errMsg1 = respMsg.Content.ReadAsStringAsync().Result;
            string errMsg2 = errMsg1.Replace(@"\r\n", "<br/>");
            string errMsg3 = errMsg1.Replace(@"\r\n", "\r\n");
            Onboarding.Common.Email.SendEmailForError(errMsg2, HttpContext.Current.Request.Url.AbsoluteUri, HttpContext.Current.User.Identity.Name);
            Onboarding.Common.Logger.LogError(new HttpException(errMsg3));
            HttpContext.Current.Response.Redirect("~/error.htm");
        }
    }
}

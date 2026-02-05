using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Security.Application;
using Onboarding.Entity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;

namespace Onboarding.Helper
{
    public class Utility
    {
        private static string username;

        public static UserProfile GetUserProfile()
        {
            string userId = RemoveUserDomain(Helper.Utility.GetCurrentUserId());
            UserProfile usrProfile = (HttpContext.Current.Session != null) ? (UserProfile)HttpContext.Current.Session["UserProfile"] : null;
            if (usrProfile == null)
            {
                WebAPIManager.WebAPIManager api = new WebAPIManager.WebAPIManager();
                usrProfile = api.GetuserProfile(userId);
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session["UserProfile"] = usrProfile;
                } 
            }
            return usrProfile;
        }

        public static bool HasPermission(string resourceCd)
        {
            return Common.AccessControl.HasPermission(GetUserProfile(), resourceCd);
        }

        public static string RemoveUserDomain(string input)
        {
            return GetAdfsUserId(input);
        }

        public static string GetAdfsUserId(string loginID)
        {
            string userID = null;
            if (!string.IsNullOrWhiteSpace(loginID))
            {
                // For ADFS SSO
                loginID = loginID.ToLower().Replace("staff.smu.edu.sg", "smustf");
                loginID = loginID.ToLower().Replace("student.smu.edu.sg", "smustu");
                if (!loginID.Contains("\\"))
                {
                    Regex reStudent = new Regex(@"\.\d{4}@.*\.smu.edu.sg");
                    if (reStudent.IsMatch(loginID))
                    {
                        loginID = "smustu\\" + loginID;
                    }
                    else
                    {
                        loginID = "smustf\\" + loginID;
                    }
                }
                if (loginID.Contains("@"))
                {
                    Regex reEmailDomain = new Regex(@"@.*smu.edu.sg");
                    loginID = reEmailDomain.Replace(loginID, string.Empty);
                }
                string[] userNameArr = loginID.Split('\\');

                if (userNameArr.Length == 1)
                {
                    userID = Sanitizer.GetSafeHtmlFragment(userNameArr[0]);
                }
                else if (userNameArr.Length == 2)
                {
                    userID = Sanitizer.GetSafeHtmlFragment(userNameArr[1]);
                }
            }
            return userID;
        }

        public static string GetCurrentUserId()
        {
            if (System.Configuration.ConfigurationManager.AppSettings["Debug_User"] != null &&
                !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["Debug_User"].ToString()))
            {
                username = System.Configuration.ConfigurationManager.AppSettings["Debug_User"].ToString();
            }
            else
            {
                if (HttpContext.Current.Request.IsAuthenticated)
                {
                    var userClaims = HttpContext.Current.User.Identity as System.Security.Claims.ClaimsIdentity;

                    username = userClaims?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value.Replace("live.com#", "");
                }
                else
                {
                    string RedirectURI = System.Configuration.ConfigurationManager.AppSettings["RedirectURL"];

                    HttpContext.Current.GetOwinContext().Authentication.Challenge(
                        new AuthenticationProperties { RedirectUri = RedirectURI },
                        OpenIdConnectAuthenticationDefaults.AuthenticationType);

                    HttpContext.Current.Response.End();
                }
            }

            return username;
        }

        public static CostCentre GetCostCentreFromTextInput(string input)
        {
            CostCentre cc = new CostCentre();
            if (!string.IsNullOrWhiteSpace(input))
            {
                WebAPIManager.WebAPIManager api = new WebAPIManager.WebAPIManager();
                List<CostCentre> costCtrList = api.GetCostCentreList();
                cc = costCtrList.Where(x => x.CostCtrDesc.ToLower().Trim() == input.ToLower().Trim()).FirstOrDefault();
                if (cc == null)
                {
                    cc = costCtrList.Where(x => x.CostCtrDesc.ToLower().Trim().EndsWith("(" + input.ToLower().Trim() + ")")).FirstOrDefault();
                }
            }
            return cc;
        }

        public static void SetActionButtons(Request request, dynamic ViewBag, bool prevBtnAllowed, bool nextBtnAllowed)
        {
            ViewBag.IsAmendAllowed = false;
            ViewBag.IsCancelAllowed = false;
            ViewBag.IsDeleteAllowed = false;
            ViewBag.IsSaveAllowed = false;
            ViewBag.IsSubmitAllowed = false;
            ViewBag.IsPrevButtonAllowed = prevBtnAllowed;
            ViewBag.IsNextButtonAllowed = nextBtnAllowed;

            if (!Helper.Utility.HasPermission(Entity.Constant.AppResource.AppCreateRequest))
            {
                ViewBag.IsSaveAllowed = false;
                HttpContext.Current.Session["RequestViewMode"] = true;
                ViewBag.IsAmendAllowed = false;
                ViewBag.IsCancelAllowed = false;
                ViewBag.IsSubmitAllowed = false;
                ViewBag.IsDeleteAllowed = false;
                ViewBag.IsViewMode = true;
            }
            else
            {
                switch (request.ReqStsCd)
                {
                    case Entity.Constant.RequestStatusCode.Draft:
                        ViewBag.IsSaveAllowed = true;
                        HttpContext.Current.Session["RequestViewMode"] = false;
                        ViewBag.IsAmendAllowed = false;
                        ViewBag.IsSubmitAllowed = true;
                        ViewBag.IsViewMode = false;
                        if (request.ReqId > 0)
                        {
                            ViewBag.IsDeleteAllowed = true;
                        }
                        break;
                    case Entity.Constant.RequestStatusCode.Submitted:
                        bool isViewMode = true;
                        if (HttpContext.Current.Session["RequestViewMode"] != null)
                        {
                            isViewMode = (bool)HttpContext.Current.Session["RequestViewMode"];
                        }
                        else
                        {
                            HttpContext.Current.Session["RequestViewMode"] = true;
                        }
                        if (isViewMode)
                        {
                            ViewBag.IsSaveAllowed = false;
                            ViewBag.IsAmendAllowed = true;
                            ViewBag.IsCancelAllowed = true;
                            ViewBag.IsSubmitAllowed = false;
                        }
                        else
                        {
                            ViewBag.IsSaveAllowed = false;
                            ViewBag.IsAmendAllowed = false;
                            ViewBag.IsCancelAllowed = true;
                            ViewBag.IsSubmitAllowed = true;
                        }
                        ViewBag.IsViewMode = isViewMode;
                        ViewBag.IsDeleteAllowed = false;
                        break;
                    case Entity.Constant.RequestStatusCode.Cancelled:
                        ViewBag.IsSaveAllowed = false;
                        HttpContext.Current.Session["RequestViewMode"] = true;
                        ViewBag.IsAmendAllowed = false;
                        ViewBag.IsCancelAllowed = false;
                        ViewBag.IsSubmitAllowed = false;
                        ViewBag.IsDeleteAllowed = false;
                        ViewBag.IsViewMode = true;
                        break;
                    case Entity.Constant.RequestStatusCode.Closed:
                        ViewBag.IsSaveAllowed = false;
                        HttpContext.Current.Session["RequestViewMode"] = true;
                        ViewBag.IsAmendAllowed = false;
                        ViewBag.IsCancelAllowed = false;
                        ViewBag.IsSubmitAllowed = false;
                        ViewBag.IsDeleteAllowed = false;
                        ViewBag.IsViewMode = true;
                        break;
                    default:
                        break;
                }
            }
        }

        public static bool IsRedirectToAgentForm()
        {
            UserProfile usrProf = GetUserProfile();
            if (usrProf.HasSystemRole(Entity.Constant.SystemRole.HRAdmin) ||
                usrProf.HasSystemRole(Entity.Constant.SystemRole.SchoolAmin))
            {
                return false;
            }
            else if (usrProf.HasSystemRole(Entity.Constant.SystemRole.NonIITSAgent) ||
                usrProf.HasSystemRole(Entity.Constant.SystemRole.Agent))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string EncodeNewLine(string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                return input.Replace("\r\n", "<br>");
            }
            else
            {
                return input;
            }
        }

        public static string DecodeNewLine(string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                return input.Replace("\r\n", "").Replace("<br>", "\r\n");
            }
            else
            {
                return input;
            }
        }

        public static void SetMandatoryValues(Entity.Request req, bool isUserTypeChanged)
        {
            if (isUserTypeChanged)
            {
                if ((req.Resources != null) && (req.Resources.IsSAPSelected == true))
                {
                    if (Common.RequestValidation.IsSAPRequired(req.EmploymentInfo.UserSubTypeCd) == false)
                    {
                        req.Resources.IsSAPSelected = false;
                    }
                }
                if ((req.Resources != null) && (req.Resources.IsISISSelected == true))
                {
                    if (Common.RequestValidation.IsISISRequired(req.EmploymentInfo.UserSubTypeCd) == false)
                    {
                        req.Resources.IsISISSelected = false;
                    }
                }
            }

            if (req.EmploymentInfo == null)
            {
                req.EmploymentInfo = new RequestEmployment();
            }

            if (req.Resources == null)
            {
                req.Resources = new RequestResources();
            }

            if (Common.RequestValidation.IsSAPRequired(req.EmploymentInfo.UserSubTypeCd))
            {
                req.Resources.IsSAPSelected = true;
            }

            if (Common.RequestValidation.IsISISRequired(req.EmploymentInfo.UserSubTypeCd))
            {
                req.Resources.IsISISSelected = true;
            }
        }
    }
}
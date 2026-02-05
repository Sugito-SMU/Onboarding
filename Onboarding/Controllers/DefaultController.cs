using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Onboarding.Models;
using Onboarding.Entity;

namespace Onboarding.Controllers
{
	public partial class DefaultController : Controller
    {
        [HttpGet]
        [Authorize]
		public ActionResult DefaultRedirect()
        {           
            UserProfile usrProf = (UserProfile)Session["UserProfile"];
			if (Session["UserProfile"] == null)
			{
				WebAPIManager.WebAPIManager api = new WebAPIManager.WebAPIManager();
                string userId = Helper.Utility.GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return View("Unauthorized");
                }

                usrProf = api.GetuserProfile(userId);
				Session["UserProfile"] = usrProf;
			}

			if ((usrProf != null) && (usrProf.IsActive))
			{
				if (usrProf.HasSystemRole(Entity.Constant.SystemRole.SuperUser))
				{
					return RedirectToAction("Dashboard", "Requestor");
				}
				else if (usrProf.HasSystemRole(Entity.Constant.SystemRole.FAAdmin))
				{
					return RedirectToAction("GeneralReport", "Report");
				}
				else if (usrProf.HasSystemRole(Entity.Constant.SystemRole.HRAdmin))
				{
					return RedirectToAction("Dashboard", "Requestor");
				}
				else if (usrProf.HasSystemRole(Entity.Constant.SystemRole.SysAdmin))
				{
					return RedirectToAction("GeneralReport", "Report");
				}
				else if (usrProf.HasSystemRole(Entity.Constant.SystemRole.SchoolAmin))
				{
					return RedirectToAction("Dashboard", "Requestor");
				}
				else if (usrProf.HasSystemRole(Entity.Constant.SystemRole.Agent))
				{
					return RedirectToAction("Dashboard", "Agent");
				}
				else if (usrProf.HasSystemRole(Entity.Constant.SystemRole.NonIITSAgent))
				{
					return RedirectToAction("GeneralReport", "Report");
				}
			}

			return View("Unauthorized");
		}
	}
}
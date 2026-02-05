using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding;
using Onboarding.Entity;
using Onboarding.Common;

namespace Onboarding.DataProvider
{
    public class UserProfileProvider
    {
        public List<Entity.UserProfile> GetUserProfileAll()
        {
            List<Entity.SystemRoleUser> roleUserList = (List<Entity.SystemRoleUser>)MCache.Get("SystemRoleUser");

            List<string> SecAccLogs = new List<string>();

            if (roleUserList == null)
            {
                try
                {
                    using (Model.SecAccEntities saCtx = new Model.SecAccEntities())
                    {
                        roleUserList = new List<SystemRoleUser>();
                        List<Model.V_GetSysUserProg> dbUserRoleList = saCtx.V_GetSysUserProg.Where(x => (x.sys_id == Entity.Constant.System.SystemCode) && (x.active_f == "Y")).ToList();
                        SecAccLogs.Add("dbUserRoleList.Count(): " + dbUserRoleList.Count());

                        List<Model.V_GetSysUserProgData> dbUserCostCentreList = saCtx.V_GetSysUserProgData.Where(x => (x.sys_id == Entity.Constant.System.SystemCode)).ToList();
                        SecAccLogs.Add("dbUserCostCentreList.Count(): " + dbUserCostCentreList.Count());

                        foreach (Model.V_GetSysUserProg dbRoleUser in dbUserRoleList)
                        {
                            Entity.SystemRoleUser roleUser = new SystemRoleUser();
                            roleUser.UserId = dbRoleUser.user_id.ToLower().Replace("smustf\\", "");
                            roleUser.SysRoleCd = dbRoleUser.prog_id.ToUpper().Trim();
                            List<Model.V_GetSysUserProgData> dbCostCtrList = dbUserCostCentreList.Where(x => x.prog_id.ToUpper().Trim() == roleUser.SysRoleCd.ToUpper().Trim()).ToList();
                            roleUser.CostCtrCdList = new List<string>();
                            foreach (Model.V_GetSysUserProgData dbCostCtr in dbCostCtrList.Where(x => x.user_id.ToLower() == dbRoleUser.user_id.ToLower()))
                            {
                                roleUser.CostCtrCdList.Add(dbCostCtr.data_param.Trim().ToUpper());
                            }
                            roleUserList.Add(roleUser);

                            SecAccLogs.Add("roleUserList.Count(): " + roleUserList.Count());
                        }
                    }
                    MCache.Set("SystemRoleUser", roleUserList);
                } 
                catch (Exception ex)
                {
                    Logger.LogError("UserProfileProvider.GetUserProfileAll: " + ex.ToString());
                    Logger.LogError("SecAccLogs: " + string.Join(", ", SecAccLogs.Where(x => !string.IsNullOrEmpty(x))));
                }
            }

            List<Entity.UserProfile> userProfileList = (List<Entity.UserProfile>)MCache.Get("UserProfile");
            if (userProfileList == null)
            {
                List<string> OnboardingLogs = new List<string>();
                List<string> PatronLogs = new List<string>();

                try
                {
                    using (Model.OnboardingEntities obCtx = new Model.OnboardingEntities())
                    {
                        using (Model.PATRONEntities patCtx = new Model.PATRONEntities())
                        {
                            List<Model.V_OnboardingPerInfo> dbPerInfoList = patCtx.V_OnboardingPerInfo.OrderByDescending(x => x.IsActive).ToList();
                            PatronLogs.Add("dbPerInfoList.Count(): " + dbPerInfoList.Count());

                            if ((dbPerInfoList != null) && (dbPerInfoList.Count > 0))
                            {
                                userProfileList = new List<UserProfile>();
                                foreach (Model.V_OnboardingPerInfo dbPerInfo in dbPerInfoList)
                                {
                                    if ((!string.IsNullOrEmpty(dbPerInfo.NtLoginId)) &&
                                        (userProfileList.Where(x => x.NtLoginId.ToLower().Trim() == dbPerInfo.NtLoginId.ToLower().Trim()).FirstOrDefault() == null))
                                    {
                                        Entity.UserProfile usrProfile = new UserProfile();
                                        usrProfile.NtLoginId = dbPerInfo.NtLoginId.ToLower().Trim();
                                        usrProfile.Name = dbPerInfo.PrefName;
                                        usrProfile.Email = dbPerInfo.SmuEmail;
                                        usrProfile.IsActive = dbPerInfo.IsActive == true ? true : false;
                                        usrProfile.SystemRoles = new List<SystemRoleUser>();
                                        List<Entity.SystemRoleUser> usrRoles = roleUserList.Where(x => x.UserId.Trim().ToLower() == usrProfile.NtLoginId.Trim().ToLower()).ToList();
                                        foreach (Entity.SystemRoleUser usrRole in usrRoles)
                                        {
                                            usrProfile.SystemRoles.Add(usrRole);
                                        }
                                        userProfileList.Add(usrProfile);
                                    }
                                }
                            }
                        }
                    }
                    MCache.Set("UserProfile", userProfileList);
                }
                catch (Exception ex)
                {
                    Logger.LogError("UserProfileProvider.GetUserProfileAll: " + ex.ToString());
                    Logger.LogError("OnboardingLogs: " + string.Join(", ", OnboardingLogs.Where(x => !string.IsNullOrEmpty(x))));
                    Logger.LogError("PatronLogs: " + string.Join(", ", PatronLogs.Where(x => !string.IsNullOrEmpty(x))));
                }
            }
            return userProfileList;
        }

        public Entity.UserProfile GetUserProfile(string userLoginId)
        {
            Entity.UserProfile usrProfile = null;
            if (!string.IsNullOrWhiteSpace(userLoginId))
            {
                List<Entity.UserProfile> userProfileList = GetUserProfileAll();
                usrProfile = userProfileList.FirstOrDefault(x => x.NtLoginId.Trim().ToLower() == userLoginId.Trim().ToLower());
            }
            return usrProfile;
        }
    }
}

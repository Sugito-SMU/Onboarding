using Newtonsoft.Json;
using Onboarding.Entity;
using Onboarding.Models;
using Onboarding.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;


namespace Onboarding.WebAPIManager
{
    public class WebAPIManager
    {
        public WebAPIManager()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        public List<RequestType> GetRequestTypeList()
        {
            List<RequestType> reqTypeList = (List<RequestType>)MCache.Get("RequestTypeList");
            if (reqTypeList == null)
            {
                var handler = new HttpClientHandler
                {
                    UseDefaultCredentials = true
                };

                if (ConfigurationManager.AppSettings["Debug_User"] != null && ConfigurationManager.AppSettings["Debug_User"].ToString().Length > 0)
                {
                    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
                }

                using (HttpClient client = new HttpClient(handler))
                {
                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebAPIUri"]);
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = client.GetAsync("api/RequestType").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = response.Content.ReadAsStringAsync().Result;
                        reqTypeList = JsonConvert.DeserializeObject<List<RequestType>>(responseData);
                        reqTypeList = reqTypeList.OrderBy(x => x.Sequence).ToList();
                        MCache.Set("RequestTypeList", reqTypeList);
                    }
                    else
                    {
                        HtmlHelpers.HandleApiResponse(response);
                        return null;
                    }
                }
            }
            return reqTypeList;
        }

        public string GetRequestTypeDesc(string Id)
        {
            List<RequestType> reqTypeList = GetRequestTypeList();
            if (reqTypeList != null)
            {
                RequestType reqType = reqTypeList.Where(x => x.ReqTypeCd.ToLower().Trim() == Id.ToLower().Trim()).FirstOrDefault();
                if (reqType != null)
                {
                    return reqType.ReqTypeDesc;
                }
            }
            return null;
        }

        public List<Race> GetRaceList()
        {
            List<Race> raceList = (List<Race>)MCache.Get("RaceList");
            if (raceList == null)
            {
                var handler = new HttpClientHandler
                {
                    UseDefaultCredentials = true
                };

                if (ConfigurationManager.AppSettings["Debug_User"] != null && ConfigurationManager.AppSettings["Debug_User"].ToString().Length > 0)
                {
                    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
                }

                using (HttpClient client = new HttpClient(handler))
                {
                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebAPIUri"]);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = client.GetAsync("api/Race").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = response.Content.ReadAsStringAsync().Result;
                        raceList = JsonConvert.DeserializeObject<List<Race>>(responseData);
                        raceList = raceList.OrderBy(x => x.RaceDesc).ToList();
                        MCache.Set("RaceList", raceList);
                    }
                    else
                    {
                        HtmlHelpers.HandleApiResponse(response);
                        return null;
                    }
                }
            }
            return raceList;
        }

        public List<Salutation> GetSalutationList()
        {
            List<Salutation> salutList = (List<Salutation>)MCache.Get("SalutationList");
            if (salutList == null)
            {
                var handler = new HttpClientHandler
                {
                    UseDefaultCredentials = true
                };

                if (ConfigurationManager.AppSettings["Debug_User"] != null && ConfigurationManager.AppSettings["Debug_User"].ToString().Length > 0)
                {
                    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
                }

                using (HttpClient client = new HttpClient(handler))
                {
                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebAPIUri"]);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = client.GetAsync("api/Salutation").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = response.Content.ReadAsStringAsync().Result;
                        salutList = JsonConvert.DeserializeObject<List<Salutation>>(responseData);
                        salutList = salutList.OrderBy(x => x.SalutDesc).ToList();
                        MCache.Set("SalutationList", salutList);
                    }
                    else
                    {
                        HtmlHelpers.HandleApiResponse(response);
                        return null;
                    }
                }
            }
            return salutList;
        }

        public List<UserType> GetUserTypeList()
        {
            List<UserType> userTypeList = (List<UserType>)MCache.Get("UserTypeList");
            if (userTypeList == null)
            {
                var handler = new HttpClientHandler
                {
                    UseDefaultCredentials = true
                };

                if (ConfigurationManager.AppSettings["Debug_User"] != null && ConfigurationManager.AppSettings["Debug_User"].ToString().Length > 0)
                {
                    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
                }

                using (HttpClient client = new HttpClient(handler))
                {
                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebAPIUri"]);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = client.GetAsync("api/UserType").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = response.Content.ReadAsStringAsync().Result;
                        userTypeList = JsonConvert.DeserializeObject<List<UserType>>(responseData);
                        userTypeList = userTypeList.OrderBy(x => x.Sequence).ToList();
                        MCache.Set("UserTypeList", userTypeList);
                    }
                    else
                    {
                        HtmlHelpers.HandleApiResponse(response);
                        return null;
                    }
                }
            }
            UserProfile usrProf = Helper.Utility.GetUserProfile();
            bool accessAllUserType = Common.AccessControl.HasPermission(usrProf, Entity.Constant.AppResource.AccessAllUserType);
            string userTypeStr = string.Empty;
            if (!accessAllUserType)
            {
                userTypeStr = Common.AccessControl.GetAllowedUserTypeString(usrProf);
            }
            return userTypeList.Where(x => (accessAllUserType == true) || (userTypeStr.Contains("|" + x.UserTypeCd.ToUpper().Trim() + "|"))).ToList();
        }

        public List<Entity.UserSubType> GetUserSubTypeList()
        {
            List<UserSubType> userSubTypeList = (List<UserSubType>)MCache.Get("UserSubTypeList");
            if (userSubTypeList == null)
            {
                var handler = new HttpClientHandler
                {
                    UseDefaultCredentials = true
                };

                if (ConfigurationManager.AppSettings["Debug_User"] != null && ConfigurationManager.AppSettings["Debug_User"].ToString().Length > 0)
                {
                    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
                }

                using (HttpClient client = new HttpClient(handler))
                {
                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebAPIUri"]);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = client.GetAsync("api/UserSubType").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = response.Content.ReadAsStringAsync().Result;
                        userSubTypeList = JsonConvert.DeserializeObject<List<UserSubType>>(responseData);
                        userSubTypeList = userSubTypeList.OrderBy(x => x.Sequence).ToList();
                        MCache.Set("UserSubTypeList", userSubTypeList);
                    }
                    else
                    {
                        HtmlHelpers.HandleApiResponse(response);
                        return null;
                    }
                }
            }

            UserProfile usrProf = Helper.Utility.GetUserProfile();
            bool accessAllUserType = Common.AccessControl.HasPermission(usrProf, Entity.Constant.AppResource.AccessAllUserType);
            string userTypeStr = string.Empty;
            if (!accessAllUserType)
            {
                userTypeStr = Common.AccessControl.GetAllowedUserTypeString(usrProf);
            }
            return userSubTypeList.Where(x => (accessAllUserType == true) || (userTypeStr.Contains("|" + x.UserTypeCd.ToUpper().Trim() + "|"))).ToList();
        }

        public List<CostCentre> GetCostCentreList()
        {
            List<CostCentre> costCentreList = GetAllCostCentreList();
            UserProfile usrProf = Helper.Utility.GetUserProfile();
            bool accessAllCostCtr = Common.AccessControl.HasPermission(usrProf, Entity.Constant.AppResource.AccessAllCostCentre);
            string usrCostCtrList = string.Empty;
            if ((!accessAllCostCtr) && (usrProf.HasSystemRole(Entity.Constant.SystemRole.SchoolAmin)))
            {
                SystemRoleUser role = usrProf.SystemRoles.Where(x => x.SysRoleCd.Trim().ToUpper() == Entity.Constant.SystemRole.SchoolAmin).FirstOrDefault();
                if (role != null)
                {
                    usrCostCtrList = role.CostCtrString;
                }
            }
            return costCentreList.Where(x => (accessAllCostCtr == true) || (usrCostCtrList.Contains("|" + x.CostCtrCd.ToUpper().Trim() + "|"))).ToList();
        }

        public List<CostCentre> GetAllCostCentreList()
        {
            List<CostCentre> costCentreList = (List<CostCentre>)MCache.Get("CostCentreList");
            if (costCentreList == null)
            {
                var handler = new HttpClientHandler
                {
                    UseDefaultCredentials = true
                };

                if (ConfigurationManager.AppSettings["Debug_User"] != null && ConfigurationManager.AppSettings["Debug_User"].ToString().Length > 0)
                {
                    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
                }

                using (HttpClient client = new HttpClient(handler))
                {
                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebAPIUri"]);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = client.GetAsync("api/CostCentre").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = response.Content.ReadAsStringAsync().Result;
                        costCentreList = JsonConvert.DeserializeObject<List<CostCentre>>(responseData);
                        costCentreList = costCentreList.OrderBy(x => x.CostCtrDesc).ToList();
                        MCache.Set("CostCentreList", costCentreList);
                    }
                    else
                    {
                        HtmlHelpers.HandleApiResponse(response);
                        return null;
                    }
                }
            }
            return costCentreList;
        }

        public List<Gender> GetGenderList()
        {
            List<Gender> genderList = (List<Gender>)MCache.Get("GenderList");
            if (genderList == null)
            {
                var handler = new HttpClientHandler
                {
                    UseDefaultCredentials = true
                };

                if (ConfigurationManager.AppSettings["Debug_User"] != null && ConfigurationManager.AppSettings["Debug_User"].ToString().Length > 0)
                {
                    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
                }

                using (HttpClient client = new HttpClient(handler))
                {
                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebAPIUri"]);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = client.GetAsync("api/Gender").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = response.Content.ReadAsStringAsync().Result;
                        genderList = JsonConvert.DeserializeObject<List<Gender>>(responseData);
                        genderList = genderList.OrderBy(x => x.GenderDesc).ToList();
                        MCache.Set("GenderList", genderList);
                    }
                    else
                    {
                        HtmlHelpers.HandleApiResponse(response);
                        return null;
                    }
                }
            }
            return genderList;
        }

        public List<RequestCountForAgent> GetAgentTaskCount(string AgentLoginID)
        {
            var handler = new HttpClientHandler
            {
                UseDefaultCredentials = true
            };

            if (ConfigurationManager.AppSettings["Debug_User"] != null && ConfigurationManager.AppSettings["Debug_User"].ToString().Length > 0)
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            }

            using (HttpClient client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebAPIUri"]);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync("api/RequestCountForAgent/" + Helper.Utility.RemoveUserDomain(AgentLoginID)).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    var requestObject = JsonConvert.DeserializeObject<List<RequestCountForAgent>>(responseData);
                    return requestObject.ToList();
                }
                else
                {
                    HtmlHelpers.HandleApiResponse(response);
                    return null;
                }
            }
        }

        public List<RequestCountForHRAdmin> GetHRAdminTaskCount(string HRAdminLoginID)
        {
            var handler = new HttpClientHandler
            {
                UseDefaultCredentials = true
            };

            if (ConfigurationManager.AppSettings["Debug_User"] != null && ConfigurationManager.AppSettings["Debug_User"].ToString().Length > 0)
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            }

            using (HttpClient client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebAPIUri"]);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync("api/RequestCountForHRAdmin/" + Helper.Utility.RemoveUserDomain(HRAdminLoginID)).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    var requestObject = JsonConvert.DeserializeObject<List<RequestCountForHRAdmin>>(responseData);
                    return requestObject.ToList();
                }
                else
                {
                    HtmlHelpers.HandleApiResponse(response);
                    return null;
                }
            }
        }

        public List<Models.AgentList> GetAgentTaskList(string AgentLoginID, string TaskCD)
        {
            var handler = new HttpClientHandler
            {
                UseDefaultCredentials = true
            };

            if (ConfigurationManager.AppSettings["Debug_User"] != null && ConfigurationManager.AppSettings["Debug_User"].ToString().Length > 0)
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            }

            using (HttpClient client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebAPIUri"]);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync("api/AgentTaskList/" + Helper.Utility.RemoveUserDomain(AgentLoginID) + "/" + TaskCD).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    var requestObject = JsonConvert.DeserializeObject<List<AgentList>>(responseData);
                    return requestObject;
                }
                else
                {
                    HtmlHelpers.HandleApiResponse(response);
                    return null;
                }
            }
        }

        public Request GetRequest(int ReqId, string UserId)
        {
            var handler = new HttpClientHandler
            {
                UseDefaultCredentials = true
            };

            if (ConfigurationManager.AppSettings["Debug_User"] != null && ConfigurationManager.AppSettings["Debug_User"].ToString().Length > 0)
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            }

            using (HttpClient client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebAPIUri"]);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync("api/Request/" + ReqId + "/" + Helper.Utility.RemoveUserDomain(UserId)).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    var requestObject = JsonConvert.DeserializeObject<Request>(responseData);
                    if (requestObject.Tasks != null)
                    {
                        requestObject.Tasks = requestObject.Tasks.OrderBy(x => x.TaskDesc).ToList();
                    }
                    return requestObject;
                }
                else
                {
                    HtmlHelpers.HandleApiResponse(response);
                    return null;
                }
            }
        }

        public RequestAttachment GetRequestAttachment(int reqAttId, string userId)
        {
            var handler = new HttpClientHandler
            {
                UseDefaultCredentials = true
            };

            if (ConfigurationManager.AppSettings["Debug_User"] != null && ConfigurationManager.AppSettings["Debug_User"].ToString().Length > 0)
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            }

            using (HttpClient client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebAPIUri"]);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync("api/RequestAttachment/" + reqAttId + "/" + Helper.Utility.RemoveUserDomain(userId)).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    var requestObject = JsonConvert.DeserializeObject<RequestAttachment>(responseData);
                    return requestObject;
                }
                else
                    return null;
            }
        }

        public Onboarding.Entity.RequestAttachment DeleteRequestAttachment(int ReqId)
        {
            var handler = new HttpClientHandler
            {
                UseDefaultCredentials = true
            };

            if (ConfigurationManager.AppSettings["Debug_User"] != null && ConfigurationManager.AppSettings["Debug_User"].ToString().Length > 0)
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            }

            using (HttpClient client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebAPIUri"]);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.DeleteAsync("api/RequestAttachment/" + ReqId).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    var requestObject = JsonConvert.DeserializeObject<Onboarding.Entity.RequestAttachment>(responseData);
                    return requestObject;
                }
                else
                {
                    HtmlHelpers.HandleApiResponse(response);
                    return null;
                }
            }
        }

        public List<Entity.RequestCountForRequestor> GetRequestCount(string userId)
        {
            var handler = new HttpClientHandler
            {
                UseDefaultCredentials = true
            };

            if (ConfigurationManager.AppSettings["Debug_User"] != null && ConfigurationManager.AppSettings["Debug_User"].ToString().Length > 0)
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            }

            using (HttpClient client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebAPIUri"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("api/RequestCountForRequestor/" + Helper.Utility.RemoveUserDomain(userId) + "/").Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsAsync<List<Entity.RequestCountForRequestor>>().Result;
                    return responseData;
                }
                else
                {
                    HtmlHelpers.HandleApiResponse(response);
                    return null;
                }
            }
        }

        public List<Entity.TaskStatus> GetAllTaskStatus()
        {
            var handler = new HttpClientHandler
            {
                UseDefaultCredentials = true
            };

            if (ConfigurationManager.AppSettings["Debug_User"] != null && ConfigurationManager.AppSettings["Debug_User"].ToString().Length > 0)
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            }

            using (HttpClient client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebAPIUri"]);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("api/TaskStatus").Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    var requestObject = JsonConvert.DeserializeObject<List<Entity.TaskStatus>>(responseData);

                    return requestObject.OrderBy(x => x.TaskStsDesc).ToList();
                }
                else
                {
                    HtmlHelpers.HandleApiResponse(response);
                    return null;
                }
            }
        }

        public Request SaveRequest(Request request, string userId)
        {
            Helper.Utility.SetMandatoryValues(request, false);

            var handler = new HttpClientHandler
            {
                UseDefaultCredentials = true
            };

            if (ConfigurationManager.AppSettings["Debug_User"] != null && ConfigurationManager.AppSettings["Debug_User"].ToString().Length > 0)
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            }

            using (HttpClient client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebAPIUri"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string strPostUri = ConfigurationManager.AppSettings["WebAPIUri"] + "api/Request/" + Helper.Utility.RemoveUserDomain(userId) + "/";
                HttpResponseMessage response = client.PostAsJsonAsync<Request>(strPostUri, request).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsAsync<Request>().Result;
                    return result;
                }
                else
                {
                    HtmlHelpers.HandleApiResponse(response);
                    return null;
                }
            }
        }

        public bool DeleteRequest(int requestId, string userId)
        {
            var handler = new HttpClientHandler
            {
                UseDefaultCredentials = true
            };

            if (ConfigurationManager.AppSettings["Debug_User"] != null && ConfigurationManager.AppSettings["Debug_User"].ToString().Length > 0)
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            }

            using (HttpClient client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebAPIUri"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string strPostUri = ConfigurationManager.AppSettings["WebAPIUri"] + "api/Request/" + requestId.ToString() + "/" + Helper.Utility.RemoveUserDomain(userId) + "/";
                HttpResponseMessage response = client.DeleteAsync(strPostUri).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsAsync<bool>().Result;
                    return result;
                }
                else
                {
                    HtmlHelpers.HandleApiResponse(response);
                    return false;
                }
            }
        }

        public Entity.Task SaveTask(int TaskId, Onboarding.Entity.Task oTask, string userId)
        {
            var handler = new HttpClientHandler
            {
                UseDefaultCredentials = true
            };

            if (ConfigurationManager.AppSettings["Debug_User"] != null && ConfigurationManager.AppSettings["Debug_User"].ToString().Length > 0)
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            }

            using (HttpClient client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebAPIUri"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.PutAsJsonAsync("api/Task/" + Helper.Utility.RemoveUserDomain(userId), oTask).Result;
                if (response.IsSuccessStatusCode)
                {                   
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    var requestObject = JsonConvert.DeserializeObject<Onboarding.Entity.Task>(responseData);
                    return requestObject;
                }
                else
                {
                    HtmlHelpers.HandleApiResponse(response);
                    return null;
                }
            }
        }

        public CASEntityADAccount GetNetworkIdFromNaas(string networkId)
        {
            string domain = ConfigurationManager.AppSettings["StaffDomain"];

            var handler = new HttpClientHandler
            {
                UseDefaultCredentials = true
            };

            if (ConfigurationManager.AppSettings["Debug_User"] != null && ConfigurationManager.AppSettings["Debug_User"].ToString().Length > 0)
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            }

            using (HttpClient client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["NaasAPIUri"]);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("api/ADAccount/" + domain + "/" + networkId + "/").Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    var requestObject = JsonConvert.DeserializeObject<CASEntityADAccount>(responseData);
                    return requestObject;
                }
                else
                {
                    HtmlHelpers.HandleApiResponse(response);
                    return null;
                }
            }
        }

        public bool PostNetworkIdToNaas(string newNetworkId, Onboarding.Entity.Request oRequest, string userId, string oldNetworkId)
        {
            CASEntityAUPUser oAupUser = new CASEntityAUPUser();
            oAupUser.Domain = ConfigurationManager.AppSettings["StaffDomain"];
            oAupUser.NtLoginId = newNetworkId;
            oAupUser.PrefNm = oRequest.PrefNm;
            DateTime empstartdate = (DateTime)oRequest.EmploymentInfo.EmplStartDt;
            
            //6 weeks before startdate for Adjunct
            if((oRequest.EmploymentInfo.UserSubTypeCd == Entity.Constant.UserSubType.FacultyAssociatedAdjunctsContractForService) || (oRequest.EmploymentInfo.UserSubTypeCd == Entity.Constant.UserSubType.FacultyAssociatedAdjunctsContractOfService))
            {
               // empstartdate = empstartdate.AddDays(-42);CR#37164
                Int32 noOfDaysAdvance = Convert.ToInt32(ConfigurationManager.AppSettings["AdvanceStartDaysForAdjunct"]);
                empstartdate = empstartdate.AddDays(-noOfDaysAdvance);
            }
                       
            if (oRequest.Resources.ELearnStartDate < empstartdate)
            {
                oAupUser.StartDt = (DateTime)oRequest.Resources.ELearnStartDate;
            }
            else
            {
                oAupUser.StartDt = empstartdate;
            }

            //oAupUser.PrsnEmail = CAS.Common.Encryption.EncryptRsa(ConfigurationManager.AppSettings["RSACertificateName"], oRequest.PrsnEmail);
            //oAupUser.MobileNo = CAS.Common.Encryption.EncryptRsa(ConfigurationManager.AppSettings["RSACertificateName"], oRequest.MobileNo);
            oAupUser.PrsnEmail = Encryption.EncryptRsa(ConfigurationManager.AppSettings["RSACertificateName"], oRequest.PrsnEmail);
            oAupUser.MobileNo = Encryption.EncryptRsa(ConfigurationManager.AppSettings["RSACertificateName"], oRequest.MobileNo);

            oAupUser.CreatedBy = Helper.Utility.RemoveUserDomain(userId);
            oAupUser.CreatedDt = DateTime.Now;
            oAupUser.ModifiedBy = Helper.Utility.RemoveUserDomain(userId);
            oAupUser.CreatedDt = DateTime.Now;

            var handler = new HttpClientHandler
            {
                UseDefaultCredentials = true
            };

            if (ConfigurationManager.AppSettings["Debug_User"] != null && ConfigurationManager.AppSettings["Debug_User"].ToString().Length > 0)
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            }

            using (HttpClient client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["NaasAPIUri"]);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if((oldNetworkId != null && oldNetworkId != oAupUser.NtLoginId) || (oRequest.ReqStsCd==Entity.Constant.RequestStatusCode.Cancelled))
                {                    
                    HttpResponseMessage naasResponsedelete = client.DeleteAsync("api/AUPUser/" + oAupUser.Domain + "/" + oldNetworkId).Result;
                    Common.Logger.LogInfo(naasResponsedelete.Content.ReadAsStringAsync().Result);
                    if (naasResponsedelete.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        return false;
                    }
                }
                if(oRequest.ReqStsCd != Entity.Constant.RequestStatusCode.Cancelled)
                {
                    HttpResponseMessage naasResponse = client.PostAsJsonAsync("api/AUPUser/" + oAupUser.Domain + "/" + oAupUser.NtLoginId + "/", oAupUser).Result;
                    Common.Logger.LogInfo(naasResponse.Content.ReadAsStringAsync().Result);
                    if (naasResponse.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        return false;
                    }
                }
            }
            return true;        
        }

        public BulkRequestSubmitResult SaveBulkRequest(BulkRequest bulkReq, string userId)
        {
            var handler = new HttpClientHandler
            {
                UseDefaultCredentials = true
            };

            if (ConfigurationManager.AppSettings["Debug_User"] != null && ConfigurationManager.AppSettings["Debug_User"].ToString().Length > 0)
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            }

            using (HttpClient client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebAPIUri"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string strPostUri = ConfigurationManager.AppSettings["WebAPIUri"] + "api/BulkRequest/" + Helper.Utility.RemoveUserDomain(userId) + "/";
                HttpResponseMessage response = client.PostAsJsonAsync<BulkRequest>(strPostUri, bulkReq).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    var requestObject = JsonConvert.DeserializeObject<BulkRequestSubmitResult>(responseData);
                    return requestObject;
                }
                else
                {
                    HtmlHelpers.HandleApiResponse(response);
                    return null;
                }
            }
        }

        public GenRptSearchResult GetGeneralReport(string userid, RptSearchCriteria Search)
        {
            var handler = new HttpClientHandler
            {
                UseDefaultCredentials = true
            };

            if (ConfigurationManager.AppSettings["Debug_User"] != null && ConfigurationManager.AppSettings["Debug_User"].ToString().Length > 0)
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            }

            using (HttpClient client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebAPIUri"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string strPostUri = ConfigurationManager.AppSettings["WebAPIUri"] + "api/GeneralReport/" + Helper.Utility.RemoveUserDomain(userid);
                HttpResponseMessage response = client.PostAsJsonAsync<RptSearchCriteria>(strPostUri, Search).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsAsync<GenRptSearchResult>().Result;
                    return result;
                }
                else
                {
                    HtmlHelpers.HandleApiResponse(response);
                    return null;
                }
            }
        }

        public RequestSearchResult SearchRequest(string userid, RptSearchCriteria Search)
        {
            var handler = new HttpClientHandler
            {
                UseDefaultCredentials = true
            };

            if (ConfigurationManager.AppSettings["Debug_User"] != null && ConfigurationManager.AppSettings["Debug_User"].ToString().Length > 0)
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            }

            using (HttpClient client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebAPIUri"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string strPostUri = ConfigurationManager.AppSettings["WebAPIUri"] + "api/SearchRequest/" + Helper.Utility.RemoveUserDomain(userid);
                HttpResponseMessage response = client.PostAsJsonAsync<RptSearchCriteria>(strPostUri, Search).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsAsync<RequestSearchResult>().Result;
                    return result;
                }
                else
                {
                    HtmlHelpers.HandleApiResponse(response);
                    return null;
                }
            }
        }

        public List<Requestor> GetRequestorList(string userId)
        {
            var handler = new HttpClientHandler
            {
                UseDefaultCredentials = true
            };

            if (ConfigurationManager.AppSettings["Debug_User"] != null && ConfigurationManager.AppSettings["Debug_User"].ToString().Length > 0)
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            }

            using (HttpClient client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebAPIUri"]);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync("api/RequestorList/" + Helper.Utility.RemoveUserDomain(userId) + "/").Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    var requestObject = JsonConvert.DeserializeObject<List<Onboarding.Entity.Requestor>>(responseData);
                    return requestObject;
                }
                else
                {
                    HtmlHelpers.HandleApiResponse(response);
                    return null;
                }
            }
        }

        public List<Onboarding.Entity.RequestStatus> GetRequestStatusList()
        {
            List<RequestStatus> reqStatusList = (List<RequestStatus>)MCache.Get("RequestStatusList");
            if (reqStatusList == null)
            {
                var handler = new HttpClientHandler
                {
                    UseDefaultCredentials = true
                };

                if (ConfigurationManager.AppSettings["Debug_User"] != null && ConfigurationManager.AppSettings["Debug_User"].ToString().Length > 0)
                {
                    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
                }

                using (HttpClient client = new HttpClient(handler))
                {
                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebAPIUri"]);
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = client.GetAsync("api/RequestStatus").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = response.Content.ReadAsStringAsync().Result;
                        reqStatusList = JsonConvert.DeserializeObject<List<Onboarding.Entity.RequestStatus>>(responseData);
                        reqStatusList = reqStatusList.OrderBy(x => x.ReqStsDesc).ToList();
                        MCache.Set("RequestStatusList", reqStatusList);
                    }
                    else
                    {
                        HtmlHelpers.HandleApiResponse(response);
                        return null;
                    }
                }
            }
            return reqStatusList;
        }

        public RequestListForRequestor GetRequestListForRequestor(string requestStatusCd, string userId)
        {
            var handler = new HttpClientHandler
            {
                UseDefaultCredentials = true
            };

            if (ConfigurationManager.AppSettings["Debug_User"] != null && ConfigurationManager.AppSettings["Debug_User"].ToString().Length > 0)
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            }

            using (HttpClient client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebAPIUri"]);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync("api/RequestListForRequestor/" + requestStatusCd + "/" + Helper.Utility.RemoveUserDomain(userId)).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    var requestObject = JsonConvert.DeserializeObject<Entity.RequestListForRequestor>(responseData);
                    return requestObject;
                }
                else
                {
                    HtmlHelpers.HandleApiResponse(response);
                    return null;
                }
            }
        }

        public RequestListForAgent GetRequestListForAgent(string taskStatusCd, string userId)
        {
            var handler = new HttpClientHandler
            {
                UseDefaultCredentials = true
            };

            if (ConfigurationManager.AppSettings["Debug_User"] != null && ConfigurationManager.AppSettings["Debug_User"].ToString().Length > 0)
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            }

            using (HttpClient client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebAPIUri"]);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync("api/RequestListForAgent/" + taskStatusCd + "/" + Helper.Utility.RemoveUserDomain(userId)).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    var requestObject = JsonConvert.DeserializeObject<Entity.RequestListForAgent>(responseData);
                    return requestObject;
                }
                else
                {
                    HtmlHelpers.HandleApiResponse(response);
                    return null;
                }
            }
        }
        public RequestListForAgent GetRequestListForHRAdmin(string taskStatusCd, string userId)
        {
            var handler = new HttpClientHandler
            {
                UseDefaultCredentials = true
            };

            if (ConfigurationManager.AppSettings["Debug_User"] != null && ConfigurationManager.AppSettings["Debug_User"].ToString().Length > 0)
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            }

            using (HttpClient client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebAPIUri"]);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync("api/RequestListForHRAdmin/" + taskStatusCd + "/" + Helper.Utility.RemoveUserDomain(userId)).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    var requestObject = JsonConvert.DeserializeObject<Entity.RequestListForAgent>(responseData);
                    return requestObject;
                }
                else
                {
                    HtmlHelpers.HandleApiResponse(response);
                    return null;
                }
            }
        }
        public UserProfile GetuserProfile(string userId)
        {
            var handler = new HttpClientHandler
            {
                UseDefaultCredentials = true
            };

            if (ConfigurationManager.AppSettings["Debug_User"] != null && ConfigurationManager.AppSettings["Debug_User"].ToString().Length > 0)
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            }

            using (HttpClient client = new HttpClient(handler))
            {
                if (ConfigurationManager.AppSettings["Debug_User"] != null && ConfigurationManager.AppSettings["Debug_User"].ToString().Length > 0)
                {
                    Common.Logger.LogInfo($"WindowsIdentity.GetCurrent().Name: {WindowsIdentity.GetCurrent().Name}");
                }

                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebAPIUri"]);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync("api/UserProfile/" + Helper.Utility.RemoveUserDomain(userId) + "/").Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    var requestObject = JsonConvert.DeserializeObject<UserProfile>(responseData);
                    return requestObject;
                }
                else
                {
                    HtmlHelpers.HandleApiResponse(response);
                    return null;
                }
            }
        }
    }
}
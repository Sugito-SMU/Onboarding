using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onboarding.Common;

namespace Onboarding.DataProvider.Mapper
{
    public static partial class MapToDb
    {
        public static void Map(Entity.Request req, Model.V_Req dbReq, string userId, DateTime sqlDateTime, Model.OnboardingEntities obCtx)
        {
            RequestTypeProvider reqTypeProv = new RequestTypeProvider();
            SalutationProvider salutProv = new SalutationProvider();
            GenderProvider genderProv = new GenderProvider();
            RaceProvider raceProv = new RaceProvider();
            RequestStatusProvider reqStsProv = new RequestStatusProvider();

            AESEncryption aes = new AESEncryption();

            if ((dbReq != null) && (req != null))
            {
                LogReqChange("Request Type", dbReq.ReqTypeCd, req.ReqTypeCd, reqTypeProv.GetRequestTypeDesc(dbReq.ReqTypeCd), reqTypeProv.GetRequestTypeDesc(req.ReqTypeCd), userId, sqlDateTime, dbReq, obCtx);
                dbReq.ReqTypeCd = req.ReqTypeCd;

                //To overcome null First Name when save as draft, assign empty space
                if (string.IsNullOrEmpty(req.FirstName))
                {
                    req.FirstName = string.Empty;
                }
                LogReqChange("First Name", dbReq.FirstNm, req.FirstName, null, null, userId, sqlDateTime, dbReq, obCtx);
                dbReq.FirstNm = req.FirstName;

                LogReqChange("Last Name", dbReq.LastNm, req.LastName, null, null, userId, sqlDateTime, dbReq, obCtx);
                dbReq.LastNm = req.LastName;

                LogReqChange("Known As", dbReq.PrefNm, req.PrefNm, null, null, userId, sqlDateTime, dbReq, obCtx);
                dbReq.PrefNm = req.PrefNm;

                LogReqChange("Salutation", dbReq.SalutCd, req.SalutCd, salutProv.GetSalutationDesc(dbReq.SalutCd), salutProv.GetSalutationDesc(req.SalutCd), userId, sqlDateTime, dbReq, obCtx);
                dbReq.SalutCd = req.SalutCd;

                LogReqChange("Gender", dbReq.GenderCd, req.GenderCd, genderProv.GetGenderDesc(dbReq.GenderCd), genderProv.GetGenderDesc(req.GenderCd), userId, sqlDateTime, dbReq, obCtx);
                dbReq.GenderCd = req.GenderCd;

                LogReqChange("Ethnic Origin", dbReq.RaceCd, req.RaceCd, raceProv.GetRaceDesc(dbReq.RaceCd), raceProv.GetRaceDesc(req.RaceCd), userId, sqlDateTime, dbReq, obCtx);
                dbReq.RaceCd = req.RaceCd;

                LogReqChange("Personal Email", dbReq.PrsnEmail, req.PrsnEmail, null, null, userId, sqlDateTime, dbReq, obCtx);
                dbReq.PrsnEmail = req.PrsnEmail;

                LogReqChange("Is Existing Staff", FormatLogValue(dbReq.IsExistingStaff), FormatLogValue(req.IsExistingStaff), null, null, userId, sqlDateTime, dbReq, obCtx);
                dbReq.IsExistingStaff = req.IsExistingStaff;

                LogReqChange("Is Existing SMU/Ex-SMU Student", FormatLogValue(dbReq.IsExSmuStd), FormatLogValue(req.IsExSmuStd), null, null, userId, sqlDateTime, dbReq, obCtx);
                dbReq.IsExSmuStd = req.IsExSmuStd;

                LogReqChange("Remarks", dbReq.Remark, req.Remark, null, null, userId, sqlDateTime, dbReq, obCtx);
                dbReq.Remark = req.Remark;

                var dbMobileNo = aes.Decrypt(dbReq.MobileNo);
                dbReq.MobileNo = aes.Encrypt(req.MobileNo);

                if (dbMobileNo != req.MobileNo)
                {
                    if (req.ReqId != 0)
                    {
                        string dbMobileNoLog = !string.IsNullOrEmpty(dbMobileNo) ? "xxxxx" + GetLastMobileNoChars(dbMobileNo) : string.Empty;
                        string reqMobileNoLog = !string.IsNullOrEmpty(req.MobileNo) ? "xxxxx" + GetLastMobileNoChars(req.MobileNo) : string.Empty;
                        LogReqChangeNoCheck("Personal Mobile No", dbMobileNoLog, reqMobileNoLog, null, null, userId, sqlDateTime, dbReq, obCtx);
                    }
                }

                if (req.ReqStsCd != Entity.Constant.RequestStatusCode.Draft)
                {
                    LogReqChange("Submitted By", dbReq.SubmittedBy, req.SubmittedBy, null, null, userId, sqlDateTime, dbReq, obCtx);
                    dbReq.SubmittedBy = userId;
                    dbReq.SubmittedDt = sqlDateTime;
                }

                dbReq.IsActive = req.IsActive;
                bool isNewRequest = false;

                if (req.ReqId == 0)
                {
                    isNewRequest = true;
                    dbReq.CreatedBy = userId;
                    dbReq.CreatedDt = sqlDateTime;
                    obCtx.V_Req.Add(dbReq);
                    Common.SaveDbChanges(obCtx);
                    req.ReqId = dbReq.ReqId;
                }
                else
                {
                    if (dbReq.ReqStsCd == Entity.Constant.RequestStatusCode.Draft)
                    {
                        isNewRequest = true;
                    }
                }

                if (req.EmploymentInfo != null)
                {
                    Model.V_ReqEmp dbReqEmp = obCtx.V_ReqEmp.Where(x => x.ReqId == dbReq.ReqId).FirstOrDefault();
                    if (dbReqEmp == null)
                    {
                        dbReqEmp = new Model.V_ReqEmp();
                        dbReqEmp.ReqId = dbReq.ReqId;
                        dbReqEmp.CreatedBy = userId;
                        dbReqEmp.CreatedDt = sqlDateTime;
                        obCtx.V_ReqEmp.Add(dbReqEmp);
                    }
                    Map(req.EmploymentInfo, userId, sqlDateTime, dbReq, dbReqEmp, obCtx);
                }
                MapResources(req.Resources, userId, sqlDateTime, dbReq, obCtx);
                MapReqAttachment(req, userId, sqlDateTime, dbReq, obCtx);
                MapTasks(req.Tasks, userId, sqlDateTime, dbReq, obCtx);

                //Note: Updating ReqStsCd on DB entity should be done last, 
                // as change logs depend on existing ReqStsCd from DB to decide whether it is an amendment or not
                // If New Status != Draft and Existing Status in DB = Draft, it is NOT an amendment
                // If New Status != Draft and Existing Status in DB != Draft, it is AMENDMENT
                dbReq.ReqStsCd = req.ReqStsCd;
            }
        }

        private static string GetLastMobileNoChars(string mobileNo)
        {
            int len = 3;
            if (!string.IsNullOrEmpty(mobileNo) && (mobileNo.Length > len))
            {
                int start = mobileNo.Length - len;
                return mobileNo.Substring(start, len);
            }
            else
            {
                return mobileNo;
            }
        }

        public static void DeleteReq(int reqId, Model.OnboardingEntities obCtx)
        {
            Model.V_Req dbReq = obCtx.V_Req.Where(x => x.ReqId == reqId).FirstOrDefault();
            if (dbReq != null)
            {
                obCtx.V_Req.Remove(dbReq);
            }
            Common.SaveDbChanges(obCtx);
        }

        public static void Map(Entity.RequestEmployment reqEmp, string userId, DateTime sqlDateTime, Model.V_Req dbReq, Model.V_ReqEmp dbReqEmp, Model.OnboardingEntities obCtx)
        {
            CostCentreProvider costCtrProv = new CostCentreProvider();
            UserTypeProvider userTypeProv = new UserTypeProvider();
            UserSubTypeProvider userSubTypeProv = new UserSubTypeProvider();

            LogReqEmplChange("Cost Centre", dbReqEmp.CostCtrCd, reqEmp.CostCtrCd, costCtrProv.GetCostCentreDesc(dbReqEmp.CostCtrCd), costCtrProv.GetCostCentreDesc(reqEmp.CostCtrCd), userId, sqlDateTime, dbReq, dbReqEmp, obCtx);
            dbReqEmp.CostCtrCd = reqEmp.CostCtrCd;

            LogReqEmplChange("User Type", dbReqEmp.UserTypeCd, reqEmp.UserTypeCd, userTypeProv.GetUserTypeDesc(dbReqEmp.UserTypeCd), userTypeProv.GetUserTypeDesc(reqEmp.UserTypeCd), userId, sqlDateTime, dbReq, dbReqEmp, obCtx);
            dbReqEmp.UserTypeCd = reqEmp.UserTypeCd;

            LogReqEmplChange("User Sub-Type", dbReqEmp.UserSubTypeCd, reqEmp.UserSubTypeCd, userSubTypeProv.GetUserSubTypeDesc(dbReqEmp.UserSubTypeCd), userSubTypeProv.GetUserSubTypeDesc(reqEmp.UserSubTypeCd), userId, sqlDateTime, dbReq, dbReqEmp, obCtx);
            dbReqEmp.UserSubTypeCd = reqEmp.UserSubTypeCd;

            LogReqEmplChange("Business Title", dbReqEmp.JobTitle, reqEmp.JobTitle, null, null, userId, sqlDateTime, dbReq, dbReqEmp, obCtx);
            dbReqEmp.JobTitle = reqEmp.JobTitle;

            LogReqEmplChange("Empoyment Start Date", FormatLogValue(dbReqEmp.EmpStartDt), FormatLogValue(reqEmp.EmplStartDt), null, null, userId, sqlDateTime, dbReq, dbReqEmp, obCtx);
            if (reqEmp.EmplStartDt != null)
                dbReqEmp.EmpStartDt = reqEmp.EmplStartDt;

            LogReqEmplChange("Empoyment End Date", FormatLogValue(dbReqEmp.EmpEndDt), FormatLogValue(reqEmp.EmplEndDt), null, null, userId, sqlDateTime, dbReq, dbReqEmp, obCtx);
            if (reqEmp.EmplEndDt != null)
                dbReqEmp.EmpEndDt = reqEmp.EmplEndDt;

            dbReqEmp.IsPrimary = true;
        }

        public static void DeleteReqEmp(int reqId, Model.OnboardingEntities obCtx)
        {
            List<Model.V_ReqEmp> listToDelete = obCtx.V_ReqEmp.Where(x => x.ReqId == reqId).ToList();
            for (int i = 0; i < listToDelete.Count; i++)
            {
                obCtx.V_ReqEmp.Remove(listToDelete[i]);
            }
            Common.SaveDbChanges(obCtx);
        }

        public static void MapResources(Entity.RequestResources reqRes, string userId, DateTime sqlDateTime, Model.V_Req dbReq, Model.OnboardingEntities obCtx)
        {
            if ((reqRes != null) && (dbReq != null))
            {
                MapReqOffAttrb(reqRes.IsPCSelected, Entity.Constant.RequestAttributeCode.IsPCSelected, userId, sqlDateTime, dbReq, obCtx);
                MapReqOffAttrb(reqRes.IsIPPhoneSelected, Entity.Constant.RequestAttributeCode.IsIPPhoneSelected, userId, sqlDateTime, dbReq, obCtx);
                MapReqOffAttrb(reqRes.CorpTitleLevelCd, Entity.Constant.RequestAttributeCode.CorpTitleLevelCd, userId, sqlDateTime, dbReq, obCtx);
                MapReqOffAttrb(Entity.Constant.CorpTitle.GetDescription(reqRes.CorpTitleLevelCd), Entity.Constant.RequestAttributeCode.CorpTitleLevelDesc, userId, sqlDateTime, dbReq, obCtx);
                MapReqOffAttrb(reqRes.Building, Entity.Constant.RequestAttributeCode.Building, userId, sqlDateTime, dbReq, obCtx);
                MapReqOffAttrb(reqRes.Floor, Entity.Constant.RequestAttributeCode.Floor, userId, sqlDateTime, dbReq, obCtx);
                MapReqOffAttrb(reqRes.Room, Entity.Constant.RequestAttributeCode.Room, userId, sqlDateTime, dbReq, obCtx);                
                MapReqSysAccAttrb(reqRes.IsNetworkIDSelected, Entity.Constant.RequestAttributeCode.IsNetworkIDSelected, userId, sqlDateTime, dbReq, obCtx);
                MapReqSysAccAttrb(reqRes.NetworkID, Entity.Constant.RequestAttributeCode.NetworkID, userId, sqlDateTime, dbReq, obCtx);
                MapReqSysAccAttrb(reqRes.NetworkIDRemarks, Entity.Constant.RequestAttributeCode.NetworkIDRemarks, userId, sqlDateTime, dbReq, obCtx);
                MapReqSysAccAttrb(reqRes.IsEmailSelected, Entity.Constant.RequestAttributeCode.IsEmailSelected, userId, sqlDateTime, dbReq, obCtx);
                MapReqSysAccAttrb(reqRes.IsSAPSelected, Entity.Constant.RequestAttributeCode.IsSAPSelected, userId, sqlDateTime, dbReq, obCtx);
                MapReqSysAccAttrb(reqRes.DARTFund, Entity.Constant.RequestAttributeCode.DARTFund, userId, sqlDateTime, dbReq, obCtx);
                MapReqSysAccAttrb(reqRes.IsISISSelected, Entity.Constant.RequestAttributeCode.IsISISSelected, userId, sqlDateTime, dbReq, obCtx);
                MapReqSysAccAttrb(reqRes.ISISRemarks, Entity.Constant.RequestAttributeCode.ISISRemarks, userId, sqlDateTime, dbReq, obCtx);
                MapReqSysAccAttrb(reqRes.IsELearnSelected, Entity.Constant.RequestAttributeCode.IsELearnSelected, userId, sqlDateTime, dbReq, obCtx);
                MapReqSysAccAttrb(reqRes.ELearnStartDate, Entity.Constant.RequestAttributeCode.ELearnStartDate, userId, sqlDateTime, dbReq, obCtx);
                MapReqSysAccAttrb(reqRes.ELearnRemarks, Entity.Constant.RequestAttributeCode.ELearnRemarks, userId, sqlDateTime, dbReq, obCtx);
                MapReqSysAccAttrb(reqRes.IsMtgRoomSelected, Entity.Constant.RequestAttributeCode.IsMtgRoomSelected, userId, sqlDateTime, dbReq, obCtx);
                MapReqSysAccAttrb(reqRes.MtgRoomDetails, Entity.Constant.RequestAttributeCode.MtgRoomDetails, userId, sqlDateTime, dbReq, obCtx);
                MapReqSysAccAttrb(reqRes.IsINetSelected, Entity.Constant.RequestAttributeCode.IsINetSelected, userId, sqlDateTime, dbReq, obCtx);
                MapReqSysAccAttrb(reqRes.IsOasisSelected, Entity.Constant.RequestAttributeCode.IsOasisSelected, userId, sqlDateTime, dbReq, obCtx);
                MapReqSysAccAttrb(reqRes.IsNextwebSelected, Entity.Constant.RequestAttributeCode.IsNextwebSelected, userId, sqlDateTime, dbReq, obCtx);
                MapReqSysAccAttrb(reqRes.IsEmailDLSelected, Entity.Constant.RequestAttributeCode.IsEmailDLSelected, userId, sqlDateTime, dbReq, obCtx);
                MapReqSysAccAttrb(reqRes.EmailDLDetails, Entity.Constant.RequestAttributeCode.EmailDLDetails, userId, sqlDateTime, dbReq, obCtx);
                MapReqSysAccAttrb(reqRes.NetworkID, Entity.Constant.RequestAttributeCode.NetworkID, userId, sqlDateTime, dbReq, obCtx);
            }
        }

        public static void MapReqOffAttrb(bool? attrbVal, string attrbCd, string userId, DateTime sqlDateTime, Model.V_Req dbReq, Model.OnboardingEntities obCtx)
        {
            RequestAttributeCodeProvider reqAttrbCdProv = new RequestAttributeCodeProvider();

            bool isNewAttrb = false;
            Model.V_ReqOffAttrb dbOffReqAttrb = obCtx.V_ReqOffAttrb.Where(x => (x.ReqId == dbReq.ReqId) && (x.ReqAttrbCd == attrbCd)).FirstOrDefault();
            if ((dbOffReqAttrb == null) && (attrbVal != null))
            {
                isNewAttrb = true;
                dbOffReqAttrb = new Model.V_ReqOffAttrb();
                dbOffReqAttrb.ReqId = dbReq.ReqId;
                dbOffReqAttrb.ReqAttrbCd = attrbCd;
                dbOffReqAttrb.CreatedBy = dbReq.CreatedBy;
                dbOffReqAttrb.CreatedDt = dbReq.CreatedDt;
                obCtx.V_ReqOffAttrb.Add(dbOffReqAttrb);
            }
            else
            {
                if (dbOffReqAttrb != null)
                {
                    LogReqOffAttrbChange(reqAttrbCdProv.GetRequestAttributeCodeDesc(attrbCd), dbOffReqAttrb.ReqAttrbVal, FormatLogValue(attrbVal), null, null, userId, sqlDateTime, dbReq, dbOffReqAttrb, obCtx);
                }
            }

            if (dbOffReqAttrb != null)
            {
                if (attrbVal != null)
                {
                    dbOffReqAttrb.ReqAttrbVal = attrbVal == true ? "Y" : "N";
                }
                else
                {
                    if (!isNewAttrb)
                    {
                        dbOffReqAttrb.ReqAttrbVal = null;
                    }
                }
            }
        }

        public static void MapReqOffAttrb(string attrbVal, string attrbCd, string userId, DateTime sqlDateTime, Model.V_Req dbReq, Model.OnboardingEntities obCtx)
        {
            RequestAttributeCodeProvider reqAttrbCdProv = new RequestAttributeCodeProvider();

            bool isNewAttrib = false;
            Model.V_ReqOffAttrb dbReqAttrb = obCtx.V_ReqOffAttrb.Where(x => (x.ReqId == dbReq.ReqId) && (x.ReqAttrbCd == attrbCd)).FirstOrDefault();
            if ((dbReqAttrb == null) && (attrbVal != null))
            {
                dbReqAttrb = new Model.V_ReqOffAttrb();
                dbReqAttrb.ReqId = dbReq.ReqId;
                dbReqAttrb.ReqAttrbCd = attrbCd;
                dbReqAttrb.CreatedBy = userId;
                dbReqAttrb.CreatedDt = sqlDateTime;
                obCtx.V_ReqOffAttrb.Add(dbReqAttrb);
            }
            else
            {
                if (dbReqAttrb != null)
                {
                    LogReqOffAttrbChange(reqAttrbCdProv.GetRequestAttributeCodeDesc(attrbCd), dbReqAttrb.ReqAttrbVal, attrbVal, null, null, userId, sqlDateTime, dbReq, dbReqAttrb, obCtx);
                }
            }

            if (dbReqAttrb != null)
            {
                if (!string.IsNullOrWhiteSpace(attrbVal))
                {
                    dbReqAttrb.ReqAttrbVal = attrbVal.Trim();
                }
                else
                {
                    if (!isNewAttrib)
                    {
                        dbReqAttrb.ReqAttrbVal = null;
                    }
                }
            }
        }

        public static void MapReqOffAttrb(DateTime? attrbVal, string attrbCd, string userId, DateTime sqlDateTime, Model.V_Req dbReq, Model.OnboardingEntities obCtx)
        {
            RequestAttributeCodeProvider reqAttrbCdProv = new RequestAttributeCodeProvider();

            bool isNewAttrib = false;
            Model.V_ReqOffAttrb dbReqAttrb = obCtx.V_ReqOffAttrb.Where(x => (x.ReqId == dbReq.ReqId) && (x.ReqAttrbCd == attrbCd)).FirstOrDefault();
            if ((dbReqAttrb == null) && (attrbVal != null))
            {
                dbReqAttrb = new Model.V_ReqOffAttrb();
                dbReqAttrb.ReqId = dbReq.ReqId;
                dbReqAttrb.ReqAttrbCd = attrbCd;
                dbReqAttrb.CreatedBy = userId;
                dbReqAttrb.CreatedDt = sqlDateTime;
                obCtx.V_ReqOffAttrb.Add(dbReqAttrb);
            }
            else
            {
                if (dbReqAttrb != null)
                {
                    LogReqOffAttrbChange(reqAttrbCdProv.GetRequestAttributeCodeDesc(attrbCd), dbReqAttrb.ReqAttrbVal, FormatLogValue(attrbVal), null, null, userId, sqlDateTime, dbReq, dbReqAttrb, obCtx);
                }
            }

            if (dbReqAttrb != null)
            {
                if (attrbVal != null)
                {
                    dbReqAttrb.ReqAttrbVal = ((DateTime)attrbVal).ToString("dd MMM yyyy HH:mm:ss");
                }
                else
                {
                    if (!isNewAttrib)
                    {
                        dbReqAttrb.ReqAttrbVal = null;
                    }
                }
            }
        }

        public static void DeleteReqOffAttrb(int reqId, Model.OnboardingEntities obCtx)
        {
            List<Model.V_ReqOffAttrb> listToDelete = obCtx.V_ReqOffAttrb.Where(x => x.ReqId == reqId).ToList();
            for (int i = 0; i < listToDelete.Count; i++)
            {
                obCtx.V_ReqOffAttrb.Remove(listToDelete[i]);
            }
            Common.SaveDbChanges(obCtx);
        }


        public static void MapReqSysAccAttrb(bool? attrbVal, string attrbCd, string userId, DateTime sqlDateTime, Model.V_Req dbReq, Model.OnboardingEntities obCtx)
        {
            RequestAttributeCodeProvider reqAttrbCdProv = new RequestAttributeCodeProvider();

            bool isNewAttrb = false;
            Model.V_ReqSysAccAttrb dbReqAttrb = obCtx.V_ReqSysAccAttrb.Where(x => (x.ReqId == dbReq.ReqId) && (x.ReqAttrbCd == attrbCd)).FirstOrDefault();
            if ((dbReqAttrb == null) && (attrbVal != null))
            {
                isNewAttrb = true;
                dbReqAttrb = new Model.V_ReqSysAccAttrb();
                dbReqAttrb.ReqId = dbReq.ReqId;
                dbReqAttrb.ReqAttrbCd = attrbCd;
                dbReqAttrb.CreatedBy = dbReq.CreatedBy;
                dbReqAttrb.CreatedDt = dbReq.CreatedDt;
                obCtx.V_ReqSysAccAttrb.Add(dbReqAttrb);
            }
            else
            {
                if (dbReqAttrb != null)
                {
                    LogReqSysAccAttrbChange(reqAttrbCdProv.GetRequestAttributeCodeDesc(attrbCd), dbReqAttrb.ReqAttrbVal, FormatLogValue(attrbVal), null, null, userId, sqlDateTime, dbReq, dbReqAttrb, obCtx);
                }
            }

            if (dbReqAttrb != null)
            {
                if (attrbVal != null)
                {
                    dbReqAttrb.ReqAttrbVal = attrbVal == true ? "Y" : "N";
                }
                else
                {
                    if (!isNewAttrb)
                    {
                        dbReqAttrb.ReqAttrbVal = null;
                    }
                }
            }
        }

        public static void MapReqSysAccAttrb(string attrbVal, string attrbCd, string userId, DateTime sqlDateTime, Model.V_Req dbReq, Model.OnboardingEntities obCtx)
        {
            RequestAttributeCodeProvider reqAttrbCdProv = new RequestAttributeCodeProvider();

            bool isNewAttrib = false;
            Model.V_ReqSysAccAttrb dbReqAttrb = obCtx.V_ReqSysAccAttrb.Where(x => (x.ReqId == dbReq.ReqId) && (x.ReqAttrbCd == attrbCd)).FirstOrDefault();
            if ((dbReqAttrb == null) && (attrbVal != null))
            {
                dbReqAttrb = new Model.V_ReqSysAccAttrb();
                dbReqAttrb.ReqId = dbReq.ReqId;
                dbReqAttrb.ReqAttrbCd = attrbCd;
                dbReqAttrb.CreatedBy = userId;
                dbReqAttrb.CreatedDt = sqlDateTime;
                obCtx.V_ReqSysAccAttrb.Add(dbReqAttrb);
            }
            else
            {
                if (dbReqAttrb != null)
                {
                    LogReqSysAccAttrbChange(reqAttrbCdProv.GetRequestAttributeCodeDesc(attrbCd), dbReqAttrb.ReqAttrbVal, attrbVal, null, null, userId, sqlDateTime, dbReq, dbReqAttrb, obCtx);
                }
            }

            if (dbReqAttrb != null)
            {
                if (!string.IsNullOrWhiteSpace(attrbVal))
                {
                    dbReqAttrb.ReqAttrbVal = attrbVal.Trim();
                }
                else
                {
                    if (!isNewAttrib)
                    {
                        dbReqAttrb.ReqAttrbVal = null;
                    }
                }
            }
        }

        public static void MapReqSysAccAttrb(DateTime? attrbVal, string attrbCd, string userId, DateTime sqlDateTime, Model.V_Req dbReq, Model.OnboardingEntities obCtx)
        {
            RequestAttributeCodeProvider reqAttrbCdProv = new RequestAttributeCodeProvider();

            bool isNewAttrib = false;
            Model.V_ReqSysAccAttrb dbReqAttrb = obCtx.V_ReqSysAccAttrb.Where(x => (x.ReqId == dbReq.ReqId) && (x.ReqAttrbCd == attrbCd)).FirstOrDefault();
            if ((dbReqAttrb == null) && (attrbVal != null))
            {
                dbReqAttrb = new Model.V_ReqSysAccAttrb();
                dbReqAttrb.ReqId = dbReq.ReqId;
                dbReqAttrb.ReqAttrbCd = attrbCd;
                dbReqAttrb.CreatedBy = userId;
                dbReqAttrb.CreatedDt = sqlDateTime;
                obCtx.V_ReqSysAccAttrb.Add(dbReqAttrb);
            }
            else
            {
                if (dbReqAttrb != null)
                {
                    LogReqSysAccAttrbChange(reqAttrbCdProv.GetRequestAttributeCodeDesc(attrbCd), dbReqAttrb.ReqAttrbVal, FormatLogValue(attrbVal), null, null, userId, sqlDateTime, dbReq, dbReqAttrb, obCtx);
                }
            }

            if (dbReqAttrb != null)
            {
                if (attrbVal != null)
                {
                    dbReqAttrb.ReqAttrbVal = ((DateTime)attrbVal).ToString("dd MMM yyyy HH:mm:ss");
                }
                else
                {
                    if (!isNewAttrib)
                    {
                        dbReqAttrb.ReqAttrbVal = null;
                    }
                }
            }
        }

        public static void DeleteReqSysAccAttrb(int reqId, Model.OnboardingEntities obCtx)
        {
            List<Model.V_ReqSysAccAttrb> listToDelete = obCtx.V_ReqSysAccAttrb.Where(x => x.ReqId == reqId).ToList();
            for (int i = 0; i < listToDelete.Count; i++)
            {
                obCtx.V_ReqSysAccAttrb.Remove(listToDelete[i]);
            }
            Common.SaveDbChanges(obCtx);
        }

        public static void MapReqAttachment(Entity.Request req, string userId, DateTime sqlDateTime, Model.V_Req dbReq, Model.OnboardingEntities obCtx)
        {
            Model.V_ReqAtt dbReqAttach = obCtx.V_ReqAtt.Where(x => x.ReqId == dbReq.ReqId).FirstOrDefault();
            if (req.Attachment != null)
            {
                if (dbReqAttach == null)
                {
                    dbReqAttach = new Model.V_ReqAtt();
                    dbReqAttach.ReqId = dbReq.ReqId;
                    dbReqAttach.CreatedBy = userId;
                    dbReqAttach.CreatedDt = sqlDateTime;
                    obCtx.V_ReqAtt.Add(dbReqAttach);
                }
                else
                {
                    dbReqAttach.ModifiedBy = userId;
                    dbReqAttach.ModifiedDt = sqlDateTime;
                }
                LogReqAttachChange("Attachment", dbReqAttach.FileNm, req.Attachment.Filename, null, null, userId, sqlDateTime, dbReq, dbReqAttach, obCtx);
                dbReqAttach.FileNm = req.Attachment.Filename;
            }
            else
            {
                if (dbReqAttach != null)
                {
                    LogReqAttachChange("Attachment", dbReqAttach.FileNm, string.Empty, null, null, userId, sqlDateTime, dbReq, dbReqAttach, obCtx);
                    obCtx.V_ReqAtt.Remove(dbReqAttach);
                }
            }
        }

        public static void MapReqAttachment(Entity.RequestAttachment reqAttach, string userId, DateTime sqlDateTime, Model.V_ReqAtt dbReqAttach, Model.OnboardingEntities obCtx)
        {
            if (reqAttach != null)
            {
                Model.V_Req dbReq = obCtx.V_Req.Where(x => x.ReqId == reqAttach.ReqId).FirstOrDefault();
                if ((dbReqAttach == null) || ((dbReqAttach != null) && (dbReqAttach.ReqAttId == 0)))
                {
                    if (dbReqAttach == null)
                    {
                        dbReqAttach = new Model.V_ReqAtt();
                    }
                    dbReqAttach.ReqId = reqAttach.ReqId;
                    dbReqAttach.CreatedBy = userId;
                    dbReqAttach.CreatedDt = sqlDateTime;
                    obCtx.V_ReqAtt.Add(dbReqAttach);
                }
                if ((dbReq != null) && (dbReq.ReqStsCd != Entity.Constant.RequestStatusCode.Draft))
                {
                        LogReqAttachChange("Attachment", dbReqAttach.FileNm, reqAttach.Filename, null, null, userId, sqlDateTime, dbReq, dbReqAttach, obCtx);
                }
                dbReqAttach.FileNm = reqAttach.Filename;
            }
            else
            {
                if ((dbReqAttach != null) && (dbReqAttach.ReqId > 0))
                {
                    Model.V_Req dbReq = obCtx.V_Req.Where(x => x.ReqId == dbReqAttach.ReqId).FirstOrDefault();
                    LogReqAttachChange("Attachment", dbReqAttach.FileNm, null, null, null, userId, sqlDateTime, dbReq, null, obCtx);
                }
            }
        }

        public static void DeleteReqAttachment(int reqId, Model.OnboardingEntities obCtx)
        {
            List<Model.V_ReqAtt> listToDelete = obCtx.V_ReqAtt.Where(x => x.ReqId == reqId).ToList();
            for (int i = 0; i < listToDelete.Count; i++)
            {
                obCtx.V_ReqAtt.Remove(listToDelete[i]);
            }
            Common.SaveDbChanges(obCtx);
        }
    }
}

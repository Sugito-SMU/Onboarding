using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onboarding.Common;

namespace Onboarding.DataProvider.Mapper
{
    public static partial class MapToEntity
    {
        public static void Map(Model.V_Req dbReq, Entity.Request req, string userId, Model.OnboardingEntities obCtx)
        {
            if ((dbReq != null) && (req != null))
            {
                RequestTypeProvider reqTypeProv = new RequestTypeProvider();
                SalutationProvider salutProv = new SalutationProvider();
                GenderProvider genderProv = new GenderProvider();
                RaceProvider raceProv = new RaceProvider();
                RequestStatusProvider reqStsProv = new RequestStatusProvider();
                UserProfileProvider usrProv = new UserProfileProvider();

                AESEncryption aes = new AESEncryption();

                req.ReqId = dbReq.ReqId;
                req.ReqTypeCd = dbReq.ReqTypeCd;
                req.ReqTypeDesc = reqTypeProv.GetRequestTypeDesc(dbReq.ReqTypeCd);
                req.FirstName = dbReq.FirstNm;
                req.LastName = dbReq.LastNm;
                if (string.IsNullOrWhiteSpace(dbReq.PrefNm))
                {
                    req.PrefNm = MapReqSysAccAttrbString(Entity.Constant.RequestAttributeCode.PropEmailDispName, dbReq, obCtx);
                }
                else
                {
                    req.PrefNm = dbReq.PrefNm;
                }
                req.SalutCd = dbReq.SalutCd;
                req.SalutDesc = salutProv.GetSalutationDesc(dbReq.SalutCd);
                req.GenderCd = dbReq.GenderCd;
                req.GenderDesc = genderProv.GetGenderDesc(dbReq.GenderCd);
                req.RaceCd = dbReq.RaceCd;
                req.RaceDesc = raceProv.GetRaceDesc(dbReq.RaceCd);
                req.PrsnEmail = dbReq.PrsnEmail;
                req.MobileNo = aes.Decrypt(dbReq.MobileNo);
                req.IsExistingStaff = dbReq.IsExistingStaff;
                req.IsExSmuStd = dbReq.IsExSmuStd;
                req.Remark = dbReq.Remark;
                req.ReqStsCd = dbReq.ReqStsCd;
                req.ReqStsDesc = reqStsProv.GetRequestStatusDesc(dbReq.ReqStsCd);
                req.SubmittedBy = dbReq.SubmittedBy;
                req.SubmittedDt = dbReq.SubmittedDt;
                if (!string.IsNullOrWhiteSpace(req.SubmittedBy))
                {
                    Entity.UserProfile usr = usrProv.GetUserProfile(req.SubmittedBy);
                    if ((usr != null) && (!string.IsNullOrWhiteSpace(usr.Name)))
                    {
                        req.SubmittedByName = usr.Name;
                    }
                    else
                    {
                        req.SubmittedByName = req.SubmittedBy;
                    }
                }
                req.IsActive = (dbReq.IsActive != null) ? (bool)dbReq.IsActive : false;
                req.IsAmended = dbReq.IsAmended;
                req.CreatedBy = dbReq.CreatedBy;
                req.CreatedDt = dbReq.CreatedDt;
                req.ModifiedBy = dbReq.ModifiedBy;
                req.ModifiedDt = dbReq.ModifiedDt;


                Model.V_ReqEmp dbReqEmp = obCtx.V_ReqEmp.Where(x => x.ReqId == dbReq.ReqId).FirstOrDefault();
                if (dbReqEmp != null)
                {
                    req.EmploymentInfo = new Entity.RequestEmployment();
                    Map(dbReqEmp, req.EmploymentInfo, obCtx);
                }
                req.Resources = new Entity.RequestResources();
                MapResources(dbReq, req.Resources, obCtx);
                MapReqAttachment(dbReq, req, obCtx);
                req.Tasks = new List<Entity.Task>();
                MapTasks(dbReq, req.Tasks, userId, obCtx);
                MapReqChangeList(dbReq, req, obCtx);
                MapOffTaskChangeList(dbReq, req, obCtx);
                MapSysAccTaskChangeList(dbReq, req, obCtx);

                if (req.TaskChangeList != null)
                {
                    req.TaskChangeList = req.TaskChangeList.OrderBy(x => x.ChangedDt).ToList();
                }

            }
        }

        public static void Map(Model.V_ReqEmp dbReqEmp, Entity.RequestEmployment reqEmp, Model.OnboardingEntities obCtx)
        {
            if ((dbReqEmp != null) && (reqEmp != null))
            {
                CostCentreProvider costCtrProv = new CostCentreProvider();
                UserTypeProvider userTypeProv = new UserTypeProvider();
                UserSubTypeProvider userSubTypeProv = new UserSubTypeProvider();

                reqEmp.ReqEmplId = dbReqEmp.ReqEmpId;
                reqEmp.CostCtrCd = dbReqEmp.CostCtrCd;
                reqEmp.CostCtrDesc = costCtrProv.GetCostCentreDesc(dbReqEmp.CostCtrCd);
                reqEmp.UserTypeCd = dbReqEmp.UserTypeCd;
                reqEmp.UserTypeDesc = userTypeProv.GetUserTypeDesc(dbReqEmp.UserTypeCd);
                reqEmp.UserSubTypeCd = dbReqEmp.UserSubTypeCd;
                reqEmp.UserSubTypeDesc = userSubTypeProv.GetUserSubTypeDesc(dbReqEmp.UserSubTypeCd);
                reqEmp.JobTitle = dbReqEmp.JobTitle;
                reqEmp.EmplStartDt = dbReqEmp.EmpStartDt;
                reqEmp.EmplEndDt = dbReqEmp.EmpEndDt;
                reqEmp.IsPrimary = dbReqEmp.IsPrimary;
                reqEmp.CreatedBy = dbReqEmp.CreatedBy;
                reqEmp.CreatedDt = dbReqEmp.CreatedDt;
                reqEmp.ModifiedBy = dbReqEmp.ModifiedBy;
                reqEmp.ModifiedDt = dbReqEmp.ModifiedDt;
            }
        }

        public static void MapResources(Model.V_Req dbReq, Entity.RequestResources reqRes, Model.OnboardingEntities obCtx)
        {
            if ((dbReq != null) && (reqRes != null))
            {
                reqRes.IsPCSelected = MapReqOffAttrbBool(Entity.Constant.RequestAttributeCode.IsPCSelected, dbReq, obCtx);
                reqRes.CorpTitleLevelCd = MapReqOffAttrbString(Entity.Constant.RequestAttributeCode.CorpTitleLevelCd, dbReq, obCtx);
                reqRes.CorpTitleLevelDesc = MapReqOffAttrbString(Entity.Constant.RequestAttributeCode.CorpTitleLevelDesc, dbReq, obCtx);
                reqRes.IsIPPhoneSelected = MapReqOffAttrbBool(Entity.Constant.RequestAttributeCode.IsIPPhoneSelected, dbReq, obCtx);
                reqRes.Building = MapReqOffAttrbString(Entity.Constant.RequestAttributeCode.Building, dbReq, obCtx);
                reqRes.Floor = MapReqOffAttrbString(Entity.Constant.RequestAttributeCode.Floor, dbReq, obCtx);
                reqRes.Room = MapReqOffAttrbString(Entity.Constant.RequestAttributeCode.Room, dbReq, obCtx);                
                reqRes.IsNetworkIDSelected = MapReqSysAccAttrbBool(Entity.Constant.RequestAttributeCode.IsNetworkIDSelected, dbReq, obCtx);
                reqRes.NetworkIDRemarks = MapReqSysAccAttrbString(Entity.Constant.RequestAttributeCode.NetworkIDRemarks, dbReq, obCtx);
                reqRes.IsEmailSelected = MapReqSysAccAttrbBool(Entity.Constant.RequestAttributeCode.IsEmailSelected, dbReq, obCtx);
                reqRes.IsSAPSelected = MapReqSysAccAttrbBool(Entity.Constant.RequestAttributeCode.IsSAPSelected, dbReq, obCtx);
                reqRes.DARTFund = MapReqSysAccAttrbString(Entity.Constant.RequestAttributeCode.DARTFund, dbReq, obCtx);
                reqRes.IsISISSelected = MapReqSysAccAttrbBool(Entity.Constant.RequestAttributeCode.IsISISSelected, dbReq, obCtx);
                reqRes.ISISRemarks = MapReqSysAccAttrbString(Entity.Constant.RequestAttributeCode.ISISRemarks, dbReq, obCtx);
                reqRes.IsELearnSelected = MapReqSysAccAttrbBool(Entity.Constant.RequestAttributeCode.IsELearnSelected, dbReq, obCtx);
                reqRes.ELearnStartDate = MapReqSysAccAttrbDateTime(Entity.Constant.RequestAttributeCode.ELearnStartDate, dbReq, obCtx);
                reqRes.ELearnRemarks = MapReqSysAccAttrbString(Entity.Constant.RequestAttributeCode.ELearnRemarks, dbReq, obCtx);
                reqRes.IsMtgRoomSelected = MapReqSysAccAttrbBool(Entity.Constant.RequestAttributeCode.IsMtgRoomSelected, dbReq, obCtx);
                reqRes.MtgRoomDetails = MapReqSysAccAttrbString(Entity.Constant.RequestAttributeCode.MtgRoomDetails, dbReq, obCtx);
                reqRes.IsINetSelected = MapReqSysAccAttrbBool(Entity.Constant.RequestAttributeCode.IsINetSelected, dbReq, obCtx);
                reqRes.IsOasisSelected = MapReqSysAccAttrbBool(Entity.Constant.RequestAttributeCode.IsOasisSelected, dbReq, obCtx);
                reqRes.IsNextwebSelected = MapReqSysAccAttrbBool(Entity.Constant.RequestAttributeCode.IsNextwebSelected, dbReq, obCtx);
                reqRes.IsEmailDLSelected = MapReqSysAccAttrbBool(Entity.Constant.RequestAttributeCode.IsEmailDLSelected, dbReq, obCtx);
                reqRes.EmailDLDetails = MapReqSysAccAttrbString(Entity.Constant.RequestAttributeCode.EmailDLDetails, dbReq, obCtx);
                reqRes.NetworkID = MapReqSysAccAttrbString(Entity.Constant.RequestAttributeCode.NetworkID, dbReq, obCtx);
            }
        }

        public static bool? MapReqOffAttrbBool(string attrbCd, Model.V_Req dbReq, Model.OnboardingEntities obCtx)
        {
            bool? attrbVal = null;
            Model.V_ReqOffAttrb dbAttrb = obCtx.V_ReqOffAttrb.Where(x => (x.ReqId == dbReq.ReqId) && (x.ReqAttrbCd == attrbCd)).FirstOrDefault();
            if (dbAttrb != null)
            {
                if (dbAttrb.ReqAttrbVal != null)
                {
                    switch (dbAttrb.ReqAttrbVal)
                    {
                        case "Y":
                            attrbVal = true;
                            break;
                        case "N":
                            attrbVal = false;
                            break;
                        default:
                            attrbVal = null;
                            break;
                    }
                }
                else
                {
                    attrbVal = null;
                }
            }
            return attrbVal;
        }

        public static string MapReqOffAttrbString(string attrbCd, Model.V_Req dbReq, Model.OnboardingEntities obCtx)
        {
            string attrbVal = null;
            Model.V_ReqOffAttrb dbAttrb = obCtx.V_ReqOffAttrb.Where(x => (x.ReqId == dbReq.ReqId) && (x.ReqAttrbCd == attrbCd)).FirstOrDefault();
            if (dbAttrb != null)
            {
                attrbVal = dbAttrb.ReqAttrbVal;
            }
            return attrbVal;
        }

        public static DateTime? MapReqOffAttrbDateTime(string attrbCd, Model.V_Req dbReq, Model.OnboardingEntities obCtx)
        {
            DateTime? attrbVal = null;
            Model.V_ReqOffAttrb dbAttrb = obCtx.V_ReqOffAttrb.Where(x => (x.ReqId == dbReq.ReqId) && (x.ReqAttrbCd == attrbCd)).FirstOrDefault();
            if (dbAttrb != null)
            {
                if (dbAttrb.ReqAttrbVal != null)
                {
                    DateTime dt = DateTime.MinValue;
                    if (DateTime.TryParse(dbAttrb.ReqAttrbVal, out dt))
                    {
                        attrbVal = dt;
                    }
                }
                else
                {
                    attrbVal = null;
                }
            }
            else
            {
                attrbVal = null;
            }
            return attrbVal;
        }
        
        public static bool? MapReqSysAccAttrbBool(string attrbCd, Model.V_Req dbReq, Model.OnboardingEntities obCtx)
        {
            bool? attrbVal = null;
            Model.V_ReqSysAccAttrb dbAttrb = obCtx.V_ReqSysAccAttrb.Where(x => (x.ReqId == dbReq.ReqId) && (x.ReqAttrbCd == attrbCd)).FirstOrDefault();
            if (dbAttrb != null)
            {
                if (dbAttrb.ReqAttrbVal != null)
                {
                    switch (dbAttrb.ReqAttrbVal)
                    {
                        case "Y":
                            attrbVal = true;
                            break;
                        case "N":
                            attrbVal = false;
                            break;
                        default:
                            attrbVal = null;
                            break;
                    }
                }
                else
                {
                    attrbVal = null;
                }
            }
            return attrbVal;
        }

        public static string MapReqSysAccAttrbString(string attrbCd, Model.V_Req dbReq, Model.OnboardingEntities obCtx)
        {
            string attrbVal = null;
            Model.V_ReqSysAccAttrb dbAttrb = obCtx.V_ReqSysAccAttrb.Where(x => (x.ReqId == dbReq.ReqId) && (x.ReqAttrbCd == attrbCd)).FirstOrDefault();
            if (dbAttrb != null)
            {
                attrbVal = dbAttrb.ReqAttrbVal;
            }
            return attrbVal;
        }

        public static DateTime? MapReqSysAccAttrbDateTime(string attrbCd, Model.V_Req dbReq, Model.OnboardingEntities obCtx)
        {
            DateTime? attrbVal = null;
            Model.V_ReqSysAccAttrb dbAttrb = obCtx.V_ReqSysAccAttrb.Where(x => (x.ReqId == dbReq.ReqId) && (x.ReqAttrbCd == attrbCd)).FirstOrDefault();
            if (dbAttrb != null)
            {
                if (dbAttrb.ReqAttrbVal != null)
                {
                    DateTime dt = DateTime.MinValue;
                    if (DateTime.TryParse(dbAttrb.ReqAttrbVal, out dt))
                    {
                        attrbVal = dt;
                    }
                }
                else
                {
                    attrbVal = null;
                }
            }
            else
            {
                attrbVal = null;
            }
            return attrbVal;
        }




        public static void MapReqAttachment(Model.V_Req dbReq, Entity.Request req, Model.OnboardingEntities obCtx)
        {
            Model.V_ReqAtt dbReqAttach = obCtx.V_ReqAtt.Where(x => x.ReqId == dbReq.ReqId).FirstOrDefault();
            if (dbReqAttach != null)
            {
                if (req.Attachment == null)
                {
                    req.Attachment = new Entity.RequestAttachment();
                }
                req.Attachment.ReqAttachId = dbReqAttach.ReqAttId;
                req.Attachment.ReqId = dbReqAttach.ReqId;
                req.Attachment.Filename = dbReqAttach.FileNm;
                req.Attachment.CreatedBy = dbReq.CreatedBy;
                req.Attachment.CreatedDt = dbReq.CreatedDt;
                req.Attachment.ModifiedBy = dbReqAttach.ModifiedBy;
                req.Attachment.ModifiedDt = dbReqAttach.ModifiedDt;
            }
            else
            {
                req.Attachment = null;
            }
        }

        public static void MapReqAttachment(Model.V_ReqAtt dbReqAtt, Entity.RequestAttachment reqAtt)
        {
            if (dbReqAtt != null)
            {
                if (reqAtt == null)
                {
                    reqAtt = new Entity.RequestAttachment();
                }
                reqAtt.ReqAttachId = dbReqAtt.ReqAttId;
                reqAtt.ReqId = dbReqAtt.ReqId;
                reqAtt.Filename = dbReqAtt.FileNm;
                reqAtt.CreatedBy = dbReqAtt.CreatedBy;
                reqAtt.CreatedDt = dbReqAtt.CreatedDt;
                reqAtt.ModifiedBy = dbReqAtt.ModifiedBy;
                reqAtt.ModifiedDt = dbReqAtt.ModifiedDt;
            }
            else
            {
                reqAtt = null;
            }
        }

        public static void MapReqChangeList(Model.V_Req dbReq, Entity.Request req, Model.OnboardingEntities obCtx)
        {
            List<Model.V_ReqLog> dbReqChangeList = obCtx.V_ReqLog.Where(x => x.ReqId == dbReq.ReqId).ToList();
            if ((dbReqChangeList != null) && (dbReqChangeList.Count > 0))
            {
                if (req.ReqChangeList == null)
                {
                    req.ReqChangeList = new List<Entity.RequestChangeLog>();
                }
                foreach (Model.V_ReqLog dbReqChangeLog in dbReqChangeList.OrderBy(x => x.ReqLogId))
                {
                    Entity.RequestChangeLog reqChangeLog = new Entity.RequestChangeLog();
                    Map(dbReqChangeLog, reqChangeLog, obCtx);
                    req.ReqChangeList.Add(reqChangeLog);
                }
            }
        }

        public static void MapOffTaskChangeList(Model.V_Req dbReq, Entity.Request req, Model.OnboardingEntities obCtx)
        {
            List<Model.V_OffTaskLog> dbOffTaskChangeList = null;
            var dbtcl = from a in obCtx.V_OffTask
                        join b in obCtx.V_OffTaskLog
                            on a.OffTaskId equals b.OffTaskId
                        where (a.ReqId == dbReq.ReqId)
                        select b;
            dbOffTaskChangeList = dbtcl.ToList<Model.V_OffTaskLog>();
            if ((dbOffTaskChangeList != null) && (dbOffTaskChangeList.Count > 0))
            {
                if (req.TaskChangeList == null)
                {
                    req.TaskChangeList = new List<Entity.TaskChangeLog>();
                }
                foreach (Model.V_OffTaskLog dbOffTaskChangeLog in dbOffTaskChangeList.OrderBy(x => x.OffTaskLogId))
                {
                    Entity.TaskChangeLog offTaskChangeLog = new Entity.TaskChangeLog();
                    Map(dbOffTaskChangeLog, offTaskChangeLog, obCtx);
                    req.TaskChangeList.Add(offTaskChangeLog);
                }
            }
        }

        public static void MapSysAccTaskChangeList(Model.V_Req dbReq, Entity.Request req, Model.OnboardingEntities obCtx)
        {
            List<Model.V_SysAccTaskLog> dbSysAccTaskChangeList = null;
            var dbtcl = from a in obCtx.V_SysAccTask
                        join b in obCtx.V_SysAccTaskLog
                            on a.SysAccTaskId equals b.SysAccTaskId
                        where (a.ReqId == dbReq.ReqId)
                        select b;
            dbSysAccTaskChangeList = dbtcl.ToList<Model.V_SysAccTaskLog>();
            if ((dbSysAccTaskChangeList != null) && (dbSysAccTaskChangeList.Count > 0))
            {
                if (req.TaskChangeList == null)
                {
                    req.TaskChangeList = new List<Entity.TaskChangeLog>();
                }
                foreach (Model.V_SysAccTaskLog dbSysAccTaskChangeLog in dbSysAccTaskChangeList.OrderBy(x => x.SysAccTaskLogId))
                {
                    Entity.TaskChangeLog SysAccTaskChangeLog = new Entity.TaskChangeLog();
                    Map(dbSysAccTaskChangeLog, SysAccTaskChangeLog, obCtx);
                    req.TaskChangeList.Add(SysAccTaskChangeLog);
                }
            }
        }
    }
}

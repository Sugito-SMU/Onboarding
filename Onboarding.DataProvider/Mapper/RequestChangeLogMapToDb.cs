using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onboarding;

namespace Onboarding.DataProvider.Mapper
{
    public static partial class MapToDb
    {

        public static void LogReqChange(string fieldName, string fromValue, string toValue, string fromValueDesc, string toValueDesc, string userId, DateTime sqlDateTime, Model.V_Req dbReq, Model.OnboardingEntities obCtx)
        {
            if ((dbReq.ReqId > 0) && (IsLogValueDifferent(fromValue, toValue)))
            {
                // Log change only if the request is **not DRAFT**
                if (dbReq.ReqStsCd != Entity.Constant.RequestStatusCode.Draft)
                {
                    dbReq.IsAmended = true;
                    MapReqChangeLog(fieldName, FormatLogDesc(fromValue, fromValueDesc), FormatLogDesc(toValue, toValueDesc), userId, sqlDateTime, dbReq.ReqId, obCtx);
                }
                dbReq.ModifiedBy = userId;
                dbReq.ModifiedDt = sqlDateTime;
            }
        }

        public static void LogReqChangeNoCheck(string fieldName, string fromValue, string toValue, string fromValueDesc, string toValueDesc, string userId, DateTime sqlDateTime, Model.V_Req dbReq, Model.OnboardingEntities obCtx)
        {
            if (dbReq.ReqId > 0)
            {
                // Log change only if the request is **not DRAFT**
                if (dbReq.ReqStsCd != Entity.Constant.RequestStatusCode.Draft)
                {
                    dbReq.IsAmended = true;
                    MapReqChangeLog(fieldName, FormatLogDesc(fromValue, fromValueDesc), FormatLogDesc(toValue, toValueDesc), userId, sqlDateTime, dbReq.ReqId, obCtx);
                }
                dbReq.ModifiedBy = userId;
                dbReq.ModifiedDt = sqlDateTime;
            }
        }


        public static void LogReqEmplChange(string fieldName, string fromValue, string toValue, string fromValueDesc, string toValueDesc, string userId, DateTime sqlDateTime, Model.V_Req dbReq, Model.V_ReqEmp dbReqEmpl, Model.OnboardingEntities obCtx)
        {
            if ((dbReq.ReqId > 0) && (dbReqEmpl.ReqEmpId > 0) && (IsLogValueDifferent(fromValue, toValue)))
            {
                // Log change only if the request is **not DRAFT**
                if (dbReq.ReqStsCd != Entity.Constant.RequestStatusCode.Draft)
                {
                    dbReq.IsAmended = true;
                    MapReqChangeLog(fieldName, FormatLogDesc(fromValue, fromValueDesc), FormatLogDesc(toValue, toValueDesc), userId, sqlDateTime, dbReq.ReqId, obCtx);
                }
                dbReq.ModifiedBy = userId;
                dbReq.ModifiedDt = sqlDateTime;
                dbReqEmpl.ModifiedBy = userId;
                dbReqEmpl.ModifiedDt = sqlDateTime;
            }
        }

        public static void LogReqOffAttrbChange(string fieldName, string fromValue, string toValue, string fromValueDesc, string toValueDesc, string userId, DateTime sqlDateTime, Model.V_Req dbReq, Model.V_ReqOffAttrb dbReqOffAttrb, Model.OnboardingEntities obCtx)
        {
            if ((dbReq.ReqId > 0) && (dbReqOffAttrb.ReqOffAttrbId > 0) && (IsLogValueDifferent(fromValue, toValue)))
            {
                // Log change only if the request is **not DRAFT**
                if (dbReq.ReqStsCd != Entity.Constant.RequestStatusCode.Draft)
                {
                    dbReq.IsAmended = true;
                    MapReqChangeLog(fieldName, FormatLogDesc(fromValue, fromValueDesc), FormatLogDesc(toValue, toValueDesc), userId, sqlDateTime, dbReq.ReqId, obCtx);
                }
                dbReq.ModifiedBy = userId;
                dbReq.ModifiedDt = sqlDateTime;
                dbReqOffAttrb.ModifiedBy = userId;
                dbReqOffAttrb.ModifiedDt = sqlDateTime;
            }
        }

        public static void LogReqSysAccAttrbChange(string fieldName, string fromValue, string toValue, string fromValueDesc, string toValueDesc, string userId, DateTime sqlDateTime, Model.V_Req dbReq, Model.V_ReqSysAccAttrb dbReqSysAccAttrb, Model.OnboardingEntities obCtx)
        {
            if ((dbReq.ReqId > 0) && (dbReqSysAccAttrb.ReqSysAccAttrbId > 0) && (IsLogValueDifferent(fromValue, toValue)))
            {
                // Log change only if the request is **not DRAFT**
                if (dbReq.ReqStsCd != Entity.Constant.RequestStatusCode.Draft)
                {
                    dbReq.IsAmended = true;
                    MapReqChangeLog(fieldName, FormatLogDesc(fromValue, fromValueDesc), FormatLogDesc(toValue, toValueDesc), userId, sqlDateTime, dbReq.ReqId, obCtx);
                }
                dbReq.ModifiedBy = userId;
                dbReq.ModifiedDt = sqlDateTime;
                dbReqSysAccAttrb.ModifiedBy = userId;
                dbReqSysAccAttrb.ModifiedDt = sqlDateTime;
            }
        }


        public static void LogReqAttachChange(string fieldName, string fromValue, string toValue, string fromValueDesc, string toValueDesc, string userId, DateTime sqlDateTime, Model.V_Req dbReq, Model.V_ReqAtt dbReqAttach, Model.OnboardingEntities obCtx)
        {
            if ((dbReq.ReqId > 0) && (dbReqAttach.ReqAttId > 0) && (IsLogValueDifferent(fromValue, toValue)))
            {
                // Log change only if the request is **not DRAFT**
                if (dbReq.ReqStsCd != Entity.Constant.RequestStatusCode.Draft)
                {
                    dbReq.IsAmended = true;
                    MapReqChangeLog(fieldName, FormatLogDesc(fromValue, fromValueDesc), FormatLogDesc(toValue, toValueDesc), userId, sqlDateTime, dbReq.ReqId, obCtx);
                }
                dbReq.ModifiedBy = userId;
                dbReq.ModifiedDt = sqlDateTime;
                dbReqAttach.ModifiedBy = userId;
                dbReqAttach.ModifiedDt = sqlDateTime;
            }
        }


        public static void MapReqChangeLog(string fieldName, string fromValue, string toValue, string userId, DateTime sqlDateTime, int reqId, Model.OnboardingEntities obCtx)
        {
            Model.V_ReqLog dbReqChangeLog = new Model.V_ReqLog();
            dbReqChangeLog.ReqId = reqId;
            dbReqChangeLog.FieldNm = fieldName != null ? fieldName : string.Empty;
            dbReqChangeLog.FromVal = fromValue != null ? fromValue : string.Empty;
            dbReqChangeLog.ToVal = toValue != null ? toValue : string.Empty;
            dbReqChangeLog.ChangeBy = userId;
            dbReqChangeLog.ChangeDt = sqlDateTime;
            obCtx.V_ReqLog.Add(dbReqChangeLog);
        }


    }
}

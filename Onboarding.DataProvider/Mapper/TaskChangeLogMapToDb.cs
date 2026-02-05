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

        public static void LogOffTaskChange(string fieldName, string fromValue, string toValue, string fromValueDesc, string toValueDesc, string userId, DateTime sqlDateTime, Model.V_Req dbReq, Model.V_OffTask dbOffTask, Model.OnboardingEntities obCtx)
        {
            if ((dbReq.ReqId > 0) && (dbOffTask.OffTaskId > 0) && (IsLogValueDifferent(fromValue, toValue)))
            {
                // Log change only if the request is **not DRAFT**
                if (dbReq.ReqStsCd != Entity.Constant.RequestStatusCode.Draft)
                {
                    MapOffTaskChangeLog(FormatTaskChangeFieldName(dbOffTask.TaskCd, fieldName), FormatLogDesc(fromValue, fromValueDesc), FormatLogDesc(toValue, toValueDesc), userId, sqlDateTime, dbOffTask.OffTaskId, obCtx);
                }
                dbOffTask.ModifiedBy = userId;
                dbOffTask.ModifiedDt = sqlDateTime;
            }
        }

        public static void LogSysAccTaskChange(string fieldName, string fromValue, string toValue, string fromValueDesc, string toValueDesc, string userId, DateTime sqlDateTime, Model.V_Req dbReq, Model.V_SysAccTask dbSysAccTask, Model.OnboardingEntities obCtx)
        {
            if ((dbReq.ReqId > 0) && (dbSysAccTask.SysAccTaskId > 0) && (IsLogValueDifferent(fromValue, toValue)))
            {
                // Log change only if the request is **not DRAFT**
                if (dbReq.ReqStsCd != Entity.Constant.RequestStatusCode.Draft)
                {
                    MapSysAccTaskChangeLog(FormatTaskChangeFieldName(dbSysAccTask.TaskCd, fieldName), FormatLogDesc(fromValue, fromValueDesc), FormatLogDesc(toValue, toValueDesc), userId, sqlDateTime, dbSysAccTask.SysAccTaskId, obCtx);
                }
                dbSysAccTask.ModifiedBy = userId;
                dbSysAccTask.ModifiedDt = sqlDateTime;
            }
        }


        public static void LogOffTaskAttrbChange(string fromValue, string toValue, string fromValueDesc, string toValueDesc, string userId, DateTime sqlDateTime, Model.V_Req dbReq, Model.V_OffTask dbOffTask, Model.V_OffTaskAttrb dbOffTaskAttrb, Model.OnboardingEntities obCtx)
        {
            TaskAttributeCodeProvider taskAttrbCodeProv = new TaskAttributeCodeProvider();

            if ((dbReq.ReqId > 0) && (dbOffTaskAttrb.OffTaskAttrbId > 0) && (IsLogValueDifferent(fromValue, toValue)))
            {
                // Log change only if the request is **not DRAFT**
                if (dbReq.ReqStsCd != Entity.Constant.RequestStatusCode.Draft)
                {
                    MapOffTaskChangeLog(FormatTaskChangeFieldName(dbOffTask.TaskCd, taskAttrbCodeProv.GetTaskAttributeCodeDesc(dbOffTaskAttrb.TaskAttrbCd)), FormatLogDesc(fromValue, fromValueDesc), FormatLogDesc(toValue, toValueDesc), userId, sqlDateTime, dbOffTask.OffTaskId, obCtx);
                }
                dbOffTask.ModifiedBy = userId;
                dbOffTask.ModifiedDt = sqlDateTime;
                dbOffTaskAttrb.ModifiedBy = userId;
                dbOffTaskAttrb.ModifiedDt = sqlDateTime;
            }
        }

        public static void LogSysAccTaskAttrbChange(string fromValue, string toValue, string fromValueDesc, string toValueDesc, string userId, DateTime sqlDateTime, Model.V_Req dbReq, Model.V_SysAccTask dbSysAccTask, Model.V_SysAccTaskAttrb dbSysAccTaskAttrb, Model.OnboardingEntities obCtx)
        {
            TaskAttributeCodeProvider taskAttrbCodeProv = new TaskAttributeCodeProvider();

            if ((dbReq.ReqId > 0) && (dbSysAccTaskAttrb.SysAccTaskAttrbId > 0) && IsLogValueDifferent(fromValue, toValue))
            {
                // Log change only if the request is **not DRAFT**
                if ((dbReq.ReqId > 0) && (dbReq.ReqStsCd != Entity.Constant.RequestStatusCode.Draft))
                {
                    MapSysAccTaskChangeLog(FormatTaskChangeFieldName(dbSysAccTask.TaskCd, taskAttrbCodeProv.GetTaskAttributeCodeDesc(dbSysAccTaskAttrb.TaskAttrbCd)), FormatLogDesc(fromValue, fromValueDesc), FormatLogDesc(toValue, toValueDesc), userId, sqlDateTime, dbSysAccTask.SysAccTaskId, obCtx);
                }

                dbSysAccTask.ModifiedBy = userId;
                dbSysAccTask.ModifiedDt = sqlDateTime;
                dbSysAccTaskAttrb.ModifiedBy = userId;
                dbSysAccTaskAttrb.ModifiedDt = sqlDateTime;
            }
        }


        public static void MapOffTaskChangeLog(string fieldName, string fromValue, string toValue, string userId, DateTime sqlDateTime, int taskId, Model.OnboardingEntities obCtx)
        {
            Model.V_OffTaskLog dbOffTaskChangeLog = new Model.V_OffTaskLog();
            dbOffTaskChangeLog.OffTaskId = taskId;
            dbOffTaskChangeLog.FieldNm = fieldName != null ? fieldName : string.Empty;
            dbOffTaskChangeLog.FromVal = fromValue != null ? fromValue : string.Empty;
            dbOffTaskChangeLog.ToVal = toValue != null ? toValue : string.Empty;
            dbOffTaskChangeLog.ChangeBy = userId;
            dbOffTaskChangeLog.ChangeDt = sqlDateTime;
            obCtx.V_OffTaskLog.Add(dbOffTaskChangeLog);
        }

        public static void MapSysAccTaskChangeLog(string fieldName, string fromValue, string toValue, string userId, DateTime sqlDateTime, int taskId, Model.OnboardingEntities obCtx)
        {
            Model.V_SysAccTaskLog dbSysAccTaskChangeLog = new Model.V_SysAccTaskLog();
            dbSysAccTaskChangeLog.SysAccTaskId = taskId;
            dbSysAccTaskChangeLog.FieldNm = fieldName != null ? fieldName : string.Empty;
            dbSysAccTaskChangeLog.FromVal = fromValue != null ? fromValue : string.Empty;
            dbSysAccTaskChangeLog.ToVal = toValue != null ? toValue : string.Empty;
            dbSysAccTaskChangeLog.ChangeBy = userId;
            dbSysAccTaskChangeLog.ChangeDt = sqlDateTime;
            obCtx.V_SysAccTaskLog.Add(dbSysAccTaskChangeLog);
        }

        public static string FormatTaskChangeFieldName(string taskCd, string fieldNm)
        {
            TaskCodeProvider taskCodeProv = new TaskCodeProvider();
            string formattedFieldNm = string.Format("{0}: {1}", taskCodeProv.GetTaskCodeDesc(taskCd), fieldNm);
            return formattedFieldNm;
        }



    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Onboarding;

namespace Onboarding.DataProvider
{
    public class RequestProvider
    {
        DataProvider.Model.OnboardingEntities _obCtx = null;
        public RequestProvider(DataProvider.Model.OnboardingEntities obCtx)
        {
            _obCtx = obCtx;
        }

        public Entity.Request GetRequest(int reqId, string userId)
        {
            Entity.Request req = null;
            if (reqId > 0)
            {                
                Model.V_Req dbReq = _obCtx.V_Req.FirstOrDefault(x => x.ReqId == reqId);
                if (dbReq != null)
                {
                    req = new Entity.Request();
                    Mapper.MapToEntity.Map(dbReq, req, userId, _obCtx);
                }
            }
            return req;
        }

        public string GetRequestStatus(int reqId)
        {
            string reqStsCd = null;
            if (reqId > 0)
            {
                Model.V_Req dbReq = _obCtx.V_Req.FirstOrDefault(x => x.ReqId == reqId);
                if (dbReq != null)
                {
                    reqStsCd = dbReq.ReqStsCd;
                }
            }
            return reqStsCd;
        }

        public int SaveRequest(Entity.Request req, string userId)
        {
            int reqId = 0;
            Model.V_Req dbReq = null;
            DateTime sqlDateTime = Common.GetSqlDateTime();

            if (req.ReqId != 0)
            {
                dbReq = _obCtx.V_Req.FirstOrDefault(x => x.ReqId == req.ReqId);
            }
            else
            {
                dbReq = new Model.V_Req();
            }
            if (dbReq != null)
            {
                Mapper.MapToDb.Map(req, dbReq, userId, sqlDateTime, _obCtx);
                Common.SaveDbChanges(_obCtx);
                reqId = dbReq.ReqId;
            }
            return reqId;
        }

        public bool DeleteRequest(int reqId, string userId)
        {
            bool success = false;
            if (reqId != 0)
            {
                //Delete is allowed if:
                //Request status is draft
                //User is the requestor him/herself
                Model.V_Req dbReq = _obCtx.V_Req.FirstOrDefault(x => x.ReqId == reqId);
                if ((dbReq != null) && (dbReq.ReqStsCd == Entity.Constant.RequestStatusCode.Draft) &&
                    (dbReq.CreatedBy.ToLower().Trim() == userId.ToLower().Trim()))
                {
                    Mapper.MapToDb.DeleteReqAttachment(reqId, _obCtx);
                    Mapper.MapToDb.DeleteReqOffAttrb(reqId, _obCtx);
                    Mapper.MapToDb.DeleteReqSysAccAttrb(reqId, _obCtx);
                    Mapper.MapToDb.DeleteReqEmp(reqId, _obCtx);
                    Mapper.MapToDb.DeleteReq(reqId, _obCtx);
                    Common.SaveDbChanges(_obCtx);
                    success = true;
                }
            }
            return success;
        }

        public Entity.RequestAttachment GetRequestAttachment(int reqAttId)
        {
            Entity.RequestAttachment reqAtt = null;
            if (reqAttId > 0)
            {
                Model.V_ReqAtt dbReqAtt = _obCtx.V_ReqAtt.FirstOrDefault(x => x.ReqAttId == reqAttId);
                if (dbReqAtt != null)
                {
                    reqAtt = new Entity.RequestAttachment();
                    Mapper.MapToEntity.MapReqAttachment(dbReqAtt, reqAtt);
                }
            }
            return reqAtt;
        }

        public int SaveRequestAttachment(Entity.RequestAttachment reqAtt, string userId)
        {
            int reqAttId = 0;
            DateTime sqlDateTime = Common.GetSqlDateTime();

            if ((reqAtt != null) && (reqAtt.ReqId > 0))
            {
                Model.V_ReqAtt dbReqAtt = _obCtx.V_ReqAtt.FirstOrDefault(x => x.ReqId == reqAtt.ReqId);
                if (dbReqAtt == null)
                {
                    dbReqAtt = new Model.V_ReqAtt();
                }
                Mapper.MapToDb.MapReqAttachment(reqAtt, userId, sqlDateTime, dbReqAtt, _obCtx);
                Common.SaveDbChanges(_obCtx);
                if (dbReqAtt != null)
                {
                    reqAttId = dbReqAtt.ReqAttId;
                }
            }
            return reqAttId;
        }

        public bool DeleteRequestAttachment(int reqAttId, string userId)
        {
            bool success = false;
            DateTime sqlDateTime = Common.GetSqlDateTime();
            if (reqAttId > 0)
            {
                Model.V_ReqAtt dbReqAtt = _obCtx.V_ReqAtt.FirstOrDefault(x => x.ReqAttId == reqAttId);
                if (dbReqAtt != null)
                {
                    Mapper.MapToDb.MapReqAttachment(null, userId, sqlDateTime, dbReqAtt, _obCtx);
                    _obCtx.V_ReqAtt.Remove(dbReqAtt);
                    Common.SaveDbChanges(_obCtx);
                    success = true;
                }
            }
            return success;
        }

    }
}

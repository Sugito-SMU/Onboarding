using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onboarding.Entity;

namespace Onboarding.Common
{
    public class TaskValidation
    {
        public static Entity.TaskValidationError ValidateTask(Onboarding.Entity.Task tsk, Onboarding.Entity.Request req)
        {

            Entity.TaskValidationError tskValError = new Entity.TaskValidationError();

            #region Mandatory Field
            if (tsk.TaskCd == Entity.Constant.TaskCode.CreateEmail)
            {
                if ((tsk.Attributes == null) ||
                    (tsk.GetTaskAttribute(Entity.Constant.TaskAttributeCode.AssignedEmailAddress) == null) ||
                    (string.IsNullOrEmpty(tsk.GetTaskAttribute(Entity.Constant.TaskAttributeCode.AssignedEmailAddress).TaskAttrbVal)))
                {
                    tskValError.SetError(Entity.Constant.TaskAttributeCode.AssignedEmailAddress, Entity.Constant.TaskAttributeMandatoryError.AssignedEmailAddress);
                }
                if ((tsk.Attributes == null) ||
                    (tsk.GetTaskAttribute(Entity.Constant.TaskAttributeCode.AssignedEmailDispNm) == null) ||
                    (string.IsNullOrEmpty(tsk.GetTaskAttribute(Entity.Constant.TaskAttributeCode.AssignedEmailDispNm).TaskAttrbVal)))
                {
                    tskValError.SetError(Entity.Constant.TaskAttributeCode.AssignedEmailDispNm, Entity.Constant.TaskAttributeMandatoryError.AssignedEmailDispNm);
                }
            }

            if (tsk.TaskCd == Entity.Constant.TaskCode.CreateNTID)
            {
                if (req.ReqStsCd != Entity.Constant.RequestStatusCode.Cancelled)
                {
                    if ((tsk.Attributes == null) ||
                        (tsk.GetTaskAttribute(Entity.Constant.TaskAttributeCode.AssignedNTID) == null) ||
                        (string.IsNullOrEmpty(tsk.GetTaskAttribute(Entity.Constant.TaskAttributeCode.AssignedNTID).TaskAttrbVal)))
                    {
                        tskValError.SetError(Entity.Constant.TaskAttributeCode.AssignedNTID, Entity.Constant.TaskAttributeMandatoryError.AssignedNTID);
                    }
                }
            }
            #endregion


            #region ValidateLength

            if (tsk.Attributes != null)
            {
                foreach (TaskAttribute tskAtt in tsk.Attributes)
                {
                    if (!string.IsNullOrEmpty(tskAtt.TaskAttrbVal) &&
                        (tskAtt.TaskAttrbVal.Length > Entity.Constant.TaskAttributeLength.GetLength(tskAtt.TaskAttrbCd)))
                    {
                        tskValError.SetError(tskAtt.TaskAttrbCd, Entity.Constant.TaskAttributeLengthError.GetErrorMessage(tskAtt.TaskAttrbCd));
                    }
                }
            }

            if (!string.IsNullOrEmpty(tsk.Remark) && 
                (tsk.Remark.Length > Entity.Constant.TaskAttributeLength.GetRemarkLength()))
            {
                tskValError.SetError(Entity.Constant.TaskAttributeCode.Remark, Entity.Constant.TaskAttributeLengthError.Remark);
            }

            #endregion


            return tskValError;
        }
    }
}

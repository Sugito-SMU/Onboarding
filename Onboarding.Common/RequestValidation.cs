using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Onboarding.Entity;

namespace Onboarding.Common
{
    public class RequestValidation
    {
        public static bool IsMandatoryField(string fieldCode, Request req, bool isBulkRequest)
        {
            bool isMandatory = false;

            if ((fieldCode == Entity.Constant.RequestField.ReqTypeCd))
            {
                isMandatory = true;
            }

            if ((fieldCode == Entity.Constant.RequestField.FirstName))
            {
                isMandatory = true;
            }

            if (fieldCode == Entity.Constant.RequestField.PrefNm)
            {
                isMandatory = true;
            }

            if ((fieldCode == Entity.Constant.RequestField.UserTypeCd))
            {
                isMandatory = true;
            }

            if ((fieldCode == Entity.Constant.RequestField.UserSubTypeCd))
            {
                isMandatory = true;
            }

            if ((fieldCode == Entity.Constant.RequestField.EmplStartDt))
            {
                isMandatory = true;
            }

            if ((fieldCode == Entity.Constant.RequestField.CostCtrCd))
            {
                isMandatory = true;
            }

            if ((fieldCode == Entity.Constant.RequestField.EmplEndDt) &&
                (req.EmploymentInfo != null) &&
                (!Validation.IsPermStaff(req.EmploymentInfo.UserSubTypeCd)))
            {
                isMandatory = true;
            }

            if ((fieldCode == Entity.Constant.RequestField.PrsnEmail) &&
                (req.EmploymentInfo != null) &&
                ((req.EmploymentInfo.UserTypeCd == Entity.Constant.UserType.Faculty) ||
                 (req.ReqTypeCd == Entity.Constant.RequestTypeCode.NewHire) || (req.ReqTypeCd == Entity.Constant.RequestTypeCode.ReEntry)))
            {
                isMandatory = true;
            }

            if ((fieldCode == Entity.Constant.RequestField.MobileNo) &&
                (req.EmploymentInfo != null) &&
                ((req.ReqTypeCd == Entity.Constant.RequestTypeCode.NewHire) || (req.ReqTypeCd == Entity.Constant.RequestTypeCode.ReEntry)))
            {
                isMandatory = true;
            }

            if ((fieldCode == Entity.Constant.RequestField.CorpTitleLevel) &&
                (req.Resources != null) &&
                (req.Resources.IsPCSelected == true))
            {
                isMandatory = true;
            }

            if ((fieldCode == Entity.Constant.RequestField.IsSAPSelected) &&
                (req.EmploymentInfo != null) &&
                (IsSAPRequired(req.EmploymentInfo.UserSubTypeCd)))
            {
                isMandatory = true;
            }

            if ((fieldCode == Entity.Constant.RequestField.DARTFund) &&
                (req.EmploymentInfo != null) &&
                (IsDartFundRequired(req.EmploymentInfo.UserSubTypeCd)) &&
                (req.Resources != null) &&
                (req.Resources.IsSAPSelected == true))
            {
                isMandatory = true;
            }

            if ((fieldCode == Entity.Constant.RequestField.IsISISSelected) &&
                (req.EmploymentInfo != null) &&
                (IsISISRequired(req.EmploymentInfo.UserSubTypeCd)))
            {
                isMandatory = true;
            }

            if ((fieldCode == Entity.Constant.RequestField.ELearnStartDate) &&
                (req.Resources != null) &&
                (req.Resources.IsELearnSelected == true))
            {
                isMandatory = true;
            }

            if ((fieldCode == Entity.Constant.RequestField.MtgRoomDetails) &&
                (req.Resources != null) &&
                (req.Resources.IsMtgRoomSelected == true))
            {
                isMandatory = true;
            }

            if ((fieldCode == Entity.Constant.RequestField.EmailDLDetails) &&
                (req.Resources != null) &&
                (req.Resources.IsEmailDLSelected == true))
            {
                isMandatory = true;
            }

            if ((fieldCode == Entity.Constant.RequestField.SupportingDocument) && (!isBulkRequest) &&
                (req.Resources != null) &&
                ((req.Resources.IsINetSelected == true) || (req.Resources.IsOasisSelected == true) || (req.Resources.IsNextwebSelected == true)))
            {
                isMandatory = true;
            }

            if ((fieldCode == Entity.Constant.RequestField.IsEmailSelected) &&
                (req.Resources != null) &&
                ((req.Resources.IsINetSelected == true) || (req.Resources.IsOasisSelected == true) || (req.Resources.IsNextwebSelected == true)))
            {
                isMandatory = true;
            }

            if (fieldCode == Entity.Constant.RequestField.Remarks)
            {
                isMandatory = true;
            }

            return isMandatory;
        }

        public BulkRequestSubmitResult ValidateBulkRequest(BulkRequest bulkReq)
        {
            BulkRequestSubmitResult result = null;

            if ((bulkReq != null) && (bulkReq.Requests != null))
            {
                result = new BulkRequestSubmitResult();
                result.Rows = new List<BulkRequestSubmitResultRow>();
                for (int i = 0; i < bulkReq.Requests.Count; i++)
                {
                    BulkRequestSubmitResultRow rowRes = new BulkRequestSubmitResultRow();
                    rowRes.FirstName = bulkReq.Requests[i].FirstName;
                    rowRes.LastName = bulkReq.Requests[i].LastName;
                    rowRes.ReqTypeDesc = bulkReq.Requests[i].ReqTypeDesc;
                    rowRes.UserTypeDesc = bulkReq.Requests[i].EmploymentInfo.UserTypeDesc;
                    rowRes.EmplStartDt = bulkReq.Requests[i].EmploymentInfo.EmplStartDt;
                    Entity.RequestValidation reqVal = ValidateRequest(bulkReq.Requests[i], true);
                    if (!reqVal.HasError())
                    {
                        rowRes.IsResultPass = true;
                    }
                    else
                    {
                        rowRes.IsResultPass = false;
                    }
                    result.Rows.Add(rowRes);
                }
            }
            return result;
        }

        public static Entity.RequestValidation ValidateRequest(Request req, bool isBulkRequest)
        {
            Entity.RequestValidation reqVal = new Entity.RequestValidation();

            #region Validate Mandatory Field

            if ((IsMandatoryField(Entity.Constant.RequestField.ReqTypeCd, req, isBulkRequest)) &&
                (string.IsNullOrWhiteSpace(req.ReqTypeCd)))
            {
                reqVal.SetError(Entity.Constant.RequestField.ReqTypeCd, Entity.Constant.RequestFieldMandatoryError.ReqTypeCd);
            }

            if ((!string.IsNullOrWhiteSpace(req.ReqTypeCd)) && (!Security.IsValidAlphaNum(req.ReqTypeCd)))
            {
                reqVal.SetError(Entity.Constant.RequestField.ReqTypeCd, Entity.Constant.RequestFieldValueError.ReqTypeCd);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.SalutCd, req, isBulkRequest)) &&
                (string.IsNullOrWhiteSpace(req.SalutCd)))
            {
                reqVal.SetError(Entity.Constant.RequestField.SalutCd, Entity.Constant.RequestFieldMandatoryError.SalutCd);
            }

            if ((!string.IsNullOrWhiteSpace(req.SalutCd)) && (!Security.IsValidAlphaNum(req.SalutCd)))
            {
                reqVal.SetError(Entity.Constant.RequestField.SalutCd, Entity.Constant.RequestFieldValueError.SalutCd);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.FirstName, req, isBulkRequest)) &&
                (string.IsNullOrWhiteSpace(req.FirstName)))
            {
                reqVal.SetError(Entity.Constant.RequestField.FirstName, Entity.Constant.RequestFieldMandatoryError.FirstName);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.LastName, req, isBulkRequest)) &&
                (string.IsNullOrWhiteSpace(req.LastName)))
            {
                reqVal.SetError(Entity.Constant.RequestField.LastName, Entity.Constant.RequestFieldMandatoryError.LastName);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.PrefNm, req, isBulkRequest)) &&
                (string.IsNullOrWhiteSpace(req.PrefNm)))
            {
                reqVal.SetError(Entity.Constant.RequestField.PrefNm, Entity.Constant.RequestFieldMandatoryError.PrefNm);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.GenderCd, req, isBulkRequest)) &&
                (string.IsNullOrWhiteSpace(req.GenderCd)))
            {
                reqVal.SetError(Entity.Constant.RequestField.GenderCd, Entity.Constant.RequestFieldMandatoryError.GenderCd);
            }

            if ((!string.IsNullOrWhiteSpace(req.GenderCd)) && (!Security.IsValidAlphaNum(req.GenderCd)))
            {
                reqVal.SetError(Entity.Constant.RequestField.GenderCd, Entity.Constant.RequestFieldValueError.GenderCd);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.PrsnEmail, req, isBulkRequest)) &&
                (string.IsNullOrWhiteSpace(req.PrsnEmail)))
            {
                reqVal.SetError(Entity.Constant.RequestField.PrsnEmail, Entity.Constant.RequestFieldMandatoryError.PrsnEmail);
            }

            if ((!string.IsNullOrWhiteSpace(req.PrsnEmail)) && (!Security.IsValidEmail(req.PrsnEmail)))
            {
                reqVal.SetError(Entity.Constant.RequestField.PrsnEmail, Entity.Constant.RequestFieldValueError.PrsnEmail);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.MobileNo, req, isBulkRequest)) &&
                (string.IsNullOrWhiteSpace(req.MobileNo)))
            {
                reqVal.SetError(Entity.Constant.RequestField.MobileNo, Entity.Constant.RequestFieldMandatoryError.MobileNo);
            }

            if ((!string.IsNullOrWhiteSpace(req.MobileNo)) && (!Security.IsValidMobileNum(req.MobileNo)))
            {
                reqVal.SetError(Entity.Constant.RequestField.MobileNo, Entity.Constant.RequestFieldValueError.MobileNo);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.RaceCd, req, isBulkRequest)) &&
                (string.IsNullOrWhiteSpace(req.RaceCd)))
            {
                reqVal.SetError(Entity.Constant.RequestField.RaceCd, Entity.Constant.RequestFieldMandatoryError.RaceCd);
            }

            if ((!string.IsNullOrWhiteSpace(req.RaceCd)) && (!Security.IsValidAlphaNum(req.RaceCd)))
            {
                reqVal.SetError(Entity.Constant.RequestField.RaceCd, Entity.Constant.RequestFieldValueError.RaceCd);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.IsExistingStaff, req, isBulkRequest)) &&
                (!req.IsExistingStaff.HasValue))
            {
                reqVal.SetError(Entity.Constant.RequestField.IsExistingStaff, Entity.Constant.RequestFieldMandatoryError.IsExistingStaff);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.IsExSmuStd, req, isBulkRequest)) &&
                (!req.IsExSmuStd.HasValue))
            {
                reqVal.SetError(Entity.Constant.RequestField.IsExSmuStd, Entity.Constant.RequestFieldMandatoryError.IsExSmuStd);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.UserTypeCd, req, isBulkRequest)) &&
                ((req.EmploymentInfo == null) ||
                 (string.IsNullOrWhiteSpace(req.EmploymentInfo.UserTypeCd))))
            {
                reqVal.SetError(Entity.Constant.RequestField.UserTypeCd, Entity.Constant.RequestFieldMandatoryError.UserTypeCd);
            }

            if ((req.EmploymentInfo != null) && (!string.IsNullOrWhiteSpace(req.EmploymentInfo.UserSubTypeCd)) && 
                (!Security.IsValidAlphaNum(req.EmploymentInfo.UserTypeCd)))
            {
                reqVal.SetError(Entity.Constant.RequestField.UserTypeCd, Entity.Constant.RequestFieldValueError.UserTypeCd);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.UserSubTypeCd, req, isBulkRequest)) &&
                 ((req.EmploymentInfo == null) ||
                 (string.IsNullOrWhiteSpace(req.EmploymentInfo.UserSubTypeCd))))
            {
                reqVal.SetError(Entity.Constant.RequestField.UserSubTypeCd, Entity.Constant.RequestFieldMandatoryError.UserSubTypeCd);
            }

            if ((req.EmploymentInfo != null) && (!string.IsNullOrWhiteSpace(req.EmploymentInfo.UserSubTypeCd)) &&
                (!Security.IsValidAlphaNum(req.EmploymentInfo.UserSubTypeCd)))
            {
                reqVal.SetError(Entity.Constant.RequestField.UserSubTypeCd, Entity.Constant.RequestFieldValueError.UserSubTypeCd);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.CostCtrCd, req, isBulkRequest)) &&
                ((req.EmploymentInfo == null) ||
                (string.IsNullOrWhiteSpace(req.EmploymentInfo.CostCtrCd))))
            {
                reqVal.SetError(Entity.Constant.RequestField.CostCtrCd, Entity.Constant.RequestFieldMandatoryError.CostCtrCd);
            }

            if ((req.EmploymentInfo != null) && (!string.IsNullOrWhiteSpace(req.EmploymentInfo.CostCtrCd)) &&
                (!Security.IsValidCostCentre(req.EmploymentInfo.CostCtrCd)))
            {
                reqVal.SetError(Entity.Constant.RequestField.CostCtrCd, Entity.Constant.RequestFieldValueError.CostCtrCd);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.EmplStartDt, req, isBulkRequest)) &&
                 ((req.EmploymentInfo == null) ||
                 (!req.EmploymentInfo.EmplStartDt.HasValue)))
            {
                reqVal.SetError(Entity.Constant.RequestField.EmplStartDt, Entity.Constant.RequestFieldMandatoryError.EmplStartDt);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.EmplEndDt, req, isBulkRequest)) &&
                 ((req.EmploymentInfo == null) || (!req.EmploymentInfo.EmplEndDt.HasValue)))
            {
                reqVal.SetError(Entity.Constant.RequestField.EmplEndDt, Entity.Constant.RequestFieldMandatoryError.EmplEndDt);
            }

            if ((req.EmploymentInfo != null) && 
                (req.EmploymentInfo.EmplStartDt.HasValue) && (req.EmploymentInfo.EmplEndDt.HasValue) &&
                (req.EmploymentInfo.EmplStartDt.Value > req.EmploymentInfo.EmplEndDt.Value))
            {
                reqVal.SetError(Entity.Constant.RequestField.EmplEndDt, Entity.Constant.RequestFieldValueError.EmplEndDt);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.JobTitle, req, isBulkRequest)) &&
                 ((req.EmploymentInfo == null) ||
                 (string.IsNullOrWhiteSpace(req.EmploymentInfo.JobTitle))))
            {
                reqVal.SetError(Entity.Constant.RequestField.JobTitle, Entity.Constant.RequestFieldMandatoryError.JobTitle);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.IsPCSelected, req, isBulkRequest)) &&
                 ((req.Resources == null) ||
                 (req.Resources.IsPCSelected != true)))
            {
                reqVal.SetError(Entity.Constant.RequestField.IsPCSelected, Entity.Constant.RequestFieldMandatoryError.IsPCSelected);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.CorpTitleLevel, req, isBulkRequest)) &&
                 ((req.Resources == null) ||
                 (string.IsNullOrWhiteSpace(req.Resources.CorpTitleLevelCd))))
            {
                reqVal.SetError(Entity.Constant.RequestField.CorpTitleLevel, Entity.Constant.RequestFieldMandatoryError.CorpTitleLevel);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.IsIPPhoneSelected, req, isBulkRequest)) &&
                 ((req.Resources == null) ||
                 (req.Resources.IsIPPhoneSelected != true)))
            {
                reqVal.SetError(Entity.Constant.RequestField.IsIPPhoneSelected, Entity.Constant.RequestFieldMandatoryError.IsIPPhoneSelected);
            }
                        
            if ((IsMandatoryField(Entity.Constant.RequestField.Building, req, isBulkRequest)) &&
                 ((req.Resources == null) ||
                 (string.IsNullOrWhiteSpace(req.Resources.Building))))
            {
                reqVal.SetError(Entity.Constant.RequestField.Building, Entity.Constant.RequestFieldMandatoryError.Building);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.Floor, req, isBulkRequest)) &&
                 ((req.Resources == null) ||
                 (string.IsNullOrWhiteSpace(req.Resources.Floor))))
            {
                reqVal.SetError(Entity.Constant.RequestField.Floor, Entity.Constant.RequestFieldMandatoryError.Floor);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.Room, req, isBulkRequest)) &&
                 ((req.Resources == null) ||
                 (string.IsNullOrWhiteSpace(req.Resources.Room))))
            {
                reqVal.SetError(Entity.Constant.RequestField.Room, Entity.Constant.RequestFieldMandatoryError.Room);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.IsEmailSelected, req, isBulkRequest)) &&
                 ((req.Resources == null) ||
                 (req.Resources.IsEmailSelected != true)))
            {
                reqVal.SetError(Entity.Constant.RequestField.IsEmailSelected, Entity.Constant.RequestFieldMandatoryError.IsEmailSelected);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.IsSAPSelected, req, isBulkRequest)) &&
                 ((req.Resources == null) ||
                 (req.Resources.IsSAPSelected != true)))
            {
                reqVal.SetError(Entity.Constant.RequestField.IsSAPSelected, Entity.Constant.RequestFieldMandatoryError.IsSAPSelected);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.DARTFund, req, isBulkRequest)) &&
                ((req.Resources == null) ||
                 (string.IsNullOrWhiteSpace(req.Resources.DARTFund))))
            {
                reqVal.SetError(Entity.Constant.RequestField.DARTFund, Entity.Constant.RequestFieldMandatoryError.DARTFund);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.IsISISSelected, req, isBulkRequest)) &&
                 ((req.Resources == null) ||
                 (req.Resources.IsISISSelected != true)))
            {
                reqVal.SetError(Entity.Constant.RequestField.IsISISSelected, Entity.Constant.RequestFieldMandatoryError.IsISISSelected);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.ISISRemarks, req, isBulkRequest)) &&
                ((req.Resources == null) ||
                 (string.IsNullOrWhiteSpace(req.Resources.ISISRemarks))))
            {
                reqVal.SetError(Entity.Constant.RequestField.ISISRemarks, Entity.Constant.RequestFieldMandatoryError.ISISRemarks);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.IsELearnSelected, req, isBulkRequest)) &&
                 ((req.Resources == null) ||
                 (req.Resources.IsELearnSelected != true)))
            {
                reqVal.SetError(Entity.Constant.RequestField.IsELearnSelected, Entity.Constant.RequestFieldMandatoryError.IsELearnSelected);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.ELearnStartDate, req, isBulkRequest)) &&
                 ((req.Resources == null) ||
                 (!req.Resources.ELearnStartDate.HasValue)))
            {
                reqVal.SetError(Entity.Constant.RequestField.ELearnStartDate, Entity.Constant.RequestFieldMandatoryError.ELearnStartDate);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.ELearnRemarks, req, isBulkRequest)) &&
                ((req.Resources == null) ||
                 (string.IsNullOrWhiteSpace(req.Resources.ELearnRemarks))))
            {
                reqVal.SetError(Entity.Constant.RequestField.ELearnRemarks, Entity.Constant.RequestFieldMandatoryError.ELearnRemarks);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.IsMtgRoomSelected, req, isBulkRequest)) &&
                 ((req.Resources == null) ||
                 (req.Resources.IsMtgRoomSelected != true)))
            {
                reqVal.SetError(Entity.Constant.RequestField.IsMtgRoomSelected, Entity.Constant.RequestFieldMandatoryError.IsMtgRoomSelected);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.MtgRoomDetails, req, isBulkRequest)) &&
                ((req.Resources == null) ||
                 (string.IsNullOrWhiteSpace(req.Resources.MtgRoomDetails))))
            {
                reqVal.SetError(Entity.Constant.RequestField.MtgRoomDetails, Entity.Constant.RequestFieldMandatoryError.MtgRoomDetails);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.IsINetSelected, req, isBulkRequest)) &&
                 ((req.Resources == null) ||
                 (req.Resources.IsINetSelected != true)))
            {
                reqVal.SetError(Entity.Constant.RequestField.IsINetSelected, Entity.Constant.RequestFieldMandatoryError.IsINetSelected);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.IsOasisSelected, req, isBulkRequest)) &&
                 ((req.Resources == null) ||
                 (req.Resources.IsOasisSelected != true)))
            {
                reqVal.SetError(Entity.Constant.RequestField.IsOasisSelected, Entity.Constant.RequestFieldMandatoryError.IsOasisSelected);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.IsNextwebSelected, req, isBulkRequest)) &&
                 ((req.Resources == null) ||
                 (req.Resources.IsNextwebSelected != true)))
            {
                reqVal.SetError(Entity.Constant.RequestField.IsNextwebSelected, Entity.Constant.RequestFieldMandatoryError.IsNextwebSelected);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.IsEmailDLSelected, req, isBulkRequest)) &&
                 ((req.Resources == null) ||
                 (req.Resources.IsEmailDLSelected != true)))
            {
                reqVal.SetError(Entity.Constant.RequestField.IsEmailDLSelected, Entity.Constant.RequestFieldMandatoryError.IsEmailDLSelected);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.EmailDLDetails, req, isBulkRequest)) &&
                ((req.Resources == null) ||
                 (string.IsNullOrWhiteSpace(req.Resources.EmailDLDetails))))
            {
                reqVal.SetError(Entity.Constant.RequestField.EmailDLDetails, Entity.Constant.RequestFieldMandatoryError.EmailDLDetails);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.SupportingDocument, req, isBulkRequest)) &&
                ((req.Attachment == null) ||
                 (string.IsNullOrWhiteSpace(req.Attachment.Filename))))
            {
                reqVal.SetError(Entity.Constant.RequestField.SupportingDocument, Entity.Constant.RequestFieldMandatoryError.SupportingDocument);
            }

            if ((req.Attachment != null) && (!string.IsNullOrWhiteSpace(req.Attachment.Filename)) &&
                (!Security.IsValidFilename(req.Attachment.Filename)))
            {
                reqVal.SetError(Entity.Constant.RequestField.SupportingDocument, Entity.Constant.RequestFieldValueError.SupportingDocument);
            }

            if ((IsMandatoryField(Entity.Constant.RequestField.Remarks, req, isBulkRequest)) &&
                 (string.IsNullOrWhiteSpace(req.Remark)))
            {
                reqVal.SetError(Entity.Constant.RequestField.Remarks, Entity.Constant.RequestFieldMandatoryError.Remarks);
            }

            #endregion

            #region Validate Field Length

            if ((!string.IsNullOrWhiteSpace(req.FirstName)) &&
                (req.FirstName.Length > Entity.Constant.RequestFieldLength.GetLength(Entity.Constant.RequestField.FirstName)))
            {
                reqVal.SetError(Entity.Constant.RequestField.FirstName, Entity.Constant.RequestFieldLengthError.FirstName);
            }

            if ((!string.IsNullOrWhiteSpace(req.LastName)) &&
                (req.LastName.Length > Entity.Constant.RequestFieldLength.GetLength(Entity.Constant.RequestField.LastName)))
            {
                reqVal.SetError(Entity.Constant.RequestField.LastName, Entity.Constant.RequestFieldLengthError.LastName);
            }

            if ((!string.IsNullOrWhiteSpace(req.PrefNm)) &&
                (req.PrefNm.Length > Entity.Constant.RequestFieldLength.GetLength(Entity.Constant.RequestField.PrefNm)))
            {
                reqVal.SetError(Entity.Constant.RequestField.PrefNm, Entity.Constant.RequestFieldLengthError.PrefNm);
            }

            if ((!string.IsNullOrWhiteSpace(req.PrsnEmail)) &&
                (req.PrsnEmail.Length > Entity.Constant.RequestFieldLength.GetLength(Entity.Constant.RequestField.PrsnEmail)))
            {
                reqVal.SetError(Entity.Constant.RequestField.PrsnEmail, Entity.Constant.RequestFieldLengthError.PrsnEmail);
            }

            if ((req.EmploymentInfo != null) && (!string.IsNullOrWhiteSpace(req.EmploymentInfo.JobTitle)) &&
                (req.EmploymentInfo.JobTitle.Length > Entity.Constant.RequestFieldLength.GetLength(Entity.Constant.RequestField.JobTitle)))
            {
                reqVal.SetError(Entity.Constant.RequestField.JobTitle, Entity.Constant.RequestFieldLengthError.JobTitle);
            }

            if ((!string.IsNullOrWhiteSpace(req.Remark)) &&
                (req.Remark.Length > Entity.Constant.RequestFieldLength.GetLength(Entity.Constant.RequestField.Remarks)))
            {
                reqVal.SetError(Entity.Constant.RequestField.Remarks, Entity.Constant.RequestFieldLengthError.Remarks);
            }

            if ((req.Resources != null) && (!string.IsNullOrWhiteSpace(req.Resources.Building)) &&
                (req.Resources.Building.Length > Entity.Constant.RequestFieldLength.GetLength(Entity.Constant.RequestField.Building)))
            {
                reqVal.SetError(Entity.Constant.RequestField.Building, Entity.Constant.RequestFieldLengthError.Building);
            }

            if ((req.Resources != null) && (!string.IsNullOrWhiteSpace(req.Resources.Floor)) &&
                (req.Resources.Floor.Length > Entity.Constant.RequestFieldLength.GetLength(Entity.Constant.RequestField.Floor)))
            {
                reqVal.SetError(Entity.Constant.RequestField.Floor, Entity.Constant.RequestFieldLengthError.Floor);
            }

            if ((req.Resources != null) && (!string.IsNullOrWhiteSpace(req.Resources.Room)) &&
                (req.Resources.Room.Length > Entity.Constant.RequestFieldLength.GetLength(Entity.Constant.RequestField.Room)))
            {
                reqVal.SetError(Entity.Constant.RequestField.Room, Entity.Constant.RequestFieldLengthError.Room);
            }

            if ((req.Resources != null) && (!string.IsNullOrWhiteSpace(req.Resources.DARTFund)) &&
                (req.Resources.DARTFund.Length > Entity.Constant.RequestFieldLength.GetLength(Entity.Constant.RequestField.DARTFund)))
            {
                reqVal.SetError(Entity.Constant.RequestField.DARTFund, Entity.Constant.RequestFieldLengthError.DARTFund);
            }

            if ((req.Resources != null) && (!string.IsNullOrWhiteSpace(req.Resources.ISISRemarks)) &&
                (req.Resources.ISISRemarks.Length > Entity.Constant.RequestFieldLength.GetLength(Entity.Constant.RequestField.ISISRemarks)))
            {
                reqVal.SetError(Entity.Constant.RequestField.ISISRemarks, Entity.Constant.RequestFieldLengthError.ISISRemarks);
            }

            if ((req.Resources != null) && (!string.IsNullOrWhiteSpace(req.Resources.ELearnRemarks)) &&
                (req.Resources.ELearnRemarks.Length > Entity.Constant.RequestFieldLength.GetLength(Entity.Constant.RequestField.ELearnRemarks)))
            {
                reqVal.SetError(Entity.Constant.RequestField.ELearnRemarks, Entity.Constant.RequestFieldLengthError.ELearnRemarks);
            }

            if ((req.Resources != null) && (!string.IsNullOrWhiteSpace(req.Resources.MtgRoomDetails)) &&
                (req.Resources.MtgRoomDetails.Length > Entity.Constant.RequestFieldLength.GetLength(Entity.Constant.RequestField.MtgRoomDetails)))
            {
                reqVal.SetError(Entity.Constant.RequestField.MtgRoomDetails, Entity.Constant.RequestFieldLengthError.MtgRoomDetails);
            }

            if ((req.Resources != null) && (!string.IsNullOrWhiteSpace(req.Resources.EmailDLDetails)) &&
                (req.Resources.EmailDLDetails.Length > Entity.Constant.RequestFieldLength.GetLength(Entity.Constant.RequestField.EmailDLDetails)))
            {
                reqVal.SetError(Entity.Constant.RequestField.EmailDLDetails, Entity.Constant.RequestFieldLengthError.EmailDLDetails);
            }

            #endregion

            return reqVal;
        }

        public static bool IsSAPRequired(string userSubTypeCd)
        {
            bool required = false;

            switch (userSubTypeCd)
            {
                case Entity.Constant.UserSubType.SeniorFacultyAdmin:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.AdminPermanent:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.AdminContract:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.AdminInstructorPermanent:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.AdminInstructorContract:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimeStanding:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimeSupport:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimeVisitingMoreThanEqual12:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimePractice:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimeEducation:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimeDistinguishedTerm:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimeResearch:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimeAdministrator:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimePostRetirement:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimeFDS:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimePostDoc:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimeTeaching:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimeEmeritus:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyAssociatedAdjunctsContractOfService:
                    required = true;
                    break;
                default:
                    required = false;
                    break;
            }
            return required;
        }

        public static bool IsISISRequired(string userSubTypeCd)
        {
            bool required = false;

            switch (userSubTypeCd)
            {
                case Entity.Constant.UserSubType.SeniorFacultyAdmin:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.AdminInstructorPermanent:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.AdminInstructorContract:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimeStanding:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimeSupport:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimeVisitingMoreThanEqual12:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimePractice:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimeEducation:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimeDistinguishedTerm:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimeResearch:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimeAdministrator:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimePostRetirement:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimeFDS:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimePostDoc:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimeTeaching:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimeEmeritus:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyAssociatedAdjunctsContractOfService:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyAssociatedAdjunctsContractForService:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyAssociatedAffiliated:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyAssociatedDistinguishedFellow:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyAssociatedPostDoc:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyAssociatedResearchFellow:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyAssociatedTeachingFellow:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyAssociatedVisitingLessThan12:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyAssociatedEmeritus:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyGraduateInstructor:
                    required = true;
                    break;
                default:
                    required = false;
                    break;
            }
            return required;
        }

        private static bool IsDartFundRequired(string userSubTypeCd)
        {
            bool required = false;

            switch (userSubTypeCd)
            {
                case Entity.Constant.UserSubType.SeniorFacultyAdmin:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimeStanding:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimeSupport:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimeVisitingMoreThanEqual12:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimePractice:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimeEducation:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimeDistinguishedTerm:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimeResearch:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimeAdministrator:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimePostRetirement:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimeFDS:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimePostDoc:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimeTeaching:
                    required = true;
                    break;
                case Entity.Constant.UserSubType.FacultyFullTimeEmeritus:
                    required = true;
                    break;
                default:
                    required = false;
                    break;
            }
            return required;
        }
    }
}

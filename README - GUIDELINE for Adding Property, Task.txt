Adding User Sub-Type
-------------------------------------------------------------------------------
(*) Update ONBOARDING_CodeTable.sql to add User Sub-Type in "UserSubTypeCd" table
       Note: Set the "Seq" number accordingly
(*) Add constant to Onboarding.Entity.Constant.UserSubType
(*) Add validation codes to Onboarding.Common.RequestValidation
        Method: IsISISRequired(string userSubTypeCd)
                IsDartFundRequired(string userSubTypeCd)
(*) Add codes in Onboarding.ServiceManager.RequestManager (only if applicable)
        Method: IsAMSRequired(Entity.Request req)


Adding Request Property
-------------------------------------------------------------------------------
(*) Update ONBOARDING_Tables.sql to add request property to table "Req"
(*) Add the new column to OnboardingModel.edmx file in Onboarding.DataProvider
(*) Add the new property in V_Req.cs file in Onboarding.DataProvider
(*) Add the new property to Onboarding.Entity.Request
(*) Add constant to Onboarding.Entity.Constant.RequestField
(*) Add constant to Onboarding.Entity.Constant.RequestFieldValueError
(*) Add constant to Onboarding.Entity.Constant.RequestFieldMandatoryError
(*) Add constant to Onboarding.Entity.Constant.RequestFieldLength
(*) Add constant to Onboarding.Entity.Constant.RequestFieldLengthError
(*) Add constant to Onboarding.Entity.Constant.RequestSection
(*) Add constant to Onboarding.Entity.Constant.ExcelColumnName
(*) Add mapping codes to Onboarding.DataProvider.Mapper.RequestMaptoDb
        Method: Map(Entity.Request req, Model.V_Req dbReq, string userId, DateTime sqlDateTime, Model.OnboardingEntities obCtx)
(*) Add mapping codes to Onboarding.DataProvider.RequestMaptoEntity
        Method: Map(Model.V_Req dbReq, Entity.Request req, string userId, Model.OnboardingEntities obCtx)
(*) Add validation codes to Onboarding.Common.RequestValidation
        Method: IsMandatoryField(string fieldCode, Request req) - mandatory field validation and length validation
        Method: ValidateRequest(Request req)
(*) Add property to Onboarding.Models.PersonalInfo (for example)
(*) Add mapping codes to Controller/Requestor/Module
        Method (example): BindEntityToModel(Request request, PersonalInfo perInfo)
(*) Add mapping codes to Controller/Agent/AgentController
        Method : BindEntityToModel(Request request, PersonalInfo perInfo) (for example)
(*) Add display codes to Views/Requestor/PersonalInfo.cshtml (for example)
        Note: Including the maxlength for text input
(*) Add display codes to Views/Agent/AgentRequest.cshtml and Views/Agent/Tasks/TaskX (if required)
(*) Add mapping codes to map Excel cell to entity in BulkRequestController.cs - BulkRequestMapToEntity method
(*) Add the column into Bulk Request Excel files:
    - Bulk Request Template
    - Sample Bulk Request
(*) Update Email Alert Program with latest OnboardingEntities




Adding Request Attribute
-------------------------------------------------------------------------------
(*) Update ONBOARDING_Code_Table_Initial_Data.sql to add request attribute from the following table:
     - ReqAttrbCd
(*) Create SQL script to add record for this request attribute to ReqAttrbCd table
(*) Add constant to Onboarding.Entity.Constant.RequestAttributeCode
(*) Add constant to Onboarding.Entity.Constant.RequestField
(*) Add constant to Onboarding.Entity.Constant.ExcelColumnName
(*) Add constant to Onboarding.Entity.Constant.RequestFieldValueError
(*) Add constant to Onboarding.Entity.Constant.RequestFieldMandatoryError
(*) Add constant to Onboarding.Entity.Constant.RequestSection
(*) Add property to Onboarding.Entity.RequestResources
(*) Add mapping codes to Onboarding.DataProvider.Mapper.RequestMaptoDb
    Depending on attribute group, can be Office Logistics / System Access
        Method: MapResources(Entity.RequestResources reqRes, string userId, DateTime sqlDateTime, Model.V_Req dbReq, Model.OnboardingEntities obCtx)
(*) Add mapping codes to Onboarding.DataProvider.RequestMaptoEntity
    Depending on attribute group, can be Office Logistics / System Access
        Method: MapResources(Model.V_Req dbReq, Entity.RequestResources reqRes, Model.OnboardingEntities obCtx)
(*) Add validation codes to Onboarding.Common.RequestValidation
        Method: IsMandatoryField(string fieldCode, Request req)
        Method: ValidateRequest(Request req)
(*) Add "SetupTask(req, .....)" from Onboarding.ServiceManager.RequestManager (only if applicable)
(*) Add property to Onboarding.Models.EmployeeInfo (for example)
(*) Add mapping codes to Controller/Requestor/Module
        Method (example): BindEntityToModel(Request request, EmployeeInfo emplInfo)
                          BindModelToEntity(EmployeeInfo emplInfo, Request request)
(*) Add mapping codes to Controller/Agent/AgentController
        Method : BindEntityToModel(Request request, EmployeeInfo emplInfo)
(*) Add display codes to Views/Requestor/EmployeeInfo.cshtml (for example)
(*) Add display codes to Views/Agent/AgentRequest.cshtml and Views/Agent/Tasks/TaskX (for example)
(*) Add mapping codes to map Excel cell to entity in BulkRequestController.cs - BulkRequestMapToEntity method
(*) Add the column into Bulk Request Excel files:
    - Bulk Request Template
    - Sample Bulk Request
(*) Update Email Alert Program with latest OnboardingEntities


Remove Request Attribute
-------------------------------------------------------------------------------
(*) Update ONBOARDING_Code_Table_Initial_Data.sql to update "ReqAttrbCd" IsActive=False
(*) Remove constant from Onboarding.Entity.Constant.RequestAttributeCode
(*) Remove constant from Onboarding.Entity.Constant.RequestField
(*) Remove constant from Onboarding.Entity.Constant.ExcelColumnName
(*) Remove constant from Onboarding.Entity.Constant.RequestSection
(*) Remove constant from Onboarding.Entity.Constant.RequestFieldValueError
(*) Remove constant from Onboarding.Entity.Constant.RequestFieldMandatoryError
(*) Remove property to Onboarding.Entity.RequestResources
(*) Remove mapping codes from Onboarding.DataProvider.RequestMaptoDb
    Depending on attribute group, can be Office Logistics / System Access
        Method: MapResources(Entity.RequestResources reqRes, string userId, DateTime sqlDateTime, Model.V_Req dbReq, Model.OnboardingEntities obCtx)
(*) Remove mapping codes from Onboarding.DataProvider.RequestMaptoEntity
    Depending on attribute group, can be Office Logistics / System Access
        Method: MapResources(Model.V_Req dbReq, Entity.RequestResources reqRes, Model.OnboardingEntities obCtx)
(*) Remove validation codes from Onboarding.Common.RequestValidation
        Method: IsMandatoryField(string fieldCode, Request req)
        Method: ValidateRequest(Request req)
(*) Remove validation codes from Onboarding.Common.RequestValidation
        Method: IsMandatoryField(string fieldCode, Request req)
        Method: ValidateRequest(Request req)
(*) Remove "SetupTask(req, .....)" from Onboarding.ServiceManager.RequestManager (only if applicable)
(*) Remove Associated Tasks (refer to the below section for Removing Task - only if applicable)
(*) Remove property from Onboarding.Models.EmployeeInfo (for example)
(*) Remove mapping codes from Controller/Requestor/Module
        Method (example): BindEntityToModel(Request request, EmployeeInfo emplInfo)
                          BindModelToEntity(EmployeeInfo emplInfo, Request request)
(*) Remove mapping codes from Controller/Agent/AgentController
        Method: BindEntityToModel(Request request, EmployeeInfo emplInfo)
(*) Remove display codes from Views/Requestor/EmployeeInfo.cshtml (for example)
(*) Remove display codes from Views/Agent/AgentRequest.cshtml (for example)
(*) Remove Excel mapping codes from BulkRequestController.cs - BulkRequestMapToEntity method
(*) Remove the column from Bulk Request Excel files:
    - Bulk Request Template
    - Sample Bulk Request
(*) Update Email Alert Program with latest OnboardingEntities


Using Remarks Field in Request (Request.Remark)
--------------------------------------------------------------------------------------------
(*) Add codes to map the field in Onboarding.DataProvider.RequestMaptoDb
(*) Add codes to map the field in Onboarding.DataProvider.RequestMaptoEntity
(*) Add validation codes to Onboarding.Common.RequestValidation
        Method: IsMandatoryField(string fieldCode, Request req)
        Method: ValidateRequest(Request req)
(*) Add property to Onboarding.Models.EmployeeInfo (for example)
(*) Add mapping codes to Controller/Requestor/Module
    Method (example): 
    - BindEntityToModel(Request request, EmployeeInfo emplInfo)
    - BindModelToEntity(EmployeeInfo emplInfo, Request request)
(*) Add display codes to Views/Requestor/EmployeeInfo.cshtml (for example)


Adding Task
--------------------------------------------------------------------------------------------
(*) Update ONBOARDING_Code_Table_Initial_Data.sql to add tasks from the following tables in sequence:
     - TaskCd
     - TaskDep
     - TaskAttrbCd
(*) Create SQL script to add records related to this task from database:
     - TaskCd
     - TaskAttrbCd
     - TaskDep
(*) Add the new task to :
     - Onboarding.Entity.Constant.TaskCode
     - Onboarding.Common.Validation
     - Onboarding.ServiceManager.RequestManager
     - Onboarding UI - /Views/Agent/Tasks
(*) Update Email Alert Program with latest OnboardingEntities


Removing Task
--------------------------------------------------------------------------------------------
(*) Update ONBOARDING_Code_Table_Initial_Data.sql to remove tasks from the following tables in sequence:
     - TaskAttrbCd
     - TaskDep
     - TaskCd
(*) Remove codes related to this task from:
     - Onboarding.Entity.Constant.TaskCode
     - Onboarding.Common.Validation
     - Onboarding.ServiceManager.RequestManager
     - Onboarding UI - /Views/Agent/Tasks
(*) Update Email Alert Program with latest OnboardingEntities



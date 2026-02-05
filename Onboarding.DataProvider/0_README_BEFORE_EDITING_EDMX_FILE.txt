- Delete the EDMX file
- Re-created EDMX file
	Item Type: ADO.NET Entity Data Model
	Item Name: OnboardingModel
	Model Contents: EF Designer from database
	Save Connections in App.Config as: OnboardingEntities
	Model Namespace: OnboardingModel
- Update the primary key for each view in Visual Studio EDMX editor
- Edit OnboardingModel.edmx using Notepad
- Replace 'store:Type="Views" store:Schema="dbo"' with 'store:Type="Tables" Schema="dbo"'
- Remove all '<DefiningQuery>'
- Remove columns that are not Primary Keys

- add the following line as a workaround for instantiating entity framework instance 
private static bool instanceExists = System.Data.Entity.SqlServer.SqlProviderServices.Instance != null;

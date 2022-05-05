# Teams Call Notification

A small code sample to demonstrate receiving Teams call notifications using the .NET Calling library.

## Getting started:

1. Setup ACS:
- Create an Azure Communication Service resource via the Azure Portal. 
- Once the resource has been deployed, under the "Keys" tab select "Show Values" and copy the primary connection string. 
- Fill in the connection string on line #20 of MainPage.xaml.cs
  
2. Create an Azure AD Application Registration:
- Within Azure AD navigate to the "App Registrations" tab, select "New Registration". 
- Provide a name for the application, choose "Accounts in this org directory only (Single tenant)", and under the "Redirect URI" select "Public client/native", and enter a redirect URI. For local development you can use "http://localhost"
- Navigate to the Application and select the "API Permissions" tab
- Click "Add a permission", and under "Azure Communication Services" select the "Teams.ManageCalls" delegated permission. Click Add permissions.
- Return to the overview page for the App Registration, and copy the "Application (client) ID" and "Directory (tenant) ID", filling these values into lines #23 & #22 of MainPage.xaml.cs

3. Running the application will prompt for an Azure AD login, before hooking up to Teams inbound call events. If it is the first time you are logging in you may be prompted to confirm the permission you selected above, in production scenarios an Azure AD administrator can grant admin consent for the entire tenant to skip this step.
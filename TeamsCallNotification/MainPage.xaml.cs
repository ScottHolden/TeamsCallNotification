using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Azure.Communication.Identity;
using Microsoft.Identity.Client;
using Azure.WinRT.Communication;
using Azure.Communication.Calling;
using Windows.UI.Core;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TeamsCallNotification
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const string ACSConnectionString = ""; // Full connection string of the ACS resource

        private const string TenantId = ""; // The Tenant ID of Azure AD
        private const string ApplicationId = ""; // Client ID of the Application Registration in Azure AD
        private const string RedirectUri = "http://localhost"; // The redirect URL configured in the application

        private const string Authority = "https://login.microsoftonline.com/" + TenantId;
        private const string ACSScope = "https://auth.msft.communication.azure.com/Teams.ManageCalls";

        private CallClient _callClient;
        private CallAgent _callAgent;
        private string _statusText = "";

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void WriteStatus(string input)
        {
            _statusText += $"{DateTime.Now.ToShortTimeString()}: {input}\n";
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => StatusTextBlock.Text = _statusText);
        }

        private async void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            WriteStatus("Starting login");

            var aadClient = PublicClientApplicationBuilder
                            .Create(ApplicationId)
                            .WithAuthority(Authority)
                            .WithRedirectUri(RedirectUri)
                            .Build();

            WriteStatus("Popping login prompt...");

            var teamsUserAadToken = await aadClient
                                    .AcquireTokenInteractive(new List<string> { ACSScope })
                                    .ExecuteAsync();

            var client = new CommunicationIdentityClient(ACSConnectionString);

            var accessToken = await client.GetTokenForTeamsUserAsync(teamsUserAadToken.AccessToken);
            WriteStatus("Recieved Token for Teams User...");
            var tokenCredential = new CommunicationTokenCredential(accessToken.Value.Token);
            _callClient = new CallClient();

            _callAgent = await _callClient.CreateCallAgent(tokenCredential, new CallAgentOptions());

            _callAgent.OnCallsUpdated += OnCallsUpdated;
            _callAgent.OnIncomingCall += OnIncomingCall;
            WriteStatus("Call Agent created!");
        }

        private void OnCallsUpdated(object sender, CallsUpdatedEventArgs args)
        {
            WriteStatus($"Calls updated, {args.AddedCalls.Count} added, {args.RemovedCalls} removed");
        }

        private void OnIncomingCall(object sender, IncomingCall incomingCall)
        {
            string callId = incomingCall.Id;
            string displayName = incomingCall.CallerInfo.DisplayName;
            string callerId = GetCallerId(incomingCall.CallerInfo.Identifier);

            WriteStatus($"New inbound call, {callId}:\n   {displayName} ({callerId})");
        }

        private string GetCallerId(ICommunicationIdentifier id)
        {
            if (id is PhoneNumberIdentifier phoneNumber) return phoneNumber.PhoneNumber;
            if (id is MicrosoftTeamsUserIdentifier teamsUser) return teamsUser.UserId;
            return "Unknown";
        }
    }
}

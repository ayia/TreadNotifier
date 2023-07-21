using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace APiTreadOrders.DAO
{
    public class GmailAcces
    {

        public static Message ReadLastEmail()
        {
            string[] scopes = { GmailService.Scope.GmailReadonly };

            UserCredential credential;
            using (var stream = new FileStream("./data/credentials2.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            // Create the Gmail API service
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "NotifierMail",
            });

            // Fetch the last email (the newest one)
            var request = service.Users.Messages.List("me");
            request.Q = "from:noreply@carinabot.com is:inbox"; // Only retrieve emails in the inbox
            request.MaxResults = 1;
            var response = request.Execute();
            var lastEmail = response.Messages?.FirstOrDefault();

            // Fetch the full email details if needed
            if (lastEmail != null)
            {
                var emailRequest = service.Users.Messages.Get("me", lastEmail.Id);
                emailRequest.Format = UsersResource.MessagesResource.GetRequest.FormatEnum.Full;
                var emailResponse = emailRequest.Execute();
                return emailResponse;
            }

            return null; // No emails found
        }

    }
}

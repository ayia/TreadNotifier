using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

public class Program
{
    public static void Main(string[] args)
    {
        
        Message lastEmail = ReadLastEmail();
        if (lastEmail != null)
        {
            string json = JsonConvert.SerializeObject(lastEmail.Payload.Headers, Formatting.Indented);

            // Extract sender information from the "From" header
            var senderHeader = lastEmail.Payload.Headers.FirstOrDefault(header => header.Name == "Sender");
           
          
            string sender = senderHeader?.Value;
            if (sender == "noreply@carinabot.com")
            {
                // Extract date-time information from the "Date" header
                var dateTimeHeader = lastEmail.Payload.Headers.FirstOrDefault(header => header.Name == "Date");
            string dateTimeString = dateTimeHeader?.Value;
            DateTime ReciveddateTime = ConvertUtcTimeStringToDateTime(dateTimeString);
            DateTime NowdateTime = DateTime.UtcNow;
            // Extract email body content
            string body = GetMessageHtmlBody(lastEmail);

            // Print or use the extracted information as needed
            Console.WriteLine($"Sender: {sender}");
            Console.WriteLine($"Date-Time: {dateTimeString}");
            Console.WriteLine($"Subject: {lastEmail.Payload.Headers.FirstOrDefault(header => header.Name == "Subject")?.Value}");

            KeyValueExtractor.ExtractKeyValuePairs( body);
            // Console.WriteLine($"Body: {body}");
        }
    }
Console.ReadLine();
    }
    public static DateTime ConvertUtcTimeStringToDateTime(string utcTimeString)
    {
        string format = "ddd, dd MMM yyyy HH:mm:ss zzz (UTC)";

        // Convert the UTC time string to a DateTimeOffset object
        DateTimeOffset dateTimeOffset = DateTimeOffset.ParseExact(utcTimeString, format, System.Globalization.CultureInfo.InvariantCulture);

        // Convert the DateTimeOffset object to a DateTime object (ignoring the offset)
        DateTime dateTime = dateTimeOffset.UtcDateTime;

        return dateTime;
    }
    public static string GetMessageHtmlBody(Message email)
    {
        string htmlBody = "";
        TraverseEmailParts(email.Payload, ref htmlBody);
        return htmlBody;
    }

    private static void TraverseEmailParts(MessagePart part, ref string htmlBody)
    {
        // Check if the current part is the HTML body
        if (part.MimeType == "text/html" && !string.IsNullOrEmpty(part.Body.Data))
        {
            htmlBody = Encoding.UTF8.GetString(Base64UrlDecode(part.Body.Data));
            return;
        }

        // If the current part has children, recursively traverse them
        if (part.Parts != null)
        {
            foreach (var childPart in part.Parts)
            {
                TraverseEmailParts(childPart, ref htmlBody);
                if (!string.IsNullOrEmpty(htmlBody))
                {
                    return; // Stop traversing if the HTML body is found
                }
            }
        }
    }

    // Helper function to decode Base64 URL safe encoded strings
    private static byte[] Base64UrlDecode(string input)
    {
        string base64 = input.Replace('-', '+').Replace('_', '/');
        return Convert.FromBase64String(base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '='));
    }

    public static Message ReadLastEmail()
    {
        string[] scopes = { GmailService.Scope.GmailReadonly };

        UserCredential credential;
        using (var stream = new FileStream("./json/credentials2.json", FileMode.Open, FileAccess.Read))
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
        request.Q = "is:inbox"; // Only retrieve emails in the inbox
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

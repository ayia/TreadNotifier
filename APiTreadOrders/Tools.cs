using APiTreadOrders.Models;
using Google.Apis.Gmail.v1.Data;
using HtmlAgilityPack;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;

namespace APiTreadOrders
{
    public class Tools
    {
        public static DateTime ConvertUtcTimeStringToDateTime(string utcTimeString)
        {
            string format = "ddd, dd MMM yyyy HH:mm:ss zzz (UTC)";

            // Convert the UTC time string to a DateTimeOffset object
            DateTimeOffset dateTimeOffset = DateTimeOffset.ParseExact(utcTimeString, format, System.Globalization.CultureInfo.InvariantCulture);

            // Convert the DateTimeOffset object to a DateTime object (ignoring the offset)
            DateTime dateTime = dateTimeOffset.UtcDateTime;

            return dateTime;
        }

        public static TreadOrder ExtractKeyValuePairs(string html)
        {
            TreadOrder a = null;
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            List<string> keys = new List<string>()
        { "P/L",
        "Market",
        "Type",
        "Amount",
        "Open price",
        "Close price" }

                ;
            // Select the <div> that contains the key-value pairs
            var mainDiv = doc.DocumentNode.SelectSingleNode("//div[@style='max-width: 600px; padding: 5px 5px 5px 5px; margin: 0 auto; background-color: #ffff; text-align: left; font-family: Arial, Helvetica, sans-serif; font-size: 14px; color: #1b1b1b; font-weight: normal;']");

            if (mainDiv != null)
            {
                 a=new TreadOrder();
                // Extract the key-value pairs inside the <div>
                var strongElements = mainDiv.SelectNodes(".//strong");

                if (strongElements != null)
                {
                    foreach (var strongElement in strongElements)
                    {
                        string key = strongElement.InnerText.Trim();
                        key = key.Replace(":", string.Empty);
                        if (keys.Contains(key))
                        {
                            string value = strongElement.NextSibling?.InnerText?.Trim();
                            switch (key)
                            {
                                case "Market":
                                    a.Market = value;
                                    break;
                                case "Type":
                                    a.Type = value;
                                    break;
                                case "P/L":
                                    string pattern = @"\+([\d.]+) pips";

                                    Match match = Regex.Match(value, pattern);

                                    if (match.Success)
                                    {
                                        string extractedValue = match.Groups[1].Value;
                                       a.TakeProfitePips =Double.Parse(extractedValue);
                                    }
                                    break;
                               
                            }

                        }
                    }
                }

             
            }
            return a;
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

        private static byte[] Base64UrlDecode(string input)
        {
            string base64 = input.Replace('-', '+').Replace('_', '/');
            return Convert.FromBase64String(base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '='));
        }

    }
}

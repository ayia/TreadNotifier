using HtmlAgilityPack;
using NotiificationAPIchartdepth.Models;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Xml;

namespace NotiificationAPIchartdepth.Tools
{
    public class Extractor
    {

        public static async Task<CookieContainer> PerformLogin(string loginUrl, string username, string password)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, loginUrl+"?email="+username+"&password="+password);
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/115.0");
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("Accept-Language", "fr,fr-FR;q=0.8,en-US;q=0.5,en;q=0.3");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("Referer", "https://www.chartdepth.com/user/login");
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Site", "same-origin");
            request.Headers.Add("Pragma", "no-cache");
            request.Headers.Add("Cache-Control", "no-cache");
            // request.Headers.Add("Cookie", "XSRF-TOKEN=eyJpdiI6IlBRZGVQRXNpY1dVWjFLcWpOSCt0XC93PT0iLCJ2YWx1ZSI6IkJFNmZsR29vNVN1Sk02bStIeUNTVTlDSGtjbmh5djZKQTlFQndXXC9HRGZKXC9kaUpmb0xLTG9BV0xiRXVNTWtNMyIsIm1hYyI6ImIwMGUwMzM4NTkxMGNlZmUwMGJkNzM5YzA0ZTVmZDM2YmFlMDg2MjEwMTQzNmQ0Yjg1ZGJkNDkxNDQxMDI4ZjMifQ%3D%3D; lang_prod=eyJpdiI6IlhWT2FJUVFoSzBnWXlFb2l0eVZESnc9PSIsInZhbHVlIjoiY09MUXg0Y1pVMzl4WStjbXE2WHZRUT09IiwibWFjIjoiMDMxOTI3YmYyYjc1ZTk2ZTRhZGRiZGFkOWY4ZDQ1ZjFlYjIwOTYzMjlmZDZmYTViYTZmMzg0MGEyZDZiMmU1NyJ9; laravel_session_prod=eyJpdiI6InRNeVIwalNYRE5pa29VZVJMT3dIMEE9PSIsInZhbHVlIjoiNUN4cHdDZXJEcnBtVGxuK1RteThXYitGT3RPeGR4TlNYYStycTRvZjFJaXhJcTZcLzliNWVIVjNjSnhHV0ZabzYiLCJtYWMiOiIzNTEwMzZmNDc4NGE2ZjIxMzBkNmI5NGViOGEwNTNkZjEyZWYwNmQ4ZmJkYzM5MWVjNzk0NDIwNjRlMjgwZjMyIn0%3D");
            var loginResponse = await client.SendAsync(request);

            // Create a new CookieContainer to store the cookies.
            var cookieContainer = new CookieContainer();

            // Get the URI of the login response to associate the cookies with the correct domain.
            var responseUri = loginResponse.RequestMessage.RequestUri;

            // Get the Set-Cookie header(s) from the response.
            if (loginResponse.Headers.TryGetValues("Set-Cookie", out var cookieHeaders))
            {
                // Parse the Set-Cookie headers and add the cookies to the CookieContainer.
                foreach (var cookieHeader in cookieHeaders)
                {
                    cookieContainer.SetCookies(responseUri, cookieHeader);
                }
            }
            return cookieContainer;



        }
        public static async Task<string> GetPageHtml(string pageUrl, CookieContainer cookieContainer)
        {
            using (var httpClientHandler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var httpClient = new HttpClient(httpClientHandler))
            {
                // Set the desired request headers, if needed
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

                // Perform the request to fetch the HTML data
                var response = await httpClient.GetAsync(pageUrl);

                // Check if the request was successful
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // Read the HTML content as a string
                    var html = await response.Content.ReadAsStringAsync();
                    return html;
                }
                else
                {
                    // Handle request failure here
                    // You can throw an exception or return null based on your requirements
                    throw new Exception("Failed to fetch page HTML!");
                }
            }
        }

        public static List<HtmlNode> ExtractTables(string html)
        {
            var tables = new List<HtmlNode>();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var matchingTables = doc.DocumentNode.SelectNodes("//table[@class='table signal-details-table']");
            if (matchingTables != null)
            {
                tables.AddRange(matchingTables);
            }

            return tables;
        }

        public static List<List<KeyValuePair<string, string>>> ExtractTableData(string html)
        {
            var tableData = new List<List<KeyValuePair<string, string>>>();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var thead = doc.DocumentNode.SelectSingleNode("//thead");
            var tbody = doc.DocumentNode.SelectSingleNode("//tbody");

            var headerCells = thead.SelectNodes("tr/th");
            var rows = tbody.SelectNodes("tr");

            if (headerCells != null && rows != null)
            {
                foreach (var row in rows)
                {
                    var rowData = new List<KeyValuePair<string, string>>();
                    var cells = row.SelectNodes("td");
                    if (cells != null && cells.Count >= headerCells.Count)
                    {
                        for (int i = 0; i < headerCells.Count; i++)
                        {
                            var key = headerCells[i].InnerText.Trim();
                            var value = cells[i].InnerText.Trim();
                            rowData.Add(new KeyValuePair<string, string>(key, value));
                        }
                        tableData.Add(rowData);
                    }
                }
            }

            return tableData;
        }



        public static List<SignalData> ParseToSignalDataList(List<List<KeyValuePair<string, string>>> tableData)
        {
            var signalDataList = new List<SignalData>();

            foreach (var rowData in tableData)
            {
                var signalData = new SignalData();

                foreach (var kvp in rowData)
                {
                    var key = kvp.Key;
                    var value = kvp.Value;

                    switch (key)
                    {
                        case "Instrument":
                            signalData.Instrument = value;
                            break;
                        case "Action":
                            signalData.Action = value;
                            break;
                        case "Type":
                            signalData.Type = value;
                            break;
                        case "Open price":
                            string[] parts = value.Split('\n');

                            signalData.OpenPrice = parts[0];
                            string format = "yyyy-MM-dd HH:mm";
                       

                            signalData.OpenTime= DateTime.ParseExact(parts[1], format, CultureInfo.InvariantCulture);
                            break;

                        case "Take Profit":
                            // Define the regular expression pattern to match numeric values.
                            string pattern = @"\d+\.\d+";

                            // Create a regular expression object.
                            Regex regex = new Regex(pattern);

                            // Find all matches in the input string.
                            MatchCollection matches = regex.Matches(value);
                        // Extract the numeric values from the matches and store them in an array.
                            string[] numericValues = new string[matches.Count];
                            for (int i = 0; i < matches.Count; i++)
                            {
                                numericValues[i] = matches[i].Value;
                            }
                            signalData.TakeProfit1 = numericValues[0];
                            signalData.TakeProfit2 = numericValues[1];

                            // signalData.TakeProfit = value;
                            break;
                        case "Stop loss":
                            signalData.StopLoss = value;
                            break;

                            // Add more cases for other keys if needed
                    }
                }

                signalDataList.Add(signalData);
            }

            return signalDataList;
        }
    }
}




    



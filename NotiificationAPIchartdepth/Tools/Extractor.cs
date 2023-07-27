using HtmlAgilityPack;
using NotiificationAPIchartdepth.Models;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using System.Net;
using System.Net.Http.Headers;
using System.Xml;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;

namespace NotiificationAPIchartdepth.Tools
{
    public class Extractor
    {

        public static async Task<CookieContainer> PerformLogin(string loginUrl, string username, string password)
        {
            EdgeOptions options = new EdgeOptions();

            // Set up EdgeDriver
            using (var driver = new EdgeDriver(options)) { 
                // Navigate to the login page
                driver.Navigate().GoToUrl(loginUrl);

                // Find and fill in the login form fields
                var usernameField = driver.FindElement(By.Name("email"));
                var passwordField = driver.FindElement(By.Name("password"));

                usernameField.SendKeys(username);
                passwordField.SendKeys(password);

                // Submit the login form
                passwordField.Submit();

                // Wait for the page to load or perform any necessary waits here

                // Extract the cookies from the browser session
                var cookies = driver.Manage().Cookies.AllCookies.ToList();

              return  ConvertCookiesToCookieContainer(cookies);
            } 
                
            }
            private static CookieContainer ConvertCookiesToCookieContainer(List<OpenQA.Selenium.Cookie> cookies)
            {
                var cookieContainer = new CookieContainer();

            foreach (var cookie in cookies)
            {
                cookieContainer.Add(new Uri(cookie.Domain), new System.Net.Cookie(cookie.Name, cookie.Value));
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
                            signalData.OpenPrice = value;
                            break;
                     
                        case "Take Profit":
                            signalData.TakeProfit = value;
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


    



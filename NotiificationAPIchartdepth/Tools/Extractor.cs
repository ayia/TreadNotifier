using HtmlAgilityPack;
using NotiificationAPIchartdepth.Models;
using System.Xml;

namespace NotiificationAPIchartdepth.Tools
{
    public class Extractor
    {
        public static async Task<string> GetHtmlAsync(string url)
        {// Create an instance of HttpClient
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    // Make an HTTP GET request to the URL
                    HttpResponseMessage response = await httpClient.GetAsync(url);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the HTML content as a string
                        string htmlContent = await response.Content.ReadAsStringAsync();

                        // Now you have the HTML content as a string
                        return (htmlContent);
                    }
                    else
                    {
                        return String.Empty;
                    }
                }
                catch (HttpRequestException e)
                {
                    return String.Empty;
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
                        case "Close price":
                            signalData.ClosePrice = value;
                            break;
                        case "Take Profit":
                            signalData.TakeProfit = value;
                            break;
                        case "Stop loss":
                            signalData.StopLoss = value;
                            break;
                        case "Result":
                            signalData.Result = value;
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


    



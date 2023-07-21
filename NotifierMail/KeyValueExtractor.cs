using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class KeyValueExtractor
{
    public static void ExtractKeyValuePairs(string html)
    {
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
            // Extract the key-value pairs inside the <div>
            var strongElements = mainDiv.SelectNodes(".//strong");

            if (strongElements != null)
            {
                foreach (var strongElement in strongElements)
                {
                    string key = strongElement.InnerText.Trim();
                   key=key.Replace(":",string.Empty);
                    if (keys.Contains(key)) { 
                    string value = strongElement.NextSibling?.InnerText?.Trim();



                    Console.WriteLine($"{key} : {value}");
                    }
                }
            }
        }
    }
}

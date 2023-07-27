namespace NotiificationAPIchartdepth.Tools
{
    public class Ectractor
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
    }
}

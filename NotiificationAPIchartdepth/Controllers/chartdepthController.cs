using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotiificationAPIchartdepth.Models;
using NotiificationAPIchartdepth.Tools;
using System.Collections.Generic;
using System.Globalization;

namespace NotiificationAPIchartdepth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class chartdepthController : ControllerBase
    {


        [HttpGet(Name = "GetNotificationController")]
        public async Task<List<SignalData>> GetAsync(string dateString)
        {

            string format = "ddMMyyyyHHmm";
            DateTime excutiondatetime = DateTime.ParseExact(dateString, format, CultureInfo.InvariantCulture);


            string loginUrl = "https://www.chartdepth.com/user/login"; // Replace with the actual login URL
            string pageUrl = "https://www.chartdepth.com/signals"; // Replace with the protected page URL
            string username = "badre.zouiri@gmail.com"; // Replace with your login credentials
            string password = "MvDNk3NC"; // Replace with your login credentials

            var cookieContainer = await Extractor.PerformLogin(loginUrl, username, password);
            if (cookieContainer != null)
            {
                string html = await Extractor.GetPageHtml(pageUrl, cookieContainer);
                List<HtmlNode> data = Extractor.ExtractTables(html);
                var tableData = Extractor.ExtractTableData(data.ToArray()[1].OuterHtml);


                return Extractor.ParseToSignalDataList(tableData);
            }
            return null;

       

        }
    }
}

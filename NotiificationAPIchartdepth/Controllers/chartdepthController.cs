using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotiificationAPIchartdepth.Models;
using NotiificationAPIchartdepth.Tools;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace NotiificationAPIchartdepth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class chartdepthController : ControllerBase
    {


        [HttpGet(Name = "GetNotificationController")]
        public async Task<string> GetAsync()
        {

            /*string format = "ddMMyyyyHHmm";
            DateTime excutiondatetime = DateTime.ParseExact(dateString, format, CultureInfo.InvariantCulture);
            DateTime today = excutiondatetime.Date;*/
            DateTime excutiondatetime = DateTime.UtcNow;
            DateTime today = excutiondatetime.Date;
            StringBuilder stringBuilder = new StringBuilder();
            List<SignalData> todayList=new List<SignalData>();
            string loginUrl = "https://www.chartdepth.com/user/login/check"; // Replace with the actual login URL
            string pageUrl = "https://www.chartdepth.com/signals?currency%5B%5D=EURUSD&currency%5B%5D=GBPUSD&currency%5B%5D=USDCAD&currency%5B%5D=AUDUSD&currency%5B%5D=NZDUSD&currency%5B%5D=USDJPY&currency%5B%5D=EURGBP&currency%5B%5D=USDCHF&currency%5B%5D=EURAUD&currency%5B%5D=EURJPY&currency%5B%5D=CADCHF&currency%5B%5D=CADJPY&currency%5B%5D=GBPJPY&currency%5B%5D=AUDNZD&currency%5B%5D=AUDCAD&currency%5B%5D=AUDCHF&currency%5B%5D=AUDJPY&currency%5B%5D=EURNZD&currency%5B%5D=EURCAD&currency%5B%5D=EURCHF&currency%5B%5D=NZDJPY&currency%5B%5D=GBPAUD&currency%5B%5D=GBPCAD&currency%5B%5D=GBPCHF&currency%5B%5D=GBPNZD&currency%5B%5D=NZDCAD&currency%5B%5D=NZDCHF&currency%5B%5D=CHFJPY&type%5B%5D=Buy&type%5B%5D=Sell&group%5B%5D=HAR&group%5B%5D=MAN&tab=Active"; // Replace with the protected page URL
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     //  string pageUrl = "https://www.chartdepth.com/signals";
            string username = "badre.zouiri@gmail.com"; // Replace with your login credentials
            string password = "MvDNk3NC"; // Replace with your login credentials
            var cookieContainer =  Extractor.GetCookieContainer();
            if (cookieContainer==null)
             cookieContainer = await Extractor.PerformLogin(loginUrl, username, password);
            if (cookieContainer != null)
            {
                string html = await Extractor.GetPageHtml(pageUrl, cookieContainer);
                List<HtmlNode> data = Extractor.ExtractTables(html);
                var tableData = Extractor.ExtractTableData(data.ToArray()[0].OuterHtml);

                todayList = Extractor.ParseToSignalDataList(tableData);
                 todayList= todayList.Where(xp =>xp.ProgressTP > 0 ).ToList();
                foreach (SignalData signal in todayList)
                {

                    // Extract the desired properties and append them to the StringBuilder
                    stringBuilder.Append(signal.Instrument).Append("|")
                                 .Append(signal.Action).Append("|")
                                 .Append(signal.OpenTime.ToString("ddMMyyyyHHmm")).Append("|")
                                 .Append(signal.TakeProfit1).Append("|")
                                 .Append(signal.TakeProfit2).Append("|")
                                 .Append(signal.TP1Hit).Append("|")
                                 .Append(signal.StopLoss).Append("|")
                                 .Append(signal.OpenPrice).Append("|")
                                 .Append(signal.ProgressTP.ToString()).Append(";");

                }



            }
            return stringBuilder.ToString();
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotiificationAPIchartdepth.Tools;
using System.Globalization;

namespace NotiificationAPIchartdepth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class chartdepthController : ControllerBase
    {


        [HttpGet(Name = "GetNotificationController")]
        public async Task<string> GetAsync(string dateString)
        {

            string format = "ddMMyyyyHHmm";
            DateTime excutiondatetime = DateTime.ParseExact(dateString, format, CultureInfo.InvariantCulture);
            string htmldata = await Ectractor.GetHtmlAsync("https://www.chartdepth.com/signals?currency%5B%5D=EURUSD&currency%5B%5D=GBPUSD&currency%5B%5D=USDCAD&currency%5B%5D=AUDUSD&currency%5B%5D=NZDUSD&currency%5B%5D=USDJPY&currency%5B%5D=EURGBP&currency%5B%5D=USDCHF&currency%5B%5D=EURAUD&currency%5B%5D=EURJPY&currency%5B%5D=CADCHF&currency%5B%5D=CADJPY&currency%5B%5D=GBPJPY&currency%5B%5D=AUDNZD&currency%5B%5D=AUDCAD&currency%5B%5D=AUDCHF&currency%5B%5D=AUDJPY&currency%5B%5D=EURNZD&currency%5B%5D=EURCAD&currency%5B%5D=EURCHF&currency%5B%5D=NZDJPY&currency%5B%5D=GBPAUD&currency%5B%5D=GBPCAD&currency%5B%5D=GBPCHF&currency%5B%5D=GBPNZD&currency%5B%5D=NZDCAD&currency%5B%5D=NZDCHF&currency%5B%5D=CHFJPY&currency%5B%5D=XAUUSD&type%5B%5D=Buy&type%5B%5D=Sell&group%5B%5D=HAR&group%5B%5D=MAN&tab=Closed");



                return "hello";

        }
    }
}

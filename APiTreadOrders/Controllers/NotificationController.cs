using APiTreadOrders.DAO;
using APiTreadOrders.Models;
using Google.Apis.Gmail.v1.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace APiTreadOrders.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        [HttpGet(Name = "GetNotificationController")]
        public string Get(string dateString)
        {
           
            string format = "dd/MM/yyyy H:mm";
            DateTime excutiondatetime = DateTime.ParseExact(dateString, format, CultureInfo.InvariantCulture);


            Message lastMessage = GmailAcces.ReadLastEmail();
        
            if (GmailAcces.ValidateMail(lastMessage, excutiondatetime))
            {
                TreadOrder a= Tools.ExtractKeyValuePairs(Tools.GetMessageHtmlBody(lastMessage));
                return a.Market + "|" + a.Type + "|" + a.TakeProfitePips;
            }
            else
                return string.Empty;

        }
    }
}

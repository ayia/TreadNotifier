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
        public TreadOrder? Get(string dateString)
        {
           
            string format = "dd/MM/yyyy H:mm";
            DateTime excutiondatetime = DateTime.ParseExact(dateString, format, CultureInfo.InvariantCulture);


            Message lastMessage = GmailAcces.ReadLastEmail();
             TreadOrder tread = null;
            if (GmailAcces.ValidateMail(lastMessage, excutiondatetime))
            {
                tread = Tools.ExtractKeyValuePairs(Tools.GetMessageHtmlBody(lastMessage));

            }

            return tread;
        }
    }
}

using APiTreadOrders.DAO;
using Google.Apis.Gmail.v1.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APiTreadOrders.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        [HttpGet(Name = "GetNotificationController")]
        public string Get()
        {
            Message lastEmail =GmailAcces.ReadLastEmail();
            return "Hi Master";
        }
    }
}

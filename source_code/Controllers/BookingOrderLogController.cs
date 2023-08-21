using DeliverBox_BE.Objects;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeliverBox_BE.Controllers
{
    [Route("api/v1/bookingOrderLog")]
    [ApiController]
    public class BookingOrderLogController : Controller
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "QY1XtCBtW6LdNwMGx36VwjJKJqKYJmNOlP30jaxP",
            BasePath = "https://slsd-capstone-project-default-rtdb.asia-southeast1.firebasedatabase.app/"
        };
        IFirebaseClient client;

        [HttpGet(template: "get-order-log")]
        public ActionResult GetOrderLog (string bookingOrderId)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("BookingOrderLog");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

                var list = new List<BookingOrderLog>();
                
                if(data != null)
                {
                    foreach(var item in data)
                    {
                        if(JsonConvert.DeserializeObject<BookingOrderLog>(((JProperty)item).Value.ToString()).bookingOrderId == bookingOrderId)
                        {
                            list.Add(JsonConvert.DeserializeObject<BookingOrderLog>(((JProperty)item).Value.ToString()));
                        }
                    }
                }

                var json = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                return Content(json, "application/json");
            } catch (Exception ex)
            {
                var result = new { errCode = 1, errMessage = ex.Message };
                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                return Content(json, "application/json");
            }
        }
    }
}

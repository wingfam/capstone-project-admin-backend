using DeliverBox_BE.Models;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeliverBox_BE.Controllers
{
    [Route("api/v1/bookingOrder")]
    [ApiController]
    public class BookingOrderController : Controller
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "QY1XtCBtW6LdNwMGx36VwjJKJqKYJmNOlP30jaxP",
            BasePath = "https://slsd-capstone-project-default-rtdb.asia-southeast1.firebasedatabase.app/"
        };
        IFirebaseClient client;

        [HttpGet(template:"get-all")]
        public ActionResult GetBookingOrders ()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("BookingOrder");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            var list = new List<BookingOrder>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<BookingOrder>(((JProperty)item).Value.ToString()));
                }
            }

            var json = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
            return Content(json, "application/json");
        }

        [HttpPost(template:"add-booking-order")]
        public ActionResult AddBookingOrder ()

    }
}

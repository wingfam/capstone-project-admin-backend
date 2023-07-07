using DeliverBox_BE.Objects;
using FireSharp.Config;
using FireSharp.Extensions;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

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

            //Include Resident data
            response = client.Get("Resident");
            data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            if (data != null)
            {
                foreach (var order in list) { 
                    foreach (var item in data)
                    {
                        var value = JsonConvert.DeserializeObject<Resident>(((JProperty)item).Value.ToJson());
                        var jvalue = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                        var r = JsonConvert.DeserializeObject<Resident>(jvalue);
                        if (r.id == order.residentId)
                        {
                            order.Resident = r;
                            break;
                        }
                    }
                }
            }

            //Include Box data
            response = client.Get("Box");
            data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            var result = new Box();
            if (data != null)
            {
                foreach (var order in list)
                {
                    foreach (var item in data)
                    {
                        var value = JsonConvert.DeserializeObject<Box>(((JProperty)item).Value.ToJson());
                        var jvalue = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                        var b = JsonConvert.DeserializeObject<Box>(jvalue);
                        if (b.id == order.boxId)
                        {
                            order.Box = b;
                            break;
                        }
                    }
                }
            }

            var json = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
            return Content(json, "application/json");
        }

        [HttpGet(template: "search")]
        public ActionResult GetBookingOrder(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("BookingOrder/" + id);
            var bookingOrder = JsonConvert.DeserializeObject<BookingOrder>(response.Body);

            response = client.Get("Resident/" + bookingOrder.residentId);
            bookingOrder.Resident = JsonConvert.DeserializeObject<Resident>(response.Body);

            response = client.Get("Box/" + bookingOrder.boxId);
            bookingOrder.Box = JsonConvert.DeserializeObject<Box>(response.Body);

            var json = JsonConvert.SerializeObject(bookingOrder, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
            return Content(json, "application/json");
        }
    }
}

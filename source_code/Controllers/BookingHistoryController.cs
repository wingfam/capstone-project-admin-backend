using DeliverBox_BE.Objects;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DeliverBox_BE.Controllers
{
    public class BookingHistoryController : Controller
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "QY1XtCBtW6LdNwMGx36VwjJKJqKYJmNOlP30jaxP",
            BasePath = "https://slsd-capstone-project-default-rtdb.asia-southeast1.firebasedatabase.app/"
        };
        IFirebaseClient client;

        [HttpGet(template: "get-all")]
        public ActionResult GetAllBookingHistory()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("BookingHistory");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            var list = new List<BookingHistory>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<BookingHistory>(((JProperty)item).Value.ToString()));
                }
            }

            var json = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
            return Content(json, "application/json");
        }

        [HttpGet(template: "get-history-by-resident")]
        public ActionResult GetBookingHistoryviaResident (string residentId)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("BookingHistory");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var result = new List<BookingHistory>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    var value = JsonConvert.DeserializeObject<BookingHistory>(((JProperty)item).Value.ToJson());
                    var jvalue = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                    var bh = JsonConvert.DeserializeObject<BookingHistory>(jvalue);
                    if (bh.residentId == residentId)
                    {
                        result.Add(bh);
                    }
                }
            }

            var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });

            //Json convert
            return Content(json, "application/json");
        }
    }
}

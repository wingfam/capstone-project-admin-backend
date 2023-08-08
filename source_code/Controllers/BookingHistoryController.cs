using DeliverBox_BE.Objects;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using FireSharp.Extensions;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace DeliverBox_BE.Controllers
{
    [Route("api/v1/history")]
    [ApiController]
    [Authorize]
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
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("BookingHistory");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

                var list = new List<BookingHistory>();
                var temp = new List<BookingHistory>();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        temp.Add(JsonConvert.DeserializeObject<BookingHistory>(((JProperty)item).Value.ToString()));
                    }
                }

                //Add Obj BookingOrder to the temp List
                var tempBOrder = new BookingOrder();
                foreach (var item in temp)
                {
                    response = client.Get("BookingOrder/" + item.bookingId);
                    tempBOrder = JsonConvert.DeserializeObject<BookingOrder>(response.Body);

                    if (tempBOrder.status.ToLower() == "done") //BookingOrder Status need tobe Done to be added
                    {
                        item.BookingOrder = tempBOrder;
                        response = client.Get("Box/" + item.BookingOrder.boxId);
                        item.BookingOrder.Box = JsonConvert.DeserializeObject<Box>(response.Body);
                        list.Add(item);
                    }
                }

                foreach (var item in list)
                {
                    response = client.Get("Resident/" + item.residentId);
                    item.Resident = JsonConvert.DeserializeObject<Bussiness>(response.Body);
                    response = client.Get("Location/" + item.Resident.locationId);
                    item.Resident.Location = JsonConvert.DeserializeObject<Location>(response.Body);
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

        [HttpGet(template: "get-history-by-resident")]
        public ActionResult GetBookingHistoryviaResident (string residentId)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("BookingHistory");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

                var result = new List<BookingHistory>();
                var temp = new List<BookingHistory>();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        var value = JsonConvert.DeserializeObject<BookingHistory>(((JProperty)item).Value.ToJson());
                        if (value.residentId == residentId)
                        {
                            temp.Add(value);
                        }
                    }
                }

                //Add Obj BookingOrder to the temp List
                var tempBOrder = new BookingOrder();
                foreach (var item in temp)
                {
                    response = client.Get("BookingOrder/" + item.bookingId);
                    tempBOrder = JsonConvert.DeserializeObject<BookingOrder>(response.Body);

                    if (tempBOrder.status.ToLower() == "done") //BookingOrder Status need tobe Done to be added
                    {
                        item.BookingOrder = tempBOrder;
                        response = client.Get("Box/" + item.BookingOrder.boxId);
                        item.BookingOrder.Box = JsonConvert.DeserializeObject<Box>(response.Body);
                        result.Add(item);
                    }
                }

                //Add Obj Resident to the list
                foreach (var item in result)
                {
                    response = client.Get("Resident/" + item.residentId);
                    item.Resident = JsonConvert.DeserializeObject<Bussiness>(response.Body);
                    response = client.Get("Location/" + item.Resident.locationId);
                    item.Resident.Location = JsonConvert.DeserializeObject<Location>(response.Body);
                }

                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });

                //Json convert
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

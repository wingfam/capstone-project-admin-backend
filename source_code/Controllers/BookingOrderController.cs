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
    [Route("api/v1/bookingorder")]
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
            try
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
                foreach (var order in list)
                {
                    response = client.Get("Resident/" + order.residentId);
                    order.Resident = JsonConvert.DeserializeObject<Resident>(response.Body);
                    response = client.Get("Location/" + order.Resident.locationId);
                    order.Resident.Location = JsonConvert.DeserializeObject<Location>(response.Body);
                }

                //Include Box data
                foreach (var order in list)
                {
                    response = client.Get("Box/" + order.boxId);
                    order.Box = JsonConvert.DeserializeObject<Box>(response.Body);
                    response = client.Get("Cabinet/" + order.Box.cabinetId);
                    order.Box.Cabinet = JsonConvert.DeserializeObject<Cabinet>(response.Body);
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

        [HttpGet(template: "get-order-by-resident-and-box")]
        public ActionResult GetOrderByResidentAndBox(string? residentId, string? boxId)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("BookingOrder");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                var list = new List<BookingOrder>();
                var temp = new BookingOrder();
                if(data != null)
                {
                    foreach (var item in data)
                    {
                        temp = JsonConvert.DeserializeObject<BookingOrder>(((JProperty)item).Value.ToString());
                        if (residentId == null)
                        {
                            if(temp.boxId == boxId)
                            {
                                list.Add(temp);
                            }
                        } else if (boxId == null)
                        {
                            if(temp.residentId == residentId)
                            {
                                list.Add(temp);
                            }
                        } else
                        {
                            if (temp.residentId == residentId && temp.boxId == boxId)
                            {
                                list.Add(temp);
                            }
                        }
                    }

                    //Include Resident data
                    foreach (var order in list)
                    {
                        response = client.Get("Resident/" + order.residentId);
                        order.Resident = JsonConvert.DeserializeObject<Resident>(response.Body);
                        response = client.Get("Location/" + order.Resident.locationId);
                        order.Resident.Location = JsonConvert.DeserializeObject<Location>(response.Body);
                    }

                    //Include Box data
                    foreach (var order in list)
                    {
                        response = client.Get("Box/" + order.boxId);
                        order.Box = JsonConvert.DeserializeObject<Box>(response.Body);
                        response = client.Get("Cabinet/" + order.Box.cabinetId);
                        order.Box.Cabinet = JsonConvert.DeserializeObject<Cabinet>(response.Body);
                    }
                }

                var json = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                var result = new { errCode = 1, errMessage = ex.Message };
                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                return Content(json, "application/json");
            }
        }

        [HttpGet(template: "search")]
        public ActionResult GetBookingOrder(string id)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("BookingOrder/" + id);
                var bookingOrder = JsonConvert.DeserializeObject<BookingOrder>(response.Body);

                response = client.Get("Resident/" + bookingOrder.residentId);
                bookingOrder.Resident = JsonConvert.DeserializeObject<Resident>(response.Body);
                response = client.Get("Location/" + bookingOrder.Resident.locationId);
                bookingOrder.Resident.Location = JsonConvert.DeserializeObject<Location>(response.Body);

                response = client.Get("Box/" + bookingOrder.boxId);
                bookingOrder.Box = JsonConvert.DeserializeObject<Box>(response.Body);
                response = client.Get("Cabinet/" + bookingOrder.Box.cabinetId);
                bookingOrder.Box.Cabinet = JsonConvert.DeserializeObject<Cabinet>(response.Body);

                var json = JsonConvert.SerializeObject(bookingOrder, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
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

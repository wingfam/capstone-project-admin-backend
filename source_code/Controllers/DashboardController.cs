using DeliverBox_BE.Objects;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace DeliverBox_BE.Controllers
{
    public class DashboardController : Controller
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "QY1XtCBtW6LdNwMGx36VwjJKJqKYJmNOlP30jaxP",
            BasePath = "https://slsd-capstone-project-default-rtdb.asia-southeast1.firebasedatabase.app/"
        };
        IFirebaseClient client;

        [HttpGet(template: "get-total-business")]
        public ActionResult GetTotalResident ()
        {
            try
            {
                client = new FireSharp.FirebaseClient (config);
                FirebaseResponse response = client.Get("Business");

                int count = 0;
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                foreach (var item in data )
                {
                    count ++; //Count the number  of resident in the data list
                }
                var result = new {obj = "Total Business", count = count};

                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                var result = new { errCode = 1, errMessage = ex.Message };
                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                return Content(json, "application/json");
            }
        }

        [HttpGet(template: "get-total-box")]
        public ActionResult GetTotalBox()
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Box");

                int count = 0;
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                foreach (var item in data)
                {
                    count++; //Count the number  of box in the data list
                }
                var result = new { obj = "Total Box", count = count };

                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                var result = new { errCode = 1, errMessage = ex.Message };
                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                return Content(json, "application/json");
            }
        }

        [HttpGet(template: "get-total-cabinet")]
        public ActionResult GetTotalCabinet()
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Cabinet");

                int count = 0;
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                foreach (var item in data)
                {
                    count++; //Count the number  of cabinet in the data list
                }
                var result = new { obj = "Total Cabinet", count = count };

                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                return Content(json, "application/json");
            }
            catch (Exception ex)    
            {
                var result = new { errCode = 1, errMessage = ex.Message };
                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                return Content(json, "application/json");
            }
        }

        [HttpGet(template: "get-total-order")]
        public ActionResult GetTotalOrder()
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("BookingOrder");
                
                int count = 0;
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                foreach (var item in data)
                {
                    count++; //Count the number  of booking order in the data list
                }
                var result = new { obj = "Total Order", count = count };

                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                var result = new { errCode = 1, errMessage = ex.Message };
                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                return Content(json, "application/json");
            }
        }

        [HttpGet(template: "get-line-char")]
        public ActionResult GetLineCharData()
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                
                FirebaseResponse response = client.Get("BookingOrder");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                int count = 0;
                BookingOrder temp = new BookingOrder();
                var result = new List<CharObject>();
                for (int i = 7; i > 0; i--)
                {
                    DateTime now = DateTime.Now;
                    now = now.AddDays(-(i - 1));
                    foreach (var item in data)
                    {
                        temp = JsonConvert.DeserializeObject<BookingOrder>(((JProperty)item).Value.ToString());
                        if (temp.createDate.Date == now.Date)
                        {
                            count++;
                        }
                    }
                    result.Add(new CharObject((-(i - 8)).ToString() , now.Day + "/" + now.Month, count));
                    count = 0;
                }

                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                var result = new { errCode = 1, errMessage = ex.Message };
                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                return Content(json, "application/json");
            }
        }

        [HttpGet(template: "get-business-order")]
        public ActionResult GetBusinessOrder ()
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);

                FirebaseResponse response = client.Get("BookingOrder");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                int count_1 = 0;
                int count_2 = 0;
                BookingOrder temp = new BookingOrder();
                var result = new List<BusniessChartObject>();
                for (int i = 7; i > 0; i--)
                {
                    DateTime now = DateTime.Now;
                    now = now.AddDays(-(i - 1));
                    foreach (var item in data)
                    {
                        temp = JsonConvert.DeserializeObject<BookingOrder>(((JProperty)item).Value.ToString());
                        if (temp.createDate.Date == now.Date)
                        {
                            if(temp.businessId == "-NZMjlIkwdasdM9WfQJx")
                            {
                                count_1 ++;
                            } else if (temp.businessId == "-NbAX14dQHa8HIDjk-Km")
                            {
                                count_2 ++;
                            }
                        }
                    }
                    result.Add(new BusniessChartObject((-(i - 8)).ToString(), now.Day + "/" + now.Month
                        , "Vinhomes", count_1, "The CBD", count_2));
                    count_1 = 0;
                    count_2 = 0;
                }

                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
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

using DeliverBox_BE.Models;
using DeliverBox_BE.Objects;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
                FirebaseResponse response = client.Get("Business");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

                var businesses = new List<Business>();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        businesses.Add(JsonConvert.DeserializeObject<Business>(((JProperty)item).Value.ToString()));
                    }
                } else
                {
                    return Content(JsonConvert.SerializeObject(new { errCode = 1, errMessage = "Can't fetch business data" }, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None }), "application/json");
                }

                response = client.Get("BookingOrder");
                data = JsonConvert.DeserializeObject<dynamic>(response.Body);

                BookingOrder temp = new BookingOrder();
                var listCharObj = new List<BusinessChartObject>();

                for (int i = 7; i > 0; i--)
                {
                    DateTime now = DateTime.Now;
                    now = now.AddDays(-(i - 1));
                    int count = 0;

                    foreach(var business in businesses)
                    {
                        foreach (var bookingOrder in data)
                        {
                            temp = JsonConvert.DeserializeObject<BookingOrder>(((JProperty)bookingOrder).Value.ToString());
                            if (temp.createDate.Date == now.Date)
                            {
                                if(temp.businessId == business.id)
                                {
                                    count++;
                                }
                            }
                        }
                        listCharObj.Add(new BusinessChartObject(now.Day + "/" + now.Month, business.businessName, count));
                        count = 0;
                    }
                }

                var result = new List<BusinessChart>();
                var orderPerDay = new Collection<BusinessChartObject>();
                int z = 0;
                for (int i = 7; i > 0; i--) {
                    DateTime now = DateTime.Now;
                    now = now.AddDays(-(i - 1));
                    
                    for (int y = 1; y <= businesses.Count(); y++)
                    {
                        orderPerDay.Add(listCharObj[z]);
                        z++;
                    }
                    
                    result.Add(new BusinessChart(now.Day + "/" + now.Month, orderPerDay));
                    orderPerDay = new Collection<BusinessChartObject>();
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

using DeliverBox_BE.Objects;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

        [HttpGet(template: "get-total-resident")]
        public ActionResult GetTotalResident ()
        {
            try
            {
                client = new FireSharp.FirebaseClient (config);
                FirebaseResponse response = client.Get("Resident");

                int count = 0;
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                foreach (var item in data )
                {
                    count ++; //Count the number  of resident in the data list
                }
                var result = new {obj = "Total Resident", count = count};

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

        //public ActionResult GetCharData1 ()
        //{
        //    try
        //    {
        //        DateTime now = DateTime.Now;
        //        client = new FireSharp.FirebaseClient(config);

        //        FirebaseResponse response = client.Get("BookingOrder");
        //        dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
        //    }
        //    catch (Exception ex)
        //    {
        //        var result = new { errCode = 1, errMessage = ex.Message };
        //        var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
        //        return Content(json, "application/json");
        //    }
        //}
    }
}

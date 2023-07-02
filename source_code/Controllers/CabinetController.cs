using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using DeliverBox_BE.Objects;
using FireSharp.Extensions;
using DeliverBox_BE.Models;

namespace DeliverCabinet_BE.Controllers
{
    [Route("api/v1/Cabinet")]
    public class CabinetController : Controller
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "QY1XtCBtW6LdNwMGx36VwjJKJqKYJmNOlP30jaxP",
            BasePath = "https://slsd-capstone-project-default-rtdb.asia-southeast1.firebasedatabase.app/"
        };
        IFirebaseClient client;

        [HttpGet(template: "get-all")]
        public ActionResult GetAllCabinet ()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Cabinet");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            var list = new List<Cabinet>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<Cabinet>(((JProperty)item).Value.ToString()));
                }
            }

            var json = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
            return Content(json, "application/json");
        }

        [HttpGet(template: "search")]
        public ActionResult SearchCabinet (string id) 
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Cabinet");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            var result = new Cabinet();
            if (data != null)
            {
                foreach (var item in data)
                {
                    var value = JsonConvert.DeserializeObject<Cabinet>(((JProperty)item).Value.ToJson());
                    var jvalue = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                    var c = JsonConvert.DeserializeObject<Cabinet>(jvalue);
                    if (c.id == id)
                    {
                        result = c;
                    }
                }
            }

            var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
            return Content(json, "application/json");
        }

        [HttpGet(template: "get-cabient-by-location")]
        public ActionResult GetCabinetviaLocation(string locationId)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Cabinet");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var result = new List<Cabinet>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    var value = JsonConvert.DeserializeObject<Cabinet>(((JProperty)item).Value.ToJson());
                    var jvalue = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                    var c = JsonConvert.DeserializeObject<Cabinet>(jvalue);
                    if (c.locationId == locationId)
                    {
                        result.Add(c);
                    }
                }
            }

            var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });

            //Json convert
            return Content(json, "application/json");
        }

        [HttpPost(template: "add-cabient")]
        public ActionResult AddWCabinet([FromBody] CabinetAddModel model)
        {
            DateTime createDate = DateTime.Now;
            try
            {
                client = new FireSharp.FirebaseClient(config);

                var c = new Cabinet(createDate, model.locationId, model.isAvaiable);

                PushResponse pushResponse = client.Push("Cabinet/", c);
                c.id = pushResponse.Result.name;
                SetResponse setResponse = client.Set("Cabinet/" + c.id, c);

                var result = new { errCode = 0, errMessage = "Success" };
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

        [HttpPut(template: "edit-cabinet")]
        public ActionResult EditCabient (string id, [FromBody] CabinetEditModel model)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Cabinet/");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            try
            {
                var cabinet = new Cabinet();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        var value = JsonConvert.DeserializeObject<Cabinet>(((JProperty)item).Value.ToJson());
                        var jvalue = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                        var c = JsonConvert.DeserializeObject<Cabinet>(jvalue);
                        if (c.id == id)
                        {
                            cabinet = c;
                        }
                    }
                }
                cabinet.locationId = model.locationId;
                cabinet.isAvaiable = model.isAvaiable;

                response = client.Update("Cabinet/" + cabinet.id, cabinet);

                var result = new { errCode = 0, errMessage = "Success" };
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

        [HttpDelete(template: "delete-cabinet")]
        public ActionResult DeleteCabinet (string id)
        {
            client = new FireSharp.FirebaseClient(config);

            FirebaseResponse response = client.Get("Cabinet");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            try
            {
                var cabinet = new Cabinet();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        var value = JsonConvert.DeserializeObject<Cabinet>(((JProperty)item).Value.ToJson());
                        var jvalue = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                        var c = JsonConvert.DeserializeObject<Cabinet>(jvalue);
                        if (c.id == id)
                        {
                            cabinet = c;
                        }
                    }
                }
                cabinet.isAvaiable = false; //Delede = Change status to false
                response = client.Update("Cabinet/" + cabinet.id, cabinet);

                var result = new { errCode = 0, errMessage = "Success" };
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
    }
}

using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using DeliverBox_BE.Models;
using DeliverBox_BE.Objects;
using FireSharp.Extensions;

namespace DeliverLocation_BE.Controllers
{
    [Route("api/v1/location")]
    public class LocationController : Controller
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "QY1XtCBtW6LdNwMGx36VwjJKJqKYJmNOlP30jaxP",
            BasePath = "https://slsd-capstone-project-default-rtdb.asia-southeast1.firebasedatabase.app/"
        };
        IFirebaseClient client;

        [HttpGet(template: "get-all")]
        public ActionResult GetAllLocation()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Location");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            var list = new List<Location>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<Location>(((JProperty)item).Value.ToString()));
                }
            }

            var json = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
            return Content(json, "application/json");
        }

        [HttpPost(template: "add-location")]
        public ActionResult AddLocation([FromBody] LocationAddModel model)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);

                var l = new Location(model.name, model.address, model.status);

                PushResponse pushResponse = client.Push("Location/", l);
                l.id = pushResponse.Result.name;
                SetResponse setResponse = client.Set("Location/" + l.id, l);

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

        [HttpPut(template: "edit-location")]
        public async Task<ActionResult> EditLocationAsync(string id, [FromBody] LocationEditModel model)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Location/");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

                    var location = new Location();
                    if (data != null)
                    {
                        foreach (var item in data)
                        {
                            var value = JsonConvert.DeserializeObject<Location>(((JProperty)item).Value.ToJson());
                            var jvalue = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                            var l = JsonConvert.DeserializeObject<Location>(jvalue);
                            if (l.id == id)
                            {
                                location = l;
                            }
                        }
                    }

                    location.name = model.name;
                    location.address = model.address;
                    location.status = model.status;

                    response = await client.UpdateAsync("Location/" + location.id, location);

                    var result = new { errCode = 0, errMessage = "Success" };
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

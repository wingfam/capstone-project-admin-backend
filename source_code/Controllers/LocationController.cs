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
    [ApiController]
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
            try
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

                foreach (var item in list)
                {
                    response = client.Get("Bussiness/" + item.bussinessId);
                    item.Bussiness = JsonConvert.DeserializeObject<Bussiness>(response.Body);
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

        [HttpGet(template: "search")]
        public ActionResult SearchLocation(string id)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Location/" + id);
                var result = JsonConvert.DeserializeObject<Location>(response.Body);

                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });

                //Json convert
                return Content(json, "application/json");
            } catch(Exception ex)
            {
                var result = new { errCode = 1, errMessage = ex.Message };
                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                return Content(json, "application/json");
            }
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
                        var l = JsonConvert.DeserializeObject<Location>(((JProperty)item).Value.ToJson());
                        if (l.id == id)
                        {
                            location = l;
                        }
                    }
                }
                if (model.name != null)
                {
                    location.name = model.name;
                }
                if (model.address  != null)
                {
                    location.address = model.address;
                }   
                if(model.bussinessId != null)
                {
                    location.bussinessId = model.bussinessId;
                }
                if(model.status != null)
                {
                    location.status = model.status;
                }

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

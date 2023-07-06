using DeliverBox_BE.Models;
using DeliverBox_BE.Objects;
using FireSharp.Config;
using FireSharp.Extensions;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace MailBoxTest.Controllers
{
    [Route("api/v1/resident")]
    [ApiController]
    public class ResidentController : Controller
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "QY1XtCBtW6LdNwMGx36VwjJKJqKYJmNOlP30jaxP",
            BasePath = "https://slsd-capstone-project-default-rtdb.asia-southeast1.firebasedatabase.app/"
        };
        IFirebaseClient client;

        [HttpGet(template:"get-all")]
        public ActionResult GetResidents()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Resident");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Resident>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<Resident>(((JProperty)item).Value.ToString()));
                }
            }

            //Search for Location
            response = client.Get("Location");
            data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            if (data != null)
            {
                foreach (var resi in list) //Loop in list
                {
                    foreach (var item in data) //Loop in location data
                    {
                        var value = JsonConvert.DeserializeObject<Location>(((JProperty)item).Value.ToJson());
                        var jvalue = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                        var l = JsonConvert.DeserializeObject<Location>(jvalue);
                        if (l.id == resi.locationId)
                        {
                            resi.Location = l;
                        }
                    }
                }
            }

            var json = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });

            //Json convert
            return Content(json, "application/json");
        }

        [HttpGet(template: "search")]
        public ActionResult GetResident(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Resident/" + id);
            var result = JsonConvert.DeserializeObject<Resident>(response.Body);

            response = client.Get("Location/" + result.locationId);
            result.Location = JsonConvert.DeserializeObject<Location>(response.Body);

            var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });

            //Json convert
            return Content(json, "application/json");
        }

        [HttpGet(template: "get-resident-by-location")]
        public ActionResult GetResidentviaLocation(string locationId)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Resident");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var result = new List<Resident>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    var value = JsonConvert.DeserializeObject<Resident>(((JProperty)item).Value.ToJson());
                    var jvalue = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                    var r = JsonConvert.DeserializeObject<Resident>(jvalue);
                    if (r.locationId == locationId)
                    {
                        result.Add(r);
                    }
                }
            }

            //Search for Location
            response = client.Get("Location");
            data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            if (data != null)
            {
                foreach (var cabi in result) //Loop in list
                {
                    foreach (var item in data) //Loop in location data
                    {
                        var value = JsonConvert.DeserializeObject<Location>(((JProperty)item).Value.ToJson());
                        var jvalue = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                        var l = JsonConvert.DeserializeObject<Location>(jvalue);
                        if (l.id == cabi.locationId)
                        {
                            cabi.Location = l;
                        }
                    }
                }
            }

            var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });

            //Json convert
            return Content(json, "application/json");
        }

        [HttpPost(template:"add-resident")]
        public ActionResult AddResident([FromBody]ResidentAddModel model)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                //Creating pushing object and put in var 
                Resident r = new Resident(model.email, model.password, model.fullname, true, model.locationId);
                var data = r;

                PushResponse response = client.Push("Resident/", data);
                data.id = response.Result.name;
                SetResponse setResponse = client.Set("Resident/" + data.id, data);

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

        [HttpPut(template:"update")]
        public async Task<ActionResult> UpdateResident (string id, [FromBody]ResidentUpdateModel model)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Resident/" + id);

                var resident = JsonConvert.DeserializeObject<dynamic>(response.Body);

                resident.isAvailable = model.isAvailable;

                response = await client.UpdateAsync("Resident/" + resident.id, resident);

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

        [HttpDelete(template: "delete")]
        public async Task<ActionResult> DeleteResident(string id)
        {
            client = new FireSharp.FirebaseClient(config);

            FirebaseResponse response = client.Get("Resident/" + id);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            try
            {
                var resident = JsonConvert.DeserializeObject<dynamic>(response.Body);

                resident.isAvaiable = false; //Delede = Change status to false
                response = await client.UpdateAsync("Resident/" + resident.id, resident);

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

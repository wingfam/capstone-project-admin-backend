using DeliverBox_BE.Models;
using FireSharp.Config;
using FireSharp.Extensions;
using FireSharp.Interfaces;
using FireSharp.Response;
using MailBoxTest.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MailBoxTest.Controllers
{
    [Route("api/v1/home")]
    [ApiController]
    public class ResidentController : Controller
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "QY1XtCBtW6LdNwMGx36VwjJKJqKYJmNOlP30jaxP",
            BasePath = "https://slsd-capstone-project-default-rtdb.asia-southeast1.firebasedatabase.app/"
        };
        IFirebaseClient client;

        private static Random random = new Random(); //Random 8 characer gen
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [HttpGet(template:"get-all")]
        public ActionResult GetResudents()
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

            var json = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });

            //Json convert
            return Content(json, "application/json");
        }

        [HttpGet(template: "search")]
        public ActionResult GetUser(string residentId)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Resident");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var result = new Resident();
            if (data != null)
            {
                foreach (var item in data)
                {
                    var value = JsonConvert.DeserializeObject<Resident>(((JProperty)item).Value.ToJson());
                    var jvalue = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                    var r = JsonConvert.DeserializeObject<Resident>(jvalue);
                    if (r.residentId.ToUpper() == residentId.ToUpper())
                    {
                        result = r;
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
                Resident r = new Resident(RandomString(8).ToUpper(), model.phone, model.email, model.password, model.fullname, true);
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
        public async Task<ActionResult> UpdateResident (string residentId, [FromBody]ResidentUpdateModel model)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Resident/");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

                var resident = new Resident();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        var value = JsonConvert.DeserializeObject<Resident>(((JProperty)item).Value.ToJson());
                        var jvalue = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                        var r = JsonConvert.DeserializeObject<Resident>(jvalue);
                        if (r.residentId.ToUpper() == residentId.ToUpper())
                        {
                            resident = r;
                        }
                    }
                }
                resident.isAvaiable = model.isAvaiable;

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
        public async Task<ActionResult> DeleteResidentAsync(string residentId)
        {
            client = new FireSharp.FirebaseClient(config);

            FirebaseResponse response = client.Get("Resident");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            try
            {
                var resident = new Resident();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        var value = JsonConvert.DeserializeObject<Resident>(((JProperty)item).Value.ToJson());
                        var jvalue = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                        var r = JsonConvert.DeserializeObject<Resident>(jvalue);
                        if (r.residentId.ToUpper() == residentId.ToUpper())
                        {
                            resident = r;
                        }
                    }
                }
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

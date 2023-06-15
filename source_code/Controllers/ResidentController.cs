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

        [HttpGet(template:"get-residents")]
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

        [HttpGet(template: "search-resident")]
        public ActionResult GetUser(string residentId)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Resident");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Resident>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    var value = JsonConvert.DeserializeObject<Resident>(((JProperty)item).Value.ToJson());
                    var jvalue = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                    var r = JsonConvert.DeserializeObject<Resident>(jvalue);
                    if (r.ResidentId.ToUpper() == residentId.ToUpper())
                    {
                        list.Add(r);
                    }
                }
            }

            var json = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });

            //Json convert
            return Content(json, "application/json");
        }

        [HttpPost(template:"add-resident")]
        public String AddResident(string phone, string email, string password, string fullname)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                //Creating pushing object and put in var
                Resident r = new Resident(RandomString(8).ToUpper(), phone, email, password, fullname, true);
                var data = r;

                PushResponse response = client.Push("Resident/", data);
                data.Id = response.Result.name;
                SetResponse setResponse = client.Set("Resident/" + data.Id, data);

                if (setResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return "Add successfully";
                }
                else
                {
                    return "Something went wrong";
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        [HttpPut(template:"Edit-resident-profile")]
        public async Task<string> UpdateResident (string res_id, string phone, string fullname)
        {
            try
            {
                FirebaseResponse response = client.Get("Resident");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

                var resident = new Resident();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        var value = JsonConvert.DeserializeObject<Resident>(((JProperty)item).Value.ToJson());
                        var jvalue = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                        var r = JsonConvert.DeserializeObject<Resident>(jvalue);
                        if (r.ResidentId.ToUpper() == res_id.ToUpper())
                        {
                            resident = r;
                        }
                    }
                }

                resident.Phone = phone;
                resident.Fullname = fullname;

                response = await client.UpdateAsync("Resident/" + resident.Id, resident);
            } catch (Exception ex)
            {
                return ex.ToString();
            }
            return "Edit Successful";
        }

        [HttpDelete(template: "delete-resident")]
        public async Task<string> DeleteResidentAsync(string id)
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
                        if (r.ResidentId.ToUpper() == id.ToUpper())
                        {
                            resident = r;
                        }
                    }
                }
                resident.IsAvaiable = false; //Delede = Change status to false
                response = await client.UpdateAsync("Resident/" + resident.Id, resident);

                return "Delete Successful";
            } catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}

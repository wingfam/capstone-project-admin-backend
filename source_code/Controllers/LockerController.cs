using FireSharp.Config;
using FireSharp.Extensions;
using FireSharp.Interfaces;
using FireSharp.Response;
using MailBoxTest.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeliverBox_BE.Controllers
{
    //Locker status:
    //0: Deleted, not avaiable
    //1: On Stand by
    //2: In use
    //3: Maintenance

    [Route("api/v1/locker")]
    [ApiController]
    public class LockerController : Controller
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
        public ActionResult GetLockers ()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Locker");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            var list = new List<Locker>();
            if(data != null)
            {
                foreach(var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<Locker>(((JProperty)item).Value.ToString()));
                }
            }

            var json = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
            return Content(json, "application/json");
        }

        [HttpGet(template: "search")]
        public ActionResult GetLocker(string lockerId)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Locker");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            var result = new Locker();
            if (data != null)
            {
                foreach (var item in data)
                {
                    var value = JsonConvert.DeserializeObject<Locker>(((JProperty)item).Value.ToJson());
                    var jvalue = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                    var r = JsonConvert.DeserializeObject<Locker>(jvalue);
                    if (r.lockerId.ToUpper() == lockerId.ToUpper())
                    {
                        result = r;
                    }
                }
            }

            var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
            return Content(json, "application/json");
        }

        [HttpPost(template:"add-locker")]
        public String AddLocker (string locker_name, int locker_status)
        {
            DateTime validDate = DateTime.Now;
            try
            {
                client = new FireSharp.FirebaseClient(config);

                Locker l = new Locker(RandomString(8).ToUpper(), locker_name, locker_status, null, validDate);
                var data = l;

                PushResponse response = client.Push("Locker/", data);
                data.id = response.Result.name;
                SetResponse setResponse = client.Set("Locker/" + data.id, data);
                
                if(setResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return "Add successfully";
                } else
                {
                    return "Something went wrong";
                }
            } catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        [HttpPut(template:"edit")]
        public async Task<string> EditLocker (string locker_id, string locker_name, int locker_status, string unlock_code)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Locker/");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            try
            {
                var locker = new Locker();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        var value = JsonConvert.DeserializeObject<Locker>(((JProperty)item).Value.ToJson());
                        var jvalue = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                        var l = JsonConvert.DeserializeObject<Locker>(jvalue);
                        if (l.lockerId.ToUpper() == locker_id.ToUpper())
                        {
                            locker = l;
                        }
                    }
                }
                locker.lockerName = locker_name;
                locker.lockerStatus = locker_status;
                locker.unlockCode = unlock_code;

                response = await client.UpdateAsync("Locker/" + locker.lockerId, locker);

                return "Edit Success";
            } catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpDelete(template: "delete")]
        public async Task<string> DeleteLockerAsync(string locker_id)
        {
            client = new FireSharp.FirebaseClient(config);

            FirebaseResponse response = client.Get("Locker");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            try
            {
                var locker = new Locker();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        var value = JsonConvert.DeserializeObject<Locker>(((JProperty)item).Value.ToJson());
                        var jvalue = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                        var l = JsonConvert.DeserializeObject<Locker>(jvalue);
                        if (l.lockerId.ToUpper() == locker_id.ToUpper())
                        {
                            locker = l;
                        }
                    }
                }
                locker.lockerStatus = 0; //Delede = Change status to 0
                response = await client.UpdateAsync("Locker/" + locker.id, locker);

                return "Delete Successful";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}
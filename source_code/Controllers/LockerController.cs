using DeliverBox_BE.Models;
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
        public ActionResult GetLocker(string id)
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
                    var l = JsonConvert.DeserializeObject<Locker>(jvalue);
                    if (l.id == id)
                    {
                        result = l;
                    }
                }
            }
            var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
            return Content(json, "application/json");
        }

        [HttpPost(template:"add-locker")]
        public ActionResult AddLocker([FromBody] LockerAddModel model)
        {
            DateTime validDate = DateTime.Now;
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Locker");

                Locker l = new Locker(model.lockerName, model.lockerStatus, null, validDate);
                var input = l;

                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        var value = JsonConvert.DeserializeObject<Locker>(((JProperty)item).Value.ToJson());
                        var jvalue = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                        var locker = JsonConvert.DeserializeObject<Locker>(jvalue);
                        if (locker.lockerName.ToLower() == model.lockerName.ToLower())
                        {
                            var errResult = new { errCode = 1, errMessage = "Invalid Locker Name" };
                            return Content(JsonConvert.SerializeObject(errResult, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None }), "application/json");
                        }
                    }
                }

                PushResponse pResponse = client.Push("Locker/", input);
                input.id = pResponse.Result.name;
                SetResponse setResponse = client.Set("Locker/" + input.id, input);

                var result = new {errCode = 0, errMessage = "Success"};
                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                return Content(json, "application/json");
            } catch (Exception ex)
            {
                var result = new { errCode = 1, errMessage = ex.Message };
                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                return Content(json, "application/json");
            }
        }

        [HttpPut(template:"edit")]
        public async Task<ActionResult> EditLocker (string id, [FromBody] LockerEditModel model)
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
                        if (l.id == id)
                        {
                            locker = l;
                        }
                    }
                }
                locker.lockerName = model.lockerName;
                locker.lockerStatus = model.lockerStatus;

                response = await client.UpdateAsync("Locker/" + locker.id, locker);

                var result = new {errCode = 0, errMessage = "Success"};
                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                
                return Content(json, "application/json");
            } catch (Exception ex)
            {
                var result = new { errCode = 1, errMessage = ex.Message };
                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                return Content(json, "application/json");
            }
        }

        [HttpPut(template: "change-unlock-code")]
        public async Task<ActionResult> ChangeUnlockCode([FromBody] LockerChangeCodeModel model)
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
                        if (l.id == model.id)
                        {
                            locker = l;
                        }
                    }
                }
                locker.unlockCode = model.unlockCode;

                response = await client.UpdateAsync("Locker/" + locker.id, locker);

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

        [HttpDelete(template: "delete")]
        public async Task<ActionResult> DeleteLockerAsync(string id)
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
                        if (l.id == id)
                        {
                            locker = l;
                        }
                    }
                }
                locker.lockerStatus = false; //Delede = Change status to false
                response = await client.UpdateAsync("Locker/" + locker.id, locker);

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
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

namespace DeliverBox_BE.Controllers
{
    [Route("api/v1/box")]
    [ApiController]
    public class BoxController : Controller
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "QY1XtCBtW6LdNwMGx36VwjJKJqKYJmNOlP30jaxP",
            BasePath = "https://slsd-capstone-project-default-rtdb.asia-southeast1.firebasedatabase.app/"
        };
        IFirebaseClient client;

        [HttpGet(template:"get-all")]
        public ActionResult GetBoxs ()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Box");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            var list = new List<Box>();
            if(data != null)
            {
                foreach(var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<Box>(((JProperty)item).Value.ToString()));
                }
            }

            //Include Cabinet
            response = client.Get("Cabinet");
            data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            if (data != null)
            {
                foreach (var box in list) //Loop each box in list
                {
                    foreach (var item in data) //Loop each cabinet for each box
                    {
                        var value = JsonConvert.DeserializeObject<Cabinet>(((JProperty)item).Value.ToJson());
                        var jvalue = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                        var c = JsonConvert.DeserializeObject<Cabinet>(jvalue);
                        if (c.id == box.cabinetId)
                        {
                            box.Cabinet = c;
                        }
                    }
                }
            }

            var json = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
            return Content(json, "application/json");
        }

        [HttpGet(template: "search")]
        public ActionResult GetBox(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Box/" + id);

            var result = JsonConvert.DeserializeObject<Box>(response.Body);

            //Include Cabinet
            response = client.Get("Cabinet");

            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            if (data != null)
            {
                foreach (var item in data) //Loop cabinet data
                {
                    var value = JsonConvert.DeserializeObject<Cabinet>(((JProperty)item).Value.ToJson());
                    var jvalue = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                    var c = JsonConvert.DeserializeObject<Cabinet>(jvalue);
                    if (c.id == result.cabinetId)
                    {
                        result.Cabinet = c;
                    }
                }
            }

            var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
            return Content(json, "application/json");
        }

        [HttpGet(template: "get-box-by-cabinet")]
        public ActionResult GetBoxviaCabinet (string cabinetId)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Box");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            var result = new List<Box>();
            if(data != null)
            {
                foreach (var item in data)
                {
                    var value = JsonConvert.DeserializeObject<Box>(((JProperty)item).Value.ToJson());
                    var jvalue = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                    var b = JsonConvert.DeserializeObject<Box>(jvalue);
                    if (b.cabinetId == cabinetId)
                    {
                        result.Add(b);
                    }
                }
            }

            //Include Cabinet
            response = client.Get("Cabinet");
            data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            if (data != null)
            {
                foreach (var box in result) //Loop each box in list
                {
                    foreach (var item in data) //Loop each cabinet for each box
                    {
                        var value = JsonConvert.DeserializeObject<Cabinet>(((JProperty)item).Value.ToJson());
                        var jvalue = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                        var c = JsonConvert.DeserializeObject<Cabinet>(jvalue);
                        if (c.id == box.cabinetId)
                        {
                            box.Cabinet = c;
                            break;
                        }
                    }
                }
            }

            var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
            //Json convert
            return Content(json, "application/json");
        }

        [HttpPost(template:"add-box")]
        public ActionResult AddBox([FromBody] BoxAddModel model)
        {
            DateTime validDate = DateTime.Now;
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Box");

                var input = new Box(model.name, model.size, model.isStore, model.isAvailable, model.cabinetId);

                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        var value = JsonConvert.DeserializeObject<Box>(((JProperty)item).Value.ToJson());
                        var jvalue = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                        var box = JsonConvert.DeserializeObject<Box>(jvalue);
                        if (box.nameBox.ToLower() == model.name.ToLower())
                        {
                            var errResult = new { errCode = 1, errMessage = "Invalid Box Name" };
                            return Content(JsonConvert.SerializeObject(errResult, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None }), "application/json");
                        }
                    }
                }

                PushResponse pResponse = client.Push("Box/", input);
                input.id = pResponse.Result.name;
                SetResponse setResponse = client.Set("Box/" + input.id, input);

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
        public async Task<ActionResult> EditBox (string id, [FromBody] BoxEditModel model)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Box/");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            try
            {
                var box = new Box();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        var value = JsonConvert.DeserializeObject<Box>(((JProperty)item).Value.ToJson());
                        var jvalue = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                        var l = JsonConvert.DeserializeObject<Box>(jvalue);
                        if (l.id == id)
                        {
                            box = l;
                        }
                    }
                }
                box.nameBox = model.name;
                box.isStore = model.isStore;
                box.isAvailable = model.isAvailable;

                response = await client.UpdateAsync("Box/" + box.id, box);

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

        [HttpDelete(template: "delete")]
        public async Task<ActionResult> DeleteBoxAsync(string id)
        {
            client = new FireSharp.FirebaseClient(config);

            FirebaseResponse response = client.Get("Box");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            try
            {
                var box = new Box();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        var value = JsonConvert.DeserializeObject<Box>(((JProperty)item).Value.ToJson());
                        var jvalue = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                        var l = JsonConvert.DeserializeObject<Box>(jvalue);
                        if (l.id == id)
                        {
                            box = l;
                        }
                    }
                }
                box.isAvailable = false; //Delede = Change status to false
                response = await client.UpdateAsync("Box/" + box.id, box);

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
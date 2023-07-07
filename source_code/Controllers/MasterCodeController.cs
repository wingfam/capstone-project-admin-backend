using DeliverBox_BE.Models;
using DeliverBox_BE.Objects;
using FireSharp.Config;
using FireSharp.Extensions;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeliverBox_BE.Controllers
{
    [Route("api/v1/masterCode")]
    public class MasterCodeController : Controller
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "QY1XtCBtW6LdNwMGx36VwjJKJqKYJmNOlP30jaxP",
            BasePath = "https://slsd-capstone-project-default-rtdb.asia-southeast1.firebasedatabase.app/"
        };
        IFirebaseClient client;

        [HttpGet(template: "get-mastercode")]
        public ActionResult GetMasterCodes()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("MasterCode/");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            var list = new List<MasterCode>();
            if(data != null)
            {
                foreach (var item in data)
                {
                    var m = JsonConvert.DeserializeObject<MasterCode>(((JProperty)item).Value.ToJson());
                    list.Add(m);
                }
            }

            var json = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
            return Content(json, "application/json");
        }

        [HttpPut(template: "edit-master-code")]
        public ActionResult EditMasterCode (string id, [FromBody] MasterCodeEditModel model)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("MasterCode/");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            try
            {
                var masterCode = new MasterCode();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        var value = JsonConvert.DeserializeObject<MasterCode>(((JProperty)item).Value.ToJson());
                        var jvalue = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                        var mc = JsonConvert.DeserializeObject<MasterCode>(jvalue);
                        if (mc.id == id)
                        {
                            masterCode = mc;
                            break;
                        }
                    }
                }
                masterCode.code = model.code;
                masterCode.isAvailable = model.isAvailable;

                response = client.Update("MasterCode/" + masterCode.id, masterCode);

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

        [HttpPost(template:("add-master-code"))]
        public ActionResult AddMasterCode([FromBody] MasterCodeAddModel model) 
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                MasterCode mcode = new MasterCode(model.code, model.isAvailable);

                PushResponse pResponse = client.Push("MasterCode/", mcode);
                mcode.id = pResponse.Result.name;
                SetResponse setResponse = client.Set("MasterCode/" + mcode.id, mcode);

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
        public ActionResult DeleteMasterCode (string id)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("MasterCode/" + id);

                var mCode = JsonConvert.DeserializeObject<MasterCode>(response.Body);
                mCode.isAvailable = false;

                response = client.Update("MasterCode/" + id, mCode);

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

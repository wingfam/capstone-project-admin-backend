using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using DeliverBox_BE.Models;
using DeliverBox_BE.Objects;
using FireSharp.Extensions;

namespace DeliverBox_BE.Controllers
{
    [Route("api/v1/warning")]
    [ApiController]
    public class AccessWarningController : Controller
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "QY1XtCBtW6LdNwMGx36VwjJKJqKYJmNOlP30jaxP",
            BasePath = "https://slsd-capstone-project-default-rtdb.asia-southeast1.firebasedatabase.app/"
        };
        IFirebaseClient client;

        [HttpGet(template: "get-all")]
        public ActionResult GetAllWarning()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("AccessWarning");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            var list = new List<AccessWarning>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<AccessWarning>(((JProperty)item).Value.ToString()));
                }
            }

            var json = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
            return Content(json, "application/json");
        }

        [HttpPost(template: "add-warning")]
        public ActionResult AddWarning([FromBody] AccessWarningAddModel model)
        {
            DateTime createDate = DateTime.Now;
            try
            {
                client = new FireSharp.FirebaseClient(config);

                var aw = new AccessWarning(model.message, model.lockerId, model.status, createDate);

                PushResponse pushResponse = client.Push("AccessWarning/", aw);
                aw.id = pushResponse.Result.name;
                SetResponse setResponse = client.Set("AccessWarning/" + aw.id, aw);

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

        [HttpPut(template: "change-read-status")]
        public async Task<ActionResult> ChangeReadStatus(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("AccessWarning/");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            try
            {
                var accessWarning = JsonConvert.DeserializeObject<AccessWarning>(response.Body);
                
                accessWarning.status = false;

                response = await client.UpdateAsync("AccessWarning/" + accessWarning.id, accessWarning);

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

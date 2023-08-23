using DeliverBox_BE.Objects;
using FireSharp.Interfaces;
using FireSharp.Response;
using FireSharp.Config;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace DeliverBox_BE.Controllers
{
    [Route("api/v1/cabinetLog")]
    [ApiController]
    public class CabinetLogController : Controller
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "QY1XtCBtW6LdNwMGx36VwjJKJqKYJmNOlP30jaxP",
            BasePath = "https://slsd-capstone-project-default-rtdb.asia-southeast1.firebasedatabase.app/"
        };
        IFirebaseClient client;

        [HttpGet(template: "get-cabinet-log")]
        public ActionResult GetCabinetLog (string cabinetId)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("CabinetLog");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

                var list = new List<CabinetLog>();

                if (data != null)
                {
                    foreach (var item in data)
                    {
                        if (JsonConvert.DeserializeObject<CabinetLog>(((JProperty)item).Value.ToString()).cabinetId == cabinetId)
                        {
                            list.Add(JsonConvert.DeserializeObject<CabinetLog>(((JProperty)item).Value.ToString()));
                        }
                    }
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
    }
}

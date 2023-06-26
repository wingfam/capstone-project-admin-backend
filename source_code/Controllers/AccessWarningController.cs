using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using MailBoxTest.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using DeliverBox_BE.Models;

namespace DeliverBox_BE.Controllers
{
    [Route("api/v1/warning")]
    public class AccessWarningController : Controller
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

        [HttpPost(template:"add-warning")]
        public String AddWarning ([FromBody] AccessWarningAddModel model)
        {
            DateTime createdDate = DateTime.Now;
            try
            {
                client = new FireSharp.FirebaseClient(config);

                AccessWarning a = new AccessWarning(model.message, model.lockerId, model.status, createdDate);
                
            } catch (Exception ex) { 
                
            }
            return "Add Succesfully";
        }
    }
}

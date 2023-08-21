using Microsoft.AspNetCore.Mvc;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Microsoft.Data.Sql;
using DeliverBox_BE.Models;
using FireSharp;

namespace DeliverBox_BE.Controllers
{
    [Route("api/v1/notification")]
    [ApiController]
    
    public class NotificationController : Controller
    {
        private static string AuthSecret = "QY1XtCBtW6LdNwMGx36VwjJKJqKYJmNOlP30jaxP";
        private static object BasePath = "https://slsd-capstone-project-default-rtdb.asia-southeast1.firebasedatabase.app/";

        Firebase.Database.FirebaseClient firebaseClient = new Firebase.Database.FirebaseClient(
            baseUrl: BasePath.ToString(),
            new FirebaseOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(AuthSecret)
            });

        private readonly IConfiguration _config;
        public NotificationController(IConfiguration config)
        {
            _config = config;
        }

        // Save fcm token of android device to firebase
        [Authorize]
        [HttpPost(template: "save-fcm-token")]
        public async Task<ActionResult> SaveFcmToken([FromBody] NotificationRequest model)
        {
            try
            {
                var response = await firebaseClient
                    .Child("Notification")
                    .OnceAsync<Object>();

                bool check = false;

                foreach (var item in response)
                {
                    dynamic value = item.Object;
                    string gotToken = value.token;

                    if (gotToken == model.Token)
                    {
                        check = true;
                    }
                }

                if (!check)
                {
                    var newToken = new Dictionary<string, string>
                    {
                        { "customerId", model.CustomerId },
                        { "token", model.Token },
                        { "data", "" }
                    };
                    await firebaseClient.Child("Notification").PostAsync(newToken);
                }

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

using Microsoft.AspNetCore.Mvc;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Microsoft.Data.Sql;
using DeliverBox_BE.Models;
using System.Diagnostics;
using NuGet.Protocol;

namespace DeliverBox_BE.Controllers
{
    [Route("api/v1/notification")]
    [ApiController]
    
    public class NotificationController : Controller
    {
        private static string AuthSecret = "QY1XtCBtW6LdNwMGx36VwjJKJqKYJmNOlP30jaxP";
        private static object BasePath = "https://slsd-capstone-project-default-rtdb.asia-southeast1.firebasedatabase.app/";

        FirebaseClient firebaseClient = new FirebaseClient(
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
        public async Task<ActionResult> SaveFcmToken([FromBody] NotificationRequest requestModel)
        {
            try
            {
                //Debug.WriteLine("Send token: " + requestModel.Token);

                var response = await firebaseClient
                    .Child("Notification")
                    .OrderBy("customerId")
                    .EqualTo(requestModel.CustomerId)
                    .OnceAsync<Object>();

                foreach (var item in response)
                {
                    dynamic value = item.Object;
                    string codeId = item.Key;
                    string gotToken = value.token;

                    //Debug.WriteLine("gotToken: " + gotToken);
                    if (gotToken != requestModel.Token)
                    {
                        Debug.WriteLine("FCM token updated with: " + requestModel.Token);
                        var updateToken = new Dictionary<string, string> { { "token", requestModel.Token } };
                        await firebaseClient.Child("Notification").Child(codeId).PatchAsync(updateToken);
                    }
                }

                var result = new { errCode = 0, errMessage = "Success" };
                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                var newToken = new Dictionary<string, string>
                    {
                        { "customerId", requestModel.CustomerId },
                        { "token", requestModel.Token },
                        { "message", "" }
                    };
                await firebaseClient.Child("Notification").PostAsync(newToken);

                var result = new { errCode = 1, errMessage = ex.Message };
                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                return Content(json, "application/json");
            }
        }
    }
}

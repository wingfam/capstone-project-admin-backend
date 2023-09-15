using Microsoft.AspNetCore.Mvc;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Microsoft.Data.Sql;
using DeliverBox_BE.Models;
using System.Diagnostics;
using NuGet.Protocol;
using System.Diagnostics.Tracing;
using Microsoft.IdentityModel.Tokens;

namespace DeliverBox_BE.Controllers
{
    [Route("api/v1/notification")]
    [ApiController]
    
    public class NotificationController : Controller
    {
        private static readonly string AuthSecret = "QY1XtCBtW6LdNwMGx36VwjJKJqKYJmNOlP30jaxP";
        private static readonly object BasePath = "https://slsd-capstone-project-default-rtdb.asia-southeast1.firebasedatabase.app/";

        readonly FirebaseClient firebaseClient = new(
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
        [HttpPost(template: "save-fcm-token")]
        //[Authorize]
        public async Task<ActionResult> SaveFcmToken([FromBody] NotificationRequest requestModel)
        {
            try
            {
                var result = new Dictionary<string, dynamic> ();

                var response = await firebaseClient
                    .Child("Notification")
                    .OrderBy("deviceId")
                    .EqualTo(requestModel.DeviceId)
                    .OnceAsync<Object>();

                //Debug.WriteLine("Result: " + response.FirstOrDefault());

                if (response.FirstOrDefault() == null)
                {
                    var newToken = new Dictionary<string, string>
                    {
                        { "deviceId", requestModel.DeviceId },
                        { "token", requestModel.Token },
                        { "message", "" }
                    };

                    await firebaseClient.Child("Notification").PostAsync(newToken);

                    result = new()
                    {
                        { "errCode", 0 },
                        { "errMessage",  "New FCM token has been created" },
                    };
                }
                else
                {
                    foreach (var item in response)
                    {
                        dynamic value = item.Object;
                        string codeId = item.Key;
                        string gotToken = value.token;

                        if (gotToken != requestModel.Token)
                        {
                            Debug.WriteLine("FCM token updated with: " + requestModel.Token);
                            var updateToken = new Dictionary<string, string> { { "token", requestModel.Token } };
                            await firebaseClient.Child("Notification").Child(codeId).PatchAsync(updateToken);

                            result = new()
                            {
                                { "errCode", 0 },
                                { "errMessage",  "FCM token has been updated" },
                            };

                            break;
                        }
                    }
                }

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

using FireSharp.Config;
using FireSharp.Extensions;
using FireSharp.Interfaces;
using FireSharp.Response;
using MailBoxTest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.DataClassification;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace MailBoxTest.Controllers
{
    [Route("api/v1/admin")]
    [ApiController]
    public class AdminController : ControllerBase
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

        [HttpPost(template: "add-admin")]
        public String AddAdmin(string username, string password)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                //Creating pushing object and put in var
                Admin r = new Admin(RandomString(8), username, password);
                var data = r;


                PushResponse response = client.Push("Admin/", data);
                data.ID = response.Result.name;
                SetResponse setResponse = client.Set("Admin/" + data.ID, data);

                if (setResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return "Add successfully";
                }
                else
                {
                    return "Something went wrong";
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        [HttpPost(template:"verify-login")]
        public ActionResult VerifyLogin(string email, string password)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);

                FirebaseResponse response = client.Get("Admin");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

                var admin = new Admin();

                int result = 0;
                if(data != null)
                {
                    foreach (var item in data)
                    {
                        var value = JsonConvert.DeserializeObject<Admin>(((JProperty)item).Value.ToJson());
                        var jvalue = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                        var r = JsonConvert.DeserializeObject<Admin>(jvalue);
                        if (r.username.ToLower() == email.ToLower())
                        {
                            if (r.password.ToLower() ==  password.ToLower())
                            {
                                result = 0; //Account valid
                                admin = r; 
                                break;
                            } 
                        } else
                        {
                            result = 1; //Account invalid
                        }
                    }
                } else {
                    result = 2; //Null data
                }
                string msg = "";


                switch (result)
                {
                    case 0:
                        msg = "Login Success";
                        break;
                    case 1:
                        msg = "Incorrect Password";
                        break;
                    case 2:
                        msg = "Invalid Email";
                        break;
                }

                var resultJson = new {LoginStatus = result, Message = msg, admin};
                //0: Success 
                //1: Incorrect Password or Username
                //2: No data found
                //Return object {LoginStatus: (number),
                //               Message: (),
                //               Object: User,
                //              }
                return Content(JsonConvert.SerializeObject(resultJson, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None }), "application/json");
            }catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }
    }
}

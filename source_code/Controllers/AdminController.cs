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
        public String AddAdmin(string phone, string email, string password, string name)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                //Creating pushing object and put in var
                Admin r = new Admin(RandomString(8), email, password, name, phone);
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

        [HttpGet(template:"verify-login")]
        public string VerifyLogin(string email, string password)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);

                FirebaseResponse response = client.Get("Admin");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

                var Admin = new Admin();

                int result = 0;
                if(data != null)
                {
                    foreach (var item in data)
                    {
                        var value = JsonConvert.DeserializeObject<Admin>(((JProperty)item).Value.ToJson());
                        var jvalue = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                        var r = JsonConvert.DeserializeObject<Admin>(jvalue);
                        if (r.admin_email.ToLower() == email.ToLower())
                        {
                            if (r.admin_password.ToLower() ==  password.ToLower())
                            {
                                result = 1; //Account valid
                            } else
                            {
                                result = 2; //Incorrect Password
                            }
                        } else
                        {
                            result = 3; //Account invalid
                        }
                    }
                } else {
                    result = 4; //Null data
                }

                switch (result)
                {
                    case 1:
                        return "Login Success";
                    case 2:
                        return "Incorrect Password";
                    case 3:
                        return "Invalid Email";
                    case 4:
                        return "No data found";
                }
                return null;
            }catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}

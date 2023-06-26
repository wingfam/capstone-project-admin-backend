using DeliverBox_BE.Models;
using FireSharp.Config;
using FireSharp.Extensions;
using FireSharp.Interfaces;
using FireSharp.Response;
using MailBoxTest.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MailBoxTest.Controllers
{
    [Route("api/v1/admin")]
    [ApiController]
    public class AdminController : Controller
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "QY1XtCBtW6LdNwMGx36VwjJKJqKYJmNOlP30jaxP",
            BasePath = "https://slsd-capstone-project-default-rtdb.asia-southeast1.firebasedatabase.app/"
        };
        IFirebaseClient client;

        //public static string EncodePasswordToBase64(string password)
        //{
        //    try
        //    {
        //        byte[] encData_byte = new byte[password.Length];
        //        encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
        //        string encodedData = Convert.ToBase64String(encData_byte);
        //        return encodedData;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error in base64Encode" + ex.Message);
        //    }
        //}

        //public string DecodeFrom64(string encodedData)
        //{
        //    System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
        //    System.Text.Decoder utf8Decode = encoder.GetDecoder();
        //    byte[] todecode_byte = Convert.FromBase64String(encodedData);
        //    int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
        //    char[] decoded_char = new char[charCount];
        //    utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
        //    string result = new String(decoded_char);
        //    return result;
        //}

        [HttpPost(template: "add-admin")]
        public String AddAdmin([FromBody] AdminAddModel model)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                //Creating pushing object and put in var
                Admin r = new Admin(model.username, model.password);
                var data = r;

                PushResponse response = client.Push("Admin/", data);
                data.id = response.Result.name;
                SetResponse setResponse = client.Set("Admin/" + data.id, data);

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
        public ActionResult VerifyLogin([FromBody] AdminLoginModel model)
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
                        if (r.username.ToLower() == model.username.ToLower())
                        {
                            if (r.password ==  model.password)
                            {
                                result = 0; //Account valid
                                admin = r; 
                                break;
                            } else
                            {
                                result = 1; //Wrong password
                                admin = null; 
                                break;
                            }
                        } else
                        {
                            result = 2; //Wrong username
                            admin = null;
                        }
                    }
                } else {
                    result = 3; //Null data
                    admin = null;
                }
                string msg = "";

                switch (result)
                {
                    case 0:
                        msg = "Login Success";
                        break;
                    case 1:
                        msg = "Incorrect password";
                        break;
                    case 2:
                        msg = "Invalid username";
                        break;
                    case 3:
                        msg = "No data found";
                        break;
                }

                var resultJson = new {LoginStatus = result, Message = msg, admin};

                return Content(JsonConvert.SerializeObject(resultJson, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None }), "application/json");
            }catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }
    }
}

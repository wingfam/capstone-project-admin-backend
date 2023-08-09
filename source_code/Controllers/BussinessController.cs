using DeliverBox_BE.Models;
using DeliverBox_BE.Objects;
using FireSharp.Config;
using FireSharp.Extensions;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace MailBoxTest.Controllers
{
    [Route("api/v1/bussiness")]
    [ApiController]
    public class BussinessController : Controller
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "QY1XtCBtW6LdNwMGx36VwjJKJqKYJmNOlP30jaxP",
            BasePath = "https://slsd-capstone-project-default-rtdb.asia-southeast1.firebasedatabase.app/"
        };
        IFirebaseClient client;

        [HttpGet(template:"get-all")]
        public ActionResult GetBussinesss()
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Bussiness");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                var list = new List<Bussiness>();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        list.Add(JsonConvert.DeserializeObject<Bussiness>(((JProperty)item).Value.ToString()));
                    }
                }

                var json = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });

                //Json convert
                return Content(json, "application/json");
            } catch (Exception ex)
            {
                var result = new { errCode = 1, errMessage = ex.Message };
                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                return Content(json, "application/json");
            }
        }

        [HttpGet(template: "search")]
        public ActionResult GetBussiness(string id)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Bussiness/" + id);
                var result = JsonConvert.DeserializeObject<Bussiness>(response.Body);

                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });

                //Json convert
                return Content(json, "application/json");
            } catch (Exception ex)
            {
                var result = new { errCode = 1, errMessage = ex.Message };
                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                return Content(json, "application/json");
            }
        }

        [HttpGet(template: "get-bussiness-by-location")]
        public ActionResult GetBussinessviaLocation(string locationId)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Location/" + locationId);
                var location = JsonConvert.DeserializeObject<Location>(response.Body);

                response = client.Get("Bussiness/" + location.bussinessId);
                var result = JsonConvert.DeserializeObject<Bussiness> (response.Body);

                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });

                //Json convert
                return Content(json, "application/json");
            } catch (Exception ex)
            {
                var result = new { errCode = 1, errMessage = ex.Message };
                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                return Content(json, "application/json");
            }
        }

        [HttpPost(template:"add-bussiness")]
        public ActionResult AddBussiness([FromBody]BussinessAddModel model)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                //Creating pushing object and put in var 
                Bussiness r = new Bussiness(model.name, model.address, model.phone, 1);
                var data = r;

                PushResponse response = client.Push("Bussiness/", data);
                data.id = response.Result.name;
                SetResponse setResponse = client.Set("Bussiness/" + data.id, data);

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

        [HttpPut(template:"update")]
        public async Task<ActionResult> UpdateBussiness (string id, [FromBody]BussinessUpdateModel model)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Bussiness/" + id);

                var bussiness = JsonConvert.DeserializeObject<Bussiness>(response.Body);

                bussiness.status = model.status;

                response = await client.UpdateAsync("Bussiness/" + bussiness.id, bussiness);

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

        [HttpDelete(template: "delete")]
        public async Task<ActionResult> DeleteBussiness(string id)
        {
            client = new FireSharp.FirebaseClient(config);

            FirebaseResponse response = client.Get("Bussiness/" + id);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            try
            {
                var bussiness = JsonConvert.DeserializeObject<dynamic>(response.Body);

                bussiness.isAvaiable = false; //Delede = Change status to false
                response = await client.UpdateAsync("Bussiness/" + bussiness.id, bussiness);

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
    }
}

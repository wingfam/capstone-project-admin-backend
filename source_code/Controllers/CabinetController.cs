using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using DeliverBox_BE.Objects;
using FireSharp.Extensions;
using DeliverBox_BE.Models;

namespace DeliverCabinet_BE.Controllers
{
    [Route("api/v1/cabinet")]
    [ApiController]
    public class CabinetController : Controller
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "QY1XtCBtW6LdNwMGx36VwjJKJqKYJmNOlP30jaxP",
            BasePath = "https://slsd-capstone-project-default-rtdb.asia-southeast1.firebasedatabase.app/"
        };
        IFirebaseClient client;


        [HttpGet(template: "get-all")]
        public ActionResult GetAllCabinet ()
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Cabinet");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

                var list = new List<Cabinet>();
                var temp = new Cabinet();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        list.Add(JsonConvert.DeserializeObject<Cabinet>(((JProperty)item).Value.ToString()));
                    }
                }

                //Search for Location
                response = client.Get("Location");
                data = JsonConvert.DeserializeObject<dynamic>(response.Body);

                if (data != null)
                {
                    foreach (var cabi in list) //Loop in list
                    {
                        foreach (var item in data) //Loop in location data
                        {
                            var value = JsonConvert.DeserializeObject<Location>(((JProperty)item).Value.ToJson());
                            var jvalue = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                            var l = JsonConvert.DeserializeObject<Location>(jvalue);
                            if (l.id == cabi.locationId)
                            {
                                cabi.Location = l;
                            }
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

        [HttpGet(template: "search")]
        public ActionResult SearchCabinet (string id) 
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Cabinet/" + id);

                var result = JsonConvert.DeserializeObject<Cabinet>(response.Body);

                response = client.Get("Location/" + result.locationId);
                result.Location = JsonConvert.DeserializeObject<Location>(response.Body);

                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                return Content(json, "application/json");
            } catch (Exception ex)
            {
                var result = new { errCode = 1, errMessage = ex.Message };
                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                return Content(json, "application/json");
            }
        }

        [HttpGet(template: "get-cabinet-by-location")]
        public ActionResult GetCabinetviaLocation(string locationId)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Cabinet");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

                var result = new List<Cabinet>();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        var c = JsonConvert.DeserializeObject<Cabinet>(((JProperty)item).Value.ToJson());
                        if (c.locationId == locationId)
                        {
                            result.Add(c);
                        }
                    }
                }

                foreach (var cabi in result) //Loop in list
                {
                    response = client.Get("Location/" + cabi.locationId);
                    cabi.Location = JsonConvert.DeserializeObject<Location>(response.Body);
                }

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

        [HttpPost(template: "add-cabinet")]
        public ActionResult AddWCabinet([FromBody] CabinetAddModel model)
        {
            DateTime createDate = DateTime.Now;
            try
            {
                client = new FireSharp.FirebaseClient(config);

                var c = new Cabinet(model.name, createDate, model.locationId, model.isAvailable);

                PushResponse pushResponse = client.Push("Cabinet/", c);
                c.id = pushResponse.Result.name;
                SetResponse setResponse = client.Set("Cabinet/" + c.id, c);

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

        [HttpPut(template: "edit-cabinet")]
        public ActionResult EditCabient (string id, [FromBody] CabinetEditModel model)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Cabinet/" + id);
                var cabinet = JsonConvert.DeserializeObject<Cabinet>(response.Body);

                cabinet.name = model.name;
                cabinet.locationId = model.locationId;
                cabinet.isAvailable = model.isAvailable;

                response = client.Update("Cabinet/" + cabinet.id, cabinet);

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

        [HttpDelete(template: "delete-cabinet")]
        public ActionResult DeleteCabinet (string id)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Cabinet/" + id);

                var cabinet = JsonConvert.DeserializeObject<Cabinet>(response.Body);
                
                cabinet.isAvailable = false; //Delede = Change status to false
                response = client.Update("Cabinet/" + cabinet.id, cabinet);

                //Set Box to false
                response = client.Get("Box");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

                var list = new List<Box>();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        list.Add(JsonConvert.DeserializeObject<Box>(((JProperty)item).Value.ToString()));
                    }
                }

                foreach (var box in list)
                {
                    box.isAvailable = false;
                    response = client.Update("Box/" + box.id, box); //Update to firebase
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

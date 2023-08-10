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
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        list.Add(JsonConvert.DeserializeObject<Cabinet>(((JProperty)item).Value.ToString()));
                    }
                }

                //Search for Location
                foreach (var item in list)
                {
                    response = client.Get("Location/" + item.locationId);
                    item.Location = JsonConvert.DeserializeObject<Location>(response.Body);
                }

                //Search for business
                foreach (var item in list)
                {
                    response = client.Get("Business/" + item.businessId);
                    item.Business = JsonConvert.DeserializeObject<Business>(response.Body);
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

                response = client.Get("Business/" + result.businessId);
                result.Business = JsonConvert.DeserializeObject<Business>(response.Body);

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
                //Search for business
                foreach (var item in result)
                {
                    response = client.Get("Business/" + item.businessId);
                    item.Business = JsonConvert.DeserializeObject<Business>(response.Body);
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

                var c = new Cabinet(model.name, createDate, 0, model.locationId, model.businessId, 0, null, null);

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

                if(model.name != null)
                {
                    cabinet.nameCabinet = model.name;
                }
                
                cabinet.locationId = model.locationId;
                cabinet.businessId = model.businessId;
                cabinet.status = model.status;
                if(model.mastercode != null)
                {
                    cabinet.masterCode = model.mastercode;
                }
                if(model.mastercodeStatus != null)
                {
                    cabinet.masterCodeStatus = model.mastercodeStatus;
                }

                response = client.Update("Cabinet/" + cabinet.id, cabinet);

                //Change Box availability
                if (cabinet.status == 1)
                {
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
                        box.status = 1;
                        response = client.Update("Box/" + box.id, box); //Update to firebase
                    }
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

        [HttpDelete(template: "delete-cabinet")]
        public ActionResult DeleteCabinet (string id)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Cabinet/" + id);

                var cabinet = JsonConvert.DeserializeObject<Cabinet>(response.Body);
                
                cabinet.status = 0; //Delede = Change status to false
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
                    box.status = 1;
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

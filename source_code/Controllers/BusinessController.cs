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
    [Route("api/v1/business")]
    [ApiController]
    public class BusinessController : Controller
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "QY1XtCBtW6LdNwMGx36VwjJKJqKYJmNOlP30jaxP",
            BasePath = "https://slsd-capstone-project-default-rtdb.asia-southeast1.firebasedatabase.app/"
        };
        IFirebaseClient client;

        [HttpGet(template:"get-all")]
        public ActionResult GetBusinesss()
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Business");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                var list = new List<Business>();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        list.Add(JsonConvert.DeserializeObject<Business>(((JProperty)item).Value.ToString()));
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
        public ActionResult GetBusiness(string id)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Business/" + id);
                var result = JsonConvert.DeserializeObject<Business>(response.Body);

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

        [HttpPost(template:"add-business")]
        public ActionResult AddBusiness([FromBody] BusinessAddModel model)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                //Creating pushing object and put in var 
                Business r = new Business(model.businessName, model.address, model.phone, 1);
                var data = r;

                PushResponse response = client.Push("Business/", data);
                data.id = response.Result.name;
                SetResponse setResponse = client.Set("Business/" + data.id, data);

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
        public async Task<ActionResult> UpdateBusiness (string id, [FromBody] BusinessUpdateModel model)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Business/" + id);

                var business = JsonConvert.DeserializeObject<Business>(response.Body);
                if(model.businessName != null)
                {
                    business.businessName = model.businessName;
                }
                if(model.address != null)
                {
                    business.address = model.address;
                }
                if(model.phone != null)
                {
                    business.phone = model.phone;
                }
                business.status = model.status;

                response = await client.UpdateAsync("Business/" + business.id, business);

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
        public async Task<ActionResult> DeleteBusiness(string id)
        {
            client = new FireSharp.FirebaseClient(config);

            FirebaseResponse response = client.Get("Business/" + id);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            try
            {
                var business = JsonConvert.DeserializeObject<Business>(response.Body);

                business.status = 0; //Delede = Change status to false
                response = await client.UpdateAsync("Business/" + business.id, business);

                response = client.Get("Cabinet");
                data = JsonConvert.DeserializeObject<dynamic>(response.Body);

                //Change Cabinet status
                var cabinets = new List<Cabinet>();
                var tempCabi = new Cabinet();
                if(data != null)
                {
                    foreach(var item in data)
                    {
                        tempCabi = JsonConvert.DeserializeObject<Cabinet>(((JProperty)item).Value.ToString());
                        if (tempCabi.businessId == id)
                        {
                            cabinets.Add(tempCabi);
                        }
                    }
                }
                foreach(var cabinet in cabinets)
                {
                    cabinet.status = 0;
                    response = client.Update("Cabinet/" + cabinet.id, cabinet);

                    var list = new List<Box>();
                    var tempBox = new Box();
                    if (data != null)
                    {
                        foreach (var item in data)
                        {
                            tempBox = JsonConvert.DeserializeObject<Box>(((JProperty)item).Value.ToString());
                            if (tempBox.cabinetId == cabinet.id)
                            {
                                list.Add(tempBox);
                            }
                        }
                    }

                    foreach (var box in list)
                    {
                        box.status = 0;
                        response = client.Update("Box/" + box.id, box); //Update to firebase
                    }
                }

                //Change Location status
                response = client.Get("Location");
                data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                var locations = new List<Location>();
                var tempLoca = new Location();
                if(data != null)
                {
                    foreach(var item in data)
                    {
                        tempLoca = JsonConvert.DeserializeObject<Location>(((JProperty)item).Value.ToString());
                        if(tempLoca.businessId == id)
                        {
                            locations.Add(tempLoca);
                        }
                    }
                }
                foreach (var location in locations)
                {
                    location.status = 0;
                    response = client.Update("Location/" + location.id, location);
                }

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

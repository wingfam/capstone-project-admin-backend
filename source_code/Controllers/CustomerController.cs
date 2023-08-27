using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DeliverBox_BE.Models;
using Firebase.Database;
using Firebase.Database.Query;
using System.Diagnostics;
using DeliverBox_BE.Objects;
using DeliverBox_BE.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DeliverBox_BE.Controllers
{
    [Route("api/v1/customer")]
    [ApiController]
    [Authorize]

    public class CustomerController : Controller
    {
        private static string secret = "QY1XtCBtW6LdNwMGx36VwjJKJqKYJmNOlP30jaxP";
        private static object path = "https://slsd-capstone-project-default-rtdb.asia-southeast1.firebasedatabase.app/";

        FirebaseClient firebaseClient = new FirebaseClient(
            baseUrl: path.ToString(),
            new FirebaseOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(secret)
            });

        [HttpGet(template: "search-location-by-businessId")]
        public async Task<ActionResult> SearchLocationByBusinessId(string businessId)
        {
            try
            {
                var response = await firebaseClient
                .Child("Location")
                .OrderBy("businessId")
                .EqualTo(businessId)
                .OnceAsync<Object>();

                List<Object> list = new();

                foreach (var item in response)
                {
                    LocationNamesModel model = new();
                    dynamic value = item.Object;
                    model.Name = value.nameLocation;
                    model.Id = value.id;
                    list.Add(model);
                }

                Dictionary<string, dynamic> dict = new Dictionary<string, dynamic>
                {
                    { "data", list }
                };

                var json = JsonConvert.SerializeObject(dict, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                var result = new { errCode = 1, errMessage = ex.Message };
                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                return Content(json, "application/json");
            }
        }

        [HttpGet(template: "get-available-box")]
        public async Task<ActionResult> FetchAvailableBox(string locationId)
        {
            try
            {
                dynamic? result = null;
                Box availableBox = new();

                List<Cabinet> cabinetList = await GetCabinetByLocationId(locationId);

                if (cabinetList.Count == 0)
                {
                    result = new { errCode = 1, errMessage = "Không có cabinet ở địa điểm này" };
                }
                else
                {
                    foreach (var cabinet in cabinetList)
                    {
                        string? cabinetId = cabinet.id;
                        availableBox = await GetAvailableBoxByCabinetId(cabinetId);
                        if (availableBox.nameBox != null)
                        {
                            var dict = new Dictionary<string, dynamic>
                            {
                                { "id", availableBox.id! },
                                { "nameBox", availableBox.nameBox! },
                                { "status", availableBox.status! },
                                { "errCode", 0 },
                                { "errMessage", "Success" },
                            };

                            result = dict;
                            break;
                        }
                    }

                    if (availableBox.nameBox == null)
                    {
                        result = new { errCode = 2, errMessage = "Không có tủ trống" };
                    }
                }

                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });

                //Json convert
                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                var result = new { errCode = 1, errMessage = ex.Message };
                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                return Content(json, "application/json");
            }
        }

        [HttpPost(template: "add-new-booking")]
        public async Task<ActionResult> AddNewBooking([FromBody] NewBookingModel model)
        {
            try
            {
                dynamic? result = null;

                RandomDigits randomDigits = new();
                string newUnlockCode = randomDigits.GenerateRandomCode();
                string logTitle = "Tạo booking";
                string logBody = $"Booking mới được tạo cho hộp tủ số {model.BoxName}, địa điểm {model.Location}";

                string bookingId = await CreateNewBooking(model, newUnlockCode);
                string bookingCode = await CreateNewBookingCode(bookingId);
                string logId = await CreateNewBookingLog(bookingId, logTitle, logBody);
                bool updateBoxStatus = await UpdateBoxStatus(model.BoxId);

                if (bookingId != null && bookingCode != null && logId != null && updateBoxStatus)
                {
                    Dictionary<string, dynamic> dict = new()
                    {
                        { "boxName", model.BoxName },
                        { "bookingCode", bookingCode },
                        { "errCode", 0 },
                        { "errMessage", "Success" }
                    };

                    result = dict;
                }
                else
                {
                    Dictionary<string, dynamic> dict = new()
                    {
                        { "errCode", 1 },
                        { "errMessage", "Something is wrong. Create booking unsuccessful" }
                    };

                    result = dict;
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

        [HttpGet(template: "get-active-booking")]
        public async Task<ActionResult> FetchActiveBooking(string customerId)
        {
            try
            {
                var response = await firebaseClient
                .Child("BookingOrder")
                .OrderBy("customerId")
                .EqualTo(customerId)
                .OnceAsync<Object>();

                List<Object> list = new();

                foreach (var item in response)
                {
                    dynamic value = item.Object;
                    if (value.status == 2 || value.status == 3)
                    {
                        ActiveBookingResponseModel model = new();
                        model.BookingId = value.id;
                        model.BoxId = value.boxId;
                        model.ValidDate = value.validDate;
                        model.Status = value.status;
                        model.UnlockCode = value.unlockCode;
                        model.BoxName = await GetBoxNameById((string)value.boxId);
                        model.BookingCode = await GetLastBookingCodeByBookingId((string)value.id);
                        list.Add(model);
                    }
                }

                Dictionary<string, dynamic> dict = new()
                {
                    { "data", list }
                };

                var json = JsonConvert.SerializeObject(dict, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });

                //Json convert
                return Content(json, "application/json");
            }
            catch (Exception ex) 
            {
                var result = new { errCode = 1, errMessage = ex.Message };
                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                return Content(json, "application/json");
            }
        }

        [HttpPost(template: "cancel-processing-booking")]
        public async Task<ActionResult> CancelProcessingBooking([FromBody] CancelProcessingBookingModel model)
        {
            try
            {
                DateTime currentTime = DateTime.Now;
                string cancelDate = currentTime.ToString("yyyy-MM-dd HH:mm");
                string logTitle = "Hủy booking";
                string logBody = $"Booking được hủy vào ngày: {cancelDate}";

                var newBookingStatus = new Dictionary<string, dynamic> { { "status", 5 }, };

                var newBoxStatus = new Dictionary<string, dynamic> { { "status", 1 }, };

                await firebaseClient
                  .Child("BookingOrder")
                  .Child(model.BookingId)
                  .PatchAsync(newBookingStatus);

                await firebaseClient
                  .Child("Box")
                  .Child(model.BoxId)
                  .PatchAsync(newBoxStatus);

                Dictionary<string, dynamic> result = new()
                {
                    { "errCode", 0 },
                    { "errMessage", $"Booking {model.BookingId} được hủy vào ngày: {cancelDate}" }
                };

                await CreateNewBookingLog(model.BookingId!, logTitle, logBody);

                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });

                //Json convert
                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                var result = new { errCode = 1, errMessage = ex.Message };
                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                return Content(json, "application/json");
            }
        }

        [HttpPost(template: "update-unlock-code")]
        public async Task<ActionResult> UpdateUnlockCode(string bookingId)
        {
            try
            {
                RandomDigits randomDigits = new RandomDigits();
                string newUnlockCode = randomDigits.GenerateRandomCode();
                string logTitle = "Cập nhật mã unlock";
                string logBody = $"Mã unlock {newUnlockCode} được cập nhật thành công";

                var newUnlockCodeData = new Dictionary<string, string> { { "unlockCode", newUnlockCode } };

                await firebaseClient
                    .Child("BookingOrder")
                    .Child(bookingId)
                    .PatchAsync(newUnlockCodeData);

                await CreateNewBookingLog(bookingId, logTitle, logBody);

                Dictionary<string, dynamic> result = new()
                {
                    { "unlockCode", newUnlockCode },
                    { "errCode", 0 },
                    { "errMessage", $"Mã unlock {newUnlockCode} được cập nhật thành công" }
                };

                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });

                //Json convert
                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                var result = new { errCode = 1, errMessage = ex.Message };
                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                return Content(json, "application/json");
            }
        }

        [HttpPost(template: "create-new-booking-code")]
        public async Task<ActionResult> AddNewBookingCode([FromBody] NewBookingCodeRequest request)
        {
            try
            {
                var newBookingCode = await CreateNewBookingCode(request.BookingId!);
                string logTitle = "Tạo mã booking";
                string logBody = $"Mã booking {newBookingCode} được tạo thành công";

                await CreateNewBookingLog(request.BookingId!, logTitle, logBody);

                Dictionary<string, dynamic> result = new()
                {
                    { "bookingCode", newBookingCode },
                    { "errCode", 0 },
                    { "errMessage", $"Mã booking mới {newBookingCode} được tạo thành công" }
                };

                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });

                //Json convert
                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                var result = new { errCode = 1, errMessage = ex.Message };
                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                return Content(json, "application/json");
            }
        }

        [HttpGet(template: "get-booking-history")]
        public async Task<ActionResult> FetchBookingHistory(string customerId)
        {
            try
            {
                var response = await firebaseClient
                .Child("BookingOrder")
                .OrderBy("customerId")
                .EqualTo(customerId)
                .OnceAsync<Object>();

                List<Object> list = new();

                foreach (var item in response)
                {
                    dynamic value = item.Object;
                    if (value.status == 4 || value.status == 5)
                    {
                        BookingHistoryResponseModel model = new();
                        model.ValidDate = value.validDate;
                        model.CreateDate = value.createDate;
                        model.Status = value.status;
                        model.BoxName = await GetBoxNameById((string)value.boxId);
                        list.Add(model);
                    }
                }

                Dictionary<string, dynamic> dict = new()
                {
                    { "data", list }
                };

                var json = JsonConvert.SerializeObject(dict, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });

                //Json convert
                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                var result = new { errCode = 1, errMessage = ex.Message };
                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                return Content(json, "application/json");
            }
        }

        [HttpGet(template: "get-notification")]
        public async Task<ActionResult> FetchNotification(string customerId)
        {
            try
            {
                var response = await firebaseClient
                .Child("Notification")
                .OrderBy("customerId")
                .EqualTo(customerId)
                .OnceAsync<Object>();

                List<Object> list = new();

                foreach (var item in response)
                {
                    dynamic? data = item.Object;
                    var model = (data.message);
                    list.Add(model);
                }
                
                Dictionary<string, dynamic> dict = new()
                {
                    { "message", list }
                };

                var json = JsonConvert.SerializeObject(dict, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });

                //Json convert
                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                var result = new { errCode = 1, errMessage = ex.Message };
                var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None });
                return Content(json, "application/json");
            }
        }

        private async Task<List<Cabinet>> GetCabinetByLocationId(string? locationId)
        {
            var list = new List<Cabinet>();
            try
            {
                var response = await firebaseClient
                    .Child("Cabinet")
                    .OrderBy("locationId")
                    .EqualTo(locationId)
                    .OnceAsync<Object>();

                foreach (var item in response)
                {
                    dynamic value = item.Object;
                    Cabinet cabinet = new Cabinet
                    {
                        id = value.id
                    };
                    list.Add(cabinet);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            //Debug.WriteLine($"Cabinet list: {list.Count()}");
            return list;
        }

        private async Task<Box> GetAvailableBoxByCabinetId(string? cabinetId)
        {
            Box box = new();
            try
            {
                var response = await firebaseClient
                    .Child("Box")
                    .OrderBy("cabinetId")
                    .EqualTo(cabinetId)
                    .OnceAsync<Box>();

                foreach (var item in response)
                {
                    dynamic value = item.Object;
                    int status = (int)(long)value.status;
                    if (status == 1)
                    {
                        box.id = value.id;
                        box.nameBox = value.nameBox;
                        box.status = value.status;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return box;
        }

        private async Task<string> GetBoxNameById(string? boxId)
        {
            string boxName = "";
            try
            {
                var response = await firebaseClient
                    .Child("Box")
                    .OrderBy("id")
                    .EqualTo(boxId)
                    .OnceAsync<Object>();

                foreach (var item in response)
                {
                    dynamic value = item.Object;
                    boxName = (string)value.nameBox;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return boxName;
        }

        private async Task<string> GetLastBookingCodeByBookingId(string? inputbookingId)
        {
            string bookingCode = "";
            try
            {
                var response = await firebaseClient
                    .Child("BookingCode")
                    .OrderBy("validDate")
                    .OnceAsync<Object>();

                foreach (var item in response)
                {
                    dynamic value = item.Object;
                    string bookingId = (string)value.bookingId;
                    if (bookingId == inputbookingId)
                    {
                        bookingCode = (string)value.bcode;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception here");
                Debug.WriteLine(ex);
            }

            return bookingCode;
        }

        private async Task<string> CreateNewBooking(NewBookingModel model, string newUnlockCode)
        {
            string bookingId = "";
            try
            {
                string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                var data = new Dictionary<string, dynamic>
                {
                    { "id", "" },
                    { "businessId", model.BusinessId },
                    { "boxId", model.BoxId },
                    { "customerId", model.CustomerId },
                    { "status", 2 },
                    { "createDate", currentTime },
                    { "validDate", model.ValidDate },
                    { "unlockCode", newUnlockCode }
                };

                var order = await firebaseClient.Child("BookingOrder").PostAsync(data);

                bookingId = order.Key;

                var newId = new Dictionary<string, dynamic> { { "id", bookingId }, };

                await firebaseClient
                  .Child("BookingOrder")
                  .Child(bookingId)
                  .PatchAsync(newId);
            } catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            
            return bookingId;
        }

        private async Task<bool> DeactivateBookingCode(string oldBookingCode)
        {
            bool isDeactivate = false;
            try
            {
                string id = "";

                var response = await firebaseClient
                    .Child("BookingCode")
                    .OrderBy("bcode")
                    .EqualTo(oldBookingCode)
                    .OnceAsync<Object>();
                
                foreach (var item in response) {
                    dynamic value = item.Object;
                    id = (string)value.id;
                    break;
                }

                if (id != null)
                {
                    var data = new Dictionary<string, dynamic>
                    {
                        { "status", 0 },
                    };

                    await firebaseClient
                        .Child("BookingCode")
                        .Child(id)
                        .PatchAsync(data);

                    isDeactivate = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return isDeactivate;
        }

        private async Task<string> CreateNewBookingCode(string bookingId)
        {
            string newBookingCode = "";
            try
            {
                DateTime currentTime = DateTime.Now.AddMinutes(10.0);
                RandomDigits randomDigits = new RandomDigits();
                newBookingCode = randomDigits.GenerateRandomCode();
                string validDate = currentTime.ToString("yyyy-MM-dd HH:mm");
                var data = new Dictionary<string, dynamic>
                {
                    { "bookingId", bookingId },
                    { "bcode", newBookingCode },
                    { "validDate", validDate },
                    { "status", 1 },
                };

                var bookingCode = await firebaseClient.Child("BookingCode").PostAsync(data);

                string bookingCodeId = bookingCode.Key;

                var newId = new Dictionary<string, dynamic> { { "id", bookingCodeId }, };

                await firebaseClient
                  .Child($"BookingCode")
                  .Child(bookingCodeId)
                  .PatchAsync(newId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            
            return newBookingCode;
        }

        private async Task<string> CreateNewBookingLog(string bookingId, string logTitle, string logBody)
        {
            string logId = "";
            try
            {
                DateTime currentTime = DateTime.Now;
                string createDate = currentTime.ToString("yyyy-MM-dd HH:mm");
                var data = new Dictionary<string, dynamic>
                {
                    { "id", "" },
                    { "bookingOrderId", bookingId },
                    { "messageTitle", logTitle },
                    { "messageBody", logBody },
                    { "messageStatus", 1 },
                    { "createDate", createDate },
                };

                var bookingLog = await firebaseClient.Child("BookingOrderLog").PostAsync(data);

                logId = bookingLog.Key;

                var newId = new Dictionary<string, dynamic> { { "id", logId }, };

                await firebaseClient
                  .Child($"BookingOrderLog")
                  .Child(logId)
                  .PatchAsync(newId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return logId;
        }

        private async Task<bool> UpdateBoxStatus(string boxId)
        {
            bool isUpdate = false;
            try
            {
                var newStatus = new Dictionary<string, dynamic>{ { "status", 0 }, };

                await firebaseClient
                    .Child("Box")
                    .Child(boxId)
                    .PatchAsync(newStatus);

                isUpdate = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return isUpdate;
        }
    }
}

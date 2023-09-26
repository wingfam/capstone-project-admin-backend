using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DeliverBox_BE.Models;
using Firebase.Database;
using Firebase.Database.Query;
using System.Diagnostics;
using DeliverBox_BE.Objects;
using DeliverBox_BE.Utils;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;

namespace DeliverBox_BE.Controllers
{
    [Route("api/v1/customer")]
    [ApiController]

    public class CustomerController : Controller
    {
        private static readonly string secret = "QY1XtCBtW6LdNwMGx36VwjJKJqKYJmNOlP30jaxP";
        private static readonly object path = "https://slsd-capstone-project-default-rtdb.asia-southeast1.firebasedatabase.app/";

        readonly FirebaseClient firebaseClient = new(
            baseUrl: path.ToString(),
            new FirebaseOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(secret)
            });

        [HttpGet(template: "search-location-by-businessId")]
        [Authorize]
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

                Dictionary<string, dynamic> dict = new()
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
        [Authorize]
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
        [Authorize]
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
                bool updateBoxStatus = await UpdateBoxProcess(model.BoxId, 2);

                if (bookingId != null && bookingCode != null && logId != null)
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
        [Authorize]
        public async Task<ActionResult> FetchActiveBooking(string deviceId, string businessId)
        {
            try
            {
                var response = await firebaseClient
                .Child("BookingOrder")
                .OrderBy("deviceId")
                .EqualTo(deviceId)
                .OnceAsync<Object>();

                List<Object> list = new();

                foreach (var item in response)
                {
                    dynamic value = item.Object;
                    int status = (int)(long)value.status;
                    string foundBusinessId = (string)value.businessId;

                    if (foundBusinessId == businessId && (value.status == 2 || value.status == 3))
                    {
                        string boxName = await GetBoxNameById((string)value.boxId);
                        string cabinetName = await GetCabinetNameByBoxId((string)value.boxId);

                        ActiveBookingResponseModel responseModel = new()
                        {
                            BookingId = value.id,
                            BoxId = value.boxId,
                            BoxName = boxName,
                            CabinetName = cabinetName,
                            ValidDate = value.validDate,
                            Status = value.status,
                        };

                        list.Add(responseModel);
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

        [HttpGet(template: "get-booking-history")]
        [Authorize]
        public async Task<ActionResult> FetchBookingHistory(string deviceId, string businessId)
        {
            try
            {
                var response = await firebaseClient
                .Child("BookingOrder")
                .OrderBy("deviceId")
                .EqualTo(deviceId)
                .OnceAsync<object>();

                List<object> list = new();

                foreach (var item in response)
                {
                    dynamic value = item.Object;
                    int status = (int)(long)value.status;
                    string foundBusinessId = (string)value.businessId;
                    string boxName = await GetBoxNameById((string)value.boxId);
                    string cabinetName = await GetCabinetNameByBoxId((string)value.boxId);

                    if (foundBusinessId == businessId && (status == 4 || status == 5))
                    {
                        BookingHistoryResponseModel model = new()
                        {
                            BookingId = value.id,
                            BoxId = value.boxId,
                            CabinetName = cabinetName,
                            BoxName = boxName,
                            ValidDate = value.validDate,
                            Status = value.status,
                        };

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

        [HttpGet(template: "get-booking-details")]
        [Authorize]
        public async Task<ActionResult> FetchBookingDetails(string bookingId, string boxId)
        {
            try
            {
                BookingDetailsResponseModel responseModel = new();

                var response = await firebaseClient
                    .Child("BookingOrder")
                    .OrderBy("id")
                    .EqualTo(bookingId)
                    .OnceAsync<Object>();

                foreach (var item in response)
                {
                    dynamic value = item.Object;

                    string unlockCode = value.unlockCode;
                    string locationName = await GetLocationByBoxId(boxId);
                    string bookingCode = await GetLastBookingCodeByBookingId(bookingId);

                    responseModel.location = locationName;
                    responseModel.bookingCode = bookingCode;
                    responseModel.unlockCode = unlockCode;
                }

                Dictionary<string, dynamic> result = new ()
                {
                    { "location", responseModel.location!},
                    { "bookingCode", responseModel.bookingCode!},
                    { "unlockCode", responseModel.unlockCode!},
                    { "errorCode", 0},
                    { "errMessage", "Lấy booking details thành công"},
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

        [HttpPost(template: "cancel-processing-booking")]
        [Authorize]
        public async Task<ActionResult> CancelProcessingBooking([FromBody] CancelProcessingBookingModel model)
        {
            try
            {
                DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
                string cancelDate = currentTime.ToString("yyyy/MM/dd HH:mm:ss");
                string logTitle = "Hủy booking";
                string logBody = $"Booking được hủy vào ngày: {cancelDate}";

                var newBookingStatus = new Dictionary<string, dynamic> { { "status", 5 }, };

                var newBoxProcess = new Dictionary<string, dynamic> { { "process", 0 }, };

                await firebaseClient
                  .Child("BookingOrder")
                  .Child(model.BookingId)
                  .PatchAsync(newBookingStatus);

                await firebaseClient
                  .Child("Box")
                  .Child(model.BoxId)
                  .PatchAsync(newBoxProcess);

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
        [Authorize]
        public async Task<ActionResult> UpdateUnlockCode(string bookingId)
        {
            try
            {
                RandomDigits randomDigits = new();
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
        [Authorize]
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

        [HttpGet(template: "get-notification")]
        [Authorize]
        public async Task<ActionResult> FetchNotification(string deviceId)
        {
            try
            {
                var response = await firebaseClient
                .Child("Notification")
                .OrderBy("deviceId")
                .EqualTo(deviceId)
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
                    int status = (int)(long)value.status;

                    if (status == 1)
                    {
                        Cabinet cabinet = new()
                        {
                            id = value.id
                        };

                        list.Add(cabinet);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return list;
        }

        private async Task<string> GetCabinetNameByBoxId(string? boxId)
        {
            string cabinetName = "";

            try
            {
                string cabinetId = await firebaseClient
                    .Child("Box")
                    .Child(boxId)
                    .Child("cabinetId")
                    .OnceSingleAsync<string>();

                cabinetName = await firebaseClient
                    .Child("Cabinet")
                    .Child(cabinetId)
                    .Child("nameCabinet")
                    .OnceSingleAsync<string>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return cabinetName;
        }

        private async Task<string> GetLocationByBoxId(string boxId)
        {
            string locationName = "";

            try
            {
                string cabinetId = await firebaseClient
                    .Child("Box")
                    .Child(boxId)
                    .Child("cabinetId")
                    .OnceSingleAsync<string>();

                string locationId = await firebaseClient
                    .Child("Cabinet")
                    .Child(cabinetId)
                    .Child("locationId")
                    .OnceSingleAsync<string>();

                locationName = await firebaseClient
                    .Child("Location")
                    .Child(locationId)
                    .Child("nameLocation")
                    .OnceSingleAsync<string>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            
            return locationName;
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
                    .OnceAsync<Object>();

                foreach (var item in response)
                {
                    dynamic value = item.Object;
                    int status = (int)(long)value.status;
                    int process = (int)(long)value.process;
                    if (status == 1 && process == 0)
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
                    break;
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
                DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
                string createDate = currentTime.ToString("yyyy/MM/dd HH:mm:ss");

                var data = new Dictionary<string, dynamic>
                {
                    { "id", "" },
                    { "businessId", model.BusinessId },
                    { "boxId", model.BoxId },
                    { "deviceId", model.DeviceId },
                    { "status", 2 },
                    { "createDate", createDate },
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
                DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.Now.AddMinutes(10.0), TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
                string validDate = currentTime.ToString("yyyy/MM/dd HH:mm:ss");

                RandomDigits randomDigits = new();
                newBookingCode = randomDigits.GenerateRandomCode();

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
                DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
                string createDate = currentTime.ToString("yyyy/MM/dd HH:mm:ss");

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

        private async Task<bool> UpdateBoxProcess(string boxId, int inputProcess)
        {
            bool isUpdate = false;

            try
            {
                var newStatus = new Dictionary<string, dynamic>{ { "process", inputProcess }, };

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

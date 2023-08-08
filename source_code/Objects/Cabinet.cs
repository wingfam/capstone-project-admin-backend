using System.Text.Json.Serialization;

namespace DeliverBox_BE.Objects
{
    public class Cabinet
    {
        public Cabinet()
        {
        }

        public Cabinet(string name, DateTime createDate, int totalBox, string locationId, string bussinessId, int status, string? mastercode, int? mastercodeStatus)
        {
            this.name = name;
            this.addDate = createDate;
            this.totalBox = totalBox;
            this.locationId = locationId;
            this.bussinessId = bussinessId;
            this.status = status;
            this.masterCode = mastercode;
            this.masterCodeStatus = masterCodeStatus;
        }

        public string? id { get; set; } = null;
        public string? name { get; set; }
        public DateTime addDate { get; set; }
        public int totalBox { get; set; }
        public string? locationId { get; set; }
        public string? bussinessId { get; set; }
        public int? status { get; set; }
        public string? masterCode { get; set; }
        public string? masterCodeStatus { get; set; }

        [JsonIgnore]
        public virtual Location? Location { get; set; }
        [JsonIgnore]
        public virtual Bussiness Bussiness { get; set; }
    }
}

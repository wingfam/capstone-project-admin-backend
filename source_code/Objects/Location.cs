using System.Text.Json.Serialization;

namespace DeliverBox_BE.Objects
{
    public class Location
    {
        public Location() { }
        public Location(string name, string address, string businessId, int status)
        {
            this.nameLocation = name;
            this.address = address;
            this.businessId = businessId;
            this.status = status;
        }

        public string? id { get; set; } = null;
        public string? nameLocation { get; set; }
        public string? address { get; set; } = null;
        public string? businessId { get; set; }
        public int? status { get; set; }

        [JsonIgnore]
        public virtual Business? Business { get; set; }
    }
}

using System.Text.Json.Serialization;

namespace DeliverBox_BE.Objects
{
    public class MasterCode
    {
        public MasterCode()
        {
        }

        public MasterCode(string code, bool isAvailable, string cabinetId)
        {
            this.code = code;
            this.isAvailable = isAvailable;
            this.cabinetId = cabinetId;
        }

        public string? id { get; set; } = null;
        public string? code { get; set; }
        public bool? isAvailable { get; set; }
        public string? cabinetId { get; set; }

        [JsonIgnore]
        public virtual Cabinet? Cabinet { get; set; }
    }
}

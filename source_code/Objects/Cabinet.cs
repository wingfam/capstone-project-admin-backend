﻿using System.Text.Json.Serialization;

namespace DeliverBox_BE.Objects
{
    public class Cabinet
    {
        public Cabinet()
        {
        }

        public Cabinet(string name, DateTime createDate, int totalBox, string locationId, string businessId, int status, string? mastercode, int? mastercodeStatus)
        {
            this.nameCabinet = name;
            this.addDate = createDate;
            this.totalBox = totalBox;
            this.locationId = locationId;
            this.businessId = businessId;
            this.status = status;
            this.masterCode = mastercode;
            this.masterCodeStatus = mastercodeStatus;
        }

        public string? id { get; set; } = null;
        public string? nameCabinet { get; set; }
        public DateTime addDate { get; set; }
        public int totalBox { get; set; }
        public string? locationId { get; set; }
        public string? businessId { get; set; }
        public int? status { get; set; }
        public string? masterCode { get; set; }
        public int? masterCodeStatus { get; set; }

        [JsonIgnore]
        public virtual Location? Location { get; set; }
        [JsonIgnore]
        public virtual Business Business { get; set; }
    }
}

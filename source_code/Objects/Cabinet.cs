﻿using System.Text.Json.Serialization;

namespace DeliverBox_BE.Objects
{
    public class Cabinet
    {
        public Cabinet()
        {
        }

        public Cabinet(string name, DateTime createDate, string locationId, bool isAvailable, string masterCodeId)
        {
            this.name = name;
            this.addDate = createDate;
            this.locationId = locationId;
            this.isAvailable = isAvailable;
            this.masterCodeId = masterCodeId;
        }

        public string? id { get; set; } = null;
        public string? name { get; set; }
        public DateTime addDate { get; set; }
        public string? locationId { get; set; }
        public bool? isAvailable { get; set; }
        public string? masterCodeId { get; set; }

        [JsonIgnore]
        public virtual Location? Location { get; set; }
        [JsonIgnore]
        public virtual MasterCode? MasterCode { get; set; }
    }
}

﻿namespace DeliverBox_BE.Models
{
    public class CabinetEditModel
    {
        public string name { get; set; }
        public string locationId { get; set; }
        public bool isAvailable { get; set; }
        public string masterCodeId { get; set; }
    }
}

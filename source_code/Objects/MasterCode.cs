namespace DeliverBox_BE.Objects
{
    public class MasterCode
    {
        public string? id { get; set; } = null;
        public string? cabinetId { get; set; } = null;
        public string? code { get; set; }
        public bool? isAvailable { get; set; }
    }
}

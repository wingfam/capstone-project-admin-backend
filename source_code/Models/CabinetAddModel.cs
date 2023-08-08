namespace DeliverBox_BE.Models
{
    public class CabinetAddModel
    {
        public string name { get; set; }
        public string locationId { get; set; }
        public string bussinessId { get; set; }
        public bool isAvailable { get; set; }
    }
}

namespace DeliverBox_BE.Models
{
    public class NewBookingModel
    {
        public string BusinessId { get; set; }
        public string CustomerId { get; set; }
        public string BoxId { get; set; }
        public string BoxName { get; set; }
        public string Location { get; set; }
        public string ValidDate { get; set; }
    }
}

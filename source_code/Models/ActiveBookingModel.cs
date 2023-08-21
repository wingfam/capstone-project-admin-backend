namespace DeliverBox_BE.Models
{
    public class ActiveBookingResponseModel
    {
        public string? BookingId { get; set; }
        public string? BoxId { get; set; }
        public string? BoxName { get; set; }
        public string? ValidDate { get; set; }
        public int? Status { get; set; }
        public string? BookingCode { get; set;}
        public string? UnlockCode { get; set;}
    }
}

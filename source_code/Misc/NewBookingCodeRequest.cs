namespace DeliverBox_BE.Models
{
    public class NewBookingCodeRequest
    {
        public string? BookingId { get; set; }
        public string? OldBookingCode { get; set; }
    }
}

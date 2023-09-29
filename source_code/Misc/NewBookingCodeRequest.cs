namespace DeliverBox_BE.Models
{
    public class NewBookingCodeRequest
    {
        public string? bookingId { get; set; }
        public string? oldBookingCode { get; set; }
    }
}

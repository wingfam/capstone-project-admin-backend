namespace DeliverBox_BE.Models
{
    public class CancelProcessingBookingModel
    {
        public string? bookingId { get; set; }
        public string? oldBookingCode { get; set; }
        public string? boxId { get; set; }
    }
}

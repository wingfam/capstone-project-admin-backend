namespace DeliverBox_BE.Models
{
    public class BookingHistoryResponseModel
    {
        public string? BookingId { get; set; }
        public string? BoxId { get; set; }
        public string? CabinetName { get; set; }
        public string? BoxName { get; set; }
        public string? ValidDate { get; set; }
        public int? Status { get; set; }
    }
}

namespace DeliverBox_BE.Models
{
    public class BookingHistoryResponseModel
    {
        public string? BoxName { get; set; }
        public string? Location { get; set; }
        public string? ValidDate { get; set; }
        public string? CreateDate { get; set; }
        public int? Status { get; set; }
    }
}

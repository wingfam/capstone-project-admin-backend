namespace DeliverBox_BE.Objects
{
    public class BookingOrderLog
    {
        public BookingOrderLog (DateTime createDate, string? messageBody, int? messageStatus, string? messageTitle, string bookingOrderId)
        {
            this.id = id;
            this.createDate = createDate;
            this.messageBody = messageBody;
            this.messageStatus = messageStatus;
            this.messageTitle = messageTitle;
            this.bookingOrderId = bookingOrderId;
        }

        public string? id {  get; set; }
        public DateTime createDate { get; set; }
        public string? messageBody { get; set; }
        public int? messageStatus { get; set; }
        public string? messageTitle { get; set; }
        public string bookingOrderId { get; set; }
    }
}

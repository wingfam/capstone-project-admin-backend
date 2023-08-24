namespace DeliverBox_BE.Objects
{
    public class CabinetLog
    {
        public CabinetLog(DateTime createDate, string? messageBody, int? messageStatus
            , string? messageTitle, string cabinetId) {
            this.createDate = createDate;
            this.messageBody = messageBody;
            this.messageStatus = messageStatus;
            this.messageTitle = messageTitle;
            this.cabinetId = cabinetId;
        }

        public string? id {  get; set; }
        public DateTime createDate { get; set; }
        public string? messageBody { get; set; }
        public int? messageStatus { get; set; }
        public string? messageTitle { get; set; }
        public string cabinetId { get; set; }
    }
}

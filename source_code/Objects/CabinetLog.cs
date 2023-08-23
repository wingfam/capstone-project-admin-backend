namespace DeliverBox_BE.Objects
{
    public class CabinetLog
    {
        public string? id {  get; set; }
        public DateTime createDate { get; set; }
        public string? messageBody { get; set; }
        public int? messageStatus { get; set; }
        public string? messageTitle { get; set; }
        public string cabinetId { get; set; }
    }
}

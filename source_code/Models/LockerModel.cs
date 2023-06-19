namespace DeliverBox_BE.Models
{
    public class LockerModel
    {
        public string lockerId { get; set; }
        public string lockerName { get; set; }
        public bool lockerStatus { get; set; }
        public string unlockCode { get; set; }
    }
}

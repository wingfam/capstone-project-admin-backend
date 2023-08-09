namespace DeliverBox_BE.Models
{
    public class CabinetEditModel
    {
        public string? name { get; set; }
        public string locationId { get; set; }
        public string businessId { get; set; }
        public int status { get; set; }
        public string? mastercode { get; set; }
        public string? mastercodeStatus { get; set; }
    }
}

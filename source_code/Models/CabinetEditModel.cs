namespace DeliverBox_BE.Models
{
    public class CabinetEditModel
    {
        public string? nameCabinet { get; set; }
        public string locationId { get; set; }
        public string businessId { get; set; }
        public int status { get; set; }
        public string? masterCode { get; set; }
        public string? masterCodeStatus { get; set; }
    }
}

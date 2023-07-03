namespace DeliverBox_BE.Objects
{
    public class Cabinet
    {
        public Cabinet()
        {
        }

        public Cabinet(string name, DateTime createDate, string locationId, bool isAvaiable)
        {
            this.name = name;
            this.addDate = createDate;
            this.locationId = locationId;
            this.isAvaiable = isAvaiable;
        }

        public string? id { get; set; } = null;
        public string? name { get; set; }
        public DateTime addDate { get; set; }
        public string? locationId { get; set; }
        public bool? isAvaiable { get; set; }
    }
}

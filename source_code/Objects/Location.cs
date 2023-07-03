namespace DeliverBox_BE.Objects
{
    public class Location
    {
        public Location() { }
        public Location(string name, string address, bool status)
        {
            this.name = name;
            this.address = address;
            this.status = status;
        }

        public string? id { get; set; } = null;
        public string? name { get; set; }
        public string? address { get; set; } = null;
        public bool? status { get; set; }
    }
}

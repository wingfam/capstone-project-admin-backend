namespace DeliverBox_BE.Objects
{
    public class BusniessChartObject
    {
        public BusniessChartObject(string? id, string? date, string? business_1_name,
            int? busniess_1_amount, string? business_2_name, int? business_2_amount)
        {
            this.id = id;
            this.date = date;
            this.business_1_name = business_1_name;
            this.business_2_name = business_2_name;
            this.business_1_amount = busniess_1_amount;
            this.business_2_amount = business_2_amount;
        }
        public string? id { get; set; }
        public string? date { get; set; }
        public string? business_1_name { get; set; }
        public int? business_1_amount { get; set; }
        public string? business_2_name { get; set; }
        public int? business_2_amount { get; set; }
    }
}
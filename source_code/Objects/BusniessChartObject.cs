namespace DeliverBox_BE.Objects
{
    public class BusinessChartObject
    {
        public BusinessChartObject(string? date, string? businessName,
            int? busniessAmount)
        {
            id = Interlocked.Increment(ref nextId);
            this.date = date;
            this.businessName = businessName;
            this.businessAmount = busniessAmount;
        }

        static int nextId;
        public int id { get; private set; }
        public string? date { get; set; }
        public string? businessName { get; set; }
        public int? businessAmount { get; set; }
    }
}
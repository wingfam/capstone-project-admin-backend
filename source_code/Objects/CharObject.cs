namespace DeliverBox_BE.Objects
{
    public class CharObject
    {
        public CharObject(string id, string day, int amount)
        {
            this.id = id;
            this.day = day;
            this.amount = amount;
        }
        public string? id { get; set; }
        public string? day { get; set; }
        public int? amount { get; set; }
    }
}

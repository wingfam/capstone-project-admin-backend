namespace DeliverBox_BE.Models
{
    public class BoxAddModel
    {
        public string name { get; set; }
        public string size { get; set; }
        public bool isStore { get; set; }
        public bool isAvaiable { get; set; }
        public string cabinetId { get; set; }
    }
}

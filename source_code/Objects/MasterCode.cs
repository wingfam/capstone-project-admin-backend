namespace DeliverBox_BE.Objects
{
    public class MasterCode
    {
        public MasterCode()
        {
        }

        public MasterCode(string code, bool isAvailable)
        {
            this.code = code;
            this.isAvailable = isAvailable;
        }

        public string? id { get; set; } = null;
        public string? code { get; set; }
        public bool? isAvailable { get; set; }
    }
}

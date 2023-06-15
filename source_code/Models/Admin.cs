namespace MailBoxTest.Models
{
    public class Admin
    {
        public Admin()
        {
        }

        public Admin(string admin_id ,string email, string password, string name, string phone)
        {
            this.admin_id = admin_id;
            this.admin_email = email;
            this.admin_password = password;
            this.admin_name = name;
            this.phone = phone;
        }

        public string? ID { get; set; }
        public string? admin_id { get; set; }
        public string? admin_name { get; set; }
        public string? admin_email { get; set; }
        public string? admin_password { get; set; }
        public string? phone { get; set; }
    }
}

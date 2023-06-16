namespace MailBoxTest.Models
{
    public class Admin
    {
        public Admin()
        {
        }

        public Admin(string admin_id ,string username, string password)
        {
            this.admin_id = admin_id;
            this.username = username;
            this.password = password;
        }
       
        public string? ID { get; set; }
        public string? admin_id { get; set; }
        public string? username { get; set; }
        public string? password { get; set; }
    }
}

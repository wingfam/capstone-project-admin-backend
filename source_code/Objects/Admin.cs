namespace MailBoxTest.Models
{
    public class Admin
    {
        public Admin()
        {
        }

        public Admin(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
       
        public string? id { get; set; }
        public string? username { get; set; }
        public string? password { get; set; }
    }
}

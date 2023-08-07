using DeliverBox_BE.Objects;

namespace DeliverBox_BE.Misc
{
    public class UserConstants
    {
        public static List<User> Users = new()
            {
                    new User(){ Phone="0123456789",Password="123456",Role="User"}
            };
    }
}

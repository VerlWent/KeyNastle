using API1.Models;

namespace API1.Resources.Classes
{
    public class UserTokenResponse
    {
        public string Token { get; set; }
        public UserT User { get; set; }
    }
}

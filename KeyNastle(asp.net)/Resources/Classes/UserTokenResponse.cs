using System.Collections.Generic;

namespace KeyNastle.Resources.Classes
{
    public class UserTokenResponse
    {
        public string Token { get; set; }
        public UserInfo User { get; set; }
    }
}

namespace KeyNastle.Resources.Classes
{
    public class UserInfo
    {
        public string Login { get; set; } = null!;

        public string NickName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;
        public string RepeatPassword { get; set; }

        public DateOnly RegistrationDate { get; set; }

        public string Salt { get; set; } = null!;

        public int RoleId { get; set; }
    }
}

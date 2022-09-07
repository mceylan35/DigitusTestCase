namespace DigitusTestCase.WebAPP.ViewModels
{
    public class UserViewModel
    {
        public object AccessFailedCount { get; internal set; }
        public object ConcurrencyStamp { get; internal set; }
        public object Email { get; internal set; }
        public object LockoutEnabled { get; internal set; }
        public string Id { get; internal set; }
        public string SecurityStamp { get; internal set; }
        public bool PhoneNumberConfirmed { get; internal set; }
        public DateTimeOffset? LockoutEnd { get; internal set; }
        public string UserName { get; internal set; }
        public bool TwoFactorEnabled { get; internal set; }
        public string PhoneNumber { get; internal set; }
        public bool EmailConfirmed { get; internal set; }
        public string NormalizedEmail { get; internal set; }
        public string NormalizedUserName { get; internal set; }
        public string PasswordHash { get; internal set; }
    }
}

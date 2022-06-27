namespace DatingApp.Models
{
    public class AppUser
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}

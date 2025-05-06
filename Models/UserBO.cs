namespace dipwebapp.Models
{
    public class UserBO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserRole {  get; set; }
        public IEnumerable<ObjectBO>? AttachedObjects { get; set; }
    }
}

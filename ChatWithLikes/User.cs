namespace ChatWithLikes
{
    public class User
    {
        public User(int userId, string username, string email)
        {
            UserId = userId;
            Username = username;
            Email = email;
        }

        public int UserId { get; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
}

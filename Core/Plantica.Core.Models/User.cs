namespace Plantica.Core.Models
{
    public class User
    {
        private User()
        {
            // EF Core requires a parameterless constructor for materialization
            // of entities from the database.
            // This constructor should not be used directly.
            // It is only for EF Core.
            Id = Ulid.NewUlid();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            IsActive = true;
            IsDeleted = false;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <exception cref="ArgumentException"></exception>
        public User(string name, string email)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name), "Name cannot be null.");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));

            Id = Ulid.NewUlid();
            Name = new UserName(name);
            Email = email;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            IsActive = true;
            IsDeleted = false;
        }

        public Ulid Id { get; }

        public UserName Name { get; private set; }
        public string Email { get; private set; }
        public string? PasswordHash { get; private set; }
        public DateTime CreatedAt { get; }
        public DateTime UpdatedAt { get; private set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public void UpdateName(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name), "Name cannot be null.");

            Name = new UserName(name);
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));

            Email = email;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));

            PasswordHash = password;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

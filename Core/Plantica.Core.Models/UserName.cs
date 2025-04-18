namespace Plantica.Core.Models
{
    /// <summary>
    /// Represents a user's name.
    /// </summary>
    public class UserName
    {
        /// <summary>
        /// Creates a new instance of the <see cref="UserName"/> class.
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="ArgumentException"></exception>
        public UserName(string value)
        {
            // check if name is null or empty
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Name cannot be null or empty.", nameof(value));

            // maximum length of 50 characters
            if (value.Length > 50)
                throw new ArgumentException("Name cannot be longer than 50 characters.", nameof(value));

            // minimum length of 3 characters
            if (value.Length < 3)
                throw new ArgumentException("Name cannot be shorter than 3 characters.", nameof(value));

            // only letters, numbers, and spaces are allowed
            if (!value.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)))
                throw new ArgumentException("Name can only contain letters, numbers, and spaces.", nameof(value));

            Value = value;
        }

        public string Value { get; }

        public override string ToString()
        {
            return Value;
        }
    }
}

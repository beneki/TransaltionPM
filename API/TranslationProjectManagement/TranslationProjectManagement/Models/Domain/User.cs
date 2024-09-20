using System.ComponentModel.DataAnnotations;
using TranslationProjectManagement.Utilities;

namespace TranslationProjectManagement.Models.Domain
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }  // Primary Key (ULID)

        [Required]
        [MaxLength(50)]  // Maximum length for the username
        public string Username { get; set; }  // Username for login

        [Required]
        [MaxLength(256)]  // Maximum length for password hash (commonly used size)
        public string PasswordHash { get; set; }  // Password hash for authentication

        [Required]
        [MaxLength(50)]  // Maximum length for the role
        public string Role { get; set; }  // Role in the system (e.g., ProjectManager, Translator)

        // Constructor to generate Guid
        public User()
        {
            Id = SequentialGuid.NewSequentialGuid();
        }
    }
}

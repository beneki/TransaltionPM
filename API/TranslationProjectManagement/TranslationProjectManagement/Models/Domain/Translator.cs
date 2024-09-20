using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TranslationProjectManagement.Utilities;

namespace TranslationProjectManagement.Models.Domain
{
    public class Translator
    {
        [Key]
        public Guid Id { get; set; }  // Primary Key (ULID)

        [Required]
        [MaxLength(50)]  // Maximum length for first name
        public string FirstName { get; set; }  // Translator's first name

        [Required]
        [MaxLength(50)]  // Maximum length for last name
        public string LastName { get; set; }  // Translator's last name

        [Required]
        [EmailAddress]  // Ensures valid email format
        [MaxLength(100)]  // Maximum length for email
        public string Email { get; set; }  // Translator's email address

  
        public ICollection<TranslatorLanguage> Languages { get; set; } = new List<TranslatorLanguage>();  // Collection of languages the translator specializes in


        // Navigation property for related tasks
        [JsonIgnore]
        public ICollection<TranslatorTask> Tasks { get; set; } = new List<TranslatorTask>();  // Initialize collection to avoid null reference issues

        // Constructor to generate Guid
        public Translator()
        {
            Id = SequentialGuid.NewSequentialGuid();
        }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TranslationProjectManagement.Utilities;
using System.Text.Json.Serialization;

namespace TranslationProjectManagement.Models.Domain
{
    public class Language
    {
        [Key]
        public Guid Id { get; set; }  // Primary Key (ULID)

        [Required]
        [MaxLength(100)]  // Maximum length for the title
        public string Name { get; set; }  // Language name


        // Many-to-many relationship with translators using the join table
        [JsonIgnore]
        public ICollection<TranslatorLanguage> TranslatorLanguages { get; set; } = new List<TranslatorLanguage>();

        // Constructor to generate Guid
        public Language()
        {
            Id = SequentialGuid.NewSequentialGuid();
        }
    }
}
using System.ComponentModel.DataAnnotations;
using TranslationProjectManagement.Utilities;

namespace TranslationProjectManagement.Models.Domain
{
    public class Project
    {
        [Key]
        public Guid Id { get; set; }  // Primary Key (ULID)

        [Required]
        [MaxLength(100)]  // Maximum length for the project name
        public string Name { get; set; }  // Name of the project

        [Required]
        [MaxLength(500)]  // Maximum length for the project description
        public string Description { get; set; }  // Detailed description of the project

        [Required]
        public DateTime StartDate { get; set; }  // Start date of the project

        // Optional end date of the project, should be in the future if provided
        public DateTime? EndDate { get; set; }

        [Required]
        public ProjectStatus Status { get; set; }  // Enum to represent project status

        // Navigation property for related tasks
        public ICollection<Task> Tasks { get; set; } = new List<Task>();  // Initialize collection to avoid null reference issues

        // Constructor to generate Guid
        public Project()
        {
            Id = SequentialGuid.NewSequentialGuid();
        }
    }

    public enum ProjectStatus
    {
        NotStarted,
        InProgress,
        Completed,
        OnHold
    }
}

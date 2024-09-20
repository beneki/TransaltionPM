using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TranslationProjectManagement.Utilities;

namespace TranslationProjectManagement.Models.Domain
{
    public class Task
    {
        [Key]
        public Guid Id { get; set; }  // Primary Key (ULID)

        [Required]
        [MaxLength(100)]  // Maximum length for the title
        public string Title { get; set; }  // Title of the task

        [Required]
        [MaxLength(500)]  // Maximum length for the description
        public string Description { get; set; }  // Detailed description of the task

        [Required]
        public DateTime DueDate { get; set; }  // Deadline for task completion

        [Required]
        public TaskStatus Status { get; set; }  // Enum to represent task status

        // Foreign Key to Project
        [Required]
        public Guid ProjectId { get; set; }
        [ForeignKey(nameof(ProjectId))]
        public Project? Project { get; set; }  // Navigation property to the associated project

        public ICollection<TranslatorTask> TranslatorTasks { get; set; } = new List<TranslatorTask>(); // Navigation property

        // Constructor to generate Guid
        public Task()
        {
            Id = SequentialGuid.NewSequentialGuid();
        }
    }

    public enum TaskStatus
    {
        Pending,
        InProgress,
        Completed,
        Blocked
    }
}
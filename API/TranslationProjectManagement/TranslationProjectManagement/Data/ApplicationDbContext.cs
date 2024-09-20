using Microsoft.EntityFrameworkCore;
using TranslationProjectManagement.Models.Domain;

namespace TranslationProjectManagement.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Models.Domain.Task> Tasks { get; set; }
        public DbSet<Translator> Translators { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<TranslatorLanguage> TranslatorLanguages { get; set; }
        public DbSet<TranslatorTask> TranslatorTasks { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuring many-to-many relationship between Translator and Language
            modelBuilder.Entity<TranslatorLanguage>()
                .HasKey(tl => new { tl.TranslatorId, tl.LanguageId });

            modelBuilder.Entity<TranslatorLanguage>()
                .HasOne(tl => tl.Translator)
                .WithMany(t => t.Languages)
                .HasForeignKey(tl => tl.TranslatorId);

            modelBuilder.Entity<TranslatorLanguage>()
                .HasOne(tl => tl.Language)
                .WithMany(l => l.TranslatorLanguages)
                .HasForeignKey(tl => tl.LanguageId);

            // Configuring many-to-many relationship between Translator and Task
            modelBuilder.Entity<TranslatorTask>()
                .HasKey(tt => new { tt.TranslatorId, tt.TaskId });

            modelBuilder.Entity<TranslatorTask>()
                .HasOne(tt => tt.Translator)
                .WithMany(t => t.Tasks)
                .HasForeignKey(tt => tt.TranslatorId);

            modelBuilder.Entity<TranslatorTask>()
                .HasOne(tt => tt.Task)
                .WithMany(t => t.TranslatorTasks)
                .HasForeignKey(tt => tt.TaskId);
        }
    }
}

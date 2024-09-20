namespace TranslationProjectManagement.Models.Domain
{
    public class TranslatorTask
    {
        public Guid TranslatorId { get; set; }
        public Translator Translator { get; set; }

        public Guid TaskId { get; set; }
        public Task Task{ get; set; }

    }
}

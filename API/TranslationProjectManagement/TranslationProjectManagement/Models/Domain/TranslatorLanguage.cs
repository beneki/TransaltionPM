namespace TranslationProjectManagement.Models.Domain
{
    public class TranslatorLanguage
    {
        public Guid TranslatorId { get; set; }
        public Translator Translator { get; set; }

        public Guid LanguageId { get; set; }
        public Language Language { get; set; }

    }
}

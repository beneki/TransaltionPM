using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TranslationProjectManagement.Data;
using TranslationProjectManagement.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TranslationProjectManagement.Utilities;

namespace TranslationProjectManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LanguagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LanguagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/languages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Language>>> GetLanguages()
        {
            return await _context.Languages.ToListAsync();
        }

        // GET: api/languages/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Language>> GetLanguage(Guid id)
        {
            var language = await _context.Languages.FindAsync(id);

            if (language == null)
            {
                return NotFound();
            }

            return language;
        }

        // POST: api/languages
        [HttpPost]
        public async Task<ActionResult<Language>> CreateLanguage(Language language)
        {
            _context.Languages.Add(language);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLanguage), new { id = language.Id }, language);
        }

        // PUT: api/languages/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLanguage(Guid id, Language language)
        {
            if (id != language.Id)
            {
                return BadRequest();
            }

            _context.Entry(language).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LanguageExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/languages/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLanguage(Guid id)
        {
            var language = await _context.Languages.FindAsync(id);
            if (language == null)
            {
                return NotFound();
            }

            _context.Languages.Remove(language);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/languages/{languageId}/addTranslator/{translatorId}
        [HttpPost("{languageId}/addTranslator/{translatorId}")]
        public async Task<IActionResult> AssignTranslatorToLanguage(Guid languageId, Guid translatorId)
        {
            var language = await _context.Languages.FindAsync(languageId);
            var translator = await _context.Translators.FindAsync(translatorId);

            if (language == null || translator == null)
            {
                return NotFound();
            }

            // Check if the translator is already assigned to this language
            var existingAssignment = await _context.TranslatorLanguages
                .FirstOrDefaultAsync(tl => tl.LanguageId == languageId && tl.TranslatorId == translatorId);

            if (existingAssignment == null)
            {
                var translatorLanguage = new TranslatorLanguage
                {
                    LanguageId = languageId,
                    TranslatorId = translatorId
                };

                _context.TranslatorLanguages.Add(translatorLanguage);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        // Helper method to check if a language exists
        private bool LanguageExists(Guid id)
        {
            return _context.Languages.Any(e => e.Id == id);
        }
    }
}

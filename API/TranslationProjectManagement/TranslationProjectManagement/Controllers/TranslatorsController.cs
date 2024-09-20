using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TranslationProjectManagement.Models.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TranslationProjectManagement.Data;

namespace TranslationProjectManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TranslatorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TranslatorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Translator>>> GetTranslators()
        {
            var translators = await _context.Translators.Include(t => t.Languages).ThenInclude(l => l.Language).ToListAsync();
            
            if (translators == null)
            {
                return NotFound();
            }
            var response = translators.Select(translator => new
            {
                Id = translator.Id,
                FirstName = translator.FirstName,
                LastName = translator.LastName,
                Email = translator.Email,
                Languages = translator.Languages.Select(l => l.Language.Name)
            });

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Translator>> GetTranslator(Guid id)
        {
            var translator = await _context.Translators.Include(t => t.Tasks)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (translator == null)
            {
                return NotFound();
            }


            return Ok(translator);
        }

        [HttpGet("task/{taskId}")]
        public async Task<ActionResult<Translator>> GetTranslatorByTaskId(Guid taskId)
        {
            // Query to fetch the first translator associated with the given taskId, including their languages
            var translator = await _context.Translators
                .Include(t => t.Languages)  // Include the translator's languages
                    .ThenInclude(tl => tl.Language)  // Include the language details
                .Where(t => _context.TranslatorTasks.Any(tt => tt.TaskId == taskId && tt.TranslatorId == t.Id))
                .FirstOrDefaultAsync();

            if (translator == null)
            {
                return NotFound();
            }

            return Ok(translator);
        }

        [HttpPost]
        public async Task<ActionResult<Translator>> CreateTranslator(Translator translator)
        {
            if (translator == null)
            {
                return BadRequest("Translator is null.");
            }

            _context.Translators.Add(translator);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTranslator), new { id = translator.Id }, translator);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTranslator(Guid id, Translator updatedTranslator)
        {
            if (id != updatedTranslator.Id)
            {
                return BadRequest("Translator ID mismatch.");
            }

            var existingTranslator = await _context.Translators.FindAsync(id);
            if (existingTranslator == null)
            {
                return NotFound();
            }

            existingTranslator.FirstName = updatedTranslator.FirstName;
            existingTranslator.LastName = updatedTranslator.LastName;
            existingTranslator.Email = updatedTranslator.Email;
            existingTranslator.Languages = updatedTranslator.Languages;

            _context.Entry(existingTranslator).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TranslatorExists(id))
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTranslator(Guid id)
        {
            var translator = await _context.Translators.FindAsync(id);
            if (translator == null)
            {
                return NotFound();
            }

            _context.Translators.Remove(translator);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TranslatorExists(Guid id)
        {
            return _context.Translators.Any(e => e.Id == id);
        }
    }
}

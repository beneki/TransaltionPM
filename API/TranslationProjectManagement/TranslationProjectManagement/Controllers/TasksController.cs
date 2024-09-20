using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TranslationProjectManagement.Models.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TranslationProjectManagement.Data;
using Task = TranslationProjectManagement.Models.Domain.Task;
using Azure.Core;

namespace TranslationProjectManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Task>>> GetTasks(Guid? projectId)
        {
            // Start with the base query
            var query = _context.Tasks.AsQueryable();

            // Apply the filter if projectId is provided
            if (projectId.HasValue)
            {
                query = query.Where(t => t.ProjectId == projectId.Value);
            }

            var tasks = await query.ToListAsync();

            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Task>> GetTask(Guid id)
        {
            var task = await _context.Tasks
                .Include(t => t.TranslatorTasks)
                    .ThenInclude(tl => tl.Translator)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            var response = new
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Status = task.Status,
                Translator = task.TranslatorTasks.Select(tt => new
                    {
                        Id = tt.Translator.Id,
                        FirstName = tt.Translator.FirstName,
                        LastName = tt.Translator.LastName,
                        Email = tt.Translator.Email
                    }
                ).FirstOrDefault(),
            };

            return Ok(response);
        }

        public class CreateTaskRequest
        {
            public Task Task { get; set; }
            public Guid? TranslatorId { get; set; }
        }
        [HttpPost]
        public async Task<ActionResult<Task>> CreateTask(CreateTaskRequest request)
        {
            if (request?.Task == null)
            {
                return BadRequest("Task is null.");
            }

            // Add the task to the database
            var entry = _context.Tasks.Add(request.Task);
            await _context.SaveChangesAsync();
            var generatedTaskId = entry.Entity.Id;

            // Check if the translator exists
            if (request.TranslatorId != Guid.Empty)
            {
                var translator = await _context.Translators.FindAsync(request.TranslatorId);
                if (translator == null)
                {
                    return BadRequest("Translator not found.");
                }

                // Create a new TranslatorTask association
                var translatorTask = new TranslatorTask
                {
                    TranslatorId = translator.Id,
                    TaskId = request.Task.Id
                };

                _context.TranslatorTasks.Add(translatorTask);
                await _context.SaveChangesAsync();
            }


            return Ok(new { isSuccess= true, id = generatedTaskId, projectId = request.Task.ProjectId});
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(Guid id, Task updatedTask)
        {
            if (id != updatedTask.Id)
            {
                return BadRequest("Task ID mismatch.");
            }

            var existingTask = await _context.Tasks.FindAsync(id);
            if (existingTask == null)
            {
                return NotFound();
            }

            // Update the task properties
            existingTask.Title = updatedTask.Title;
            existingTask.Description = updatedTask.Description;
            existingTask.DueDate = updatedTask.DueDate;
            existingTask.Status = updatedTask.Status;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { isSuccess = true, task = existingTask });
        }

        [HttpPut("{taskId}/translator/{translatorId}")]
        public async Task<IActionResult> UpdateTaskTranslator(Guid taskId, Guid translatorId)
        {
            // Find the TranslatorTask by taskId
            var translatorTask = await _context.TranslatorTasks
                .FirstOrDefaultAsync(tt => tt.TaskId == taskId);

            if (translatorTask == null)
            {
                return NotFound();
            }

            // Remove the existing TranslatorTask
            _context.TranslatorTasks.Remove(translatorTask);
            await _context.SaveChangesAsync();

            // Create a new TranslatorTask with the updated translatorId
            var newTranslatorTask = new TranslatorTask
            {
                TaskId = taskId,
                TranslatorId = translatorId
            };

            _context.TranslatorTasks.Add(newTranslatorTask);
            await _context.SaveChangesAsync();

            // Find the translator by ID
            var translator = await _context.Translators
                .Where(t => t.Id == translatorId)
                .FirstOrDefaultAsync();

            if (translator == null)
            {
                return NotFound();
            }

            return Ok(new { isSuccess = true, translator });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            // Remove task and its relationships
            _context.TranslatorTasks.RemoveRange(_context.TranslatorTasks.Where(tt => tt.TaskId == id));
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/tasks/{taskId}/assignTranslator/{translatorId}
        [HttpPost("{taskId}/assignTranslator/{translatorId}")]
        public async Task<IActionResult> AssignTranslatorToTask(Guid taskId, Guid translatorId)
        {
            // Fetch the task and translator from the database
            var task = await _context.Tasks.FindAsync(taskId);
            var translator = await _context.Translators.FindAsync(translatorId);

            if (task == null || translator == null)
            {
                return NotFound("Task or Translator not found.");
            }

            // Check if the assignment already exists
            var existingAssignment = await _context.TranslatorTasks
                .AnyAsync(tt => tt.TaskId == taskId && tt.TranslatorId == translatorId);

            if (!existingAssignment)
            {
                // Create the new assignment
                var translatorTask = new TranslatorTask
                {
                    TaskId = taskId,
                    TranslatorId = translatorId
                };

                _context.TranslatorTasks.Add(translatorTask);
                await _context.SaveChangesAsync();
            }

            return Ok("Translator assigned to task successfully.");
        }

        private bool TaskExists(Guid id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
}

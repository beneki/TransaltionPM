using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TranslationProjectManagement.Models.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TranslationProjectManagement.Data;
using Task = TranslationProjectManagement.Models.Domain.Task;

namespace TranslationProjectManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        public class ProjectCountsDto
        {
            public int[] countsFirstYear { get; set; }
            public int[] countsSecondYear { get; set; }
        }
        [HttpGet("getProjectCountsByYears/{firstYear}/to/{secondYear}/status/{status}")]
        // Method to calculate project counts for each month
        public async Task<ProjectCountsDto> GetProjectCountsByYears(int firstYear, int secondYear, int status)
        {
            // Initialize the counts for each month (January to December)
            int[] counts2023 = new int[12];
            int[] counts2024 = new int[12];

            var projectStatus = (ProjectStatus)status;

            // Get counts for 2023
            var projectCounts2023 = await _context.Projects
                .Where(p => p.StartDate.Year == 2023 && p.Status == projectStatus)
                .GroupBy(p => p.StartDate.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToListAsync();

            // Fill counts for 2023
            foreach (var item in projectCounts2023)
            {
                counts2023[item.Month - 1] = item.Count; // Month is 1-based, adjust to 0-based index
            }

            // Get counts for 2024
            var projectCounts2024 = await _context.Projects
                .Where(p => p.StartDate.Year == 2024 && p.Status == projectStatus)
                .GroupBy(p => p.StartDate.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToListAsync();

            // Fill counts for 2024
            foreach (var item in projectCounts2024)
            {
                counts2024[item.Month - 1] = item.Count; // Month is 1-based, adjust to 0-based index
            }

            // Return the counts using the DTO
            return new ProjectCountsDto
            {
                countsFirstYear = counts2023,
                countsSecondYear = counts2024
            };
        }

        [HttpGet("getProjectCountsByYearsPerTranslator/{firstYear}/to/{secondYear}/status/{status}/translator/{translatorId}")]
        // Method to calculate project counts for each month, filtered by translator
        public async Task<ProjectCountsDto> GetProjectCountsByYearsPerTranslator(int firstYear, int secondYear, int status, Guid translatorId)
        {
            // Initialize the counts for each month (January to December)
            int[] countsFirstYear = new int[12];
            int[] countsSecondYear = new int[12];

            // Convert the status parameter to the ProjectStatus enum
            var projectStatus = (ProjectStatus)status;

            // Get counts for the first year (e.g., 2023), filtered by translatorId
            var projectCountsFirstYear = await _context.Projects
                .Where(p => p.StartDate.Year == firstYear && p.Status == projectStatus && p.Tasks.Any(t => t.TranslatorTasks.Any(tt => tt.TranslatorId == translatorId)))
                .GroupBy(p => p.StartDate.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToListAsync();

            // Fill counts for the first year
            foreach (var item in projectCountsFirstYear)
            {
                countsFirstYear[item.Month - 1] = item.Count; // Month is 1-based, adjust to 0-based index
            }

            // Get counts for the second year (e.g., 2024), filtered by translatorId
            var projectCountsSecondYear = await _context.Projects
                .Where(p => p.StartDate.Year == secondYear && p.Status == projectStatus && p.Tasks.Any(t => t.TranslatorTasks.Any(tt => tt.TranslatorId == translatorId)))
                .GroupBy(p => p.StartDate.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToListAsync();

            // Fill counts for the second year
            foreach (var item in projectCountsSecondYear)
            {
                countsSecondYear[item.Month - 1] = item.Count; // Month is 1-based, adjust to 0-based index
            }

            // Return the counts using the DTO
            return new ProjectCountsDto
            {
                countsFirstYear = countsFirstYear,
                countsSecondYear = countsSecondYear
            };
        }



        // GET: api/reports/translator/{translatorId}
        [HttpGet("translator/{translatorId}")]
        public async Task<ActionResult<IEnumerable<Task>>> GetTasksByTranslatorReport(Guid translatorId)
        {
            // Query to get tasks associated with a given translator through the join table
            var tasks = await _context.TranslatorTasks
                .Where(tt => tt.TranslatorId == translatorId)
                .Select(tt => tt.Task)  // Select tasks related to the translator
                .Include(t => t.Project)  // Include related project
                .ToListAsync();

            if (tasks == null || !tasks.Any())
            {
                return NotFound("No tasks found for the specified translator.");
            }

            return Ok(tasks);
        }

        // GET: api/reports/project/{projectId}
        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<IEnumerable<Task>>> GetTasksByProjectReport(Guid projectId)
        {
            // Fetch tasks by projectId and include related entities
            var tasks = await _context.Tasks
                .Where(t => t.ProjectId == projectId)
                .Include(t => t.Project) // Include the associated project
                .Include(t => t.TranslatorTasks) // Include the join table
                .ThenInclude(tt => tt.Translator) // Include the translator details
                .ToListAsync();

            // Check if any tasks were found
            if (tasks == null || !tasks.Any())
            {
                return NotFound("No tasks found for the specified project.");
            }

            return Ok(tasks);
        }
    }
}

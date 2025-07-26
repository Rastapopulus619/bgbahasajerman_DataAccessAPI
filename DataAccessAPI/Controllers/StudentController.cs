using Microsoft.AspNetCore.Mvc;
using bgbahasajerman_DataAccessLibrary.DataAccess;

namespace bgbahasajerman_DataAccessAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly QueryExecutor _queryExecutor;

        public StudentController(QueryExecutor queryExecutor)
        {
            _queryExecutor = queryExecutor;
        }

        [HttpGet("name")]
        public async Task<ActionResult<string>> GetFirstStudentName()
        {
            // Adjust the column name if your table uses a different one
            var name = await _queryExecutor.QueryFirstAsync<string>(
                "SELECT name FROM students LIMIT 1"
            );

            if (string.IsNullOrEmpty(name))
                return NotFound("No students found.");

            return Ok(name);
        }
    }
}
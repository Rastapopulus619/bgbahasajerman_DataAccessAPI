using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bgbahasajerman_BusinessLogic.Interfaces;          // IStudentService
using bgbahasajerman_DataAccessLibrary.Models;           // IListStudentModel (business model interfaces)
using DataAccessAPI.Models;

namespace DataAccessAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetAll()
        {
            var bizStudents = await _studentService.GetAllStudentsAsync();
            var dtos = bizStudents.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("{studentNumber:int}")]
        public async Task<ActionResult<StudentDto?>> GetByNumber(int studentNumber)
        {
            var biz = await _studentService.GetStudentByNumberAsync(studentNumber);
            if (biz == null) return NotFound();
            return Ok(MapToDto(biz));
        }

        private static StudentDto MapToDto(IListStudentModel m)
        {
            return new StudentDto
            {
                StudentID = m.StudentID,
                StudentNumber = m.StudentNumber,
                Name = m.Name ?? string.Empty,
                Title = m.Title
            };
        }
    }
}

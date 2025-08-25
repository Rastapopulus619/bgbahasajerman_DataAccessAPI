using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bgbahasajerman_BusinessLogic.Interfaces;
using bgbahasajerman_DataAccessLibrary.Repositories.Students;
using bgbahasajerman_DataAccessLibrary.Models;
using Microsoft.Extensions.Logging;

namespace bgbahasajerman_BusinessLogic.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _repo;
        private readonly ILogger<StudentService> _logger;
        private static readonly string[] AllowedTitles = new[] { "Herr", "Frau" };

        public StudentService(IStudentRepository repo)
        {
            _repo = repo;
        }

        public async Task<IReadOnlyList<IListStudentModel>> GetAllStudentsAsync()
        {
            var students = await _repo.GetAllStudentsAsync();
            var list = new List<IListStudentModel>();

            foreach (var s in students)
            {
                try
                {
                    list.Add(ValidateAndMapToListModel(s));
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Skipping invalid student {StudentID}", s.StudentID);
                }
            }

            return list;
        }

        public async Task<IListStudentModel?> GetStudentByNumberAsync(int studentNumber)
        {
            if (studentNumber <= 0) throw new ArgumentException("studentNumber must be positive", nameof(studentNumber));

            var student = await _repo.GetStudentByNumberAsync(studentNumber);
            return student == null ? null : ValidateAndMapToListModel(student);
        }

        private IListStudentModel ValidateAndMapToListModel(StudentModel m)
        {
            if (string.IsNullOrWhiteSpace(m.Name))
                throw new InvalidOperationException($"Student {m.StudentNumber} has empty Name");

            if (m.StudentNumber <= 0)
                throw new InvalidOperationException("Invalid StudentNumber");

            // Title can be either null or any string — both are acceptable.
            return new ListStudentModel
            {
                StudentID = m.StudentID,
                StudentNumber = m.StudentNumber,
                Name = m.Name,
                Title = m.Title   // passes through unchanged, may be null or set
            };
        }
    }
}

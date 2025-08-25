using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bgbahasajerman_BusinessLogic.Interfaces;
using bgbahasajerman_DataAccessLibrary.Repositories.Students;
using bgbahasajerman_DataAccessLibrary.Models;

namespace bgbahasajerman_BusinessLogic.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _repo;
        private static readonly string[] AllowedTitles = new[] { "Herr", "Frau" };

        public StudentService(IStudentRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<IListStudentModel>> GetAllStudentsAsync()
        {
            var students = await _repo.GetAllStudentsAsync();
            return students.Select(ValidateAndMapToListModel);
        }

        public async Task<IListStudentModel?> GetStudentByNumberAsync(int studentNumber)
        {
            if (studentNumber <= 0) throw new ArgumentException("studentNumber must be positive", nameof(studentNumber));

            var student = await _repo.GetStudentByNumberAsync(studentNumber);
            return student == null ? null : ValidateAndMapToListModel(student);
        }

        private IListStudentModel ValidateAndMapToListModel(StudentModel m)
        {
            if (string.IsNullOrWhiteSpace(m.Name)) throw new InvalidOperationException($"Student {m.StudentNumber} has empty Name");
            if (m.StudentNumber <= 0) throw new InvalidOperationException("Invalid StudentNumber");
            if (m.Title != null && !AllowedTitles.Contains(m.Title)) throw new InvalidOperationException($"Invalid Title: {m.Title}");

            return new ListStudentModel
            {
                StudentID = m.StudentID,
                StudentNumber = m.StudentNumber,
                Name = m.Name,
                Title = m.Title
            };
        }
    }
}

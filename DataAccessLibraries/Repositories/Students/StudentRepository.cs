using System.Collections.Generic;
using System.Threading.Tasks;
using bgbahasajerman_DataAccessLibrary.Models;
using bgbahasajerman_DataAccessLibrary.DataAccess;

namespace bgbahasajerman_DataAccessLibrary.Repositories.Students
{
    public class StudentRepository : IStudentRepository
    {
        private readonly QueryExecutor _queryExecutor;

        public StudentRepository(QueryExecutor queryExecutor)
        {
            _queryExecutor = queryExecutor;
        }

        public Task<IEnumerable<StudentModel>> GetAllStudentsAsync()
        {
            const string sql = "SELECT StudentID, StudentNumber, Name, Title FROM students;";
            return _queryExecutor.QueryAsync<StudentModel>(sql);
        }

        public Task<StudentModel?> GetStudentByNumberAsync(int studentNumber)
        {
            const string sql = "SELECT StudentID, StudentNumber, Name, Title FROM students WHERE StudentNumber = @StudentNumber;";
            return _queryExecutor.QueryFirstAsync<StudentModel>(sql, new { StudentNumber = studentNumber });
        }

        public Task<int> CreateStudentAsync(StudentModel model)
        {
            const string sql = "INSERT INTO students (StudentNumber, Name, Title) VALUES (@StudentNumber, @Name, @Title); SELECT LAST_INSERT_ID();";
            return _queryExecutor.ExecuteScalarAsync<int>(sql, model);
        }

        public Task UpdateStudentAsync(StudentModel model)
        {
            const string sql = "UPDATE students SET StudentNumber = @StudentNumber, Name = @Name, Title = @Title WHERE StudentID = @StudentID;";
            return _queryExecutor.ExecuteAsync(sql, model);
        }

        public Task DeleteStudentAsync(int studentId)
        {
            const string sql = "DELETE FROM students WHERE StudentID = @StudentID;";
            return _queryExecutor.ExecuteAsync(sql, new { StudentID = studentId });
        }
    }
}

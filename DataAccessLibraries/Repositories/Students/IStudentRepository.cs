using System.Collections.Generic;
using System.Threading.Tasks;
using bgbahasajerman_DataAccessLibrary.Models;

namespace bgbahasajerman_DataAccessLibrary.Repositories.Students
{
    public interface IStudentRepository
    {
        Task<IEnumerable<StudentModel>> GetAllStudentsAsync();
        Task<StudentModel?> GetStudentByNumberAsync(int studentNumber);
        Task<int> CreateStudentAsync(StudentModel model);
        Task UpdateStudentAsync(StudentModel model);
        Task DeleteStudentAsync(int studentId);
    }
}

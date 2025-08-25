using System.Collections.Generic;
using System.Threading.Tasks;
using bgbahasajerman_DataAccessLibrary.Models;

namespace bgbahasajerman_BusinessLogic.Interfaces
{
    public interface IStudentService
    {
        Task<IReadOnlyList<IListStudentModel>> GetAllStudentsAsync();
        Task<IListStudentModel?> GetStudentByNumberAsync(int studentNumber);
    }
}

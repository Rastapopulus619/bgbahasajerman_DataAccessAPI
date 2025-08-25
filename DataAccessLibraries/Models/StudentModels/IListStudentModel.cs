using System;

namespace bgbahasajerman_DataAccessLibrary.Models
{
    /// <summary>
    /// Interface representing a lightweight student listing model.
    /// Matches the public surface of ListStudentModel/StudentModel.
    /// </summary>
    public interface IListStudentModel
    {
        int StudentID { get; set; }
        int StudentNumber { get; set; }
        string Name { get; set; }
        string? Title { get; set; }
    }
}

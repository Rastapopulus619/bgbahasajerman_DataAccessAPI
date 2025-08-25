using System;

namespace bgbahasajerman_DataAccessLibrary.Models
{
    /// <summary>
    /// Represents a student record for list views.
    /// Implements IListStudentModel.
    /// </summary>
    public class ListStudentModel : IListStudentModel
    {
        public int StudentID { get; set; }
        public int StudentNumber { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Title { get; set; }
    }
}

using System;

namespace bgbahasajerman_DataAccessLibrary.Models
{
    /// <summary>
    /// Full student model implementing IStudent.
    /// </summary>
    public class StudentModel : IStudent
    {
        public int StudentID { get; set; }
        public int StudentNumber { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Title { get; set; }
    }
}

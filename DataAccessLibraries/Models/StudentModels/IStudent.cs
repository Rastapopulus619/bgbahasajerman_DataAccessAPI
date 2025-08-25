using System;

namespace bgbahasajerman_DataAccessLibrary.Models
{
    /// <summary>
    /// Interface representing the properties of a Student record.
    /// </summary>
    public interface IStudent
    {
        int StudentID { get; set; }
        int StudentNumber { get; set; }
        string Name { get; set; }
        string? Title { get; set; }
    }
}

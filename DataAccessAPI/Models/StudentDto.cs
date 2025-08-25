namespace DataAccessAPI.Models
{
    public class StudentDto
    {
        public int StudentID { get; set; }
        public int StudentNumber { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Title { get; set; }
    }
}
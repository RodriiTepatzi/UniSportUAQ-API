namespace UniSportUAQ_API.Data.DTO
{
    public class AttendanceDTO
    {
        public string? Id { get; set; }
        public string? StudentId { get; set; }
        public string? CourseId { get; set; }
        public DateTime Date { get; set; }
        public bool AttendanceClass { get; set; }
    }
}

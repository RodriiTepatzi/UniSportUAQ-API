namespace UniSportUAQ_API.Data.Schemas
{
    public class AttendanceSchema
    {
        public string? Id { get; set; }
        public string? StudentId { get; set; }
        public string? CourseId { get; set; }
        public DateTime Date { get; set; }
        public bool AttendanceClass { get; set; }


    }
}

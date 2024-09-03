namespace UniSportUAQ_API.Data.DTO
{
    public class CourseDTO
    {
        public string? Id { get; set; }
        public string? CourseName { get; set; }
        public string? InstructorId { get; set; }
        public string? Day { get; set; }
        public string? StartHour { get; set; }
        public string? EndHour { get; set; }
        public string? Description { get; set; }
        public string? CoursePictureUrl { get; set; }
        public int MaxUsers { get; set; }
    }
}

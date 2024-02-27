namespace UniSportUAQ_API.Data.Schemas
{
    public class CourseSchema
    {

        public string? Id { get; set; }
        public string? CourseName { get; set; }
        public string? InstructorId { get; set; }
		public string? Day { get; set; }
		public string? Hour { get; set; }
		public int MaxUsers { get; set; }
    }
}

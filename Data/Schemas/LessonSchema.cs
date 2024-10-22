namespace UniSportUAQ_API.Data.Schemas
{
	public class LessonSchema
	{
		public class CreateLessonModel
		{
			public string? CourseName { get; set; }
			public string? InstructorId { get; set; }
			public string? Description { get; set; }
			public string? SubjectId { get; set; }
			public string? Location { get; set; }
			public int MaxUsers { get; set; }
			public int MinAttendances { get; set; }
			public DateTime StartDate { get; set; }
			public DateTime EndDate { get; set; }
			public List<HorarioSchema>? Schedules { get; set; }
		}
	}
}

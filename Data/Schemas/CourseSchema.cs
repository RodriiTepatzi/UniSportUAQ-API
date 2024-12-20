﻿namespace UniSportUAQ_API.Data.Schemas
{
    public class CourseSchema
    {

        public string? Id { get; set; }
        public string? CourseName { get; set; }
        public string? InstructorId { get; set; }
        public List<HorarioSchema>? Schedules { get; set; }
        public DateTime StartDate {get; set;}
        public DateTime EndDate {get; set;}
		public string? Description { get; set; }
        public string? SubjectId { get; set; }
		public int MaxUsers { get; set; }
        public string? location { get; set; }

        public int MinAttendances { get; set; }


    }
}

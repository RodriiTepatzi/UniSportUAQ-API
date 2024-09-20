﻿using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data.DTO
{
    public class CourseDTO
    {
        public string? Id { get; set; }
        public string? CourseName { get; set; }
        public string? InstructorName { get; set; }
        public string? InstructorPicture { get; set; }
        public string? InstructorId { get; set; }
        public string? Day { get; set; }
        public int MaxUsers { get; set; }
        public int CurrentUsers { get; set; }
        public IEnumerable<Horario>? Horarios { get; set; }
        public string? StartHour { get; set; }
        public string? EndHour { get; set; }
        public string? Description { get; set; }
        public string? Link { get; set; }
        public string? Location { get; set; }
        public bool? IsVirtual { get; set; }
        public string? CoursePictureUrl { get; set; }

        
    }
}

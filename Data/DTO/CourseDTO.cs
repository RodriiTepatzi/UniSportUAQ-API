using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data.DTO
{
    public class CourseDTO
    {
        public string? Id { get; set; }
        public string? CourseName { get; set; }
        public string? InstructorName { get; set; }
        public string? InstructorPicture { get; set; }
        public string? InstructorId { get; set; }
        public int MaxUsers { get; set; }
        public int CurrentUsers { get; set; }
		public string? WorkshopId { get; set; }
		public int MinAttendances { get; set; }
        public List<HorarioDTO>? Schedules { get; set; }
        public string? Description { get; set; }
        public string? Link { get; set; }
        public string? Location { get; set; }
        public bool? IsVirtual { get; set; }
        public string? CoursePictureUrl { get; set; }
		public bool? IsActive { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        
    }
}

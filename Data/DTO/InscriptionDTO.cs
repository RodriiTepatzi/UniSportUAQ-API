namespace UniSportUAQ_API.Data.DTO
{
    public class InscriptionDTO
    {
        public string? Id { get; set; }
        public string? Expediente { get; set; }
        public string? CourseId { get; set; }
        public string? CourseName { get; set; }
        public string? CourseDescription { get; set; }
		public string? StudentName { get; set; }
		public string? StudentId { get; set; }
		public bool Accredit { get; set; }
    }
}

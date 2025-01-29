namespace UniSportUAQ_API.Data.DTO
{
	public class CartaVerificationDTO
	{
		public string? Id { get; set; }
		public string? VerificationCode { get; set; }
		public string? Url { get; set; }
		public UserDTO? Student { get; set; }
		public UserDTO? Instructor { get; set; }
		public CourseDTO? Course { get; set; }
	}
}

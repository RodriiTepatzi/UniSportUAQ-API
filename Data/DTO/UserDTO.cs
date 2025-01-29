namespace UniSportUAQ_API.Data.DTO
{
    public class UserDTO
    {
        public string? Id { get; set; }
        public string? Expediente { get; set; }
        public string? PictureUrl { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
		public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsStudent { get; set; }
        public bool IsInstructor { get; set; }


    }
}

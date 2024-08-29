namespace UniSportUAQ_API.Data.Schemas
{
    public class UserSchema
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public bool IsInFIF { get; set; }
        public int Semester { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsStudent { get; set; }
        public bool IsInstructor { get; set; }
        public string? PictureUrl { get; set; }

    }
}

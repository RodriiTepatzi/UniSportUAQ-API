namespace UniSportUAQ_API.Data.Schemas
{
	public class StudentSchema
	{
		public string? Id { get; set; }
		public string? Name { get; set; }
		public string? LastName { get; set; }
		public string? Email { get; set; }
		public string? PhoneNumber { get; set; }
		public string? Expediente { get; set; }
		public string? Password { get; set; }
		//
		public int Group {  get; set; }
		public int Semester { get; set; }
		public string? StudyPlan { get; set; }
		

    }
}

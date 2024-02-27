namespace UniSportUAQ_API.Data.Consts
{
	public class ResponseMessages
	{
		public const string OBJECT_NOT_FOUND = "Unable to locate the object in the database with the identifier you provided.";
		public const string UNAUTHORIZED = "You do not have access to this endpoint.";
		public const string BAD_REQUEST = "The data you provided was not properly provided. Please check our documentation.";
		public const string INTERNAL_ERROR = "Internal error. Please contact and provide us some details about how this was generated.";
		public const string ENTITY_EXISTS = "An entity with this email, id or expediente already exists. Please verify the data to avoid duplicates.";
        public const string STUDENT_ID_NEEDED = "Please provide at least the student id for this endpoint. It is required to complete the process.";
        public const string ALREADY_IN_COURSE = "This user is already in this course.";
		public const string NOT_FOUND_IN_COURSE = "This user was not found in this course.";
		public const string EXCEEDED_MAX_USERS = "This user cannot be enrolled due to max users already registered on this course.";
		public const string ERROR_REMOVING_USER_FROM_COURSE = "This user cannot be removed due to an internal error from this course.";
	}
}

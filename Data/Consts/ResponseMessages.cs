namespace UniSportUAQ_API.Data.Consts
{
	public class ResponseMessages
	{
		public const string OBJECT_NOT_FOUND = "Unable to locate the object in the database with the identifier you provided.";
		public const string UNAUTHORIZED = "You do not have access to this endpoint.";
		public const string BAD_REQUEST = "The data you provided was not properly provided. Please check our documentation.";
		public const string INTERNAL_ERROR = "Internal error. Please contact and provide us some details about how this was generated.";
		public const string ENTITY_EXISTS = "An entity with this email, id or expediente already exists. Please verify the data to avoid duplicates.";
        public const string ATTENDANCE_ENTITY_EXISTS = "An attendance has been taken already for this day. Please verify the data to avoid duplicates";
        public const string STUDENT_ID_NEEDED = "Please provide at least the student id for this endpoint. It is required to complete the process.";
        public const string ALREADY_IN_COURSE = "This user is already in this course.";
		public const string NOT_FOUND_IN_COURSE = "This user was not found in this course.";
		public const string EXCEEDED_MAX_USERS = "This user cannot be enrolled due to max users already registered on this course.";
		public const string ERROR_REMOVING_USER_FROM_COURSE = "This user cannot be removed due to an internal error from this course.";
		public const string ERROR_PROMOTING = "We can only promote a student to either Instructor or Admin.";
		public const string LIBERATION_LIMIT = "Student liberation letter limit(3) has reached";
		public const string INSTRUCTOR_HINDERED = "An instructor cannot be in further than one course at the same time. Please change Day or Start and End hour to avoid problems.";
		public const string END_INSCRIPTIONS_ERROR = "There is an error ending al inscriptions in this course";
		public const string COURSE_ENDED = "This course has already end";
		public const string NONE_INSCRIPTION_COURSE = "There is not found inscriptions with this course.";
        public const string BAD_COURSE_DAY = "You can not take asistance in days out of course days";
		public const string STREAM_ERROR = "Can not convert stream into bytes[]";
		public const string STUDENT_NOT_ACCREDITED = "Can not generate Carta liberacion this student is not accredited in this course";


    }
}

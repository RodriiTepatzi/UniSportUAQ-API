using UniSportUAQ_API.Data.Base;

namespace UniSportUAQ_API.Data.Consts
{
	public class ResponseErrors
	{
		public static ErrorBase AuthDuplicatedUser = new ErrorBase { Code = "AU001", Description = "User already exists" };
		public static ErrorBase AuthInvalidData = new ErrorBase { Code = "AU002", Description = "Invalid data" };
		public static ErrorBase AuthInvalidCredentials = new ErrorBase { Code = "AU003", Description = "Invalid credentials" };
		public static ErrorBase AuthErrorCreatingUser = new ErrorBase { Code = "AU004", Description = "Error creating user" };
		public static ErrorBase AuthUserEmailAlreadyExists = new ErrorBase { Code = "AU005", Description = "User email already registered" };
		public static ErrorBase AuthUserExpedienteAlreadyExists = new ErrorBase { Code = "AU006", Description = "User identification number already registered" };
		public static ErrorBase AuthInvalidToken = new ErrorBase { Code = "AU007", Description = "Invalid token" };
		public static ErrorBase AuthInvalidRefreshToken = new ErrorBase { Code = "AU008", Description = "Invalid refresh token" };
		public static ErrorBase AuthUserNotFound = new ErrorBase { Code = "AU009", Description = "User not found" };
		public static ErrorBase AuthErrorUpdatingUser = new ErrorBase { Code = "AU010", Description = "Error updating user" };
		public static ErrorBase AuthInvalidCurrentPassword = new ErrorBase { Code = "AU011", Description = "Invalid current password" };
		public static ErrorBase AuthErrorChangingPassword = new ErrorBase { Code = "AU012", Description = "Error changing password" };
		public static ErrorBase AuthRefreshTokenExpired = new ErrorBase { Code = "AU013", Description = "Refresh token expired" };

		//User
		public static ErrorBase UserNotAnStudent = new ErrorBase { Code = "USR001", Description = "User founded but is not an student" };
		public static ErrorBase UserNotAnInstructor = new ErrorBase { Code = "USR002", Description = "User founded but is not an instructor" };
		public static ErrorBase UserNotAnAdmin = new ErrorBase { Code = "USR003", Description = "User founded but is not an Admin" };
		//Entity
		public static ErrorBase EntityNotExist = new ErrorBase { Code = "ENT001", Description = "Entity/Object Does not exist" };
        //Files

        public static ErrorBase ConvertImageError = new ErrorBase { Code = "FIL001", Description = "Can not convert the data you provided into a image"};
        public static ErrorBase DeleteFileError = new ErrorBase { Code = "FIL002", Description = "Can not delete the data you provided OR not founded in our system" };
        public static ErrorBase FilterStartEndContradiction = new ErrorBase { Code = "FIL003", Description = "Incosnistent Start and End request" };
        public static ErrorBase FilterInvalidSearchTerm = new ErrorBase { Code = "FIL004", Description = "Invalid Search term" };
        //Attributes
        public static ErrorBase AttributeEmaiInvalidlFormat = new ErrorBase { Code = "ATTR001", Description = "Invalid Email format" };
		public static ErrorBase AttributeIdInvalidlFormat = new ErrorBase { Code = "ATTR002", Description = "Invalid Id format" };
		public static ErrorBase AttributeExpedienteInvalidlFormat = new ErrorBase { Code = "ATTR003", Description = "Invalid Expediente format" };
        public static ErrorBase AttributeIsInstructorFalse = new ErrorBase { Code = "ATTR004", Description = "this user Is not an instructor" };
        public static ErrorBase AttributeSchemaEmpty = new ErrorBase { Code = "ATTR005", Description = "schema Empty" };
        public static ErrorBase AttributeNameEmpty = new ErrorBase { Code = "ATTR006", Description = "Name Empty" };
        public static ErrorBase AttributeHorariosEmpty = new ErrorBase { Code = "ATTR007", Description = "horarios Empty" };
        public static ErrorBase AttributeEmptyOrNull = new ErrorBase { Code = "ATTR008", Description = "Invalid attribute format, empty or null" };

        //Filters
        
		//server
		public static ErrorBase ServerDataBaseError = new ErrorBase { Code = "DB001", Description = "Internal System Error" };
		//system
		public static ErrorBase SysErrorPromoting = new ErrorBase { Code = "SYS001", Description = "Only promote a student to either Instructor/Admin." };


		public static ErrorBase DataNotFound = new ErrorBase { Code = "ND001", Description = "No data was found." };


        public static ErrorBase SysErrorUserAlredyThisRole= new ErrorBase { Code = "SYS001", Description = "Only promote a student to either Instructor/Admin." };

        public static ErrorBase ServerDataBaseErrorUpdating = new ErrorBase { Code = "SYS002", Description = "Not possible for update/create this entity try again later" };
        //courses 
        public static ErrorBase CourseNotFound = new ErrorBase { Code = "COU000", Description = "This course does not exist" };
        public static ErrorBase CourseNoneInscription = new ErrorBase { Code = "COU001", Description = "This course does not contain inscriptions" };
        public static ErrorBase CourseNotFoundInscription = new ErrorBase { Code = "COU002", Description = "This course does not contain this inscription" };
        public static ErrorBase CourseErrorRemoving = new ErrorBase { Code = "COU003", Description = "Not possible to remove this inscription from this course" };
        public static ErrorBase CourseInstructorHindered = new ErrorBase { Code = "COU004", Description = "An instructor cannot be in further than one course at the same time. Please change Day or Start and End hour to avoid problems." };
        public static ErrorBase CourseCanNotEnd = new ErrorBase { Code = "COU005", Description = "Can not end this course, please provide info to solve this problem" };
        public static ErrorBase CourseExceedMaxUsers = new ErrorBase { Code = "COU006", Description = "This user cannot be enrolled due to max users already registered on this course." };
        public static ErrorBase CourseHorarioConfict= new ErrorBase { Code = "COU007", Description = "One or many horarios share day and the active hours are in conflict, try uncrossed hours" };
		public static ErrorBase CourseEndInscriptionProblem = new ErrorBase { Code = "COU008", Description = "Courses ended BUT Could not end inscriptions for the folowing users id" };
        public static ErrorBase CourseInscriptionAttendanceProblemUpdate = new ErrorBase { Code = "COU009", Description = "Can not update the following attendance(s) please provide this identifiers" };
        public static ErrorBase CourseHasEnded = new ErrorBase { Code = "COU010", Description = "This course has already ended" };
        public static ErrorBase CourseHasNotEnded = new ErrorBase { Code = "COU011", Description = "This course has NOT ended" };
        public static ErrorBase CourseStartOrEndateMinValue = new ErrorBase { Code = "COU012", Description = "The Start or endate are not in the correct format it is reciving in min value" };
        public static ErrorBase CourseStartEndateContradiction = new ErrorBase { Code = "COU013", Description = "The Start or endate are in contradiction verify if stardate < endate" };

        //inscriptions
        public static ErrorBase InscriptionNotAecredit= new ErrorBase { Code = "INS001", Description = "Not possible to accredit this course to this inscription, please check asistence quantity" };
        public static ErrorBase InscriptionNotEnded = new ErrorBase { Code = "INS002", Description = "Not possible to end this/these inscription(s) for this course, please check provide info for asistance" };
        public static ErrorBase InscriptionAlreadyExist = new ErrorBase { Code = "INS003", Description = "Not possible inscribe to this course, user already insripted" };
        public static ErrorBase InscriptionStudentAlredyInscripted = new ErrorBase { Code = "INS004", Description = "Not possible inscribe to more than one course, user already insripted in one course" };
        //user prefs
        public static ErrorBase UserPrefAlreadyExist = new ErrorBase { Code = "UPRF001", Description = "Not possible to create user preff becuase already exist" };
        //carta
        public static ErrorBase CartasErrorGenerating = new ErrorBase { Code = "CART001", Description = "Not possible to generate cartas for the following Expedientes, please provide us this expedienst and course Id" };
        public static ErrorBase CartasAlreadyExist= new ErrorBase { Code = "CART002", Description = "Not possible to generate cartas because there is alreadya a carta for this inscription" };

    }
}

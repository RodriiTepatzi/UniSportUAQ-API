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
        
        //User
        public static ErrorBase UserNotAnStudent = new ErrorBase { Code = "USR001", Description = "User founded but is not an student" };
        public static ErrorBase UserNotAnInstructor = new ErrorBase { Code = "USR002", Description = "User founded but is not an instructor" };
        public static ErrorBase UserNotAnAdmin = new ErrorBase { Code = "USR003", Description = "User founded but is not an Admin" };
        //Entity
        public static ErrorBase EntityNotExist = new ErrorBase { Code= "ENT001", Description = "Entity/Object Does not exist"};
		//Attributes
        public static ErrorBase AttributeEmaiInvalidlFormat = new ErrorBase { Code = "ATTR001", Description = "Invalid Email format" };
        public static ErrorBase AttributeIdInvalidlFormat = new ErrorBase { Code = "ATTR002", Description = "Invalid Id format" };
        public static ErrorBase AttributeExpedienteInvalidlFormat = new ErrorBase { Code = "ATTR003", Description = "Invalid Expediente format" };
		//Filters
		public static ErrorBase FilterStartEndContradiction = new ErrorBase { Code = "FIL001", Description="Incosnistent Start and End request" };
        public static ErrorBase FilterInvalidSearchTerm = new ErrorBase { Code = "FIL002", Description = "Invalid Search term" };
        //server
        public static ErrorBase ServerDataBaseError = new ErrorBase { Code = "DB001", Description = "Internal System Error" };
        //system
        public static ErrorBase SysErrorPromoting = new ErrorBase { Code = "SYS001", Description = "Only promote a student to either Instructor/Admin." };


    }
}

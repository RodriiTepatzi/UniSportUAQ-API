﻿using UniSportUAQ_API.Data.Base;

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

		//User
		public static ErrorBase UserNotAnStudent = new ErrorBase { Code = "USR001", Description = "User founded but is not an student" };
		public static ErrorBase UserNotAnInstructor = new ErrorBase { Code = "USR002", Description = "User founded but is not an instructor" };
		public static ErrorBase UserNotAnAdmin = new ErrorBase { Code = "USR003", Description = "User founded but is not an Admin" };
		//Entity
		public static ErrorBase EntityNotExist = new ErrorBase { Code = "ENT001", Description = "Entity/Object Does not exist" };
        
        
        
		//Attributes
		public static ErrorBase AttributeEmaiInvalidlFormat = new ErrorBase { Code = "ATTR001", Description = "Invalid Email format" };
		public static ErrorBase AttributeIdInvalidlFormat = new ErrorBase { Code = "ATTR002", Description = "Invalid Id format" };
		public static ErrorBase AttributeExpedienteInvalidlFormat = new ErrorBase { Code = "ATTR003", Description = "Invalid Expediente format" };
		//Filters
		public static ErrorBase FilterStartEndContradiction = new ErrorBase { Code = "FIL001", Description = "Incosnistent Start and End request" };
		public static ErrorBase FilterInvalidSearchTerm = new ErrorBase { Code = "FIL002", Description = "Invalid Search term" };
		//server
		public static ErrorBase ServerDataBaseError = new ErrorBase { Code = "DB001", Description = "Internal System Error" };
		//system
		public static ErrorBase SysErrorPromoting = new ErrorBase { Code = "SYS001", Description = "Only promote a student to either Instructor/Admin." };


		public static ErrorBase DataNotFound = new ErrorBase { Code = "ND001", Description = "No data was found." };

		public static ErrorBase AttributeEmptyOrNull = new ErrorBase { Code = "ATTR004", Description = "Invalid attribute format, emp   ty or null" };

        public static ErrorBase SysErrorUserAlredyThisRole= new ErrorBase { Code = "SYS001", Description = "Only promote a student to either Instructor/Admin." };

        public static ErrorBase ServerDataBaseErrorUpdating = new ErrorBase { Code = "SYS002", Description = "Not possible for update this entity" };
        //courses 
        public static ErrorBase CourseNoneInscription = new ErrorBase { Code = "COU001", Description = "This course does not contain inscriptions" };
        public static ErrorBase CourseNotFoundInscription = new ErrorBase { Code = "COU002", Description = "This course does not contain this inscription" };
        public static ErrorBase CourseErrorRemoving = new ErrorBase { Code = "COU003", Description = "Not possible to remove this inscription from this course" };
        public static ErrorBase CourseInstructorHindered = new ErrorBase { Code = "COU004", Description = "An instructor cannot be in further than one course at the same time. Please change Day or Start and End hour to avoid problems." };
        public static ErrorBase CourseCanNotEnd = new ErrorBase { Code = "COU005", Description = "Can not end this course, please provide info to solve this problem" };
        public static ErrorBase CourseExceedMaxUsers = new ErrorBase { Code = "COU006", Description = "This user cannot be enrolled due to max users already registered on this course." };


        //inscriptions
        public static ErrorBase InscriptionNotAecredit= new ErrorBase { Code = "INS001", Description = "Not possible to accredit this course to this inscription, please check asistence quantity" };
        public static ErrorBase InscriptionNotEnded = new ErrorBase { Code = "INS002", Description = "Not possible to end this/these inscription(s) for this course, please check provide info for asistance" };
        public static ErrorBase InscriptionAlreadyExist = new ErrorBase { Code = "INS003", Description = "Not possible inscribe to this course, user already insripted" };
        public static ErrorBase InscriptionStudentAlredyInscripted = new ErrorBase { Code = "INS004", Description = "Not possible inscribe to more than one course, user already insripted in one course" };

	}
}

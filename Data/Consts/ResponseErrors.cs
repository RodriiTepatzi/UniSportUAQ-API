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


	}
}

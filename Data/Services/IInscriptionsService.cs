using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data.Services
{
    public interface IInscriptionsService
    {
        /// <summary>
        /// Method to check if a user is in this course or not.
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="studentId"></param>
        /// <returns>It will return a true value in case the user provided is already in this course. Otherwise it will always return false.</returns>
        Task<bool> CheckInscriptionByCourseIdAndStudentIdAsync(string courseId, string studentId);
		Task<int> GetStudentCoursesCountAsync(string id);
		Task<List<Inscription>> GetInscriptionsByStudentAsync(string id);
		Task<List<Inscription>> GetFinishedInscriptionsByStudentAsync(string id);
		Task<bool> RemoveInscriptionByCourseIdAndStudentIdAsync(string courseId, string studentId);
		Task<Inscription> CreateInscriptionAsync(string courseId, string studentId);
    }
}

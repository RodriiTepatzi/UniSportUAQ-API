using Microsoft.EntityFrameworkCore;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;

namespace UniSportUAQ_API.Data.Services

{
    public class AttendancesService : IAttendancesService
    {

        private readonly AppDbContext _context;
        private readonly ICoursesService _coursesService;
        private readonly IStudentsService _studentsService;

        public AttendancesService(AppDbContext context, ICoursesService coursesService, IStudentsService studentsService)
        {
            _context = context;
            _coursesService = coursesService;
            _studentsService = studentsService;
        }

        public async Task<Attendance> CreateAttendanceAsync(AttendanceSchema attendanceSchema)
        {
            var attendance = new Attendance
            {
                Id = attendanceSchema.Id,
                StudentId = attendanceSchema.StudentId,
                CourseId = attendanceSchema.CourseId,
                Date = attendanceSchema.Date,
                AttendanceClass = attendanceSchema.AttendanceClass,
            };

            var entity = await _context.Attendances.AddAsync(attendance);
            var attendanceGen = entity.Entity;

            attendanceGen.Course = await _coursesService.GetCourseByIdAsync(attendanceSchema.CourseId!);
            attendanceGen.Student = await _studentsService.GetStudentByIdAsync(attendanceSchema.StudentId!);


            return attendanceGen;
        }



        public async Task<Attendance?> GetAttendanceByIdAsync(string id)
        {
            try
            {
                var result = await _context.Attendances.SingleAsync(
                i => i.Id == id);

                var entity = _context.Entry(result);



                if (entity.State == EntityState.Unchanged)
                {
                    return entity.Entity;
                }
                else
                {
                    return entity.Entity;
                }
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public async Task<Attendance?> GetAttendanceByStudentIdAsync(string studentId)
        {

            try
            {
                var result = await _context.Attendances.SingleAsync(
                i => i.StudentId == studentId);

                var entity = _context.Entry(result);


                if (entity.State == EntityState.Unchanged)
                {
                    return entity.Entity;
                }
                else
                {
                    return entity.Entity;
                }
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public async Task<Attendance?> GetAttendanceByCourseIdAsync(string courseId)
        {
            try
            {
                var result = await _context.Attendances.SingleAsync(
                i => i.CourseId == courseId);

                var entity = _context.Entry(result);



                if (entity.State == EntityState.Unchanged)
                {
                    return entity.Entity;
                }
                else
                {
                    return entity.Entity;
                }
            }
            catch (InvalidOperationException)
            {
                return null;
            }

        }

        public async Task<Attendance?> GetAttendanceByDateAsync(DateTime date)
        {
            DateTime fechaInicio = date.Date; // esto será a las 00:00 del día
            DateTime fechaFin = date.Date.AddHours(23).AddMinutes(59); // esto será a las 23:59 del día

            try
            {
                var result = await _context.Attendances.SingleAsync(
                att =>
                att.Date >= fechaInicio && att.Date <= fechaFin);

                var entity = _context.Entry(result);



                if (entity.State == EntityState.Unchanged)
                {
                    return entity.Entity;
                }
                else
                {
                    return entity.Entity;
                }
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public async Task<Attendance?> GetAttendancesAsync(string courseId, string studentId)
        {

            try
            {
                var result = await _context.Attendances.SingleAsync(

                    att => att.CourseId == courseId &&
                    att.StudentId == studentId
                    );

                var entity = _context.Entry(result);

                if (entity.State == EntityState.Unchanged)
                {
                    return entity.Entity;
                }
                else
                {
                    return entity.Entity;
                }
            }
            catch (InvalidOperationException)
            {
                return null;
            }


        }





    }
}

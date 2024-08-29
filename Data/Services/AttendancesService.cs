using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Base;

namespace UniSportUAQ_API.Data.Services

{
    public class AttendancesService : EntityBaseRepository<Attendance>, IAttendancesService
    {

        private readonly AppDbContext _context;

        public AttendancesService(AppDbContext context, ICoursesService coursesService) : base(context)
        {
			_context = context;
        }

        public async Task<Attendance?> CreateAttendanceAsync(Attendance attendance) {

            var CourseCheckDay = await _context.Courses.SingleOrDefaultAsync(
                    c => c.Id == attendance.CourseId
                );

            if (CourseCheckDay is null) return null;



            var entity = _context.Entry(attendance);

            var result = entity.Entity;

            entity.State = EntityState.Added;

            await _context.SaveChangesAsync();

            return result;
        }

    }
}

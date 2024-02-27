﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.IdentityModel.Tokens;
using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data.Services
{
	public class CoursesService : ICoursesService
	{
		private readonly AppDbContext _context;

		public CoursesService(AppDbContext context) {
			_context = context;
		}

		public async Task<Course> CreateCourseAsync(Course course) {

			var entity = _context.Entry(course);
			
			var result  = entity.Entity;
			entity.State = EntityState.Added;

			await _context.SaveChangesAsync();

			return result;
		}

		public async Task<Course?> GetCourseByIdAsync(string id) {

			try
			{
				var result = await _context.Courses
					.Include(i => i.Instructor)
					.SingleAsync(i => i.Id == id);

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

        public async Task<List<Course>> GetCoursesByIdInstructor(string instructorId) 
        {
			var result = await _context.Courses
				.Include(i => i.Instructor)
				.Where(i => i.InstructorId == instructorId)
				.ToListAsync();

            return result;
        }

        public async Task<Course> UpdateCourseAsync(Course course)
        {
            EntityEntry entityEntry = _context.Entry(course);

            entityEntry.State = EntityState.Modified;

            await _context.SaveChangesAsync();

			var newCourse = await GetCourseByIdAsync(course.Id!);

            return course;
        }

		public async Task<List<Course>> GetAllCoursesAsync()
		{
			var result = await _context.Courses.Where(
				c => c.IsActive == true
			).ToListAsync();

			return result;
		}

		public async Task<List<Course>> GetAllInactiveCoursesAsync()
		{
			var result = await _context.Courses.Where(
				c => c.IsActive == false
			).ToListAsync();

			return result;
		}

		public async Task<List<Course>> GetActivesCoursesByIdInstructor(string instructorId)
		{
			var result = await _context.Courses
				.Include(i => i.Instructor)
				.Where(i => i.InstructorId == instructorId && i.IsActive == true)
				.ToListAsync();

			return result;
		}

		public async Task<List<Course>> GetInactivesCoursesByIdInstructor(string instructorId)
		{
			var result = await _context.Courses
				.Include(i => i.Instructor)
				.Where(i => i.InstructorId == instructorId && i.IsActive == false)
				.ToListAsync();

			return result;
		}
	}
}

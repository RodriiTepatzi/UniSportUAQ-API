﻿using  UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data.Services
{
    public interface ICoursesService
    {

        Task<Course> CreateCourseAsync(Course course);

        Task<Course?> GetCourseByIdAsync(string id);

        Task<List<Course>> GetCourseByNameAsync(string name);

        Task<List<Course>> GetCourseByIdInstructor(string id_instructor);

        Task<Course> UpdateCourseAsync(Course course);

    }
}

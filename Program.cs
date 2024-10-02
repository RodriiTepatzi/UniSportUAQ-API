using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UniSportUAQ_API.Data;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Services;
using UniSportUAQ_API.Models;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UniSportUAQ_API.Controllers;

namespace UniSportUAQ_API
{
    public class Program
    {
        public static IConfiguration? Configuration { get; private set; }

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            Configuration = builder.Configuration;

            builder.Services.AddDbContext<AppDbContext>(
                options => options.UseSqlServer(
                    Configuration.GetConnectionString("DevelopmentConnectionString"),
                    providerOptions => providerOptions.EnableRetryOnFailure()
                ))
                .AddIdentityCore<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey
                    (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                };
            });

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            builder.Services.AddTransient<IStudentsService, StudentsService>();
            builder.Services.AddTransient<IInstructorsService, InstructorsService>();
            builder.Services.AddTransient<IAdminsService, AdminsService>();
            builder.Services.AddTransient<IUsersService, UsersService>();
            builder.Services.AddTransient<ICoursesService, CoursesService>();
            builder.Services.AddTransient<IInscriptionsService, InscriptionsService>();
            builder.Services.AddTransient<IAttendancesService, AttendancesService>();
            builder.Services.AddTransient<ICartasLiberacionService, CartasLiberacionService>();
            builder.Services.AddTransient<ISubjectsService, SubjectsService>();
            builder.Services.AddTransient<IUtilsService, UtilsService>();
            builder.Services.AddTransient<ITimePeriodsService, TimePeriodsService>();
            builder.Services.AddTransient<IHorariosService, HorariosService>();
            builder.Services.AddTransient<IHangfireJobsService, HangfireJobsService>();

            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSwaggerGen();

            //hangfire
            builder.Services.AddHangfire((sp, config) => {
                var connectionHangfire = sp.GetRequiredService<IConfiguration>().GetConnectionString("DevelopmentConnectionString");
                config.UseSqlServerStorage(connectionHangfire); 
            
            });
            builder.Services.AddHangfireServer();
            //
            

            //

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );

            app.UseHangfireDashboard("/hangfire");


            //DatabaseInitializer.FeedUsersAndRoles(app);
            //DatabaseInitializer.FeedDatabase(app);
            //DatabaseInitializer.FeedInscriptions(app);
            //DatabaseInitializer.FeedAttendances(app);

            app.Run();
        }
    }
}
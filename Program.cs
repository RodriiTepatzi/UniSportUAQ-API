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
using UniSportUAQ_API.Hubs;
using Hangfire.Dashboard;
using Microsoft.Extensions.Options;

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
					Configuration.GetConnectionString("DefaultConnectionString"),
					providerOptions => providerOptions.EnableRetryOnFailure()
				));

			builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowAll",
					builder =>
					{
						builder.WithOrigins("https://localhost:port", "https://deportetroyanos.azurewebsites.net")
							   .AllowAnyHeader()
							   .AllowAnyMethod()
							   .AllowCredentials();  // Importante para SignalR
					});
			});

			builder.Services.AddSignalR();

			builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
				.AddEntityFrameworkStores<AppDbContext>();

			builder.Services.AddMemoryCache();

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

            builder.Services.AddHangfire((sp, config) => {
                var connectionHangfire = sp.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnectionString");
                config.UseSqlServerStorage(connectionHangfire);
            });

            builder.Services.AddHangfireServer();




			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting(); // Setup routing

			app.UseCors("AllowAll");

			app.UseSwagger(); // Swagger documentation
			app.UseSwaggerUI();

			// JWT Authentication middleware
			app.UseAuthentication(); // Handle JWT tokens
			app.UseAuthorization();  // Authorization middleware

			// Map SignalR Hub after authentication is configured
			app.MapHub<LessonHub>("/hubs/lessons"); // SignalR

			// Map default route
			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			// Configure Hangfire dashboard
			app.UseHangfireDashboard("/hangfire", new DashboardOptions
			{
				Authorization = new IDashboardAuthorizationFilter[] { }
			});

			// DatabaseInitializer.FeedUsersAndRoles(app);
			// DatabaseInitializer.FeedDatabase(app);
			// DatabaseInitializer.FeedInscriptions(app);
			// DatabaseInitializer.FeedAttendances(app);

			app.Run();
		}
    }
}
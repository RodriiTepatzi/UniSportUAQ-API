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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UniSportUAQ_API.Controllers;
using UniSportUAQ_API.Hubs;
using Hangfire.Dashboard;
using Microsoft.Extensions.Options;
using Hangfire.MySql;
using System.Transactions;
using DotNetEnv;
using Hangfire.SqlServer;

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
            Env.Load();

            var mysqlConnectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING");
            var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
            var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
            var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");

            builder.Services.AddDbContext<AppDbContext>(
                options => options.UseSqlServer(
                    mysqlConnectionString!,
                    providerOptions => providerOptions.EnableRetryOnFailure()
                ));


            builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowAll",
					builder =>
					{
						builder.WithOrigins("https://localhost:port", "https://uaq.azurewebsites.net")
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
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey
                    (Encoding.UTF8.GetBytes(jwtKey!)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
				};
			});

			builder.Services.AddAuthorization(options =>
			{ 
				options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
				options.AddPolicy("RequireInstructorRole", policy => policy.RequireRole("Instructor"));
				options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User"));
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

            builder.Services.AddHangfire((sp, config) =>
            {
                config.UseStorage(new SqlServerStorage(mysqlConnectionString, new SqlServerStorageOptions
                {
                    //TransactionIsolationLevel = IsolationLevel.ReadCommitted,
                    QueuePollInterval = TimeSpan.FromSeconds(15),
                    JobExpirationCheckInterval = TimeSpan.FromHours(1),
                    CountersAggregateInterval = TimeSpan.FromMinutes(5),
                    PrepareSchemaIfNecessary = true,
                    DashboardJobListLimit = 50000,
                    TransactionTimeout = TimeSpan.FromMinutes(1),
                    //TablesPrefix = "Hangfire"
                }));
            });

            builder.Services.AddHangfireServer();

			var app = builder.Build();

			using (var scope = app.Services.CreateScope())
			{
				var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
				dbContext.Database.Migrate();
			}

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
			DatabaseInitializer.FeedRoles(app);

			app.Run();
		}
    }
}
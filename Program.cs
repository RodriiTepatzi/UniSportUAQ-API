using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UniSportUAQ_API.Data;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Services;
using UniSportUAQ_API.Models;

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

			builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnectionString")))
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
					ValidateLifetime = false, // Change to 'true' on production to avoid creating multiple tokens.
					ValidateIssuerSigningKey = true,
					ClockSkew = TimeSpan.Zero,
				};
			});

			builder.Services.AddTransient<IStudentsService, StudentsService>();
			builder.Services.AddTransient<IInstructorsService, InstructorsService>();
            builder.Services.AddTransient<IAdminsService, AdminsService>();
			builder.Services.AddTransient<IUsersService, UsersService>();

			builder.Services.AddControllers();
			builder.Services.AddHttpContextAccessor();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

            app.MapPost("/security/createToken",
            [AllowAnonymous]
            async
            (JWTRequest user) =>
            {
                if (!string.IsNullOrEmpty(user.Username) && !string.IsNullOrEmpty(user.Password))
                {
                    using (var serviceScope = app.Services.CreateScope())
                    {
                        var userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                        var applicationUser = await userManager!.FindByNameAsync(user.Username);
                        var id = await userManager.GetUserIdAsync(applicationUser);
                        if (applicationUser != null)
                        {
                            var checkPwd = await userManager.CheckPasswordAsync(applicationUser, user.Password);
                            if (!checkPwd)
                            {
                                return Results.Unauthorized();
                            }
                        }
                        else
                        {
                            return Results.BadRequest("No user with that data exists.");
                        }
                    }
                }
                else
                {
                    return Results.BadRequest("All fields are needed.");
                }

                var issuer = builder.Configuration["Jwt:Issuer"];
                var audience = builder.Configuration["Jwt:Audience"];
                var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]!);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim("Id", Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                        new Claim(JwtRegisteredClaimNames.Email, user.Username),
                        new Claim(JwtRegisteredClaimNames.Jti,
                        Guid.NewGuid().ToString())
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(15),
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = new SigningCredentials
                    (new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = tokenHandler.WriteToken(token);
                var stringToken = tokenHandler.WriteToken(token);
                return Results.Ok(stringToken);
            });

            app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			DatabaseInitializer.FeedUsersAndRoles(app);
			DatabaseInitializer.FeedDatabase(app);

			app.Run();
		}
	}
}

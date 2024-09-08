using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UniSportUAQ_API.Data.Base;

namespace UniSportUAQ_API.Data.Models
{
#pragma warning disable CS8767 // La nulabilidad de los tipos de referencia del tipo de parámetro no coincide con el miembro implementado de forma implícita (posiblemente debido a los atributos de nulabilidad).
    public class ApplicationUser : IdentityUser, IEntityBase
#pragma warning restore CS8767 // La nulabilidad de los tipos de referencia del tipo de parámetro no coincide con el miembro implementado de forma implícita (posiblemente debido a los atributos de nulabilidad).
    {
        public string? Expediente { get; set; }
		public string? PictureUrl { get; set; }
		public string? Name { get; set; }
		public string? LastName { get; set; }
		
		public int Group { get; set; }
		public int Semester { get; set; }

		public bool IsStudent { get; set; }
		public bool IsInstructor { get; set; }
		public bool IsAdmin { get; set; }

		[NotMapped]
		public string FullName {

			get { return $"{Name} {LastName}"; }
		}


		[DefaultValue("False")]
		public bool IsInFIF { get; set; }
		public bool IsActive { get; set; }

		[StringLength(10)]
		public string? StudyPlan { get; set; }
		public DateTime RegisteredDateTime { get; set; }


		// Authentication

		public string? RefreshToken { get; set; }
		public DateTime? RefreshTokenExpiryTime { get; set; }

		public Dictionary<string, object> ToDictionary => new Dictionary<string, object>
		{
			{ nameof(Id), Id },
			{ nameof(Name), Name is not null ? Name : "" },
			{ nameof(LastName), LastName is not null ? LastName : "" },
			{ nameof(FullName), FullName is not null ? FullName: ""},
			{ nameof(PhoneNumber), PhoneNumber is not null ? PhoneNumber : "" },
			{ nameof(Email), Email is not null ? Email : "" },
			{ nameof(UserName), UserName is not null ? UserName : "" },
			{ nameof(Expediente), Expediente is not null ? Expediente : "" },
			{ nameof(IsInFIF), IsInFIF },
			{ nameof(Semester), Semester },
			{ nameof(IsActive), IsActive },
			{ nameof(IsAdmin), IsAdmin },
			{ nameof(IsStudent), IsStudent },
			{ nameof(IsInstructor), IsInstructor },
			{ nameof(RegisteredDateTime), RegisteredDateTime },
			{ nameof(PictureUrl), PictureUrl is not null ? PictureUrl : "" },
		};
	}
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using UniSportUAQ_API.Data.Base;

namespace UniSportUAQ_API.Data.Models
{

    public class TimePeriod : IEntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }

        [Required]
        public DateTime DateStart { get; set; }

        [Required]
        public DateTime DateEnd { get; set; }

		[Required]
		public bool IsActive { get; set; }
    }


}

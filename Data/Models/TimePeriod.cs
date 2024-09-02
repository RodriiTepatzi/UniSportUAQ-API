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
        public string? Period { get; set; }

        [Required]
        public string? Type { get; set; }

        [Required]
        public DateTime DateStart { get; set; }

        [Required]
        public DateTime DateEnd { get; set; }

        public Dictionary<string, object> Dictionary => new Dictionary<string, object>{


                    { nameof(Id), Id ?? string.Empty },
                    { nameof(Period), Period is not null ? Period : "" },
                    { nameof(Type), Type is not null ? Type : "" },
                    { nameof(DateStart),DateStart },
                    { nameof(DateEnd),DateEnd },



        };
    }


}

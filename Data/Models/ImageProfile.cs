using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using UniSportUAQ_API.Data.Base;

namespace UniSportUAQ_API.Data.Models
{
    public class ImageProfile
    {
        [Required]
        public byte[] Image { get; set; }

        public string? FileFormat { get; set; }

    }
}
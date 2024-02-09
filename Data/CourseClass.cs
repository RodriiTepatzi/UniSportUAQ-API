﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UniSportUAQ_API.Data
{
    public class CourseClass
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string? Day { get; set; }

        [Required]
        public string? Hour { get; set; }

        [Required]
        public int Quota { get; set; }

        [Required]
        public int Id_Course;

        [ForeignKey("Id_Course")]
        public virtual Course Course { get; set; }



    }
}

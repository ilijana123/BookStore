using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MVCBookk.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        [Required]
        [StringLength(450)]
        [Display(Name = "App User")]
        public string AppUser { get; set; }
        [Required]
        [StringLength(500)]
        public string Comment { get; set; }
        public int? Rating { get; set; }
        public Book? Book { get; set; }
    }
}

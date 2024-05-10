using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCBookk.Models
{
    public class UserBooks
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        [Required]
        [StringLength(450)]
        [Display(Name = "App User")]
        public string AppUser { get; set; }
        public Book? Book { get; set; }

    }
}

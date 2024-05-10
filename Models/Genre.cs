using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCBookk.Models
{
    public class Genre
    {
        public int Id { get; set; }
        [Display(Name = "Genre Name")]
        [StringLength(50)]
        [Required]
        public string GenreName { get; set; }
        public ICollection<BookGenre>? Books{ get; set;}
    }
}

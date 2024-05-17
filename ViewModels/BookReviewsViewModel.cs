
using Microsoft.AspNetCore.Mvc.Rendering;
using MVCBookk.Models;
using System.Collections.Generic;

namespace MVCBookk.ViewModels
{
    public class BookReviewsViewModel
    {
        public IList<Review>? Reviews{ get; set; }
        public Book Book { get; set; }
        public UserBooks UserBook { get; set; }
        public int BookId { get; set; } // Add BookId property
        public string Comment { get; set; } // Add Comment property
        public int Rating { get; set; } // Add Rating property
        public string AppUser { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using MVCBookk.Models;
using System.Collections.Generic;

namespace MVCBookk.ViewModels
{
    public class BookGenreViewModel
    {
        public IList<Book> Books { get; set; }
        public SelectList Genres { get; set; }
        public List<Author> Authors { get; set; } 
        public string BookGenre { get; set; }
        public string BookAuthor { get; set; }
        public string SearchString { get; set; }
    }
}
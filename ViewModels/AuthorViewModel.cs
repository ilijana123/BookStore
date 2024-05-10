using MVCBookk.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVCBookk.ViewModels
{
    public class AuthorViewModel
    {
        public Author Author { get; set; }
        public List<int>? SelectedBooks { get; set; }
        public SelectList? BookOptions { get; set; }
    }
}

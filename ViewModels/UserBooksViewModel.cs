
using Microsoft.AspNetCore.Mvc.Rendering;
using MVCBookk.Models;
using System.Collections.Generic;

namespace MVCBookk.ViewModels
{
    public class UserBooksViewModel
    {
        public UserBooks? UserBook { get; set; }
        public IList<Book> Books { get; set; }

    }
}
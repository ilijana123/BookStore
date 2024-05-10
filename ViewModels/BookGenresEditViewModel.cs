using Microsoft.AspNetCore.Mvc.Rendering;
using MVCBookk.Models;
using System.Collections.Generic;
namespace MVCBookk.ViewModels
{
	public class BookGenresEditViewModel
	{
		public Book Book { get; set; }
		public IEnumerable<int>? SelectedGenres { get; set; }
        public IEnumerable<SelectListItem> GenreList { get; set; }

    }
}
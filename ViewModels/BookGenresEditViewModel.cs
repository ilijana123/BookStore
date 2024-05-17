using Microsoft.AspNetCore.Mvc.Rendering;
using MVCBookk.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace MVCBookk.ViewModels
{
	public class BookGenresEditViewModel
	{
		public Book Book { get; set; }
		public IEnumerable<int>? SelectedGenres { get; set; }
		public IEnumerable<SelectListItem>? GenreList { get; set; }
		//[Required(ErrorMessage="Select File")]
		public IFormFile? File { get; set; }
        [StringLength(int.MaxValue)]
        [Display(Name = "Upload your book in pdf format")]
        public string? DownloadUrl { get; set; }
		public IFormFile? Download { get; set; }

	}
}
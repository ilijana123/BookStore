using MVCBookk.Models;
using System.ComponentModel.DataAnnotations;

namespace MVCBookk.ViewModels
{
	public class EditBookViewModel
	{
		public int Id { get; set; }
		[Required]
		public string Title { get; set; }
		public int? YearPublished { get; set; }
		public int? NumPages { get; set; }
		public string? Description { get; set; }
		public string? Publisher { get; set; }
		public IFormFile? FrontPage { get; set; }
		public string? FrontPageURL { get; set; }
		public string? DownloadUrl { get; set; }
		public IEnumerable<Author>? Authors { get; set; }
		public int AuthorId { get; set; }
		public IEnumerable<Genre>? Genres { get; set; }
		public IEnumerable<int> GenreIds { get; set; }
        public IFormFile? Download { get; set; }
    }
}
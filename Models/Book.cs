using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MVCBookk.Models
{
    public class Book
    {
        public int Id { get; set; }
        [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$")]
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        [Display(Name = "Year Published")]
        public int? YearPublished { get; set; }
        public int? NumPages { get; set; }
        [StringLength(int.MaxValue)]
        public string? Description { get; set; }
        [StringLength(50)]
        public string? Publisher { get; set; }
        [StringLength(int.MaxValue)]
        [Display(Name = "Front Page")]

        public string? FrontPage { get; set; }
        [StringLength(int.MaxValue)]
        [Display(Name = "Download Url")]
        public  string? DownloadUrl { get; set; }
        [Display(Name = "Average Rating")]
        public double? AverageRating
        {
            get
            {
                if (Reviews != null && Reviews.Any() && Reviews.All(r => r.Rating != null))
                {
                    double totalRating = Reviews.Sum(r => r.Rating.Value);
                    double averageRating = totalRating / Reviews.Count;
                    return Math.Round(averageRating, 1);
                }

                return null;
            }
        }
        public Book()
        {
            Genres = new List<BookGenre>();
        }
        public int AuthorId { get; set; }
        public Author? Author { get; set; }
        public ICollection<BookGenre> Genres { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<UserBooks>? UserBooks { get; set; }

    }
}

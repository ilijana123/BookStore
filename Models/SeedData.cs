using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MVCBookk.Data;
using MVCBookk.Models;
using System;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MVCBookk.Areas.Identity.Data;
namespace MVCBookk.Models
{
    public class SeedData
    {
        public static async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<MVCBookkUser>>();
            IdentityResult roleResult;
            //Add Admin Role
            var roleCheck = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin")); }
            MVCBookkUser user = await UserManager.FindByEmailAsync("admin@mvcbook.com");
            if (user == null)
            {
                var User = new MVCBookkUser();
                User.Email = "admin@mvcbook.com";
                User.UserName = "admin@mvcbook.com";
                string userPWD = "Admin123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Admin
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Admin"); }
            }
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MVCBookkContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<MVCBookkContext>>()))
            {
                CreateUserRoles(serviceProvider).Wait();
                if (context.Book.Any() || context.Author.Any() || context.Genre.Any() || context.Review.Any() || context.UserBooks.Any())
                {
                    return;   // DB has been seeded
                }
                
                if (!context.Author.Any(a => a.FirstName == "John" && a.LastName == "Green"))
                {
                    context.Author.AddRange(
                        new Author { /*Id = 1,*/ FirstName = "John", LastName = "Green", BirthDate = DateTime.Parse("1977-8-24"), Nationality = "American", Gender = "Male" },
                        new Author { /*Id = 2, */ FirstName = "Paulo", LastName = "Coelho", BirthDate = DateTime.Parse("1947-8-24"), Nationality = "Brazilian", Gender = "Male" },
                        new Author { /*Id = 3, */ FirstName = "Jane", LastName = "Austen", BirthDate = DateTime.Parse("1775-12-16"), Nationality = "British", Gender = "Female" },
                        new Author { /*Id = 4, */ FirstName = "Khaled", LastName = "Hosseini", BirthDate = DateTime.Parse("1965-3-4"), Nationality = "Afghan-American", Gender = "Male" },
                        new Author { /*Id = 5, */ FirstName = "Bram", LastName = "Stoker", BirthDate = DateTime.Parse("1847-11-8"), Nationality = "Irish", Gender = "Male" },
                        new Author { /*Id = 6, */ FirstName = "Mary", LastName = "Shelley", BirthDate = DateTime.Parse("1797-8-30"), Nationality = "British", Gender = "Female" }
                    );
                }
                context.SaveChanges();
                context.Book.AddRange(
                    new Book
                    {
                        //Id = 1,
                        Title = "The Fault in Our Stars",
                        YearPublished = 2012,
                        NumPages = 313,
                        Description = "The fault, dear Brutus, is not in our stars, but in ourselves.",
                        Publisher = "Dutton Books",
                        FrontPage = "https://upload.wikimedia.org/wikipedia/en/4/41/The_Fault_in_Our_Stars_%28Official_Film_Poster%29.png",
                        DownloadUrl = "https://www.juhsd.net/site/handlers/filedownload.ashx?moduleinstanceid=4480&dataid=7745&FileName=The-Fault-in-Our-Stars.pdf",
                        AuthorId = context.Author.Single(a => a.FirstName == "John" && a.LastName == "Green").Id
                    },
                    new Book
                    {
                        //Id = 2,
                        Title = "Paper Towns",
                        YearPublished = 2008,
                        NumPages = 305,
                        Description = "It's a story about friendship, self-discovery, and the complexities of human connection.",
                        Publisher = "Dutton Books",
                        FrontPage = "https://resizing.flixster.com/-XZAfHZM39UwaGJIFWKAE8fS0ak=/v3/t/assets/p11346721_p_v8_ab.jpg",
                        DownloadUrl = "https://home.iitk.ac.in/~ajayraj/Books/Paper_Towns.pdf",
                        AuthorId = context.Author.Single(a => a.FirstName == "John" && a.LastName == "Green").Id
                    },
                     new Book
                     {
                         //Id = 3,
                         Title = "The Alchemist",
                         YearPublished = 1988,
                         NumPages = 163,
                         Description = "he Alchemist follows Andalusian shepherd boy Santiago as he chases his recurring dream of treasure near the Egyptian pyramids, on the way to which he meets mentors, falls in love, and, most importantly, discovers the meaning of life.",
                         Publisher = "HarperCollins Publishers",
                         FrontPage = "https://m.media-amazon.com/images/I/61HAE8zahLL._AC_UF1000,1000_QL80_DpWeblab_.jpg",
                         DownloadUrl = "https://archive.org/details/thealchemist_201908",
                         AuthorId = context.Author.Single(a => a.FirstName == "Paulo" && a.LastName == "Coelho").Id
                     },
                    new Book
                    {
                        //Id = 4,
                        Title = "The Zahir",
                        YearPublished = 2005,
                        NumPages = 336,
                        Description = "The Zahir means 'the obvious' or 'conspicuous' in Arabic. The story revolves around the life of the narrator, a bestselling novelist, and in particular his search for his missing wife, Esther. He enjoys all the privileges that money and celebrity bring.",
                        Publisher = "Harper Perennial",
                        FrontPage = "https://i.gr-assets.com/images/S/compressed.photo.goodreads.com/books/1493044059l/1427.jpg",
                        DownloadUrl = "https://onlynovels.wordpress.com/wp-content/uploads/2010/08/coelho-paulo-the-zahir.pdf",
                        AuthorId = context.Author.Single(a => a.FirstName == "Paulo" && a.LastName == "Coelho").Id
                    },
                    new Book
                    {
                        // Id = 5,
                        Title = "Pride and Prejudice",
                        YearPublished = 1813,
                        NumPages = 254,
                        Description = "Pride and Prejudice follows the turbulent relationship between Elizabeth Bennet, the daughter of a country gentleman, and Fitzwilliam Darcy, a rich aristocratic landowner.",
                        Publisher = "Thomas Egerton",
                        FrontPage = "https://cdn2.penguin.com.au/covers/original/9780141949055.jpg",
                        DownloadUrl = "https://archive.org/details/prideprejudiceno00austuoft",
                        AuthorId = context.Author.Single(a => a.FirstName == "Jane" && a.LastName == "Austen").Id
                    },
                    new Book
                    {
                        //Id = 6,
                        Title = "The Kite Runner",
                        YearPublished = 2003,
                        NumPages = 372,
                        Description = "The Kite Runner is the story of Amir, a Sunni Muslim, who struggles to find his place in the world because of the aftereffects and fallout from a series of traumatic childhood events.",
                        Publisher = "BLOOMSBURY",
                        FrontPage = "https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1579036753i/77203.jpg",
                        DownloadUrl = "https://ia803000.us.archive.org/27/items/TheKiteRunnerPDF_201905/The-Kite-Runner-PDF.pdf",
                        AuthorId = context.Author.Single(a => a.FirstName == "Khaled" && a.LastName == "Hosseini").Id
                    },
                    new Book
                    {
                        //Id = 7,
                        Title = "Dracula",
                        YearPublished = 1897,
                        NumPages = 448,
                        Description = "The story of Dracula involves a vampire and his attempts to move to London and begin feeding on the local population. A coalition of protagonists serve as vampire hunters and attempt to stop and destroy Count Dracula.",
                        Publisher = "Simon & Schuster",
                        FrontPage = "https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1387151694i/17245.jpg",
                        DownloadUrl = "https://archive.org/details/draculabr00stokuoft/page/n5/mode/2up",
                        AuthorId = context.Author.Single(a => a.FirstName == "Bram" && a.LastName == "Stoker").Id
                    },
                    new Book
                    {
                        //Id = 8,
                        Title = "Frankenstein",
                        YearPublished = 1818,
                        NumPages = 260,
                        Description = "\"Frankenstein\" by Mary Shelley is a classic gothic novel that explores the dangers of ambition and the consequences of playing god. It tells the story of a young scientist who creates a monster that ultimately destroys his life and the lives of those around him.",
                        Publisher = "Lackington, Hughes, Harding, Mavor & Jones",
                        FrontPage = "https://m.media-amazon.com/images/I/81EU992zdwL._AC_UF1000,1000_QL80_.jpg",
                        DownloadUrl = "https://archive.org/details/mary-shelley_frankenstein",
                        AuthorId = context.Author.Single(a => a.FirstName == "Mary" && a.LastName == "Shelley").Id
                    }

                );
                context.SaveChanges();
                context.Genre.AddRange(
                  new Genre { /*Id = 1,*/ GenreName = "Realistic fiction" },
                  new Genre { /*Id = 2,*/ GenreName = "Mystery" },
                  new Genre { /*Id = 3,*/ GenreName = "Novel" },
                  new Genre { /*Id = 4,*/ GenreName = "Fiction" },
                  new Genre { /*Id = 5, */GenreName = "Romance" },
                  new Genre { /*Id = 6, */GenreName = "History" },
                   new Genre { /*Id = 7, */GenreName = "Horror" },
                  new Genre { /*Id = 8, */GenreName = "Gothic" },
                  new Genre { /*Id = 9, */GenreName = "Science fiction" }
              );
                context.SaveChanges();
                context.BookGenre.AddRange(
                    new BookGenre { BookId = 1, GenreId = 1 },
                    new BookGenre { BookId = 1, GenreId = 3 },
                    new BookGenre { BookId = 2, GenreId = 2 },
                    new BookGenre { BookId = 2, GenreId = 3 },
                    new BookGenre { BookId = 3, GenreId = 4 },
                    new BookGenre { BookId = 4, GenreId = 3 },
                    new BookGenre { BookId = 5, GenreId = 3 },
                    new BookGenre { BookId = 5, GenreId = 5 },
                    new BookGenre { BookId = 6, GenreId = 4 },
                    new BookGenre { BookId = 6, GenreId = 6 },
                    new BookGenre { BookId = 7, GenreId = 8 },
                    new BookGenre { BookId = 7, GenreId = 8 }

                );
                context.SaveChanges();
                context.UserBooks.AddRange(
               new UserBooks
               {
                   BookId = 1,
                   AppUser = "User1"
               },
               new UserBooks
               {
                   BookId = 1,
                   AppUser = "User2"
               },
                new UserBooks
                {
                    BookId = 2,
                    AppUser = "User3"
                },
               new UserBooks
               {
                   BookId = 3,
                   AppUser = "User4"
               },
                new UserBooks
                {
                    BookId = 4,
                    AppUser = "User5"
                },
               new UserBooks
               {
                   BookId = 5,
                   AppUser = "User6"
               }

            );
                context.SaveChanges();
                context.Review.AddRange(
                new Review
                {
                    BookId = 1,
                    AppUser = "User1",
                    Comment = "Good book!",
                    Rating = 5
                },
                new Review
                {
                    BookId = 2,
                    AppUser = "User2",
                    Comment = "Interesting read.",
                    Rating = 4
                },
                new Review
                {
                    BookId = 2,
                    AppUser = "User1",
                    Comment = "Great book!",
                    Rating = 10
                },
                new Review
                {
                    BookId = 3,
                    AppUser = "User5",
                    Comment = "Interesting read.",
                    Rating = 7
                }, new Review
                {
                    BookId = 3,
                    AppUser = "User2",
                    Comment = "Okay!",
                    Rating = 5
                },
                new Review
                {
                    BookId = 3,
                    AppUser = "User2",
                    Comment = "Boring.",
                    Rating = 4
                }
            );
                context.SaveChanges();
            }
        }
    }
}


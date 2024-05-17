using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using MVCBookk.Data;
using MVCBookk.Models;
using MVCBookk.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using Microsoft.AspNetCore.Http;
using MVCBookk.Interfaces;
using Microsoft.AspNetCore.Hosting;
namespace MVCBookk.Controllers
{
    public class BooksController : Controller
    {
        private readonly MVCBookkContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IPhotoService _photoService;

        public BooksController(MVCBookkContext context, IWebHostEnvironment webHostEnvironment, IPhotoService photoService)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _photoService = photoService;
        }

        [HttpPost]
        public IActionResult Index(IFormFile? File)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                if (File != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(File.FileName);
                    string fileDir = Path.Combine(wwwRootPath, @"images");

                    if (!System.IO.Directory.Exists(fileDir))
                        System.IO.Directory.CreateDirectory(fileDir);

                    using (var fileStream = new FileStream(Path.Combine(fileDir, fileName), FileMode.Create))
                    {
                        File.CopyTo(fileStream);
                    }

                    string fileRelativeUrl = @"/images/" + fileName;
                    ViewData["ImageUrl"] = fileRelativeUrl;
                }
            }
            return View();
        }
        public async Task<IActionResult> Index(string searchString, string bookGenre, string bookAuthor)
        {
            var books = _context.Book
            .Include(b => b.Author)
            .Include(b => b.Genres)
            .Include(b => b.Reviews) 
            .AsEnumerable();

            if (!string.IsNullOrEmpty(searchString))
            {
                books = books.Where(b => b.Title.ToLower().Contains(searchString.ToLower()));
            }

            if (!string.IsNullOrEmpty(bookGenre))
            {
                books = books.Where(b => b.Genres.Any(bg => bg.Genre.GenreName == bookGenre));
            }

            if (!string.IsNullOrEmpty(bookAuthor))
            {
                books = books.Where(b => b.Author.FullName == bookAuthor);
            }

            var authors = await _context.Author.ToListAsync();
            var genres = await _context.Genre.ToListAsync();

            var genreSelectList = new SelectList(genres, nameof(Genre.GenreName), nameof(Genre.GenreName));
            var authorSelectList = new SelectList(authors, nameof(Author.FullName), nameof(Author.FullName));

            var bookGenreVM = new BookGenreViewModel
            {
                Books = books.ToList(), 
                Genres = genreSelectList,
                Authors = authors,
                BookAuthor = bookAuthor,
                BookGenre = bookGenre,
                SearchString = searchString
            };

            return View(bookGenreVM);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Author)
                .Include(b => b.Genres)
                .ThenInclude(bg => bg.Genre) // Include the Genre associated with BookGenre
                .Include(b => b.Reviews)
                .Include(b=>b.UserBooks)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            // Calculate average rating
            double? averageRating = null;
            if (book.Reviews != null && book.Reviews.Any() && book.Reviews.All(r => r.Rating != null))
            {
                int totalRating = book.Reviews.Sum(r => r.Rating.Value);
                averageRating = (double)totalRating / book.Reviews.Count;
            }

            ViewBag.AverageRating = averageRating;

            return View(book);
        }
        // GET: Books/Create
        //[Authorize(Roles = "Admin")]

        public IActionResult Create()
        {
            var authors = _context.Set<Author>().ToList();
            if (!authors.Any())
            {
                ViewData["Message"] = "There are currently no authors available. Please create an author first.";
            }

            // Initialize GenreList with an empty list if no authors are available
            var genreList = _context.Genre.Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.GenreName }).ToList();

            ViewData["AuthorId"] = new SelectList(authors, "Id", "FullName");
            return View(new BookGenresEditViewModel { GenreList = genreList });
        }
        /*
             [HttpPost]
             [ValidateAntiForgeryToken]
             [Authorize(Roles = "Admin")]

             public async Task<IActionResult> Create([Bind("Id,Title,YearPublished,NumPages,Description,Publisher,FrontPage,DownloadUrl,AverageRating,AuthorId")] Book book, IEnumerable<int> SelectedGenres)
             {
                 if (ModelState.IsValid && book.AuthorId != 0)
                 {
                     // Add the book to the context
                     _context.Add(book);
                     await _context.SaveChangesAsync();

                     // Associate selected genres with the book
                     if (SelectedGenres != null && SelectedGenres.Any())
                     {
                         foreach (var genreId in SelectedGenres)
                         {
                             _context.BookGenre.Add(new BookGenre { BookId = book.Id, GenreId = genreId });
                         }
                         await _context.SaveChangesAsync();
                     }

                     return RedirectToAction(nameof(Index));
                 }

                 // Handle validation error (e.g., display error message)
                 ViewData["AuthorId"] = new SelectList(_context.Set<Author>(), "Id", "FullName", book.AuthorId);
                 return View(book);
             }
             */
        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Create(BookGenresEditViewModel viewModel)
        {
            if (ModelState.IsValid && viewModel.Book.AuthorId != 0 && viewModel.File != null)
            {
                // Save the image to wwwroot/images folder
                    string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                //string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "images");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.File.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                viewModel.File.CopyTo(new FileStream(filePath, FileMode.Create));

                // Update the book object with the file path
                viewModel.Book.FrontPage = uniqueFileName;

                // Add the book to the context
                _context.Add(viewModel.Book);
                await _context.SaveChangesAsync();

                // Associate selected genres with the book
                if (viewModel.SelectedGenres != null && viewModel.SelectedGenres.Any())
                {
                    foreach (var genreId in viewModel.SelectedGenres)
                    {
                        _context.BookGenre.Add(new BookGenre { BookId = viewModel.Book.Id, GenreId = genreId });
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
 
            // Handle validation error (e.g., display error message)
            var authors = _context.Set<Author>().ToList();
            if (!authors.Any())
            {
                ViewData["Message"] = "There are currently no authors available. Please create an author first.";
            }
            ViewData["AuthorId"] = new SelectList(authors, "Id", "FullName", viewModel.Book.AuthorId);
            return View(viewModel);
        }
        */
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(BookGenresEditViewModel bookVM)
        {
            if (ModelState.IsValid && bookVM.Book.AuthorId != 0)
            {
                var result = await _photoService.AddPhotoAsync(bookVM.File);
                bookVM.Book.FrontPage = result.Url.ToString();

                if (bookVM.Download != null)
                {
                    string folder = "files/";
                    bookVM.Book.DownloadUrl = await UploadFile(folder, bookVM.Download); // Assign DownloadUrl here
                }

                // Add the book to the context
                _context.Add(bookVM.Book);
                await _context.SaveChangesAsync();

                // Associate selected genres with the book
                if (bookVM.SelectedGenres != null && bookVM.SelectedGenres.Any())
                {
                    foreach (var genreId in bookVM.SelectedGenres)
                    {
                        _context.BookGenre.Add(new BookGenre { BookId = bookVM.Book.Id, GenreId = genreId });
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            // Handle validation error (e.g., display error message)
            ViewData["AuthorId"] = new SelectList(_context.Set<Author>(), "Id", "FullName", bookVM.Book.AuthorId);
            return View(bookVM);
        }
        private async Task<string> UploadFile(string folderPath, IFormFile file)
        {
            if (folderPath == null)
            {
                throw new ArgumentNullException(nameof(folderPath), "Folder path cannot be null.");
            }

            // Combine folder path and file name
            string fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(folderPath, fileName);

            // Get the absolute server path
            string absoluteServerPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath);

            // Save the file
            using (var stream = new FileStream(absoluteServerPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return the URL relative to the web root
            return "/" + filePath.Replace("\\", "/");
        }
        /*
        // GET: Books/Edit/5

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = _context.Book.Where(m => m.Id == id).Include(m => m.Genres).FirstOrDefault();
            if (book == null)
            {
                return NotFound();
            }

            var genres = await _context.Genre.ToListAsync(); // Fetch genres from the database
            var viewModel = new BookGenresEditViewModel
            {
                Book = book,
                GenreList = genres.Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.GenreName }),
                SelectedGenres = book.Genres.Select(sa => sa.GenreId)
            };

            ViewData["AuthorId"] = new SelectList(_context.Author, "Id", "FullName", book.AuthorId);
            return View(viewModel);
        }


        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,YearPublished,NumPages,Description,Publisher,FrontPage,DownloadUrl,AuthorId")] Book book, int[] selectedGenres)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);

                    var existingGenres = _context.BookGenre.Where(bg => bg.BookId == id);
                    _context.BookGenre.RemoveRange(existingGenres);

                    if (selectedGenres != null)
                    {
                        foreach (var genreId in selectedGenres)
                        {
                            _context.Add(new BookGenre { BookId = book.Id, GenreId = genreId });
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Genres = new MultiSelectList(await _context.Genre.ToListAsync(), "Id", "GenreName", selectedGenres);
            ViewBag.AuthorId = new SelectList(_context.Author, "Id", "FullName", book.AuthorId);
            return View(book);
        }
        */
        public async Task<IActionResult> Edit(int id)
        {
            Book book = await GetByIdAsyncNoTracking(id);
            if (book == null)
            {
                return View("Error");
            }
            IEnumerable<Author> authors = await GetAllAuthors();
            IEnumerable<Genre> genres = await GetAllGenres();
            List<int> genreIds = new List<int>();
            foreach (var genre in book.Genres)
            {
                genreIds.Add(genre.GenreId);
            }
            EditBookViewModel bookVM = new EditBookViewModel()
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                YearPublished = book.YearPublished,
                NumPages = book.NumPages,
                Publisher = book.Publisher,
                FrontPageURL = book.FrontPage,
                DownloadUrl = book.DownloadUrl,
                Authors = authors,
                Genres = genres,
                AuthorId = book.AuthorId,
                GenreIds = genreIds,
            };
            return View(bookVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditBookViewModel bookVM)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<Author> authors = await GetAllAuthors();
                bookVM.Authors = authors;
                IEnumerable<Genre> genres = await GetAllGenres();
                bookVM.Genres = genres;
                ModelState.AddModelError("", "Failed to edit book");
                return View(bookVM);
            }
            var book = await GetByIdAsyncNoTracking(id);
            if (book != null)
            {
                try
                {
                    if (bookVM.FrontPage != null || bookVM.Download != null)
                    {
                        await _photoService.DeletePhotoAsync(book.FrontPage);
                        string folder = "files/";
                        bookVM.DownloadUrl = await UploadFile(folder, bookVM.Download); // Assign new DownloadUrl here
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Could not delete photo");
                    IEnumerable<Author> authors = await GetAllAuthors();
                    bookVM.Authors = authors;
                    IEnumerable<Genre> genres = await GetAllGenres();
                    bookVM.Genres = genres;
                    return View(bookVM);
                }
                var photoResult = "";
                if (bookVM.FrontPage != null)
                {
                    photoResult = (await _photoService.AddPhotoAsync(bookVM.FrontPage)).Url.ToString();
                }
                else
                {
                    photoResult = book.FrontPage;
                }

                Book newBook = new Book()
                {
                    Id = bookVM.Id,
                    Title = bookVM.Title,
                    Description = bookVM.Description,
                    YearPublished = bookVM.YearPublished,
                    NumPages = bookVM.NumPages,
                    Publisher = bookVM.Publisher,
                    FrontPage = photoResult,
                    DownloadUrl = bookVM.DownloadUrl,
                    AuthorId = bookVM.AuthorId,
                };
                Update(newBook);
                /*IEnumerable<BookGenre> bookGenres = await GetAll();
                foreach (var bg in bookGenres)
                {
                    if (bg.BookId == id)
                    {
                        Delete(bg);
                    }
                }*/
                await DeleteBookGenresByBookId(id);
                foreach (var genreId in bookVM.GenreIds)
                {
                    BookGenre bookGenre = new BookGenre()
                    {
                        BookId = book.Id,
                        GenreId = genreId,
                    };
                    Add(bookGenre);
                }

                return Redirect("/Books/Details/" + id);
            }
            else
            {
                IEnumerable<Author> authors = await GetAllAuthors();
                bookVM.Authors = authors;
                IEnumerable<Genre> genres = await GetAllGenres();
                bookVM.Genres = genres;
                return View(bookVM);
            }
        }
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Author)
                .Include(b => b.Genres)
                .ThenInclude(b => b.Genre)
                .Include(b => b.Reviews) // Include reviews to calculate average rating
                .FirstOrDefaultAsync(m => m.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            var viewModel = new BookGenresEditViewModel
            {
                Book = book,
                GenreList = _context.Genre.Select(g => new SelectListItem
                {
                    Value = g.Id.ToString(),
                    Text = g.GenreName
                })
            };

            ViewBag.AverageRating = book.AverageRating;

            return View(viewModel);
        }
        public async Task DeleteBookGenresByBookId(int bookId)
        {
            var bookGenres = await _context.BookGenre.Where(bg => bg.BookId == bookId).ToListAsync();
            _context.BookGenre.RemoveRange(bookGenres);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Author>> GetAllAuthors()
        {
            return await _context.Author.ToListAsync();
        }
        public async Task<IEnumerable<Genre>> GetAllGenres()
        {
            return await _context.Genre.ToListAsync();
        }
        public async Task<IEnumerable<Book>> GetBooksByAuthorId(int id)
        {
            return await _context.Book.Include(a => a.Author).Include(r => r.Reviews).Include(bg => bg.Genres).ThenInclude(g => g.Genre).Where(a => a.AuthorId.Equals(id)).ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetBooksByGenreId(int id)
        {
            List<BookGenre> bookGenres = await _context.BookGenre.Where(g => g.GenreId.Equals(id)).ToListAsync();
            List<Book> books = new List<Book>();
            foreach (var bg in bookGenres)
            {
                Book book = await GetByIdAsync(bg.BookId);
                books.Add(book);
            }
            return books;
        }

        public async Task<IEnumerable<Book>> GetBooksByName(string name)
        {
            return await _context.Book.Include(a => a.Author).Include(r => r.Reviews).Include(bg => bg.Genres).ThenInclude(g => g.Genre).Where(n => n.Title.Contains(name)).ToListAsync();
        }

        public async Task<Book> GetByIdAsync(int id)
        {
            return await _context.Book.Include(a => a.Author).Include(r => r.Reviews).Include(bg => bg.Genres).ThenInclude(g => g.Genre).FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Book> GetByIdAsyncNoTracking(int id)
        {
            return await _context.Book.Include(a => a.Author).Include(r => r.Reviews).Include(bg => bg.Genres).ThenInclude(g => g.Genre).AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Book> GetLastBook()
        {
            return await _context.Book.FirstOrDefaultAsync(i => i.Id == _context.Book.Max(i => i.Id));
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Book book)
        {
            /*if (book != null)
            {
                ShowEntityState(_context);
                _context.Entry(book).State = EntityState.Modified;
                ShowEntityState(_context);
            }*/
            /*var trackedReference = _context.Books.Local.SingleOrDefault(i => i.Id == book.Id);
            if (trackedReference == null)
            {
                _context.Books.Update(book);
            }
            else if (!Object.ReferenceEquals(trackedReference, book))
            {
                Mapper.ReferenceEquals(book, trackedReference);
            }*/
            _context.Book.Update(book);
            return Save();
        }
        public bool Add(BookGenre bookGenre)
        {
            _context.BookGenre.Add(bookGenre);
            return Save();
        }
        public async Task<IEnumerable<Book>> GetAll()
        {
            return await _context.Book
                .Include(a => a.Author)
                .Include(r => r.Reviews)
                .Include(bg => bg.Genres)
                    .ThenInclude(g => g.Genre)
                .ToListAsync();
        }
        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(BookGenresEditViewModel viewModel)
        {
            var book = await _context.Book.FindAsync(viewModel.Book.Id);
            if (book != null)
            {
                _context.Book.Remove(book);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.Id == id);
        }
      
    }
}

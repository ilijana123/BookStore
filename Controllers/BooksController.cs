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
using MVCBookk.Data;
using MVCBookk.Models;
namespace MVCBook.Controllers
{
    public class BooksController : Controller
    {
        private readonly MVCBookkContext _context;

        public BooksController(MVCBookkContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, string bookGenre, string bookAuthor)
        {
            var books = _context.Book
            .Include(b => b.Author)
            .Include(b => b.Genres)
            .Include(b => b.Reviews) // Include reviews
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
                Books = books.ToList(), // Materialize the query
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
        // GET: Movies/Create
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
        [HttpPost]
        [ValidateAntiForgeryToken]
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
        /*
        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var book = _context.Book.Where(m => m.Id == id).Include(m => m.Genres).First();
            if (book == null)
            {
                return NotFound();
            }
            var genres = _context.Genre.AsEnumerable();
            genres=genres.OrderBy(s => s.GenreName);
            BookGenresEditViewModel viewmodel = new BookGenresEditViewModel
            {
                Book=book,
                GenreList = new MultiSelectList(genres, "Id", "GenreName"),
                SelectedGenres = book.Genres.Select(sa => sa.GenreId)
            };
            ViewData["AuthorId"] = new SelectList(_context.Author, "Id", "FullName", book.AuthorId);
            return View(viewmodel);
        }
        */
        // GET: Books/Edit/5
        // GET: Books/Edit/5

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
        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookGenresEditViewModel viewmodel)
        {
            if (id != viewmodel.Book.Id) { return NotFound(); }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewmodel.Book);
                    await _context.SaveChangesAsync();
                    IEnumerable<int> newGenreList = viewmodel.SelectedGenres;
                    IEnumerable<int> prevGenreList = _context.BookGenre.Where(s => s.BookId == id).Select(s => s.GenreId);
                    IQueryable<BookGenre> toBeRemoved = _context.BookGenre.Where(s => s.BookId == id);
                    if (newGenreList != null)
                    {
                        toBeRemoved = toBeRemoved.Where(s => !newGenreList.Contains(s.GenreId));
                        foreach (int genreId in newGenreList)
                        {
                            if (!prevGenreList.Any(s => s == genreId))
                            {
                                _context.BookGenre.Add(new BookGenre { GenreId = genreId, BookId = id });
                            }
                        }
                    }
                    _context.BookGenre.RemoveRange(toBeRemoved);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(viewmodel.Book.Id)) { return NotFound(); }
                    else { throw; }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Author, "Id", "FullName", viewmodel.Book.AuthorId);
            return View(viewmodel);
        }
         */

 // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                _context.Book.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.Id == id);
        }
    }
}

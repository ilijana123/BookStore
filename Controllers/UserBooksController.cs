using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCBookk.Data;
using MVCBookk.Models;

namespace MVCBookk.Controllers
{
    public class UserBooksController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly MVCBookkContext _context;
        public UserBooksController(MVCBookkContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User.GetUserId();
        }
        // GET: UserBooks
        public async Task<IActionResult> Index()
        {
            var mVCBookkContext = _context.UserBooks.Include(u => u.Book);
            return View(await mVCBookkContext.ToListAsync());
        }
        // GET: UserBooks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userBooks = await _context.UserBooks
                .Include(u => u.Book)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userBooks == null)
            {
                return NotFound();
            }

            return View(userBooks);
        }

        // GET: UserBooks/Create
        //[Authorize(Roles = "User")]
        public IActionResult Create()
        {
            ViewData["BookId"] = new SelectList(_context.Book, "Id", "Title");
            return View();
        }

        // POST: UserBooks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        //[Authorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BookId,AppUser")] UserBooks userBooks)
        {
            if (ModelState.IsValid)
            {
                // Check if a UserBooks entry already exists for the specified book and user
                var existingUserBook = await _context.UserBooks
                    .FirstOrDefaultAsync(ub => ub.BookId == userBooks.BookId && ub.AppUser == userBooks.AppUser);

                if (existingUserBook == null)
                {
                    // No existing entry found, add the new UserBooks entry
                    _context.Add(userBooks);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // Existing entry found, return a message or handle the situation as needed
                    ModelState.AddModelError("", "User already has this book.");
                    ViewData["BookId"] = new SelectList(_context.Book, "Id", "Title", userBooks.BookId);
                    return View(userBooks);
                }

                // Redirect to the details page of the book
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookId"] = new SelectList(_context.Book, "Id", "Title", userBooks.BookId);
            return View(userBooks);
        }

        // GET: UserBooks/Edit/5
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userBooks = await _context.UserBooks.FindAsync(id);
            if (userBooks == null)
            {
                return NotFound();
            }
            ViewData["BookId"] = new SelectList(_context.Book, "Id", "Title", userBooks.BookId);
            return View(userBooks);
        }

        // POST: UserBooks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookId,AppUser")] UserBooks userBooks)
        {
            if (id != userBooks.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userBooks);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserBooksExists(userBooks.Id))
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
            ViewData["BookId"] = new SelectList(_context.Book, "Id", "Title", userBooks.BookId);
            return View(userBooks);
        }

        // GET: UserBooks/Delete/5
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userBooks = await _context.UserBooks
                .Include(u => u.Book)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userBooks == null)
            {
                return NotFound();
            }

            return View(userBooks);
        }

        // POST: UserBooks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userBooks = await _context.UserBooks.FindAsync(id);
            if (userBooks != null)
            {
                _context.UserBooks.Remove(userBooks);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        private bool UserBooksExists(int id)
        {
            return _context.UserBooks.Any(e => e.Id == id);
        }
        /*
        public string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User.GetUserId();
        }
        public string GetCurrentUserUsername()
        {
            var curUser = GetCurrentUserId();
            var username = _context.Users.FirstOrDefault(i => i.Id == curUser).UserName;
            return username;
        }
        public async Task<List<Book>> GetAllUserBooks()
        {
            var curUser = GetCurrentUserId();
            List<UserBooks> userBooks =await _context.UserBooks.Where(u => u.AppUserId == curUser).ToListAsync();
            List<Book> myBooks = new List<Book>();
            foreach(var book in userBooks)
            {
                var myBook = await _context.Books.Include(a => a.Author).Include(r => r.Reviews).Include(bg => bg.BookGenres).ThenInclude(g => g.Genre).FirstOrDefaultAsync(i => i.Id == book.BookId);
                myBooks.Add(myBook);
            }
            return myBooks;
        }


        public bool HasBook(int bookId)
        {
            var curUser = GetCurrentUserId();
            var hasBook = _context.UserBooks.FirstOrDefault(u => u.AppUserId == curUser && u.BookId == bookId);
            return hasBook != null ? true : false;
        }


        public async Task<AppUser> GetUserById(string id)
        {
            return await _context.Users.FindAsync(id);
        }


        public bool Add(UserBooks userBooks)
        {
            _context.UserBooks.Add(userBooks);
            return Save();
        }

        public bool Update(UserBooks userBooks)
        {
            _context.UserBooks.Update(userBooks);
            return Save();
        }

        public bool Delete(UserBooks userBooks)
        {
            _context.UserBooks.Remove(userBooks);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
        */
    }
}

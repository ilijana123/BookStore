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
    public class BookGenresController : Controller
    {
        private readonly MVCBookkContext _context;

        public BookGenresController(MVCBookkContext context)
        {
            _context = context;
        }

        // GET: BookGenres
        public async Task<IActionResult> Index()
        {
            var mVCBookkContext = _context.BookGenre.Include(b => b.Book).Include(b => b.Genre);
            return View(await mVCBookkContext.ToListAsync());
        }

        // GET: BookGenres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookGenre = await _context.BookGenre
                .Include(b => b.Book)
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookGenre == null)
            {
                return NotFound();
            }

            return View(bookGenre);
        }

        // GET: BookGenres/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["BookId"] = new SelectList(_context.Book, "Id", "Title");
            ViewData["GenreId"] = new SelectList(_context.Set<Genre>(), "Id", "GenreName");
            return View();
        }

        // POST: BookGenres/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,BookId,GenreId")] BookGenre bookGenre)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bookGenre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookId"] = new SelectList(_context.Book, "Id", "Title", bookGenre.BookId);
            ViewData["GenreId"] = new SelectList(_context.Set<Genre>(), "Id", "GenreName", bookGenre.GenreId);
            return View(bookGenre);
        }

        // GET: BookGenres/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookGenre = await _context.BookGenre.FindAsync(id);
            if (bookGenre == null)
            {
                return NotFound();
            }
            ViewData["BookId"] = new SelectList(_context.Book, "Id", "Title", bookGenre.BookId);
            ViewData["GenreId"] = new SelectList(_context.Set<Genre>(), "Id", "GenreName", bookGenre.GenreId);
            return View(bookGenre);
        }

        // POST: BookGenres/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookId,GenreId")] BookGenre bookGenre)
        {
            if (id != bookGenre.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bookGenre);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookGenreExists(bookGenre.Id))
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
            ViewData["BookId"] = new SelectList(_context.Book, "Id", "Title", bookGenre.BookId);
            ViewData["GenreId"] = new SelectList(_context.Set<Genre>(), "Id", "GenreName", bookGenre.GenreId);
            return View(bookGenre);
        }

        // GET: BookGenres/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookGenre = await _context.BookGenre
                .Include(b => b.Book)
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookGenre == null)
            {
                return NotFound();
            }

            return View(bookGenre);
        }

        // POST: BookGenres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bookGenre = await _context.BookGenre.FindAsync(id);
            if (bookGenre != null)
            {
                _context.BookGenre.Remove(bookGenre);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookGenreExists(int id)
        {
            return _context.BookGenre.Any(e => e.Id == id);
        }
    }
}

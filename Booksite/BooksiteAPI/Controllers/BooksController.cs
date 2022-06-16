using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BooksiteAPI.Models;
using BooksiteAPI.Data;

namespace BooksiteAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BooksiteContext _context;

        public BooksController(BooksiteContext context)
        {
            _context = context;
        }

		// GET: api/Books
		[HttpGet]
		public async Task<ActionResult<CatalogPageDto>>
			GetBooks(int page = 1, int items = 5)
		{
			if (page < 1 || items < 1) {
				return BadRequest();
			}

			if (_context.Books == null)
			{
				return NotFound();
			}

			page -= 1;

			var dbCount = await _context.Books.CountAsync();
			var dbBooks = await _context.Books.OrderByDescending(b => b.BQuantity)
				.Skip(page * items).Take(items).Select(b =>
				new BookDto
				{
					Isbn = b.BIsbn,
					Title = b.BTitle,
					Author = b.BAuthor,
					Quantity = b.BQuantity,
					Price = b.BPrice ?? 0,
					CoverFile = b.BCoverFile
				}).ToListAsync();
			return new CatalogPageDto { books = dbBooks, count = dbCount };
		}

		// GET: api/Books/5
		[HttpGet("{isbn}")]
        public async Task<ActionResult<BookDetailsDto>> GetBook(string isbn)
        {
          if (_context.Books == null)
          {
              return NotFound();
          }
			var book = await _context.Books.
				Include(b => b.BGenreNavigation).Select(b =>
				new BookDetailsDto
				{
					Isbn = b.BIsbn,
					Title = b.BTitle,
					Author = b.BAuthor,
					Quantity = b.BQuantity,
					Price = b.BPrice ?? 0,
					CoverFile = b.BCoverFile,
					Genre = b.BGenreNavigation.GName
				}
			).SingleOrDefaultAsync(b => b.Isbn == isbn);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

		[HttpGet("search")]
		public async Task<ActionResult<List<BookSearchResDto>>> 
			GetBookByTitle(string title)
		{
			title = title.Trim();
			if(title.Length < 3)
			{
				return BadRequest();
			}

			if (_context.Books == null)
			{
				return NotFound();
			}
			var SearchRes = await _context.Books
				.Where(b => b.BTitle.Contains(title))
				.OrderByDescending(b => b.BQuantity)
				.Take(5)
				.Select(b =>
				new BookSearchResDto
				{
					Isbn = b.BIsbn,
					Title = b.BTitle,
					Author = b.BAuthor,
				}
			).ToListAsync();

			if (SearchRes == null)
			{
				return NotFound();
			}

			return SearchRes;
		}


		[HttpPut("{id}")]
        public async Task<IActionResult> PutBook(string id, Book book)
        {
            if (id != book.BIsbn)
            {
                return BadRequest();
            }

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
          if (_context.Books == null)
          {
              return Problem("Entity set 'BooksiteContext.Books'  is null.");
          }
            _context.Books.Add(book);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BookExists(book.BIsbn))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBook", new { id = book.BIsbn }, book);
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(string id)
        {
            if (_context.Books == null)
            {
                return NotFound();
            }
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(string id)
        {
            return (_context.Books?.Any(e => e.BIsbn == id)).GetValueOrDefault();
        }
    }
}

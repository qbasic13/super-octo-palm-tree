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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
        {
          if (_context.Books == null)
          {
              return NotFound();
          }
			return await _context.Books.Select(b =>
				new BookDto
				{
					Isbn = b.BIsbn,
					Title = b.BTitle,
					Author = b.BAuthor,
					Quantity = b.BQuantity,
					Price = b.BPrice ?? 0,
					CoverFile = b.BCoverFile
				}).ToListAsync();
		}

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

        [HttpPut("{isbn}")]
        public async Task<IActionResult> PutBook(string isbn, Book book)
        {
            if (isbn != book.BIsbn)
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
    }
}

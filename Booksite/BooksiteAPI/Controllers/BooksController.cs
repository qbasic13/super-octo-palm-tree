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
        public async Task<ActionResult<CatalogPageDto>>
            GetBooks(int page = 1, int items = 5)
        {
            if (page < 1 || items < 1)
            {
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
            return new CatalogPageDto { Books = dbBooks, Count = dbCount };
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<BookSearchResDto>>>
            GetBookByTitle(string title)
        {
            title = title.Trim();
            if (title.Length < 3)
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

        private bool BookExists(string id)
        {
            return (_context.Books?.Any(e => e.BIsbn == id)).GetValueOrDefault();
        }
    }
}

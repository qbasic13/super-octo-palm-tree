using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BooksiteAPI.Models;
using BooksiteAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace BooksiteAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _books;

        public BookController(IBookService books)
        {
            _books = books;
        }

        [HttpGet]
        public async Task<ActionResult<BookDetailsDto>> GetBook(string isbn)
        {
            var BookSearchResult = await _books.GetBookAsync(isbn);
            if(BookSearchResult.Isbn == "not_found")
                return NotFound();

            if (BookSearchResult.Isbn == "bad_isbn")
                return BadRequest();

            return BookSearchResult;
        }

        [Authorize(Roles = "admin")]
        [HttpPost("edit")]
        public async Task<ActionResult<BookDetailsDto>> 
            EditBook(BookDetailsDto changedBook)
        {
            var BookEditResult = await _books.EditBookAsync(changedBook);
            if (BookEditResult.Isbn == "not_found")
                return NotFound();

            if (BookEditResult.Isbn == "bad_edit_details")
                return BadRequest();

            return BookEditResult;
        }

        [Authorize(Roles = "admin")]
        [HttpPost("add")]
        public async Task<ActionResult<BookDetailsDto>>
            AddBook(BookDetailsDto changedBook)
        {
            var BookEditResult = await _books.EditBookAsync(changedBook, true);
            if (BookEditResult.Isbn == "not_found")
                return NotFound();

            if (BookEditResult.Isbn == "bad_edit_details")
                return BadRequest();

            return BookEditResult;
        }

        [Authorize(Roles = "admin")]
        [HttpPost("upload")]
        public async Task<ActionResult<bool>> 
            UploadCover([FromForm]CoverUploadDto coverUploadForm)
        {
            var uploadResult = await _books.CoverUploadAsync(coverUploadForm);
            return uploadResult ? uploadResult : NotFound();
        }

        [HttpGet("genres")]
        public async Task<ActionResult<string[]>> GetGenres()
        {
            var BookEditResult = await _books.GetGenresAsync();
            if (BookEditResult == null || BookEditResult.Contains("!not_found"))
                return NotFound();

            return BookEditResult.ToArray();
        }
    }
}

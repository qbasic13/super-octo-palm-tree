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
        private readonly IBookService _book;

        public BookController(IBookService book)
        {
            _book = book;
        }

        [HttpGet]
        public async Task<ActionResult<BookDetailsDto>> GetBook(string isbn)
        {
            var BookSearchResult = await _book.GetBookAsync(isbn);
            if (BookSearchResult.Isbn == "not_found")
                return NotFound();

            if (BookSearchResult.Isbn == "bad_isbn")
                return BadRequest();

            return BookSearchResult;
        }

        [HttpGet("cart")]
        public async Task<ActionResult<CartDetailsResDto>>
            GetCartBooks(string isbns)
        {
            string[] isbnsArr = isbns.Split('_');
            if (isbnsArr == null)
                return BadRequest();

            CartDetailsResDto cartDetails = new CartDetailsResDto();
            List<BookDetailsDto> cartBooks = new List<BookDetailsDto>();
            foreach (string isbn in isbnsArr)
            {
                var bookDetails = await _book.GetBookAsync(isbn);
                if (bookDetails.Isbn == "not_found")
                    return NotFound();

                if (bookDetails.Isbn == "bad_edit_details")
                    return BadRequest();

                cartBooks.Add(bookDetails);
            }

            cartDetails.Details = cartBooks.ToArray();
            return cartDetails;
        }

        [Authorize(Roles = "admin")]
        [HttpPost("edit")]
        public async Task<ActionResult<BookDetailsDto>>
            EditBook(BookDetailsDto changedBook)
        {
            var BookEditResult = await _book.EditBookAsync(changedBook);
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
            var BookEditResult = await _book.EditBookAsync(changedBook, true);
            if (BookEditResult.Isbn == "not_found")
                return NotFound();

            if (BookEditResult.Isbn == "bad_edit_details")
                return BadRequest();

            return BookEditResult;
        }

        [Authorize(Roles = "admin")]
        [HttpPost("upload")]
        public async Task<ActionResult<bool>>
            UploadCover([FromForm] CoverUploadDto coverUploadForm)
        {
            var uploadResult = await _book.CoverUploadAsync(coverUploadForm);
            return uploadResult ? uploadResult : NotFound();
        }

        [HttpGet("genres")]
        public async Task<ActionResult<string[]>> GetGenres()
        {
            var BookEditResult = await _book.GetGenresAsync();
            if (BookEditResult == null || BookEditResult.Contains("!not_found"))
                return NotFound();

            return BookEditResult.ToArray();
        }
    }
}

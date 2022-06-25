using System.Text.RegularExpressions;
using BooksiteAPI.Data;
using BooksiteAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BooksiteAPI.Services
{
    public class BookService : IBookService
    {
        private readonly BooksiteContext _context;
        private readonly IFileService _files;

        public BookService(BooksiteContext context,
            IFileService files)
        {
            _context = context;
            _files = files;
        }

        public async Task<BookDetailsDto> GetBookAsync(string isbn)
        {
            if (_context.Books == null)
                return new BookDetailsDto { Isbn = "not_found" };

            isbn = isbn.Trim();
            if (!ValidateIsbn(isbn))
                return new BookDetailsDto { Isbn = "bad_isbn" };

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
                    Genre = b.BGenreNavigation.GName,
                    PublishYear = b.BPublishYear
                }
            ).SingleOrDefaultAsync(b => b.Isbn == isbn);

            if (book == null)
                return new BookDetailsDto { Isbn = "not_found" };

            return book;
        }

        public async Task<BookDetailsDto> EditBookAsync(
            BookDetailsDto editedBook, bool isAdding = false)
        {
            if (_context.Books == null)
                return new BookDetailsDto { Isbn = "not_found" };

            if (editedBook.Isbn == null || editedBook.Title == null
                || editedBook.Author == null || editedBook.Genre == null)
                return new BookDetailsDto { Isbn = "bad_edit_details" };

            if (!ValidateIsbn(editedBook.Isbn!))
                return new BookDetailsDto { Isbn = "bad_edit_details" };

            //check if chosen by user genre exists
            var newBookGenre = await _context.Genres
                        .Where(g => g.GName == editedBook.Genre).FirstOrDefaultAsync();
            if (newBookGenre == null)
                return new BookDetailsDto { Isbn = "bad_edit_details" };

            //check ranges
            if (editedBook.PublishYear < 0
                || editedBook.PublishYear > DateTime.Now.AddYears(2).Year
                || editedBook.Price < 0 || editedBook.Quantity < 0)
                return new BookDetailsDto { Isbn = "bad_edit_details" };

            //check if book with given isbn exists
            var book = await _context.Books.Include(b => b.BGenreNavigation).
                Where(b => b.BIsbn == editedBook.Isbn).SingleOrDefaultAsync();

            if (book == null && !isAdding)
                return new BookDetailsDto { Isbn = "not_found" };

            if (book == null)
            {
                book = new Book
                {
                    BIsbn = editedBook.Isbn,
                    BTitle = editedBook.Title,
                    BAuthor = editedBook.Author,
                    BPrice = editedBook.Price,
                    BQuantity = editedBook.Quantity,
                    BPublishYear = editedBook.PublishYear,
                    BCoverFile = editedBook.CoverFile,
                    BGenreNavigation = newBookGenre
                };
                _context.Books.Add(book);
            }
            else
            {
                book.BIsbn = editedBook.Isbn;
                book.BTitle = editedBook.Title;
                book.BAuthor = editedBook.Author;
                book.BPrice = editedBook.Price;
                book.BQuantity = editedBook.Quantity;
                book.BPublishYear = editedBook.PublishYear;
                book.BCoverFile = editedBook.CoverFile;
                book.BGenreNavigation = newBookGenre;
            }
            await _context.SaveChangesAsync();
            return editedBook;
        }

        public async Task<bool> CoverUploadAsync(CoverUploadDto coverUploadForm)
        {
            if (coverUploadForm.file == null ||
                coverUploadForm.isbn == null ||
                !ValidateIsbn(coverUploadForm.isbn))
            {
                return false;
            }

            var book = await _context.Books
                .Where(b => b.BIsbn == coverUploadForm.isbn)
                .FirstOrDefaultAsync();
            if (book == null)
                return false;

            string fileExt = new FileInfo(
                coverUploadForm.file.FileName).Extension;
            string newFileName = Guid.NewGuid().ToString() + fileExt;

            var UploadResult = await _files
                .UploadImageAsync(coverUploadForm.file!,
                "covers", newFileName);
            if (!UploadResult)
                return false;

            book.BCoverFile = newFileName;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<string>> GetGenresAsync()
        {
            if (_context.Genres == null)
                return new List<string>() { "!not_found" };

            var genres = await _context.Genres.Select(
                e => new string(e.GName)).ToListAsync();
            if (genres == null)
                return new List<string>() { "!not_found" };
            return genres;
        }

        public bool ValidateIsbn(string isbn)
        {
            isbn.Trim();
            if (isbn.Length != 13)
                return false;

            var regex = new Regex(@"^[0-9]{13}$");
            if (!regex.IsMatch(isbn))
                return false;

            int checkDigit = 0;
            for (int i = 1; i < isbn.Length; i++)
            {
                checkDigit += int.Parse(isbn.Substring(i - 1, 1)) * ((i % 2) > 0 ? 1 : 3);
            }
            checkDigit = checkDigit % 10;
            checkDigit = checkDigit != 0 ? 10 - checkDigit : 0;
            int isbnDigit = int.Parse(isbn.Substring(12, 1));
            return checkDigit == isbnDigit;
        }
    }
}

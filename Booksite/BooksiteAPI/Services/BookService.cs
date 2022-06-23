using System.Text.RegularExpressions;
using BooksiteAPI.Data;
using BooksiteAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BooksiteAPI.Services
{
    public class BookService : IBookService
    {
        private readonly BooksiteContext _context;
        public BookService(BooksiteContext context)
        {
            _context = context;
        }

        public async Task<BookDetailsDto> GetBookAsync(string isbn)
        {
            if (_context.Books == null)
                return await Task.FromResult(new BookDetailsDto { Isbn = "not_found" });

            isbn = isbn.Trim();
            if (!ValidateIsbn(isbn))
                return await Task.FromResult(new BookDetailsDto { Isbn = "bad_isbn" });

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
                return await Task.FromResult(new BookDetailsDto { Isbn = "not_found" });

            return book;
        }

        public async Task<BookDetailsDto> EditBookAsync(
            BookDetailsDto editedBook, bool isAdding = false)
        {
            if (_context.Books == null)
                return await Task.FromResult(new BookDetailsDto { Isbn = "not_found" });

            if (editedBook.Isbn == null || editedBook.Title == null
                || editedBook.Author == null || editedBook.Genre == null)
                return await Task.FromResult(new BookDetailsDto { Isbn = "bad_edit_details" });

            if (!ValidateIsbn(editedBook.Isbn!))
                return await Task.FromResult(new BookDetailsDto { Isbn = "bad_edit_details" });

            //check if chosen by user genre exists
            var newBookGenre = await _context.Genres
                        .Where(g => g.GName == editedBook.Genre).FirstOrDefaultAsync();
            if (newBookGenre == null)
                return await Task.FromResult(new BookDetailsDto { Isbn = "bad_edit_details" });

            //check ranges
            if (editedBook.PublishYear < 0
                || editedBook.PublishYear > DateTime.Now.AddYears(2).Year
                || editedBook.Price < 0 || editedBook.Quantity < 0)
                return await Task.FromResult(new BookDetailsDto { Isbn = "bad_edit_details" });

            //check if book with given isbn exists
            var book = await _context.Books.Include(b => b.BGenreNavigation).
                Where(b => b.BIsbn == editedBook.Isbn).SingleOrDefaultAsync();

            if (book == null && !isAdding)
                return await Task.FromResult(new BookDetailsDto { Isbn = "not_found" });

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

        bool ValidateIsbn(string isbn)
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
            return int.Parse(isbn.Substring(12, 1)) == checkDigit;
        }
    }
}

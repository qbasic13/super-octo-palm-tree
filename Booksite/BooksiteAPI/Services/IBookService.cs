using BooksiteAPI.Models;

namespace BooksiteAPI.Services
{
    public interface IBookService
    {
        Task<BookDetailsDto> GetBookAsync(string isbn);
        Task<BookDetailsDto> EditBookAsync(
            BookDetailsDto editedBook, bool isAdding = false);
        Task<IEnumerable<string>> GetGenresAsync();
        bool ValidateIsbn(string isbn);
    }
}

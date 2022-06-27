namespace BooksiteAPI.Services
{
    public interface IFileService
    {
        string PathToImgRoot { get; }
        public Task<bool> UploadImageAsync
            (IFormFile file, string destPath, string destName);
    }
}

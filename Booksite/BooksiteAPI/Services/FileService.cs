namespace BooksiteAPI.Services
{
    public class FileService : IFileService
    {
        private IConfiguration _config;

        public string PathToImgRoot { get; }

        public FileService(IConfiguration config)
        {
            _config = config;
            PathToImgRoot = _config["ConfiguredPathToImages"];
        }

        public async Task<bool> UploadImageAsync
            (IFormFile file, string destPath, string destName)
        {
            if (file.Length == 0)
                return false;

            var fullPath = Path.Combine(PathToImgRoot,
                destPath, destName);

            using (var stream = new FileStream(fullPath, FileMode.CreateNew))
            {
                await file.CopyToAsync(stream);
                return true;
            }
            return false;
        }
    }
}

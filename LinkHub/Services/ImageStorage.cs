namespace LinkHub.Services
{
    public class ImageStorage
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImageStorage(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;

        }

        public async Task<string> AddImageAsync(IFormFile image)
        {
            string ext = Path.GetExtension(image.FileName);
            string fileName = Guid.NewGuid().ToString() + ext;

            string folder = "image/";
            folder += fileName;
            string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);

            using (FileStream DestinationStream = new FileStream(serverFolder, FileMode.Create))
            {
                await image.CopyToAsync(DestinationStream);
            }

            return fileName;
        }

        public void DeleteImage(string fileName)
        {
            string folderDel = "image/" + fileName;
            string pathDel = Path.Combine(_webHostEnvironment.WebRootPath, folderDel);
            File.Delete(pathDel);
        }
    }
}

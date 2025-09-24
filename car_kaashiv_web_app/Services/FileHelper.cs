namespace car_kaashiv_web_app.Services
{
    public static class FileHelper
    {
        public static async Task<(bool Success,string? FilePath,string?ErrorMessage)>SavePartImageAsync(IFormFile imageFile,
            string folderName="parts")
        {
            if (imageFile == null || imageFile.Length == 0)
                return (false, null, "No image file provided.");
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/images/{folderName}");
            Directory.CreateDirectory(uploadsFolder);
            var fileName = Path.GetFileName(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            if (System.IO.File.Exists(filePath))
                return (false, null, "A file with this name already exists.Please rename the file");
            using var stream = new FileStream(filePath, FileMode.Create);
            await imageFile.CopyToAsync(stream);
            return (true, $"/images/{folderName}/{fileName}", null);
        }

    }
}

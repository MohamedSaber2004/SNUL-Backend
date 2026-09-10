using Microsoft.Extensions.Localization;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Services;
using SNUL.Shared.Localization;

namespace Attachment.Services.API.Infrastructure
{
    public class BaseFileService : IBaseFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IStringLocalizer<Messages> _localizer;

        private string WebRootPath => UploadPaths.GetStorageRoot()
            ?? _webHostEnvironment.WebRootPath
            ?? Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot");

        public BaseFileService(
            IWebHostEnvironment webHostEnvironment,
            IStringLocalizer<Messages> localizer)
        {
            _webHostEnvironment = webHostEnvironment;
            _localizer = localizer;
        }

        public async Task<(bool Uploaded, string Result)> UploadFileAsync(IFormFile file, string folderPath)
        {
            if (file == null || file.Length == 0)
                return (false, LocalizationKeys.AttachmentMessages.FileEmpty);

            try
            {
                string uploadsFolder = Path.Combine(WebRootPath, folderPath);

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = GetUniqueFileName(file.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                await using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await file.CopyToAsync(fileStream);
                }

                string relativePath = Path.Combine(folderPath, uniqueFileName).Replace("\\", "/");

                return (true, relativePath);
            }
            catch (Exception)
            {
                return (false, LocalizationKeys.AttachmentMessages.UploadFailed);
            }
        }

        public bool FileExists(string? fullFilePath)
        {
            if (string.IsNullOrWhiteSpace(fullFilePath))
                return false;

            var filePath = GetSafeFilePath(fullFilePath.TrimStart('/'));
            return filePath != null && File.Exists(filePath);
        }

        public async Task<bool> DeleteFileAsync(string fileName, string folderPath)
        {
            try
            {
                var filePath = GetSafeFilePath(Path.Combine(folderPath, fileName));
                if (filePath == null)
                    return false;

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GetUniqueFileName(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            return Guid.NewGuid().ToString("N") + extension;
        }

        public Task<(bool Success, string Result)> DownloadFileAsync(string folderPath, string fileName)
        {
            var relativePath = Path.Combine(folderPath, fileName).Replace("\\", "/");

            if (FileExists(relativePath))
            {
                return Task.FromResult((true, relativePath));
            }

            return Task.FromResult((false, LocalizationKeys.AttachmentMessages.FileNotFound));
        }

        private string? GetSafeFilePath(string relativePath)
        {
            var webRootPath = Path.GetFullPath(WebRootPath);
            var fullPath = Path.GetFullPath(Path.Combine(webRootPath, relativePath.TrimStart('/', '\\')));

            if (!fullPath.StartsWith(webRootPath, StringComparison.OrdinalIgnoreCase))
                return null;

            return fullPath;
        }
    }
}

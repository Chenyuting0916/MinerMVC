using MinerMVC.Security;

namespace MinerMVC.Services.Image;

public class ImageService : IImageService
{
    private const string DefaultImageName = "default.png";
    private readonly string _folderPath;
    private readonly string _legacyFolderPath;

    public ImageService(IWebHostEnvironment hostingEnvironment)
    {
        _folderPath = Path.Combine(hostingEnvironment.ContentRootPath, "App_Data", "contents");
        _legacyFolderPath = Path.Combine(hostingEnvironment.WebRootPath, "Contents");
        Directory.CreateDirectory(_folderPath);
    }

    public async Task<string> AddImage(IFormFile? image)
    {
        if (image is null || image.Length == 0)
            return DefaultImageName;

        ValidateImage(image);

        var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
        var imageFileName = Guid.NewGuid().ToString("N") + extension;
        var fullPath = SafePath.ResolveUnderRoot(_folderPath, imageFileName)
                       ?? throw new InvalidOperationException("無法儲存圖片。");

        await using Stream fileStream = new FileStream(fullPath, FileMode.Create);
        await image.CopyToAsync(fileStream);
        return imageFileName;
    }

    public void DeleteImage(string? imageName)
    {
        if (imageName is null or DefaultImageName || !UploadRules.IsAllowedImageFileName(imageName))
            return;

        foreach (var root in new[] { _folderPath, _legacyFolderPath })
        {
            var fullPath = SafePath.ResolveUnderRoot(root, imageName);
            if (fullPath != null && File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }

    public async Task<string> EditImage(IFormFile? newImage, string oldImageName)
    {
        if (newImage is null || newImage.Length == 0)
            return oldImageName;

        var newImageName = await AddImage(newImage);
        DeleteImage(oldImageName);
        return newImageName;
    }

    public string? TryResolveImagePath(string? imageName)
    {
        if (!UploadRules.IsAllowedImageFileName(imageName))
            return null;

        foreach (var root in new[] { _folderPath, _legacyFolderPath })
        {
            var fullPath = SafePath.ResolveUnderRoot(root, imageName);
            if (fullPath != null && File.Exists(fullPath))
                return fullPath;
        }

        return null;
    }

    private static void ValidateImage(IFormFile image)
    {
        if (image.Length > UploadRules.MaxImageBytes ||
            !UploadRules.IsAllowedImageFileName(SafePath.GetClientFileName(image.FileName)))
            throw new InvalidOperationException("只接受 jpg、jpeg、png、gif 或 webp 圖片，且大小不可超過 5 MB。");
    }
}

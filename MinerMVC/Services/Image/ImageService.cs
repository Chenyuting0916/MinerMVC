namespace MinerMVC.Services.Image;

public class ImageService : IImageService
{
    private const string DefaultImageName = "default.png";
    private readonly string _folderPath;

    public ImageService(IWebHostEnvironment hostingEnvironment)
    {
        _folderPath = Path.Combine(hostingEnvironment.WebRootPath, "Contents");
    }

    public async Task<string> AddImage(IFormFile? image)
    {
        if (image == null) return DefaultImageName;
        
        var imageFileName = Guid.NewGuid() + image.FileName;
        var fullPath = Path.Combine(_folderPath, imageFileName);
        await using Stream fileStream = new FileStream(fullPath, FileMode.Create);
        await image.CopyToAsync(fileStream);
        return imageFileName;
    }

    public void DeleteImage(string? imageName)
    {
        if (imageName is null or DefaultImageName) return;
        
        var fullPath = Path.Combine(_folderPath, imageName);
        
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    public void EditImage()
    {
    }
}
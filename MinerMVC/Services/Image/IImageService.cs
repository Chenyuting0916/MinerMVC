namespace MinerMVC.Services.Image;

public interface IImageService
{
    public Task<string> AddImage(IFormFile? image);
    public void DeleteImage(string? imageName);
    public Task<string> EditImage(IFormFile? newImage, string oldImageName);
}
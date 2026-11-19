namespace MinerMVC.Security;

public static class UploadRules
{
    public const long MaxImageBytes = 5 * 1024 * 1024;
    public const long MaxPdfBytes = 50 * 1024 * 1024;
    public const long MaxAudioBytes = 25 * 1024 * 1024;
    public const int MaxPdfFiles = 30;

    private static readonly HashSet<string> ImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".webp"
    };

    private static readonly HashSet<string> AudioExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".wav", ".mp3", ".m4a", ".mp4", ".ogg", ".webm", ".flac", ".aac", ".wma"
    };

    public static bool IsAllowedImageFileName(string? fileName)
    {
        return SafePath.IsSafeFileName(fileName)
               && ImageExtensions.Contains(Path.GetExtension(fileName) ?? string.Empty);
    }

    public static bool IsAllowedAudioExtension(string? extension)
    {
        return !string.IsNullOrEmpty(extension)
               && extension.Length <= 10
               && AudioExtensions.Contains(extension);
    }

    public static string ImageContentType(string path)
    {
        return Path.GetExtension(path).ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }
}

namespace MinerMVC.Security;

public static class SafePath
{
    public static string GetClientFileName(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return string.Empty;

        return Path.GetFileName(fileName.Replace('\\', '/'));
    }

    public static bool IsSafeFileName(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return false;

        if (!string.Equals(fileName, Path.GetFileName(fileName), StringComparison.Ordinal))
            return false;

        if (fileName.IndexOfAny(new[] { '/', '\\', ':' }) >= 0)
            return false;

        return !fileName.Contains("..", StringComparison.Ordinal);
    }

    public static string? ResolveUnderRoot(string root, string? fileName)
    {
        if (!IsSafeFileName(fileName))
            return null;

        var rootFull = Path.GetFullPath(root);
        var full = Path.GetFullPath(Path.Combine(rootFull, fileName!));
        var comparison = OperatingSystem.IsWindows()
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;

        var rootPrefix = rootFull.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                         + Path.DirectorySeparatorChar;
        if (!full.StartsWith(rootPrefix, comparison))
            return null;

        return full;
    }
}

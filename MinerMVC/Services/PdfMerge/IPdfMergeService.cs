namespace MinerMVC.Services.PdfMerge
{
    public interface IPdfMergeService
    {
        Task<PdfMergeResult> MergePdfFilesAsync(IEnumerable<IFormFile> pdfFiles);
        
        PdfPreviewInfo GetPdfInfo(string fileName);
        
        void CleanupTemporaryFiles();
        
        bool DeleteFile(string fileName);
    }
    
    public class PdfMergeResult
    {
        public bool IsSuccess { get; set; }
        public string? FileName { get; set; }
        public string? DownloadUrl { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class PdfPreviewInfo
    {
        public bool IsValid { get; set; }
        public int PageCount { get; set; }
        public long FileSizeBytes { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
        
        // 易於閱讀的檔案大小
        public string FormattedFileSize 
        { 
            get 
            {
                if (FileSizeBytes < 1024)
                    return $"{FileSizeBytes} B";
                else if (FileSizeBytes < 1048576)
                    return $"{(FileSizeBytes / 1024.0):F1} KB";
                else
                    return $"{(FileSizeBytes / 1048576.0):F1} MB";
            } 
        }
    }
} 
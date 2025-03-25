namespace MinerMVC.Services.PdfMerge.Models
{
    /// <summary>
    /// PDF合併結果
    /// </summary>
    public class PdfMergeResult
    {
        public bool IsSuccess { get; set; }
        public string? FileName { get; set; }
        public string? DownloadUrl { get; set; }
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// PDF預覽資訊
    /// </summary>
    public class PdfPreviewInfo
    {
        public bool IsValid { get; set; }
        public int PageCount { get; set; }
        public long FileSizeBytes { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
        
        /// <summary>
        /// 易於閱讀的檔案大小
        /// </summary>
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
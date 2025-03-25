using iText.Kernel.Exceptions;

namespace MinerMVC.Services.PdfMerge
{
    // PDF 合併服務實現
    public class PdfMergeService : IPdfMergeService
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly string _uploadsFolder;
        private readonly ILogger<PdfMergeService> _logger;
        private readonly PdfAdapter _pdfAdapter;

        public PdfMergeService(
            IWebHostEnvironment hostEnvironment, 
            ILogger<PdfMergeService> logger,
            PdfAdapter pdfAdapter)
        {
            _hostEnvironment = hostEnvironment;
            _logger = logger;
            _pdfAdapter = pdfAdapter;
            _uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads");
            EnsureUploadsFolderExists();
        }

        // 合併多個 PDF 檔案
        public async Task<PdfMergeResult> MergePdfFilesAsync(IEnumerable<IFormFile> pdfFiles)
        {
            if (!IsValidPdfFileCollection(pdfFiles))
            {
                return CreateErrorResult("請至少上傳兩個PDF檔案");
            }

            List<string> tempFilePaths = new();

            try
            {
                string mergedFileName = GenerateMergedFileName();
                string mergedPdfPath = Path.Combine(_uploadsFolder, mergedFileName);

                _logger.LogInformation("開始合併PDF檔案，輸出路徑: {mergedPdfPath}", mergedPdfPath);
                
                // 上傳並驗證所有檔案
                var validationResult = await UploadAndValidateFilesAsync(pdfFiles, tempFilePaths);
                if (!validationResult.IsSuccess)
                {
                    DeleteTemporaryFiles(tempFilePaths);
                    return validationResult;
                }
                
                // 合併檔案
                bool mergeResult = _pdfAdapter.MergePdfFiles(tempFilePaths, mergedPdfPath);
                
                // 清理臨時文件
                DeleteTemporaryFiles(tempFilePaths);
                
                if (!mergeResult)
                {
                    return CreateErrorResult("合併PDF過程中發生錯誤，請檢查PDF檔案格式是否正確");
                }
                
                // 驗證合併結果
                if (!IsValidOutputFile(mergedPdfPath))
                {
                    return CreateErrorResult("PDF合併過程中發生錯誤，無法產生有效的輸出檔案");
                }

                _logger.LogInformation("PDF合併成功，檔案路徑: {mergedPdfPath}", mergedPdfPath);
                
                return new PdfMergeResult
                {
                    IsSuccess = true,
                    FileName = mergedFileName,
                    DownloadUrl = $"/uploads/{mergedFileName}"
                };
            }
            catch (PdfException ex)
            {
                _logger.LogError(ex, "PDF處理錯誤: {message}", ex.Message);
                DeleteTemporaryFiles(tempFilePaths);
                return CreateErrorResult($"處理PDF時發生錯誤: {ex.Message}");
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "檔案操作錯誤: {message}", ex.Message);
                DeleteTemporaryFiles(tempFilePaths);
                return CreateErrorResult($"檔案處理錯誤: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PDF合併異常: {message}", ex.Message);
                DeleteTemporaryFiles(tempFilePaths);
                return CreateErrorResult($"合併PDF時發生錯誤: {ex.Message}");
            }
        }

        // 獲取 PDF 檔案的詳細資訊
        public PdfPreviewInfo GetPdfInfo(string fileName)
        {
            string filePath = Path.Combine(_uploadsFolder, fileName);
            return _pdfAdapter.GetPdfInfo(filePath);
        }

        // 清理過期臨時文件
        public void CleanupTemporaryFiles()
        {
            try
            {
                if (!Directory.Exists(_uploadsFolder))
                {
                    return;
                }

                var cutoffTime = DateTime.Now.AddHours(-1); // 清理一小時前的文件
                var filesToDelete = Directory.GetFiles(_uploadsFolder)
                    .Select(f => new FileInfo(f))
                    .Where(f => f.CreationTime < cutoffTime)
                    .ToList();

                if (filesToDelete.Count > 0)
                {
                    _logger.LogInformation("清理 {count} 個過期的臨時PDF檔案", filesToDelete.Count);

                    foreach (var file in filesToDelete)
                    {
                        try
                        {
                            file.Delete();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "清理過期檔案時發生錯誤: {fileName}", file.Name);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "清理臨時文件時發生錯誤");
            }
        }
        
        // 刪除指定文件
        public bool DeleteFile(string fileName)
        {
            try
            {
                string filePath = Path.Combine(_uploadsFolder, fileName);
                
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _logger.LogInformation("成功刪除檔案: {fileName}", fileName);
                    return true;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除檔案時發生錯誤: {fileName}", fileName);
                return false;
            }
        }

        // 確保上傳資料夾存在
        private void EnsureUploadsFolderExists()
        {
            if (!Directory.Exists(_uploadsFolder))
            {
                Directory.CreateDirectory(_uploadsFolder);
            }
        }

        // 生成合併後的檔案名稱
        private string GenerateMergedFileName()
        {
            return $"merged_{DateTime.Now:yyyyMMddHHmmss}.pdf";
        }

        // 保存上傳的檔案
        private async Task<string> SaveUploadedFileAsync(IFormFile pdfFile)
        {
            string tempFilePath = Path.Combine(_uploadsFolder, GetSafeFileName(pdfFile.FileName));
            
            using var fileStream = new FileStream(tempFilePath, FileMode.Create);
            await pdfFile.CopyToAsync(fileStream);
            
            return tempFilePath;
        }

        // 刪除臨時檔案
        private void DeleteTemporaryFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "刪除臨時檔案時發生錯誤");
                }
            }
        }
        
        // 刪除多個臨時檔案
        private void DeleteTemporaryFiles(IEnumerable<string> filePaths)
        {
            foreach (var filePath in filePaths)
            {
                DeleteTemporaryFile(filePath);
            }
        }

        // 生成安全的檔案名稱
        private string GetSafeFileName(string fileName)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);
            string uniqueIdentifier = Guid.NewGuid().ToString()[..8];
            
            return $"{fileNameWithoutExtension}_{uniqueIdentifier}{extension}";
        }

        // 檢查 PDF 檔案集合是否有效
        private bool IsValidPdfFileCollection(IEnumerable<IFormFile> pdfFiles)
        {
            return pdfFiles != null && pdfFiles.Any() && pdfFiles.Count() >= 2;
        }
        
        // 創建錯誤結果
        private PdfMergeResult CreateErrorResult(string errorMessage)
        {
            return new PdfMergeResult
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }
        
        // 上傳並驗證所有檔案
        private async Task<PdfMergeResult> UploadAndValidateFilesAsync(
            IEnumerable<IFormFile> pdfFiles, 
            List<string> tempFilePaths)
        {
            foreach (var pdfFile in pdfFiles)
            {
                string tempFilePath = await SaveUploadedFileAsync(pdfFile);
                
                if (!_pdfAdapter.ValidatePdf(tempFilePath))
                {
                    return CreateErrorResult($"檔案 '{pdfFile.FileName}' 不是有效的PDF格式或已損壞");
                }
                
                tempFilePaths.Add(tempFilePath);
            }
            
            return new PdfMergeResult { IsSuccess = true };
        }
        
        // 檢查合併後的檔案是否有效
        private bool IsValidOutputFile(string filePath)
        {
            return File.Exists(filePath) && new FileInfo(filePath).Length > 0;
        }
    }
} 
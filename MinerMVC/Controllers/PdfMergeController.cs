using Microsoft.AspNetCore.Mvc;
using MinerMVC.Security;
using MinerMVC.Services.PdfMerge;

namespace MinerMVC.Controllers
{
    // PDF 合併控制器
    public class PdfMergeController : Controller
    {
        private readonly IPdfMergeService _pdfMergeService;
        private readonly ILogger<PdfMergeController> _logger;
        
        // 用於定期清理的設置
        private static readonly TimeSpan FileCleanupAge = TimeSpan.FromHours(1); // 文件保留時間
        private static readonly object CleanupLock = new();
        private static DateTime _lastCleanupTime = DateTime.MinValue;

        public PdfMergeController(
            IPdfMergeService pdfMergeService, 
            ILogger<PdfMergeController> logger)
        {
            _pdfMergeService = pdfMergeService;
            _logger = logger;
        }

        // 首頁
        public IActionResult Index()
        {
            // 嘗試清理過期文件
            TryCleanupTemporaryFiles();
            return View();
        }

        // 合併 PDF 檔案
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(100 * 1024 * 1024)] // 限制上傳大小為100MB
        public async Task<IActionResult> Merge(List<IFormFile> pdfFiles)
        {
            if (pdfFiles == null || pdfFiles.Count < 2)
            {
                ViewBag.ErrorMessage = "請至少上傳兩個PDF檔案";
                return View("Index");
            }

            try
            {
                var result = await _pdfMergeService.MergePdfFilesAsync(pdfFiles);

                if (!result.IsSuccess)
                {
                    ViewBag.ErrorMessage = result.ErrorMessage;
                    return View("Index");
                }

                if (result.FileName == null || !IsGeneratedFileExists(result.FileName))
                {
                    return View("Index");
                }

                SetSuccessViewBag(result);
                return View("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PDF合併過程中發生未處理的異常");
                ViewBag.ErrorMessage = "合併PDF時發生錯誤，請稍後再試";
                return View("Index");
            }
        }

        // 檢查生成的檔案是否存在
        private bool IsGeneratedFileExists(string fileName)
        {
            var filePath = _pdfMergeService.ResolveUploadPath(fileName);
            if (filePath == null || !System.IO.File.Exists(filePath))
            {
                _logger.LogWarning("合併後的檔案不存在: {filePath}", filePath);
                ViewBag.ErrorMessage = "合併後的檔案不存在，請重試";
                return false;
            }
            
            return true;
        }
        
        // 設置成功的ViewBag數據
        private void SetSuccessViewBag(PdfMergeResult result)
        {
            ViewBag.SuccessMessage = "PDF檔案合併成功！";
            ViewBag.DownloadUrl = result.DownloadUrl;
            ViewBag.FileName = result.FileName;
            ViewBag.PreviewUrl = $"/PdfMerge/Preview?fileName={result.FileName}";
            ViewBag.DeleteAfterDownload = true;  // 提示用戶檔案會在下載後刪除
        }

        [HttpGet]
        public IActionResult Preview(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest("未指定檔案名稱");
            }

            try
            {
                var pdfPath = _pdfMergeService.ResolveUploadPath(fileName);
                if (pdfPath == null || !System.IO.File.Exists(pdfPath))
                {
                    _logger.LogWarning("請求預覽的PDF檔案不存在: {fileName}", fileName);
                    return NotFound("找不到指定的PDF檔案");
                }

                // 記錄PDF預覽記錄以進行審計
                _logger.LogInformation("User {ip} previewed file: {fileName}", 
                    HttpContext.Connection.RemoteIpAddress, fileName);

                return PhysicalFile(pdfPath, "application/pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "預覽PDF檔案時發生錯誤: {fileName}", fileName);
                return StatusCode(500, "預覽PDF檔案時發生錯誤");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(UploadRules.MaxPdfBytes)]
        public async Task<IActionResult> UploadForPreview(IFormFile pdfFile)
        {
            if (pdfFile == null || pdfFile.Length == 0)
            {
                return Json(new { success = false, message = "未提供檔案或檔案為空" });
            }

            if (pdfFile.Length > UploadRules.MaxPdfBytes ||
                !SafePath.GetClientFileName(pdfFile.FileName).EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return Json(new { success = false, message = "只支援 50 MB 以內的 PDF 檔案" });
            }

            try
            {
                Directory.CreateDirectory(_pdfMergeService.UploadsDirectory);

                string uniqueFileName = $"preview_{Guid.NewGuid():N}.pdf";
                var filePath = _pdfMergeService.ResolveUploadPath(uniqueFileName);
                if (filePath == null)
                {
                    return Json(new { success = false, message = "無法儲存預覽檔案" });
                }

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await pdfFile.CopyToAsync(fileStream);
                }
                
                // 記錄檔案上傳資訊以進行審計
                _logger.LogInformation("User {ip} uploaded file for preview: {originalName}, saved as {uniqueName}", 
                    HttpContext.Connection.RemoteIpAddress, pdfFile.FileName, uniqueFileName);

                return Json(new { 
                    success = true, 
                    fileName = uniqueFileName,
                    previewUrl = $"/PdfMerge/Preview?fileName={uniqueFileName}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "上傳PDF預覽檔案時發生錯誤: {fileName}", pdfFile.FileName);
                return Json(new { success = false, message = "上傳預覽檔案時發生錯誤，請稍後再試" });
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return Json(new { success = false, message = "檔案名稱不能為空" });
            }
            
            try
            {
                var filePath = _pdfMergeService.ResolveUploadPath(fileName);
                if (filePath != null && System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    _logger.LogInformation("用戶刪除檔案: {fileName}", fileName);
                    return Json(new { success = true });
                }
                
                return Json(new { success = false, message = "找不到指定的檔案" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除檔案時發生錯誤: {fileName}", fileName);
                return Json(new { success = false, message = "刪除檔案時發生錯誤，請稍後再試" });
            }
        }

        // 獲取PDF文件的詳細信息
        [HttpGet]
        public IActionResult GetPdfInfo(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return Json(new { success = false, message = "文件名不能為空" });
            }
            
            try
            {
                var filePath = _pdfMergeService.ResolveUploadPath(fileName);
                if (filePath == null || !System.IO.File.Exists(filePath))
                {
                    return Json(new { success = false, message = "文件不存在" });
                }
                
                var pdfInfo = _pdfMergeService.GetPdfInfo(fileName);
                return Json(new 
                { 
                    success = true, 
                    info = pdfInfo 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取PDF信息時發生錯誤: {fileName}", fileName);
                return Json(new { success = false, message = "取得PDF資訊時發生錯誤，請稍後再試" });
            }
        }

        // 清理過期的臨時檔案，避免檔案外流風險
        private void TryCleanupTemporaryFiles()
        {
            // 使用鎖，確保同一時間只有一個請求執行清理
            if (!Monitor.TryEnter(CleanupLock))
            {
                return;
            }

            try
            {
                // 檢查是否需要執行清理（每小時最多執行一次）
                var now = DateTime.Now;
                if ((now - _lastCleanupTime).TotalMinutes < 60)
                {
                    return;
                }

                _lastCleanupTime = now;
                string uploadsFolder = _pdfMergeService.UploadsDirectory;
                
                if (!Directory.Exists(uploadsFolder))
                {
                    return;
                }

                var cutoffTime = now.Subtract(FileCleanupAge);
                var filesToDelete = Directory.GetFiles(uploadsFolder)
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
            finally
            {
                Monitor.Exit(CleanupLock);
            }
        }

        // 下載檔案
        [HttpGet]
        public IActionResult Download(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest("未指定檔案名稱");
            }

            try
            {
                var filePath = _pdfMergeService.ResolveUploadPath(fileName);
                if (filePath == null || !System.IO.File.Exists(filePath))
                {
                    _logger.LogWarning("請求下載的PDF檔案不存在: {fileName}", fileName);
                    return NotFound("找不到指定的PDF檔案");
                }

                // 記錄下載記錄以進行審計
                _logger.LogInformation("User {ip} downloaded file: {fileName}", 
                    HttpContext.Connection.RemoteIpAddress, fileName);
                
                return PhysicalFile(filePath, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "下載PDF檔案時發生錯誤: {fileName}", fileName);
                return StatusCode(500, "下載PDF檔案時發生錯誤");
            }
        }
    }
} 
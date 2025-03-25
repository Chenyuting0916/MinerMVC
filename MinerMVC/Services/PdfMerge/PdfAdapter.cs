using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using Microsoft.Extensions.Logging;
using MinerMVC.Services.PdfMerge.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace MinerMVC.Services.PdfMerge
{
    // PDF 處理適配器，隔離 iText API 調用以便於未來升級和異常處理
    public class PdfAdapter
    {
        private readonly ILogger<PdfAdapter> _logger;

        public PdfAdapter(ILogger<PdfAdapter> logger)
        {
            _logger = logger;
        }

        // 檢查 PDF 檔案是否有效
        public bool ValidatePdf(string filePath)
        {
            try
            {
                using var reader = new PdfReader(filePath);
                using var doc = new PdfDocument(reader);
                return doc.GetNumberOfPages() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "PDF驗證失敗");
                return false;
            }
        }

        // 合併多個 PDF 檔案
        public bool MergePdfFiles(IEnumerable<string> inputFiles, string outputPath)
        {
            try
            {
                using var writer = new PdfWriter(outputPath);
                using var pdfDoc = new PdfDocument(writer);
                var merger = new PdfMerger(pdfDoc);

                foreach (var file in inputFiles)
                {
                    if (!ValidatePdf(file))
                    {
                        continue;
                    }

                    using var reader = new PdfReader(file);
                    using var sourceDoc = new PdfDocument(reader);
                    merger.Merge(sourceDoc, 1, sourceDoc.GetNumberOfPages());
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // 獲取 PDF 頁數
        public int GetPageCount(string filePath)
        {
            try
            {
                using var reader = new PdfReader(filePath);
                using var doc = new PdfDocument(reader);
                return doc.GetNumberOfPages();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        // 獲取 PDF 檔案的詳細資訊
        public PdfPreviewInfo GetPdfInfo(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return new PdfPreviewInfo 
                    { 
                        IsValid = false,
                        ErrorMessage = "檔案不存在"
                    };
                }
                
                var fileInfo = new FileInfo(filePath);
                
                using var reader = new PdfReader(filePath);
                using var doc = new PdfDocument(reader);
                
                return new PdfPreviewInfo
                {
                    IsValid = true,
                    PageCount = doc.GetNumberOfPages(),
                    FileSizeBytes = fileInfo.Length,
                    FileName = Path.GetFileName(filePath)
                };
            }
            catch (Exception ex)
            {
                return new PdfPreviewInfo 
                { 
                    IsValid = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
} 
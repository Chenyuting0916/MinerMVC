using Microsoft.AspNetCore.Http;

namespace MinerMVC.Services.Transcription
{
    public interface ITranscriptionService
    {
        /// <summary>
        /// 檢查是否有任何語音模型存在
        /// </summary>
        /// <returns>如果至少有一個模型存在，則返回 true</returns>
        bool IsAnyModelAvailable();
        
        /// <summary>
        /// 轉換語音文件到文字
        /// </summary>
        /// <param name="audioFile">音訊檔案</param>
        /// <param name="language">語言代碼</param>
        /// <returns>轉換結果</returns>
        Task<TranscriptionResult> TranscribeAsync(IFormFile audioFile, string language);
        
        /// <summary>
        /// 獲取特定語言的名稱
        /// </summary>
        /// <param name="languageCode">語言代碼</param>
        /// <returns>語言名稱</returns>
        string GetLanguageName(string languageCode);
    }
    
    /// <summary>
    /// 語音轉文字結果
    /// </summary>
    public class TranscriptionResult
    {
        public bool Success { get; set; }
        public string Text { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }
} 
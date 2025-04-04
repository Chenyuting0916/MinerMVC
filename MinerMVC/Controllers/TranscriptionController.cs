using Microsoft.AspNetCore.Mvc;
using MinerMVC.Services.Transcription;

namespace MinerMVC.Controllers
{
    public class TranscriptionController : Controller
    {
        private readonly ITranscriptionService _transcriptionService;
        private readonly ILogger<TranscriptionController> _logger;

        public TranscriptionController(ITranscriptionService transcriptionService, ILogger<TranscriptionController> logger)
        {
            _transcriptionService = transcriptionService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.ModelExists = _transcriptionService.IsAnyModelAvailable();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Transcribe(IFormFile audioFile, string language)
        {
            try
            {
                var result = await _transcriptionService.TranscribeAsync(audioFile, language);
                
                // 正確處理從 Vosk 返回的文本
                if (result.Success && !string.IsNullOrEmpty(result.Text))
                {
                    // 替換換行符以正確在前端顯示
                    result.Text = result.Text.Replace("\r\n", "\n").Replace("\r", "\n");
                }
                
                return Json(new { success = result.Success, text = result.Text, error = result.ErrorMessage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "處理語音轉文字請求時發生錯誤");
                return Json(new { success = false, error = "處理請求時發生未預期的錯誤" });
            }
        }
    }
} 
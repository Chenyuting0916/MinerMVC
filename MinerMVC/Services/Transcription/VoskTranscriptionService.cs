using Microsoft.AspNetCore.Http;
using NAudio.Wave;
using System.Text;
using Vosk;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Linq;
using System.Collections.Generic;

namespace MinerMVC.Services.Transcription
{
    public class VoskTranscriptionService : ITranscriptionService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<VoskTranscriptionService> _logger;
        private readonly Dictionary<string, string> _modelPaths;

        public VoskTranscriptionService(IWebHostEnvironment environment, ILogger<VoskTranscriptionService> logger)
        {
            _environment = environment;
            _logger = logger;
            
            // 定義支援的語言及其正確的模型路徑
            _modelPaths = new Dictionary<string, string>
            {
                { "zh-CN", Path.Combine(_environment.WebRootPath, "vosk-model-small-cn") },
                { "en-US", Path.Combine(_environment.WebRootPath, "vosk-model-small-en") },
                { "ja-JP", Path.Combine(_environment.WebRootPath, "vosk-model-small-jp") }
            };
        }

        /// <inheritdoc />
        public bool IsAnyModelAvailable()
        {
            foreach (var modelPath in _modelPaths.Values)
            {
                if (IsModelAvailable(modelPath))
                {
                    return true;
                }
            }
            return false;
        }

        /// <inheritdoc />
        public async Task<TranscriptionResult> TranscribeAsync(IFormFile audioFile, string language)
        {
            // 驗證輸入
            if (audioFile == null || audioFile.Length == 0)
            {
                return new TranscriptionResult
                {
                    Success = false,
                    ErrorMessage = "請上傳音訊檔案"
                };
            }
            
            // 驗證語言選擇
            if (string.IsNullOrEmpty(language) || !_modelPaths.ContainsKey(language))
            {
                return new TranscriptionResult
                {
                    Success = false,
                    ErrorMessage = "不支援的語言"
                };
            }
            
            string modelPath = _modelPaths[language];
            
            // 確保模型存在
            if (!IsModelAvailable(modelPath))
            {
                var errorMsg = $"未找到{GetLanguageName(language)}語音模型，請先下載";
                _logger.LogError(errorMsg);
                return new TranscriptionResult
                {
                    Success = false,
                    ErrorMessage = errorMsg
                };
            }
            
            _logger.LogInformation($"開始處理 {audioFile.FileName}（{GetLanguageName(language)}）");
            
            // 臨時檔案路徑
            var tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + Path.GetExtension(audioFile.FileName));
            var wavFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".wav");
            
            try
            {
                // 儲存上傳的檔案
                using (var stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await audioFile.CopyToAsync(stream);
                }

                // 轉換音訊為 WAV 格式
                using (var reader = new MediaFoundationReader(tempFilePath))
                {
                    // 如果需要，轉換為適合語音辨識的格式
                    if (reader.WaveFormat.SampleRate != 16000 || 
                        reader.WaveFormat.Channels != 1 || 
                        reader.WaveFormat.BitsPerSample != 16)
                    {
                        _logger.LogInformation("轉換音頻格式...");
                        using (var resampler = new MediaFoundationResampler(
                            reader, new WaveFormat(16000, 16, 1)))
                        {
                            WaveFileWriter.CreateWaveFile(wavFilePath, resampler);
                        }
                    }
                    else
                    {
                        WaveFileWriter.CreateWaveFile(wavFilePath, reader);
                    }
                }
                
                _logger.LogInformation("開始語音識別...");
                string recognizedText = await RecognizeSpeechAsync(wavFilePath, modelPath);
                
                if (string.IsNullOrWhiteSpace(recognizedText))
                {
                    _logger.LogWarning("無法識別語音內容");
                    return new TranscriptionResult
                    {
                        Success = true,
                        Text = "未能識別語音內容，請確認音頻質量並重試。"
                    };
                }
                
                _logger.LogInformation("語音識別成功完成");
                return new TranscriptionResult
                {
                    Success = true,
                    Text = recognizedText
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "語音識別過程中發生錯誤");
                return new TranscriptionResult
                {
                    Success = false,
                    ErrorMessage = $"處理過程中發生錯誤: {ex.Message}"
                };
            }
            finally
            {
                // 清理臨時檔案
                CleanupFiles(tempFilePath, wavFilePath);
            }
        }

        /// <inheritdoc />
        public string GetLanguageName(string languageCode)
        {
            return languageCode switch
            {
                "zh-CN" => "中文",
                "en-US" => "英文",
                "ja-JP" => "日文",
                _ => languageCode
            };
        }
        
        #region 私有方法
        
        /// <summary>
        /// 檢查模型是否可用
        /// </summary>
        private bool IsModelAvailable(string modelPath)
        {
            if (!Directory.Exists(modelPath))
            {
                return false;
            }
            
            // 檢查模型資料夾中是否有必要的子目錄和檔案
            try
            {
                var requiredDirs = new[] { "am", "conf", "graph", "ivector" };
                foreach (var dir in requiredDirs)
                {
                    var path = Path.Combine(modelPath, dir);
                    if (!Directory.Exists(path))
                    {
                        return false;
                    }
                }
                
                // 檢查模型檔案
                return File.Exists(Path.Combine(modelPath, "README"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "檢查模型資料夾時發生錯誤");
                return false;
            }
        }
        
        /// <summary>
        /// 使用 Vosk 進行語音識別
        /// </summary>
        private async Task<string> RecognizeSpeechAsync(string wavFilePath, string modelPath)
        {
            return await Task.Run(() =>
            {
                _logger.LogInformation($"開始處理音檔，使用模型：{Path.GetFileName(modelPath)}");
                var startTime = DateTime.Now;
                var result = new StringBuilder();
                
                try
                {
                    _logger.LogInformation("載入模型中...");
                    using (var model = new Model(modelPath))
                    {
                        _logger.LogInformation("模型載入完成");
                        // 根據語言選擇適當的參數
                        float sampleRate = 16000.0f;
                        
                        using (var recognizer = new VoskRecognizer(model, sampleRate))
                        {
                            recognizer.SetMaxAlternatives(5);  // 增加可能的替代結果數量
                            recognizer.SetWords(true);         // 獲取單詞時間戳
                            
                            // 特別針對日文模型進行設置
                            if (modelPath.Contains("jp"))
                            {
                                recognizer.SetPartialWords(true);
                            }
                            
                            var buffer = new byte[8192]; // 增加緩衝區大小以提高效率
                            
                            using (var waveFile = new WaveFileReader(wavFilePath))
                            {
                                long totalBytes = waveFile.Length;
                                long processedBytes = 0;
                                int bytesRead;
                                bool hasResults = false;
                                
                                while ((bytesRead = waveFile.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    processedBytes += bytesRead;
                                    if (processedBytes % (totalBytes / 5) < buffer.Length) // 減少進度更新頻率
                                    {
                                        _logger.LogInformation($"處理進度：{(double)processedBytes / totalBytes:P0}");
                                    }
                                    
                                    if (recognizer.AcceptWaveform(buffer, bytesRead))
                                    {
                                        var jsonResult = recognizer.Result();
                                        AppendTextFromJson(result, jsonResult);
                                        hasResults = true;
                                    }
                                    else
                                    {
                                        var partialResult = recognizer.PartialResult();
                                        // 不記錄部分結果
                                    }
                                }
                                
                                // 獲取最後的結果
                                var finalResult = recognizer.FinalResult();
                                AppendTextFromJson(result, finalResult);
                                
                                if (!hasResults && string.IsNullOrWhiteSpace(result.ToString()))
                                {
                                    _logger.LogWarning("未獲得任何識別結果，可能是音頻內容不匹配語言模型或音頻質量問題");
                                }
                            }
                        }
                    }
                    
                    var finalText = result.ToString().Trim();
                    var duration = DateTime.Now - startTime;
                    _logger.LogInformation($"語音識別完成，耗時：{duration.TotalSeconds:F1}秒");
                    
                    return finalText;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "語音識別過程中發生異常");
                    throw;
                }
            });
        }
        
        /// <summary>
        /// 從 JSON 響應中提取文本
        /// </summary>
        private void AppendTextFromJson(StringBuilder builder, string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return;
            }
            
            try
            {
                // 使用System.Text.Json正確解析JSON，處理Unicode字元
                using (JsonDocument document = JsonDocument.Parse(json))
                {
                    string text = null;
                    
                    // 優先取得text欄位
                    if (document.RootElement.TryGetProperty("text", out JsonElement textElement) && 
                        textElement.ValueKind == JsonValueKind.String)
                    {
                        text = textElement.GetString();
                    }
                    // 若沒有text欄位，檢查alternatives陣列
                    else if (document.RootElement.TryGetProperty("alternatives", out JsonElement alternativesElement) && 
                             alternativesElement.ValueKind == JsonValueKind.Array && 
                             alternativesElement.GetArrayLength() > 0)
                    {
                        // 取第一個最高可信度的結果
                        var firstAlternative = alternativesElement[0];
                        if (firstAlternative.TryGetProperty("text", out JsonElement altTextElement) && 
                            altTextElement.ValueKind == JsonValueKind.String)
                        {
                            text = altTextElement.GetString();
                        }
                    }
                    // 檢查partial欄位
                    else if (document.RootElement.TryGetProperty("partial", out JsonElement partialElement) && 
                             partialElement.ValueKind == JsonValueKind.String)
                    {
                        text = partialElement.GetString();
                        // 部分結果只添加文本而不換行
                        if (!string.IsNullOrEmpty(text))
                        {
                            builder.Append(text + " ");
                        }
                        return;
                    }
                    
                    if (!string.IsNullOrEmpty(text))
                    {
                        ProcessAndFormatText(builder, text);
                    }
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "解析JSON時發生錯誤");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "處理JSON時發生未知錯誤");
            }
        }
        
        /// <summary>
        /// 處理和格式化文本，包括智能分段
        /// </summary>
        private void ProcessAndFormatText(StringBuilder builder, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }
            
            // 首先移除開頭結尾的空白
            text = text.Trim();
            
            // 針對不同語言的特殊處理
            if (ContainsJapanese(text))
            {
                // 日語處理：根據常見句尾詞進行分割
                var segments = SplitJapaneseText(text);
                foreach (var segment in segments)
                {
                    AddTextWithLineBreaks(builder, segment);
                }
            }
            else
            {
                // 其他語言（英文、中文等）使用原有的方式
                AddTextWithLineBreaks(builder, text);
            }
        }
        
        /// <summary>
        /// 判斷文本是否包含日文字符
        /// </summary>
        private bool ContainsJapanese(string text)
        {
            // 日文平假名範圍：U+3040 - U+309F
            // 日文片假名範圍：U+30A0 - U+30FF
            return text.Any(c => (c >= '\u3040' && c <= '\u309F') || (c >= '\u30A0' && c <= '\u30FF'));
        }
        
        /// <summary>
        /// 智能分割日文文本
        /// </summary>
        private List<string> SplitJapaneseText(string text)
        {
            List<string> segments = new List<string>();
            
            // 先按空格分割成詞
            string[] words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            if (words.Length == 0)
            {
                return segments;
            }
            
            // 日文句子結束詞（常用的是表示結束的詞）
            string[] strongEndWords = { "です", "ます", "ました", "でした", "ません", "ございます", "くださいませ", "ですか" };
            
            // 次要的句尾詞（可能表示句子結束，但不一定是段落結束）
            string[] weakEndWords = { "ください", "なく", "ない", "なし", "だ", "た", "ている", "ていた", "とき", "もの", "こと" };
            
            StringBuilder currentSegment = new StringBuilder();
            int wordCount = 0;
            
            foreach (string word in words)
            {
                currentSegment.Append(word + " ");
                wordCount++;
                
                // 檢查是否達到分段條件
                bool shouldSplit = false;
                bool isStrongBreak = false;
                
                // 1. 詞數達到一定數量
                if (wordCount >= 12)
                {
                    shouldSplit = true;
                }
                
                // 2. 詞以日文強句尾詞結束（段落分隔）
                foreach (string endWord in strongEndWords)
                {
                    if (word.EndsWith(endWord))
                    {
                        shouldSplit = true;
                        isStrongBreak = true;
                        break;
                    }
                }
                
                // 3. 詞以日文弱句尾詞結束（句子分隔）
                if (!shouldSplit)
                {
                    foreach (string endWord in weakEndWords)
                    {
                        if (word.EndsWith(endWord) && wordCount >= 5)
                        {
                            shouldSplit = true;
                            break;
                        }
                    }
                }
                
                // 如果應該分段
                if (shouldSplit)
                {
                    string segment = currentSegment.ToString().Trim();
                    segments.Add(segment);
                    
                    // 如果是強句尾詞，添加一個空段落以增加視覺間隔
                    if (isStrongBreak && segments.Count > 0)
                    {
                        segments.Add("");
                    }
                    
                    currentSegment.Clear();
                    wordCount = 0;
                }
            }
            
            // 添加最後一個片段（如果有的話）
            if (currentSegment.Length > 0)
            {
                segments.Add(currentSegment.ToString().Trim());
            }
            
            return segments;
        }
        
        /// <summary>
        /// 添加文本並在句子結束時添加換行
        /// </summary>
        private void AddTextWithLineBreaks(StringBuilder builder, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                // 添加空行
                builder.AppendLine();
                return;
            }
            
            // 日文句子通常以句號、問號或驚嘆號結束
            char[] japaneseEndMarks = new char[] { '。', '？', '！', '?', '!', '.', '…' };
            
            // 檢查文本是否已經以句子結束符號結尾
            bool endsWithMark = text.Length > 0 && japaneseEndMarks.Contains(text[text.Length - 1]);
            
            // 如果文本中包含特定的日語詞彙，可能表示句子的結束
            string[] japaneseEndWords = new string[] { "です", "ます", "ました", "でした", "ません", "ください" };
            
            // 處理文本中沒有標點符號的情況
            if (!endsWithMark && text.Length > 0)
            {
                // 檢查文本是否以日語句尾詞結束
                foreach (var endWord in japaneseEndWords)
                {
                    if (text.EndsWith(endWord))
                    {
                        endsWithMark = true;
                        break;
                    }
                }
            }
            
            builder.Append(text);
            
            // 添加換行
            builder.AppendLine();
        }
        
        /// <summary>
        /// 清理臨時檔案
        /// </summary>
        private void CleanupFiles(params string[] filePaths)
        {
            foreach (var path in filePaths)
            {
                try
                {
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "刪除臨時檔案時發生錯誤: {FilePath}", path);
                }
            }
        }
        
        #endregion
    }
} 
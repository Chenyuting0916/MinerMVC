# TimeMiner

TimeMiner 是一個現代化的工作時間管理工具，提供直觀的介面和強大的功能，幫助您更好地管理和追蹤工作時間。

🌐 **生產環境**: [timeminer.azurewebsites.net](https://timeminer.azurewebsites.net/)

## 功能特點

- 📊 自定義 Excel 表格管理
- ⏲️ 工作時間追蹤器
- 🌓 深色/淺色主題切換
- 📱 響應式設計

## 技術棧

- ASP.NET Core MVC
- Entity Framework Core
- Bootstrap 5
- jQuery
- Boxicons

## 開始使用

### 系統要求

- .NET 6.0 SDK 或更高版本
- SQL Server（LocalDB 或完整版本）
- Node.js（可選，用於前端開發）

### 安裝步驟

1. 克隆倉庫：
```bash
git clone https://github.com/yourusername/TimeMiner.git
```

2. 進入專案目錄：
```bash
cd TimeMiner
```

3. 還原依賴項：
```bash
dotnet restore
```

4. 更新數據庫：
```bash
dotnet ef database update
```

5. 運行應用：
```bash
dotnet run
```

應用將在 `https://localhost:5001` 和 `http://localhost:5000` 運行。

## 配置

1. 數據庫連接字符串在 `appsettings.json` 中配置
2. 文件上傳路徑配置在 `appsettings.json` 中的 `FileUploadSettings` 部分

## 功能模塊

### Custom Excel
- 創建和管理自定義表格
- 上傳圖片附件
- 驗證狀態管理

### Timer
- 工作時間追蹤
- 自動保存進度
- 統計報表

## 貢獻

歡迎提交 Pull Requests 來改進這個專案。對於重大更改，請先開 issue 討論您想要改變的內容。

## 授權

本專案採用 MIT 授權 - 查看 [LICENSE](LICENSE) 文件了解詳情。

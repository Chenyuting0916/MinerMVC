# TimeMiner

[English](#english) | [繁體中文](#繁體中文)

## English

TimeMiner is a modern work time management tool offering an intuitive interface and powerful features to help you better manage and track your work time.

🌐 **Production Environment**: [timeminer.azurewebsites.net](https://timeminer.azurewebsites.net/)

### Features

- 📊 Custom Excel table management
- ⏲️ Work time tracker
- 🌓 Dark/Light theme toggle
- 📱 Responsive design

### Tech Stack

- ASP.NET Core MVC
- Entity Framework Core
- Bootstrap 5
- jQuery
- Boxicons

### Getting Started

#### System Requirements

- .NET 6.0 SDK or higher
- SQL Server (LocalDB or full version)
- Node.js (optional, for frontend development)

#### Installation Steps

1. Clone the repository:
```bash
git clone https://github.com/yourusername/TimeMiner.git
```

2. Navigate to the project directory:
```bash
cd TimeMiner
```

3. Restore dependencies:
```bash
dotnet restore
```

4. Copy `MinerMVC/appsettings.Development.json.example` to `MinerMVC/appsettings.Development.json`, then update the database:
```bash
dotnet ef database update
```

5. Run the application:
```bash
dotnet run
```

The application will run at `https://localhost:5001` and `http://localhost:5000`.

### Configuration

Do not commit passwords or production connection strings. `appsettings.json` keeps the connection strings empty.

1. Local database: copy `MinerMVC/appsettings.Development.json.example` to `MinerMVC/appsettings.Development.json` (this file is gitignored) and change the local password.
2. Azure App Service application settings (set the values in Azure, not in git):
   - `ConnectionStrings__CustomExcelDbContext`
   - `ConnectionStrings__DailyTaskDbContext`
   - `Auth__Username`
   - `Auth__Password`

   Portal connection strings named `CustomExcelDbContext` and `DailyTaskDbContext` are also read.
3. Custom Excel and Daily Task require that login. PDF merge, timer, and transcription stay available without it.

### Modules

#### Custom Excel
- Create and manage custom tables
- Upload image attachments
- Verification status management

#### Timer
- Work time tracking
- Automatic progress saving
- Statistical reports

### Contributing

Pull requests are welcome to improve this project. For major changes, please open an issue first to discuss what you would like to change.

### License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 繁體中文

TimeMiner 是一個現代化的工作時間管理工具，提供直觀的介面和強大的功能，幫助您更好地管理和追蹤工作時間。

🌐 **生產環境**: [timeminer.azurewebsites.net](https://timeminer.azurewebsites.net/)

### 功能特點

- 📊 自定義 Excel 表格管理
- ⏲️ 工作時間追蹤器
- 🌓 深色/淺色主題切換
- 📱 響應式設計

### 技術棧

- ASP.NET Core MVC
- Entity Framework Core
- Bootstrap 5
- jQuery
- Boxicons

### 開始使用

#### 系統要求

- .NET 6.0 SDK 或更高版本
- SQL Server（LocalDB 或完整版本）
- Node.js（可選，用於前端開發）

#### 安裝步驟

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

4. 將 `MinerMVC/appsettings.Development.json.example` 複製為 `MinerMVC/appsettings.Development.json`，再更新數據庫：
```bash
dotnet ef database update
```

5. 運行應用：
```bash
dotnet run
```

應用將在 `https://localhost:5001` 和 `http://localhost:5000` 運行。

### 配置

不要把密碼或正式環境連線字串提交到 git。`appsettings.json` 裡的連線字串保持空白。

1. 本機資料庫：把 `MinerMVC/appsettings.Development.json.example` 複製成 `MinerMVC/appsettings.Development.json`（此檔已被 git 忽略），並改掉本機密碼。
2. Azure App Service 應用程式設定（值只放在 Azure）：
   - `ConnectionStrings__CustomExcelDbContext`
   - `ConnectionStrings__DailyTaskDbContext`
   - `Auth__Username`
   - `Auth__Password`

   入口網站「連接字串」若名稱是 `CustomExcelDbContext` 與 `DailyTaskDbContext`，程式也會讀取。
3. Custom Excel 與 Daily Task 需要上述登入。PDF 合併、計時器與語音轉文字不需登入。

### 功能模塊

#### Custom Excel
- 創建和管理自定義表格
- 上傳圖片附件
- 驗證狀態管理

#### Timer
- 工作時間追蹤
- 自動保存進度
- 統計報表

### 貢獻

歡迎提交 Pull Requests 來改進這個專案。對於重大更改，請先開 issue 討論您想要改變的內容。

### 授權

本專案採用 MIT 授權 - 查看 [LICENSE](LICENSE) 文件了解詳情。

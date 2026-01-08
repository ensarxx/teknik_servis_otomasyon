# Otomatik Arıza Teşhisi - Yapılandırma Örnekleri

## 1. PowerShell Kurulum Scripti

Dosya: `setup_openrouter.ps1`

```powershell
# OpenRouter API Anahtarını ayarla
param(
    [string]$ApiKey = ""
)

if ([string]::IsNullOrWhiteSpace($ApiKey)) {
    Write-Host "Kullanım: .\setup_openrouter.ps1 -ApiKey 'sk-or-v1-xxxxxxxxxxxxx'"
    exit 1
}

# Admin kontrolü
if (-not ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Write-Host "Bu script'i Administrator olarak çalıştırmalısınız!" -ForegroundColor Red
    exit 1
}

# Environment variable'ı ayarla
[Environment]::SetEnvironmentVariable("OPENROUTER_API_KEY", $ApiKey, "User")

Write-Host "✓ OPENROUTER_API_KEY başarıyla ayarlandı" -ForegroundColor Green
Write-Host "! Lütfen bilgisayarı yeniden başlatın veya IDE'yi kapatıp açın" -ForegroundColor Yellow
```

**Kullanım:**
```powershell
.\setup_openrouter.ps1 -ApiKey "sk-or-v1-xxxxxxxxxxxxx"
```

## 2. Batch Kurulum Scripti

Dosya: `setup_openrouter.bat`

```batch
@echo off
setlocal enabledelayedexpansion

if "%1"=="" (
    echo Kullanım: setup_openrouter.bat "sk-or-v1-xxxxxxxxxxxxx"
    exit /b 1
)

setx OPENROUTER_API_KEY %1

if %errorlevel% equ 0 (
    echo.
    echo ✓ OPENROUTER_API_KEY başarıyla ayarlandı
    echo.
    echo ! Lütfen bilgisayarı yeniden başlatın veya IDE'yi kapatıp açın
    echo.
) else (
    echo ✗ Hata oluştu!
    exit /b 1
)

pause
```

**Kullanım:**
```batch
setup_openrouter.bat "sk-or-v1-xxxxxxxxxxxxx"
```

## 3. Environment File (.env) - İleride Kullanım İçin

Dosya: `.env`

```env
# OpenRouter Configuration
OPENROUTER_API_KEY=sk-or-v1-xxxxxxxxxxxxx

# OpenRouter Settings (Opsiyonel)
OPENROUTER_MODEL=openrouter/auto
OPENROUTER_TIMEOUT=30
OPENROUTER_CACHE_ENABLED=true

# Application Settings
AI_TESHIS_ENABLED=true
AI_TESHIS_AUTO_FILL=true
```

**.gitignore'a ekle:**
```
.env
.env.local
.env.*.local
```

## 4. Docker Compose Örneği

Dosya: `docker-compose.yml`

```yaml
version: '3.8'

services:
  teknik-servis-otomasyon:
    build:
      context: .
      dockerfile: Dockerfile
    environment:
      - OPENROUTER_API_KEY=${OPENROUTER_API_KEY}
      - DATABASE_CONNECTION_STRING=${DATABASE_CONNECTION_STRING}
    ports:
      - "3000:3000"
    volumes:
      - ./Data:/app/Data
    depends_on:
      - mysql

  mysql:
    image: mysql:8.0
    environment:
      MYSQL_ROOT_PASSWORD: root
      MYSQL_DATABASE: teknik_servis
    ports:
      - "3306:3306"
    volumes:
      - mysql_data:/var/lib/mysql

volumes:
  mysql_data:
```

**.env dosyası ekle:**
```env
OPENROUTER_API_KEY=sk-or-v1-xxxxxxxxxxxxx
DATABASE_CONNECTION_STRING=Server=mysql;Database=teknik_servis;User=root;Password=root;
```

## 5. GitHub Actions CI/CD Örneği

Dosya: `.github/workflows/build.yml`

```yaml
name: Build & Test

on: [push, pull_request]

jobs:
  build:
    runs-on: windows-latest
    
    steps:
    - uses: actions/checkout@v2
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: '8.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --configuration Release --no-restore
    
    - name: Run tests
      run: dotnet test --configuration Release --no-build --verbosity normal
      env:
        OPENROUTER_API_KEY: ${{ secrets.OPENROUTER_API_KEY }}
```

**GitHub Secrets ekle:**
1. Repository → Settings → Secrets and Variables → Actions
2. `OPENROUTER_API_KEY` ekle

## 6. Azure Key Vault Entegrasyonu

Dosya: `Helpers/KeyVaultHelper.cs`

```csharp
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

public static class KeyVaultHelper
{
    private static readonly SecureClient _client;
    
    static KeyVaultHelper()
    {
        var vaultUri = new Uri("https://your-keyvault.vault.azure.net/");
        _client = new SecretClient(vaultUri, new DefaultAzureCredential());
    }
    
    public static async Task<string> GetOpenRouterApiKeyAsync()
    {
        try
        {
            var secret = await _client.GetSecretAsync("openrouter-api-key");
            return secret.Value.Value;
        }
        catch
        {
            // Fallback to environment variable
            return Environment.GetEnvironmentVariable("OPENROUTER_API_KEY") ?? "";
        }
    }
}
```

**LlmHelper.cs'i güncelle:**
```csharp
var apiKey = await KeyVaultHelper.GetOpenRouterApiKeyAsync();
```

## 7. Logging Konfigürasyonu

Dosya: `appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "TeknikServisOtomasyon.Helpers.LlmHelper": "Debug"
    }
  },
  "OpenRouter": {
    "Enabled": true,
    "Timeout": 30,
    "CacheEnabled": true,
    "CacheDurationMinutes": 60,
    "Model": "openrouter/auto",
    "Temperature": 0.7,
    "MaxTokens": 1000
  }
}
```

**LlmHelper.cs'i güncellemek için:**
```csharp
public class OpenRouterOptions
{
    public bool Enabled { get; set; } = true;
    public int Timeout { get; set; } = 30;
    public bool CacheEnabled { get; set; } = true;
    public string Model { get; set; } = "openrouter/auto";
    public double Temperature { get; set; } = 0.7;
    public int MaxTokens { get; set; } = 1000;
}
```

## 8. Prometheus Monitoring Örneği

Dosya: `Helpers/MetricsHelper.cs`

```csharp
using Prometheus;

public static class MetricsHelper
{
    private static readonly Counter _aiTeshisCount = Counter
        .Create("ai_teshis_total", "Toplam AI teşhis işlemi")
        .Labels("status");
    
    private static readonly Histogram _aiTeshisLatency = Histogram
        .Create("ai_teshis_latency_seconds", "AI teşhis latency")
        .Buckets(0.1, 0.5, 1.0, 2.0, 5.0);
    
    public static async Task<T> MeasureAiTeshisAsync<T>(
        Func<Task<T>> operation)
    {
        using var timer = _aiTeshisLatency.NewTimer();
        try
        {
            var result = await operation();
            _aiTeshisCount.Labels("success").Inc();
            return result;
        }
        catch (Exception ex)
        {
            _aiTeshisCount.Labels("error").Inc();
            throw;
        }
    }
}
```

## 9. Denetim Günlüğü (Audit Log)

Dosya: `Models/AiTeshisAuditLog.cs`

```csharp
public class AiTeshisAuditLog
{
    public int Id { get; set; }
    public int ServisId { get; set; }
    public string Ariza { get; set; }
    public string ArizaDetay { get; set; }
    public string CihazTuru { get; set; }
    public string Marka { get; set; }
    public string Model { get; set; }
    public string AiResponse { get; set; }
    public int? TeknikerId { get; set; }
    public bool OtoFillKullanildi { get; set; }
    public DateTime IslemTarihi { get; set; } = DateTime.Now;
    public TimeSpan IslemSuresi { get; set; }
    public bool Basarili { get; set; }
}
```

## 10. Başlatma Kılavuzu

Dosya: `QUICKSTART.md`

```markdown
# Hızlı Başlangıç

## 1. Hazırlık (1 dakika)
```powershell
# Powershell'i Admin olarak açın
[Environment]::SetEnvironmentVariable("OPENROUTER_API_KEY", "YOUR_KEY_HERE", "User")
```

## 2. OpenRouter API Anahtarı Al (2 dakika)
- openrouter.ai'ye git
- Hesap oluştur
- API Keys → Yeni anahtarını kopyala

## 3. Uygulamayı Başlat (1 dakika)
```bash
dotnet run
```

## 4. Test Et (1 dakika)
- Servis Kaydı → Yeni
- Müşteri ve Cihaz seç
- Arıza gir
- 🤖 AI Teşhis tıkla

## Tadaa! 🎉
```

---

## Seçim Rehberi

| Senaryo | Önerilen Kurulum |
|---------|-----------------|
| Geliştirme | Environment Variable |
| Üretim | Key Vault / Secrets Manager |
| Docker | docker-compose + .env |
| CI/CD | GitHub Actions + Secrets |
| Enterprise | Azure Key Vault |

## Güvenlik İpuçları

✅ **Yapın:**
- API anahtarını `.gitignore`'a ekle
- Secrets Manager kullan
- Environment variable olarak ayarla

❌ **Yapmayın:**
- Kodun içine API anahtarını hardcode etme
- Git'e API anahtarını commit etme
- Public repo'ya anahtarı push etme

---

**Versiyon**: 1.0
**Tarih**: Ocak 2026

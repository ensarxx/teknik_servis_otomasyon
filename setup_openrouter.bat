@echo off
REM OpenRouter API Anahtarini Ayarlamak İçin Batch Script
REM Kullanım: setup_openrouter.bat "sk-or-v1-xxxxxxxxxxxxx"

setlocal enabledelayedexpansion

REM Renkli çıktı için ANSI escape codes
for /F %%A in ('echo prompt $H ^| cmd') do set "BS=%%A"

echo.
echo ====================================================
echo   OpenRouter API Anahtarı Kurulum Scripti
echo ====================================================
echo.

REM Parametreleri kontrol et
if "%1"=="" (
    echo [!] Hata: API anahtarı sağlanmadı
    echo.
    echo Kullanım: setup_openrouter.bat "sk-or-v1-xxxxxxxxxxxxx"
    echo.
    echo Adımlar:
    echo 1. OpenRouter.ai'ye gidin
    echo 2. Hesap oluşturun
    echo 3. API Keys sekmesinden anahtarı kopyalayın
    echo 4. Bu scripti şu şekilde çalıştırın:
    echo    setup_openrouter.bat "sk-or-v1-xxxxxxxxxxxxx"
    echo.
    pause
    exit /b 1
)

REM Admin kontrolü
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo [!] Hata: Bu script'i Administrator olarak çalıştırmalısınız
    echo.
    echo Lütfen:
    echo 1. Command Prompt'u sağ tıkla
    echo 2. "Yönetici olarak çalıştır" seçeneğini tıkla
    echo 3. Scripti tekrar çalıştır
    echo.
    pause
    exit /b 1
)

echo [*] Ayarlanıyor: OPENROUTER_API_KEY
echo.

REM Environment variable'ı ayarla
setx OPENROUTER_API_KEY %1 >nul 2>&1

if %errorlevel% equ 0 (
    echo [+] Başarı! OPENROUTER_API_KEY ayarlandı
    echo.
    echo [i] Bilgiler:
    echo     - Değer: %1
    echo     - Kapsamı: User (Bu bilgisayar için)
    echo     - Başlangıç: Sonraki başlatmada aktif
    echo.
    echo [!] ÖNEMLİ:
    echo     - Bilgisayarı yeniden başlatın VEYA
    echo     - IDE'yi kapatıp açın
    echo.
    echo [+] Doğrulama (isteğe bağlı):
    echo     - PowerShell'i açın
    echo     - Şunu yazın: $env:OPENROUTER_API_KEY
    echo     - API anahtarınızı görüyor musunuz?
    echo.
) else (
    echo [-] Hata: Environment variable ayarlanırken sorun oluştu
    echo.
    echo Lütfen:
    echo 1. Administratör olarak çalıştırdığınızı kontrol edin
    echo 2. API anahtarını (tırnak işaretleri dahil) tekrar yazın
    echo 3. Antivirus/Firewall kontrol edin
    echo.
    pause
    exit /b 1
)

pause

# OpenRouter API Anahtarını Ayarlamak İçin PowerShell Script
# Kullanım: .\setup_openrouter.ps1 -ApiKey "sk-or-v1-xxxxxxxxxxxxx"

param(
    [Parameter(Position = 0, Mandatory = $false)]
    [string]$ApiKey = ""
)

# Başlık
Write-Host ""
Write-Host "====================================================" -ForegroundColor Cyan
Write-Host "  OpenRouter API Anahtarı Kurulum Scripti" -ForegroundColor Cyan
Write-Host "====================================================" -ForegroundColor Cyan
Write-Host ""

# API anahtarını gir
if ([string]::IsNullOrWhiteSpace($ApiKey)) {
    Write-Host "[i] Lütfen OpenRouter API anahtarınızı girin:" -ForegroundColor Yellow
    Write-Host "    (Örn: sk-or-v1-xxxxxxxxxxxxx)" -ForegroundColor Gray
    Write-Host ""
    
    $ApiKey = Read-Host "API Anahtarı"
    
    if ([string]::IsNullOrWhiteSpace($ApiKey)) {
        Write-Host ""
        Write-Host "[-] Hata: API anahtarı boş bırakılamaz!" -ForegroundColor Red
        Write-Host ""
        Write-Host "[+] Adımlar:" -ForegroundColor Cyan
        Write-Host "    1. OpenRouter.ai'ye gidin: https://openrouter.ai"
        Write-Host "    2. Ücretsiz hesap oluşturun"
        Write-Host "    3. API Keys sekmesine gidin"
        Write-Host "    4. Yeni bir API anahtarı oluşturun"
        Write-Host "    5. Anahtarı kopyalayıp yapıştırın"
        Write-Host ""
        exit 1
    }
}

Write-Host ""

# Admin kontrolü
$isAdmin = ([System.Security.Principal.WindowsPrincipal][System.Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([System.Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    Write-Host "[-] Hata: Bu script'i Administrator olarak çalıştırmalısınız!" -ForegroundColor Red
    Write-Host ""
    Write-Host "[+] Çözüm:" -ForegroundColor Yellow
    Write-Host "    1. PowerShell'i sağ tıkla"
    Write-Host "    2. 'Yönetici olarak çalıştır' seçeneğini tıkla"
    Write-Host "    3. Scripti tekrar çalıştır"
    Write-Host ""
    Write-Host "Komut: .\setup_openrouter.ps1 -ApiKey '$ApiKey'" -ForegroundColor Gray
    Write-Host ""
    exit 1
}

# API anahtarını kontrol et
Write-Host "[*] Anahtarı doğrulanıyor..." -ForegroundColor White

if ($ApiKey -notmatch '^sk-or-v1-[a-zA-Z0-9]+$') {
    Write-Host ""
    Write-Host "[-] Hata: Geçersiz API anahtarı formatı!" -ForegroundColor Red
    Write-Host ""
    Write-Host "[i] Beklenen format: sk-or-v1-xxxxxxxxxxxxx" -ForegroundColor Gray
    Write-Host "[i] Aldığınız değer: $($ApiKey.Substring(0, [Math]::Min(15, $ApiKey.Length)))..." -ForegroundColor Gray
    Write-Host ""
    exit 1
}

# Environment variable'ı ayarla
Write-Host "[*] Ayarlanıyor: OPENROUTER_API_KEY" -ForegroundColor White

try {
    [Environment]::SetEnvironmentVariable("OPENROUTER_API_KEY", $ApiKey, "User")
    
    if ($LASTEXITCODE -eq 0 -or [Environment]::GetEnvironmentVariable("OPENROUTER_API_KEY", "User") -eq $ApiKey) {
        Write-Host "[+] Başarı! OPENROUTER_API_KEY ayarlandı" -ForegroundColor Green
        Write-Host ""
        
        # Özet göster
        Write-Host "[i] Bilgiler:" -ForegroundColor Cyan
        Write-Host "    - Değer: $($ApiKey.Substring(0, 12))...****" -ForegroundColor Gray
        Write-Host "    - Kapsamı: User (Bu bilgisayar)" -ForegroundColor Gray
        Write-Host "    - Başlangıç: Sonraki açılışta aktif" -ForegroundColor Gray
        Write-Host ""
        
        Write-Host "[!] ÖNEMLİ:" -ForegroundColor Yellow
        Write-Host "    - Bilgisayarı yeniden başlatın VEYA" -ForegroundColor White
        Write-Host "    - IDE'yi (Visual Studio vb) kapatıp açın" -ForegroundColor White
        Write-Host ""
        
        Write-Host "[+] Doğrulama (isteğe bağlı):" -ForegroundColor Cyan
        Write-Host "    1. PowerShell'i yeniden açın" -ForegroundColor Gray
        Write-Host "    2. Şunu yazın: \$env:OPENROUTER_API_KEY" -ForegroundColor Gray
        Write-Host "    3. API anahtarınızı görmeliyiz" -ForegroundColor Gray
        Write-Host ""
        
        Write-Host "[+] Kullanıma Başla:" -ForegroundColor Green
        Write-Host "    1. Uygulamayı çalıştır" -ForegroundColor Gray
        Write-Host "    2. Servis Kaydı oluştur" -ForegroundColor Gray
        Write-Host "    3. Müşteri ve cihaz seç" -ForegroundColor Gray
        Write-Host "    4. Arıza açıklaması gir" -ForegroundColor Gray
        Write-Host "    5. '🤖 AI Teşhis' butonuna tıkla" -ForegroundColor Gray
        Write-Host ""
        
    } else {
        throw "Environment variable ataması başarısız"
    }
}
catch {
    Write-Host ""
    Write-Host "[-] Hata: Environment variable ayarlanırken sorun oluştu" -ForegroundColor Red
    Write-Host "    $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "[+] Çözüm denemesi:" -ForegroundColor Yellow
    Write-Host "    1. Administrator olarak çalıştırdığınızı kontrol edin" -ForegroundColor Gray
    Write-Host "    2. PowerShell penceresini kapatıp açın" -ForegroundColor Gray
    Write-Host "    3. Antivirus/Firewall tarafından engellenmediğini kontrol edin" -ForegroundColor Gray
    Write-Host "    4. Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser" -ForegroundColor Gray
    Write-Host ""
    exit 1
}

# Başarı mesajı
Write-Host "✓ Kurulum tamamlandı!" -ForegroundColor Green
Write-Host ""

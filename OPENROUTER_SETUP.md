# OpenRouter API Entegrasyonu - Kurulum Kılavuzu

## 🚀 Hızlı Başlangıç

Otomatik Arıza Teşhisi özelliğini kullanmak için OpenRouter API'yi yapılandırmanız gerekmektedir.

## 📋 Adımlar

### 1. OpenRouter API Anahtarı Alın

1. **OpenRouter.ai** websitesini ziyaret edin: https://openrouter.ai
2. Ücretsiz hesap oluşturun
3. **API Keys** sekmesine gidin
4. Yeni bir API anahtarı oluşturun
5. API anahtarını kopyalayın (Örnek: `sk-or-v1-xxxxxxxxxxxxx`)

### 2. Environment Variable Ayarı

#### Windows (PowerShell)

```powershell
# PowerShell'de çalıştırın (Administrator olarak)
[Environment]::SetEnvironmentVariable("OPENROUTER_API_KEY", "sk-or-v1-xxxxxxxxxxxxx", "User")
```

Ardından bilgisayarı yeniden başlatın veya IDE'yi kapatıp açın.

#### Windows (Cmd)

```cmd
setx OPENROUTER_API_KEY "sk-or-v1-xxxxxxxxxxxxx"
```

Ardından komut istemini kapatıp açın.

#### Linux/Mac

```bash
export OPENROUTER_API_KEY="sk-or-v1-xxxxxxxxxxxxx"
# ~/.bashrc veya ~/.zshrc'ye eklemeyi unutmayın (kalıcı yapabilmek için)
echo 'export OPENROUTER_API_KEY="sk-or-v1-xxxxxxxxxxxxx"' >> ~/.bashrc
source ~/.bashrc
```

### 3. Doğrulama

Ayarı kontrol etmek için:

#### PowerShell
```powershell
$env:OPENROUTER_API_KEY
```

#### Cmd
```cmd
echo %OPENROUTER_API_KEY%
```

Çıktı: `sk-or-v1-xxxxxxxxxxxxx` (maskelenerek gösterilecek)

## 🎯 OpenRouter Ücretsiz Model Seçenekleri

OpenRouter, aşağıdaki ücretsiz modelleri sunar:

- **Llama-2** - Hızlı ve uygun maliyetli
- **Mistral** - Dengeli performans
- **Neural-Chat** - İyi sohbet yetenekleri

> **Not**: Otomatik seçim (`openrouter/auto`) en iyi modeli otomatik olarak seçer.

## 🔐 Güvenlik İpuçları

- API anahtarınızı asla halka açık yerlerde paylaşmayın
- `.env` dosyaları kullanıyorsanız, bunları `.gitignore`'a ekleyin
- Environment variable'ı sadece gerekli makinelerde ayarlayın

## 🧪 Test Etme

Uygulamada:

1. Yeni Servis Kaydı oluşturun
2. Bir cihaz seçin
3. Arıza açıklaması girin
4. **🤖 AI Teşhis** butonuna tıklayın
5. Sonuçlar görüntülenecektir

## ⚠️ Sorun Giderme

### "API anahtarı yapılandırılmadı" hatası

- Environment variable'ın doğru adlandırıldığını kontrol edin: `OPENROUTER_API_KEY`
- IDE'yi veya uygulamayı yeniden başlatmayı deneyin

### "API Hatası 401" veya "Unauthorized"

- API anahtarının doğru kopya edildiğini kontrol edin
- Anahtarın aktif olduğunu OpenRouter web sitesinde kontrol edin

### "Zaman aşımı" hatası

- İnternet bağlantısını kontrol edin
- OpenRouter sunucularının düzgün çalışıp çalışmadığını kontrol edin

### "Yanıt parse hatası"

- Cihaz bilgilerinin eksiksiz olduğunu kontrol edin
- Arıza açıklamasının boş olmadığını kontrol edin

## 📞 Destek

Sorun giderme için:
1. Hata mesajını not alın
2. OpenRouter API belgelerine bakın: https://openrouter.ai/docs
3. Sistem yöneticisine başvurun

## 💡 İpucu: Custom Model Kullanma

İleride custom model kullanmak isterseniz, `LlmHelper.cs` dosyasında bu satırı değiştirin:

```csharp
model = "openrouter/auto",  // İstenilen modeli yazın: "meta-llama/llama-2-7b-chat"
```

---

**Son Güncelleme**: Ocak 2026
**Versiyon**: 1.0

# 🤖 Otomatik Arıza Teşhisi - Teknik Dokümantasyon

## 📋 İçindekiler
1. [Mimarı](#mimarı)
2. [API Entegrasyonu](#api-entegrasyonu)
3. [Kodda Kullanım](#kodda-kullanım)
4. [Veri Akışı](#veri-akışı)
5. [Hata Yönetimi](#hata-yönetimi)
6. [Genişletme](#genişletme)

## 🏗️ Mimarı

### Dosya Yapısı

```
Helpers/
  └── LlmHelper.cs          # OpenRouter API entegrasyonu
  
Forms/Modules/
  ├── ServisKayitForm.cs    # Servis kaydı formu (AI butonu ekli)
  └── ArizaTeshisiForm.cs   # Teşhis sonuçlarını gösteren dialog
```

### Bileşenler

#### 1. **LlmHelper.cs**
OpenRouter API'ye bağlanmadan sorumlu:
- `ArizaTeshisiAsync()` - Ana yöntem, arızayı AI'ye gönderir
- `GeneratePrompt()` - LLM için prompt oluşturur
- `ParseTeşhisResponse()` - JSON yanıtını parse eder
- `IsConfigured()` - API anahtarı kontrolü

#### 2. **ArizaTeshisiForm.cs**
Teşhis sonuçlarını görüntüler:
- `InitializeComponent()` - UI oluşturur
- `BtnKopyala_Click()` - Sonuçları panoya kopyalar
- `GenerateText()` - Kopyalanacak metni oluşturur

#### 3. **ServisKayitForm.cs**
Servis kaydı formuna entegre:
- **AI Teşhis butonu** - Arıza alanının yanında
- `BtnAiTeshis_Click()` - Event handler
- **Otomatik doldurma** - Yapılan İşlemler alanında

## 🔌 API Entegrasyonu

### OpenRouter API Çağrısı

```csharp
POST https://openrouter.ai/api/v1/chat/completions

Headers:
  Authorization: Bearer sk-or-v1-xxxxx
  HTTP-Referer: https://teknik-servis-otomasyon.local
  X-Title: Teknik Servis Otomasyon

Body:
{
  "model": "openrouter/auto",
  "messages": [
    {
      "role": "system",
      "content": "Sen bir teknik servis uzmanısın..."
    },
    {
      "role": "user", 
      "content": "Arıza: ... Cihaz: ..."
    }
  ],
  "temperature": 0.7,
  "max_tokens": 1000
}
```

### Yanıt Format

```json
{
  "success": true,
  "olasıSorunlar": [
    "Sorun 1",
    "Sorun 2"
  ],
  "çözümÖnerileri": [
    "Adım 1",
    "Adım 2"
  ],
  "kontrol": [
    "Kontrol 1",
    "Kontrol 2"
  ],
  "uyarı": "Eğer varsa uyarı metni"
}
```

## 💻 Kodda Kullanım

### Basit Kullanım

```csharp
var response = await LlmHelper.ArizaTeshisiAsync(
    cihazTuru: "Dizüstü Bilgisayar",
    marka: "ASUS",
    model: "VivoBook 15",
    arizaAciklamasi: "Ekran açılmıyor, fan ses yapıyor",
    arizaDetay: "Dün düşürülmüştü"
);

if (response.Success)
{
    // Sonuçlarla çalış
    foreach (var sorun in response.OlasıSorunlar)
    {
        Console.WriteLine($"• {sorun}");
    }
}
else
{
    MessageBox.Show(response.HataMesaji);
}
```

### Form Entegrasyonu

```csharp
private async void BtnAiTeshis_Click(object? sender, EventArgs e)
{
    // 1. Validasyon
    if (!LlmHelper.IsConfigured()) return;
    
    // 2. Form verilerini al
    var ariza = txtAriza.Text;
    var cihaz = await _cihazRepository.GetByIdAsync(cihazId);
    
    // 3. AI'yi çağır
    var teshis = await LlmHelper.ArizaTeshisiAsync(
        cihaz.CihazTuru,
        cihaz.Marka,
        cihaz.Model,
        ariza
    );
    
    // 4. Sonuçları göster
    var form = new ArizaTeshisiForm(teshis);
    form.ShowDialog();
}
```

## 📊 Veri Akışı

```
ServisKayitForm
    ↓
    [Arıza gir + AI Teşhis tıkla]
    ↓
LlmHelper.ArizaTeshisiAsync()
    ↓
    [Prompt oluştur]
    ↓
    [OpenRouter API'ye POST]
    ↓
OpenRouter Sunucuları
    ↓
    [LLM modeli (Llama/Mistral) arızayı analiz eder]
    ↓
    [JSON yanıtı döndür]
    ↓
ParseTeşhisResponse()
    ↓
    [JSON'u parse et]
    ↓
ArizaTeshisiForm
    ↓
    [Sonuçları göster]
```

## ⚠️ Hata Yönetimi

### Hata Türleri ve Ele Alınması

#### 1. Yapılandırma Hatası
```csharp
if (!LlmHelper.IsConfigured())
{
    // Environment variable yok
    // → Kullanıcıya bildirim ver
    // → API anahtarı almak için yönlendir
}
```

#### 2. Network Hatası
```csharp
catch (HttpRequestException ex)
{
    // OpenRouter'a erişilemiyor
    // → İnternet bağlantısını kontrol et
    // → Firewall/VPN kontrol et
}
```

#### 3. API Hatası
```csharp
if (!response.IsSuccessStatusCode)
{
    // API anahtarı geçersiz veya limit aşıldı
    // → Hata kodunu oku (401, 429, 500)
    // → Uygun mesaj göster
}
```

#### 4. Parse Hatası
```csharp
catch (JsonException ex)
{
    // JSON format yanlış
    // → LLM'in doğru format döndürmediği
    // → Prompt'u düzeltmeyi dene
}
```

## 🔧 Genişletme

### Custom Model Kullanma

```csharp
// LlmHelper.cs line ~50'de
var request = new
{
    model = "meta-llama/llama-2-7b-chat",  // Veya diğer model
    // ...
};
```

**Mevcut Ücretsiz Modeller:**
- `openrouter/auto` - Otomatik seçim (önerilir)
- `meta-llama/llama-2-7b-chat`
- `mistralai/mistral-7b-instruct`
- `neural-chat`

### Custom Prompt

```csharp
private static string GeneratePrompt(...)
{
    // Kendi prompt'unuzu oluşturun
    return $@"
        Siz bir teknik destek uzmanısınız.
        Aşağıdaki arızaya çözüm önerileri sunun:
        
        Cihaz: {cihazTuru}
        Arıza: {arizaAciklamasi}
        
        Lütfen JSON olarak döndürün...
    ";
}
```

### Response İçeriğini Genişletme

```csharp
public class TeşhisResponse
{
    // Mevcut alanlar...
    
    // Yeni alanlar ekle:
    public double GuvenirlikSkoru { get; set; }
    public List<string> KullanilabilirParcalar { get; set; }
    public string TahminiSüresi { get; set; }
}
```

## 📈 Performans Optimizasyonları

### 1. Caching
```csharp
// Aynı arıza + cihaz kombinasyonları cache'le
private static Dictionary<string, TeşhisResponse> _cache = new();

public static async Task<TeşhisResponse> ArizaTeshisiAsync(...)
{
    var key = $"{cihazTuru}_{marka}_{model}_{arizaAciklamasi}";
    if (_cache.ContainsKey(key))
        return _cache[key];
    
    var result = await FetchFromAPI(...);
    _cache[key] = result;
    return result;
}
```

### 2. Timeout Ayarı
```csharp
_httpClient.Timeout = TimeSpan.FromSeconds(30);
```

### 3. Rate Limiting
```csharp
// Çok hızlı çağrıları engelle
private static DateTime _lastCall = DateTime.MinValue;

public static async Task<TeşhisResponse> ArizaTeshisiAsync(...)
{
    var timeSinceLastCall = DateTime.Now - _lastCall;
    if (timeSinceLastCall < TimeSpan.FromSeconds(2))
    {
        await Task.Delay(TimeSpan.FromSeconds(2) - timeSinceLastCall);
    }
    _lastCall = DateTime.Now;
    // ...
}
```

## 🧪 Test Etme

### Unit Test Örneği

```csharp
[TestMethod]
public async Task ArizaTeshisiAsync_ValidInput_ReturnsSuccess()
{
    var response = await LlmHelper.ArizaTeshisiAsync(
        "Laptop",
        "Dell",
        "XPS 13",
        "Ekran açılmıyor"
    );
    
    Assert.IsTrue(response.Success);
    Assert.IsTrue(response.OlasıSorunlar.Count > 0);
    Assert.IsTrue(response.ÇözümÖnerileri.Count > 0);
}

[TestMethod]
public async Task ArizaTeshisiAsync_NoApiKey_ReturnsFalse()
{
    // OPENROUTER_API_KEY'i kaldır
    Assert.IsFalse(LlmHelper.IsConfigured());
}
```

### Manual Test

1. API anahtarını ayarla
2. ServisKayitForm'u aç
3. Cihaz ve arıza gir
4. "AI Teşhis" tıkla
5. Sonuçları kontrol et

## 📋 Bakım ve Monitoring

### Logging Ekle

```csharp
// LlmHelper.cs'de
Debug.WriteLine($"OpenRouter API çağrısı: {sw.ElapsedMilliseconds}ms");
Debug.WriteLine($"Token kullanımı: input={usage.PromptTokens}, output={usage.CompletionTokens}");
```

### Error Tracking

```csharp
catch (Exception ex)
{
    // Logging servisi ile kayıt et
    _logger.Error($"AI Teşhis hatası: {ex.Message}", ex);
    throw;
}
```

---

**Versiyon**: 1.0
**Tarih**: Ocak 2026
**Yazı**: GitHub Copilot

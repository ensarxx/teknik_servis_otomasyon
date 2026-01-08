# 🤖 Otomatik Arıza Teşhisi Entegrasyonu

## 📝 Özet

Teknik Servis Otomasyon uygulamasına **OpenRouter LLM API** entegrasyonu eklendi.

- ✅ **Ücretsiz** - OpenRouter.ai ücretsiz tabaka
- ✅ **Hızlı** - Anında arıza analizi
- ✅ **Kolay** - Tek butona tıkla
- ✅ **Akıllı** - Cihaz ve arızaya göre kişiselleştirilmiş öneriler

## ⚡ Hızlı Kurulum (5 dakika)

### 1️⃣ API Anahtarı Al

```bash
1. OpenRouter.ai'ye git
2. Ücretsiz hesap oluştur
3. API anahtarını kopyala
```

### 2️⃣ Environment Variable Ayarla

**Windows PowerShell (Yönetici):**
```powershell
[Environment]::SetEnvironmentVariable("OPENROUTER_API_KEY", "sk-or-v1-xxxxxxxxxxxxx", "User")
```

**Windows Command Prompt:**
```cmd
setx OPENROUTER_API_KEY "sk-or-v1-xxxxxxxxxxxxx"
```

### 3️⃣ Uygulamayı Yeniden Başlat

## 🎯 Kullanım

1. **Yeni Servis Kaydı** oluştur
2. **Müşteri** seç
3. **Cihaz** seç
4. **Arıza** alanına müşteri şikayetini yaz
5. **🤖 AI Teşhis** butonuna tıkla
6. **Sonuçları** görüntüle ve kullan

```
Arıza: "Ekranın köşesinde kırmızı renk çıkıyor"
↓
🤖 AI Teşhis
↓
• Olası Sorunlar: GPU hatası, kablo sorunu, ekran PIxel sorunu
• Çözüm Adımları: Sürücü güncelle, BIOS sıfırla, ekran testi yap
• Kontrol Noktaları: Diğer ekranda test et, sıcaklık kontrol
• Uyarı: Ekran değişimi pahalı olabilir, veri yedekle
```

## 📁 Ekli Dosyalar

| Dosya | Açıklama |
|-------|----------|
| `Helpers/LlmHelper.cs` | OpenRouter API entegrasyonu |
| `Forms/Modules/ArizaTeshisiForm.cs` | Teşhis sonuçları dialog |
| `AI_TESHIS_KULLANICI_KILAVUZU.md` | Kullanıcı rehberi |
| `AI_TESHIS_TEKNIK_DOKUMANTASYON.md` | Teknik detaylar |
| `OPENROUTER_SETUP.md` | Kurulum kılavuzu |

## 🔧 Teknik Detaylar

### LLM Model
- **OpenRouter Auto** - Otomatik en iyi modeli seçer
- Llama 2, Mistral, Neural Chat gibi ücretsiz modeller

### API Çağrısı
```
POST https://openrouter.ai/api/v1/chat/completions
Authorization: Bearer $OPENROUTER_API_KEY
Content-Type: application/json
```

### Fonksiyonlar

#### `LlmHelper.ArizaTeshisiAsync()`
Ana metod - arıza analizi yapan

```csharp
var teshis = await LlmHelper.ArizaTeshisiAsync(
    cihazTuru: "Masaüstü",
    marka: "Dell",
    model: "OptiPlex 7000",
    arizaAciklamasi: "Açılmıyor",
    arizaDetay: "Power butonuna basınca ışık yanmıyor"
);

if (teshis.Success)
{
    foreach (var sorun in teshis.OlasıSorunlar)
        Console.WriteLine($"• {sorun}");
}
```

#### `LlmHelper.IsConfigured()`
API anahtarı kontrolü

```csharp
if (!LlmHelper.IsConfigured())
    MessageBox.Show("API anahtarını yapılandırın");
```

## 📊 Response Yapısı

```json
{
  "success": true,
  "olasıSorunlar": [
    "PSU (güç kaynağı) hatası",
    "Anakart sorunu",
    "RAM hatasız"
  ],
  "çözümÖnerileri": [
    "PSU'yu başka güç kaynağıyla test et",
    "RAM'i çıkart ve temizle",
    "CMOS pili sıfırla"
  ],
  "kontrol": [
    "Fan dönüyor mu?",
    "LED ışıkları yanıyor mu?"
  ],
  "uyarı": "Açılmazsa anakartı kontrol etmeyi dene"
}
```

## ⚙️ Yapılandırma

### Environment Variable
```
OPENROUTER_API_KEY = sk-or-v1-xxxxxxxxxxxxx
```

### Custom Model
`LlmHelper.cs` ~ satır 50'de değiştir:
```csharp
model = "meta-llama/llama-2-7b-chat"  // Diğer modelini yaz
```

## 🐛 Sorun Giderme

| Sorun | Çözüm |
|-------|-------|
| API anahtarı yapılandırılmadı | Env var ekle, bilgisayar yeniden başlat |
| 401 Unauthorized | API anahtarını kontrol et |
| Zaman aşımı | İnternet bağlantısını kontrol et |
| Parse hatası | Arıza açıklamasını daha detaylı yaz |

Detaylı sorun giderme: `OPENROUTER_SETUP.md` adresine bakın.

## 🌐 Önemli Linkler

- **OpenRouter**: https://openrouter.ai
- **API Belgesi**: https://openrouter.ai/docs
- **Model Listesi**: https://openrouter.ai/models
- **Fiyatlandırma**: https://openrouter.ai/pricing

## 📈 Sonraki Adımlar

- [ ] Caching ekle (tekrar eden arızalar)
- [ ] Database'e log tut (AI öneriler vs gerçek sonuç)
- [ ] Raporlama (AI teşhis istatistikleri)
- [ ] Multi-dil desteği (İngilizce, Almanca, vs)
- [ ] Custom model entegrasyonu

## 💡 İpuçları

1. **Detaylı arıza açıklaması yazın** - "Bozuk" yerine tam belirt
2. **Cihazı doğru seçin** - Model önemli
3. **Sonuçları danışmanlık amaçlı kullanın** - Teknik bilgi daima geçerli
4. **Kopyala butonu** - Müşteri raporunda kullanabilir
5. **Otomatik doldur** - Başlangıç çerçevesi oluşturur

## 📞 Destek

- `AI_TESHIS_KULLANICI_KILAVUZU.md` - Kullanıcılar için
- `AI_TESHIS_TEKNIK_DOKUMANTASYON.md` - Geliştiriciler için
- `OPENROUTER_SETUP.md` - Kurulum adımları

---

**Versiyon**: 1.0  
**Tarih**: Ocak 2026  
**Gereksinimler**: .NET 8.0+, İnternet, OpenRouter API anahtarı  
**Lisans**: Proje ile aynı

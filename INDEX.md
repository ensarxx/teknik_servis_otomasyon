# 🤖 Otomatik Arıza Teşhisi - Dokümantasyon İndeksi

## 📚 Dokümantasyon Haritası

```
📦 TEKNIK SERVİS OTOMASYON
├── 🔧 KURULUM & KONFIGÜRASYON
│   ├── 📖 OPENROUTER_SETUP.md          ← ⭐ BAŞLANGIÇ (İLK OKU!)
│   ├── 🏃 AI_TESHIS_README.md          ← Hızlı özet (5 dakika)
│   ├── 🛠️ setup_openrouter.ps1         ← PowerShell kurulum scripti
│   ├── 🛠️ setup_openrouter.bat         ← Batch kurulum scripti
│   └── ⚙️ KONFIGURASYONLAR.md          ← İleri yapılandırma
│
├── 📖 KULLANMAK İÇİN
│   ├── 👥 AI_TESHIS_KULLANICI_KILAVUZU.md  ← Teknisyenler için (önemli!)
│   ├── 📋 DEGISIKLIKLER_OZETI.md           ← Neler yapıldı?
│   └── 📑 INDEX.md                         ← Bu dosya
│
├── 💻 GELİŞTİRME İÇİN
│   ├── 🏗️ AI_TESHIS_TEKNIK_DOKUMANTASYON.md  ← Mimarı ve kod
│   ├── 📚 Helpers/LlmHelper.cs                ← Ana sınıf
│   └── 🎨 Forms/Modules/ArizaTeshisiForm.cs  ← UI Sınıfı
│
└── 🚀 BAŞLANGAÇ KONTROL LİSTESİ
    ├── ✓ OpenRouter API anahtarı al
    ├── ✓ Environment variable ayarla
    ├── ✓ Bilgisayarı yeniden başlat
    ├── ✓ Uygulamayı çalıştır
    └── ✓ Servis Kaydında test et
```

---

## 🎯 Rol Bazında Rehber

### 👨‍💼 Yönetici/Sistem Yöneticisi

**Hedef**: Sistemi kurmak ve yapılandırmak

1. **Başla**: `OPENROUTER_SETUP.md` oku
2. **Kur**: Script kullan
   ```powershell
   .\setup_openrouter.ps1 -ApiKey "sk-or-v1-xxxxxxxxxxxxx"
   ```
3. **Doğrula**: Environment variable kontrol et
4. **Test Et**: Uygulamayı çalıştır

**İlgili Dosyalar**:
- OPENROUTER_SETUP.md
- setup_openrouter.ps1 / setup_openrouter.bat
- KONFIGURASYONLAR.md

---

### 👨‍🔧 Teknisyen/Servis Temsilcisi

**Hedef**: AI Teşhisini kullanarak arızaları teşhis etmek

1. **Öğren**: `AI_TESHIS_KULLANICI_KILAVUZU.md` oku
2. **Kullan**:
   - Servis Kaydı Oluştur
   - Arıza açıklası gir
   - 🤖 AI Teşhis tıkla
   - Sonuçları incele
3. **Uygula**: Önerilen çözüm adımlarını takip et

**İlgili Dosyalar**:
- AI_TESHIS_KULLANICI_KILAVUZU.md
- AI_TESHIS_README.md

---

### 👨‍💻 Yazılım Geliştirici

**Hedef**: Sistemi geliştirmek, genişletmek, iyileştirmek

1. **Mimarı Öğren**: `AI_TESHIS_TEKNIK_DOKUMANTASYON.md` oku
2. **Kodu İncele**:
   - Helpers/LlmHelper.cs (API entegrasyonu)
   - Forms/Modules/ArizaTeshisiForm.cs (UI)
   - Forms/Modules/ServisKayitForm.cs (entegrasyon)
3. **Genişlet**: Teknik dokümantasyon'daki talimatları takip et
4. **Test Et**: Kodunu test et ve doğrula

**İlgili Dosyalar**:
- AI_TESHIS_TEKNIK_DOKUMANTASYON.md
- Helpers/LlmHelper.cs
- Forms/Modules/ArizaTeshisiForm.cs
- DEGISIKLIKLER_OZETI.md

---

### 🏢 Proje Yöneticisi/Sahip

**Hedef**: Yapılan işi anlamak ve proje durumunu takip etmek

1. **Özet Al**: `AI_TESHIS_README.md` oku
2. **Detay Gör**: `DEGISIKLIKLER_OZETI.md` oku
3. **Durumu Kontrol**: Bu INDEX dosyasını gözden geçir

**İlgili Dosyalar**:
- AI_TESHIS_README.md
- DEGISIKLIKLER_OZETI.md
- OPENROUTER_SETUP.md (kurulum zamanı)

---

## 📋 Dosya Açıklamaları

### 🔴 ÖNEMLİ (İlk Oku)

#### `OPENROUTER_SETUP.md`
- **Kime**: Yöneticiler
- **Nedir**: Adım adım kurulum kılavuzu
- **Zamanı**: 5-10 dakika
- **İçerik**:
  - API anahtarı alma
  - Environment variable ayarlama
  - Sorun giderme
  - Platform spesifik talimatlar (Windows, Linux, Mac)

#### `AI_TESHIS_KULLANICI_KILAVUZU.md`
- **Kime**: Teknisyenler
- **Nedir**: Uygulamada nasıl kullanılır
- **Zamanı**: 10-15 dakika
- **İçerik**:
  - Hızlı başlangıç
  - Adım adım kullanım
  - Örnek senaryolar
  - Sık sorulan sorular
  - Sorun giderme

---

### 🟠 ÖNEMLİ (Derinlemesine Bilgi)

#### `AI_TESHIS_README.md`
- **Kime**: Herkes (hızlı bakış)
- **Nedir**: Projenin özeti
- **Zamanı**: 2-3 dakika
- **İçerik**:
  - Özet
  - Hızlı kurulum
  - Dosya yapısı
  - Teknik detaylar (yüksek seviye)
  - Sorun giderme

#### `AI_TESHIS_TEKNIK_DOKUMANTASYON.md`
- **Kime**: Geliştiriciler
- **Nedir**: Teknik mimarı ve kod detayları
- **Zamanı**: 30-45 dakika
- **İçerik**:
  - Mimarı
  - API entegrasyonu detayları
  - Kod örnekleri
  - Veri akışı
  - Hata yönetimi
  - Genişletme rehberi
  - Test etme
  - Performans optimizasyonu

---

### 🟡 BİLGİSEL (Arka Plan)

#### `DEGISIKLIKLER_OZETI.md`
- **Kime**: Proje yöneticileri ve geliştiriciler
- **Nedir**: Neler yapıldı ve nasıl
- **Zamanı**: 20-30 dakika
- **İçerik**:
  - Yapılan değişikliklerin detayı
  - Yeni dosyalar
  - Güncellenen dosyalar
  - API entegrasyonu detayları
  - Bileşen açıklamaları
  - Test durumu
  - Sonraki adımlar

#### `KONFIGURASYONLAR.md`
- **Kime**: DevOps ve ileri kullanıcılar
- **Nedir**: İleri yapılandırma örnekleri
- **Zamanı**: 15-30 dakika
- **İçerik**:
  - PowerShell kurulum scripti
  - Batch kurulum scripti
  - .env dosyası örneği
  - Docker Compose örneği
  - GitHub Actions örneği
  - Azure Key Vault entegrasyonu
  - Logging konfigürasyonu
  - Prometheus monitoring

---

### 🟢 YARDIMCI (Araçlar)

#### `setup_openrouter.ps1`
- **Tür**: PowerShell Script
- **Kime**: Windows kullanıcıları
- **Nedir**: API anahtarını otomatik ayarla
- **Kullanım**:
  ```powershell
  .\setup_openrouter.ps1 -ApiKey "sk-or-v1-xxxxxxxxxxxxx"
  ```

#### `setup_openrouter.bat`
- **Tür**: Batch Script
- **Kime**: Windows (Command Prompt)
- **Nedir**: API anahtarını otomatik ayarla
- **Kullanım**:
  ```cmd
  setup_openrouter.bat "sk-or-v1-xxxxxxxxxxxxx"
  ```

---

### 💜 KOD DOSYALARI

#### `Helpers/LlmHelper.cs`
- **Amaç**: OpenRouter API entegrasyonu
- **Sınıflar**:
  - `LlmHelper` (statik yardımcı sınıf)
  - `TeşhisResponse` (veri modeli)
- **Ana Metodlar**:
  - `ArizaTeshisiAsync()` - Arıza analizi
  - `IsConfigured()` - Konfigürasyon kontrolü
- **Satır**: ~200

#### `Forms/Modules/ArizaTeshisiForm.cs`
- **Amaç**: Teşhis sonuçlarını gösteren dialog
- **Sınıflar**:
  - `ArizaTeshisiForm` (Windows Form)
- **Özellikler**:
  - Renkli gösterim
  - Kopyala butonu
  - Responsive layout
- **Satır**: ~300

#### `Forms/Modules/ServisKayitForm.cs`
- **Değişiklik**: AI Teşhis butonu eklendi
- **Yeni Event**: `BtnAiTeshis_Click()`
- **Yeni Buton**: 🤖 AI Teşhis
- **Değişik Satırlar**: ~150

---

## 🚀 Hızlı Başlangıç Yolları

### Senaryo 1: "Sistemi Kurmak"
```
1. OPENROUTER_SETUP.md oku
2. OpenRouter.ai'ye git ve anahtarı al
3. setup_openrouter.ps1 veya .bat çalıştır
4. Bilgisayarı yeniden başlat
5. Uygulamayı aç ve test et
⏱️ Toplam: ~15 dakika
```

### Senaryo 2: "Sistemi Kullanmak"
```
1. AI_TESHIS_KULLANICI_KILAVUZU.md oku
2. Servis Kaydı oluştur
3. Müşteri ve cihaz seç
4. Arıza gir
5. 🤖 AI Teşhis tıkla
6. Sonuçları incele
⏱️ Toplam: ~5 dakika (+ AI işlem süresi)
```

### Senaryo 3: "Kodu Geliştirmek"
```
1. AI_TESHIS_TEKNIK_DOKUMANTASYON.md oku
2. Helpers/LlmHelper.cs incele
3. DEGISIKLIKLER_OZETI.md oku
4. Değişiklik yap
5. Kod test et
⏱️ Toplam: ~1-2 saat
```

---

## 🎓 Öğrenme Yolu

### Seviye 1: Temeller (15 dakika)
- [ ] AI_TESHIS_README.md
- [ ] OPENROUTER_SETUP.md (ilk 5 dakika)

### Seviye 2: Kullanım (30 dakika)
- [ ] OPENROUTER_SETUP.md (tam)
- [ ] AI_TESHIS_KULLANICI_KILAVUZU.md
- [ ] Sistemi çalıştır ve test et

### Seviye 3: Teknik Bilgi (2 saat)
- [ ] AI_TESHIS_TEKNIK_DOKUMANTASYON.md
- [ ] DEGISIKLIKLER_OZETI.md
- [ ] Kod dosyalarını incele
- [ ] Genişletme örneğini dene

### Seviye 4: Master (4+ saat)
- [ ] Tüm dokümantasyonu oku
- [ ] Kod tabanını derinlemesine incele
- [ ] Kendi genişletmeleri yap
- [ ] PR/Commit talebi oluştur

---

## 📞 Hızlı Referans

### Sık Sorulan Sorular

**S: Başlangıç adımları nedir?**
A: OPENROUTER_SETUP.md → setup_openrouter.ps1 → Test

**S: Nasıl kullanılır?**
A: AI_TESHIS_KULLANICI_KILAVUZU.md oku

**S: Teknik detaylar nelerdir?**
A: AI_TESHIS_TEKNIK_DOKUMANTASYON.md oku

**S: Neler yapıldı?**
A: DEGISIKLIKLER_OZETI.md oku

**S: Script'i nasıl çalıştırırım?**
A: setup_openrouter.ps1 veya setup_openrouter.bat kullan

---

## 📊 Dokümantasyon İstatistikleri

| Dosya | Tür | Satır | Okuma Süresi |
|-------|-----|-------|--------------|
| OPENROUTER_SETUP.md | Markdown | 180 | 10 min |
| AI_TESHIS_README.md | Markdown | 160 | 5 min |
| AI_TESHIS_KULLANICI_KILAVUZU.md | Markdown | 350 | 15 min |
| AI_TESHIS_TEKNIK_DOKUMANTASYON.md | Markdown | 450 | 30 min |
| DEGISIKLIKLER_OZETI.md | Markdown | 380 | 20 min |
| KONFIGURASYONLAR.md | Markdown | 320 | 15 min |
| INDEX.md | Markdown | 450 | 15 min |
| setup_openrouter.ps1 | Script | 80 | Otomatik |
| setup_openrouter.bat | Script | 70 | Otomatik |
| LlmHelper.cs | C# | 200 | 20 min |
| ArizaTeshisiForm.cs | C# | 300 | 20 min |
| ServisKayitForm.cs | C# | 150 değişti | 10 min |
| **TOPLAM** | - | **3500+** | **2.5 saat** |

---

## ✅ Kontrol Listesi

### Kurulum Öncesi
- [ ] Internet bağlantısı var mı?
- [ ] Admin erişimi var mı?
- [ ] OpenRouter API anahtarı var mı?

### Kurulum Sırası
- [ ] OPENROUTER_SETUP.md oku
- [ ] Script çalıştırıldı
- [ ] Environment variable ayarlandı
- [ ] Bilgisayar yeniden başlatıldı

### Kurulum Sonrası
- [ ] Uygulamayı aç
- [ ] Yeni Servis Kaydı oluştur
- [ ] Arıza gir
- [ ] 🤖 AI Teşhis tıkla
- [ ] Sonuçlar göründü mü?

### Sorun Giderme
- [ ] OPENROUTER_SETUP.md sorun giderme okudun
- [ ] Environment variable'ı kontrol ettin
- [ ] IDE'yi yeniden başlatttın
- [ ] Bilgisayarı yeniden başlatttın

---

## 🔗 Harici Linkler

- **OpenRouter**: https://openrouter.ai
- **API Dokümantasyonu**: https://openrouter.ai/docs
- **Modeller**: https://openrouter.ai/models
- **Fiyatlandırma**: https://openrouter.ai/pricing

---

## 📝 Versiyon Tarihi

| Versiyon | Tarih | Detaylar |
|----------|-------|---------|
| 1.0 | Ocak 2026 | İlk release |
| 1.1 (Planlanmış) | - | Caching + Logging |
| 2.0 (Planlanmış) | - | Database integration |

---

**Oluşturma Tarihi**: Ocak 2026
**Son Güncelleme**: Ocak 2026
**Durum**: ✅ Tamamlandı ve Test Edildi
**Hazır**: ✅ Üretim için

---

## 🆘 Yardım Almak

1. **Belirtiniz**: Hangi adımda sıkışıp kaldınız?
2. **Kontrol**: İlgili dokümantasyonu yeniden okuyun
3. **Script**: Script kullanmayı deneyin
4. **Sorun Giderme**: Relevan "Sorun Giderme" bölümü okuyun
5. **Destek**: Sistem yöneticisine başvurun

---

✨ **Mutlu Teşhis Yapınız!** ✨

# 🤖 Otomatik Arıza Teşhisi - Kullanıcı Kılavuzu

## ✨ Özellik Özeti

Teknik Servis Otomasyon uygulamasına **OpenRouter LLM entegrasyonu** eklendi. Artık müşterinin tanımladığı arızaya göre **Yapay Zeka tarafından otomatik olarak**:

- 🔍 **Olası sorunlar** önerileri alıyor
- 💡 **Çözüm adımları** alıyor
- ✓ **Kontrol noktaları** alıyor
- ⚠️ **Güvenlik uyarıları** alıyor

## 🚀 Hızlı Başlangıç

### 1. API Anahtarı Alın (5 dakika)

OpenRouter ücretsiz ve kullanımı kolay. Şu adımları takip edin:

```
1. OpenRouter.ai sitesine gidin
2. Ücretsiz hesap oluşturun
3. API Keys sekmesinden anahtarı kopyalayın
```

### 2. API Anahtarını Yapılandırın

**Windows PowerShell** (Yönetici modunda):
```powershell
[Environment]::SetEnvironmentVariable("OPENROUTER_API_KEY", "sk-or-v1-xxxxxxxxxxxxx", "User")
```

Bilgisayarı yeniden başlatın.

### 3. Uygulamada Kullanın

## 📖 Adım Adım Kullanım

### Servis Kaydı Formunda AI Teşhis Kullanma

1. **Servis Kaydı Oluştur** kısmına gidin
2. **Müşteri** seçin
3. **Cihaz** seçin (otomatik olarak cihaz türü, marka, model alınacak)
4. **Arıza** alanına müşterinin şikayetini yazın
   - Örnek: "Ekranın alt tarafında kalın siyah çizgi var"
5. **(Opsiyonel)** **Arıza Detayı** alanına ek bilgiler ekleyin
   - Örnek: "Cihaz 2 gün önce düşürülmüş, kurudan çalışıyor"
6. **🤖 AI Teşhis** butonuna tıklayın
7. **Sonuçları görün** - otomatik olarak:
   - Olası sorunlar listesi
   - Önerilen çözüm adımları
   - Kontrol noktaları
   - Varsa güvenlik uyarıları

### Sonuçlarla Ne Yapılır?

**Dialog penceresinde**:
- ✓ **Sonuçları incele** - öneriler ve sorun analizi
- 📋 **Kopyala** - sonuçları kopyalayarak not veya raporda kullan
- 🔄 **Otomatik Doldur** - önerilen çözüm adımlarını "Yapılan İşlemler" alanına eklet

## 🎓 Örnek Senaryolar

### Senaryo 1: Bilgisayar Açılmıyor

```
Cihaz: Masaüstü Bilgisayar (Dell, OptiPlex 7000)
Arıza: "Açılmıyor, power butonuna basınca ışık yanmıyor"
Arıza Detayı: "Dün aniden kapandı, şimdi açılmıyor"

AI Sonuçları:
• Olası Sorunlar: PSU sorunu, RAM hatası, MB sorunu
• Çözüm: Hava akışı kontrol, RAM teste tabi tut, vs.
```

### Senaryo 2: Yazıcı Sorunsalı

```
Cihaz: Yazıcı (HP, LaserJet M404)
Arıza: "Kağıt sıkışıyor, hata kodı gösteriyor"
Arıza Detayı: "Her çıktıda sıkışıyor, nerde olduğunu bilmiyorum"

AI Sonuçları:
• Olası Sorunlar: Roller kötüleşme, papır sensörü hatasızlık
• Çözüm: Tepsiye kontrol, sensör test, roller temizlik
```

## ❓ Sık Sorulan Sorular

### S: Gerçekten ücretsiz mi?
**C:** Evet! OpenRouter.ai ücretsiz bir tabaka sunar. Kayıt olduğunuzda bilgisayar / ay kredi alırsınız.

### S: İnternet bağlantısı gerekli mi?
**C:** Evet, OpenRouter bir bulut servisidir. İnternet bağlantınız olmalı.

### S: Veri nereye gidiyor?
**C:** Arıza açıklaması ve cihaz bilgileri OpenRouter sunucularına gönderiliyor. Verileri OpenRouter gizlilik politikasına göre işliyor.

### S: Sonuçlar doğru mu?
**C:** AI tarafından sağlanan önerilerin danışmanlık amaçlı olduğunu unutmayın. Teknisyen her zaman teknik bilgiye dayanmalı.

### S: Offline çalışabilir mi?
**C:** Hayır, OpenRouter bulut tabanlıdır. Offline modda çalışmaz.

## 🔧 Sorun Giderme

### "API anahtarı yapılandırılmadı" hatası

```
Çözüm:
1. OPENROUTER_API_KEY environment variable'ını kontrol et
2. Değer doğru mu kontrol et
3. Bilgisayarı yeniden başlat
4. IDE'yi kapatıp aç
```

### "Zaman aşımı" hatası

```
Çözüm:
1. İnternet bağlantınızı kontrol edin
2. OpenRouter sunucularının aktif olduğunu kontrol edin
3. Firewall/VPN tarafından engellenip engellenmediğini kontrol edin
```

### "Yanıt parse hatası"

```
Çözüm:
1. Arıza açıklamasının boş olmadığını kontrol edin
2. Cihaz bilgilerinin doğru seçildiğini kontrol edin
3. Örnek: "Bozuk" yerine "Ekran siyah, fan gürültülü" gibi detaylı yazın
```

## 💡 İpuçları

1. **Detaylı yazın** - "Bozuk" yerine "Ekran siyah, hoparlörden gürültü geliyor" yazın
2. **Cihaz türünü doğru seçin** - Markası ve modeli etkiler
3. **Sonuçları inceleyin** - AI önerileri referans için, teknik bilgiye göre kararınızı verin
4. **Kopyala butonu** - Müşteri notlarına rapor için sonuçları ekleyebilirsiniz
5. **Otomatik doldur** - Başlangıçta iyi bir çerçeve oluşturur, sonra teknik bilgile tamamlayın

## 📚 Daha Fazla Bilgi

- **OpenRouter Dokümantasyon**: https://openrouter.ai/docs
- **API Anahtarı Yönetimi**: https://openrouter.ai/keys
- **Model Seçenekleri**: https://openrouter.ai/models

---

**Versiyon**: 1.0
**Tarih**: Ocak 2026
**Gereksinimler**: İnternet bağlantısı, OpenRouter API anahtarı

---

## 📞 Destek

Sorun yaşıyorsanız:
1. Bu kılavuzu yeniden okuyun
2. Sorun Giderme bölümüne bakın
3. Sistem yöneticisine başvurun

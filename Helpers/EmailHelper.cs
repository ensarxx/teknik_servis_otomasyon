using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TeknikServisOtomasyon.Data.Repositories;
using TeknikServisOtomasyon.Models;

namespace TeknikServisOtomasyon.Helpers
{
    public static class EmailHelper
    {
        private static EmailAyarlari? _ayarlar;

        public static async Task<bool> LoadAyarlarAsync()
        {
            try
            {
                var repository = new EmailAyarlariRepository();
                _ayarlar = await repository.GetAsync();
                return _ayarlar != null && _ayarlar.EmailAktif;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<(bool success, string message)> SendEmailAsync(string aliciEmail, string konu, string icerik)
        {
            try
            {
                if (_ayarlar == null)
                    await LoadAyarlarAsync();

                if (_ayarlar == null || !_ayarlar.EmailAktif)
                    return (false, "E-posta ayarları yapılandırılmamış veya devre dışı.");

                if (string.IsNullOrEmpty(aliciEmail))
                    return (false, "Alıcı e-posta adresi boş.");

                using var smtpClient = new SmtpClient(_ayarlar.SmtpServer)
                {
                    Port = _ayarlar.SmtpPort,
                    Credentials = new NetworkCredential(_ayarlar.GondericiEmail, _ayarlar.GondericiSifre),
                    EnableSsl = _ayarlar.SslKullan,
                    Timeout = 30000
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_ayarlar.GondericiEmail, _ayarlar.GondericiAdi),
                    Subject = konu,
                    Body = icerik,
                    IsBodyHtml = true,
                    BodyEncoding = Encoding.UTF8,
                    SubjectEncoding = Encoding.UTF8
                };

                mailMessage.To.Add(aliciEmail);

                await smtpClient.SendMailAsync(mailMessage);
                return (true, "E-posta başarıyla gönderildi.");
            }
            catch (Exception ex)
            {
                return (false, $"E-posta gönderilirken hata: {ex.Message}");
            }
        }

        public static async Task<(bool success, string message)> SendServisKayitEmailAsync(ServisKaydi servis, Musteri musteri, Cihaz cihaz)
        {
            if (string.IsNullOrEmpty(musteri.Email))
                return (false, "Müşteri e-posta adresi tanımlı değil.");

            var konu = $"Servis Kaydı Oluşturuldu - {servis.ServisNo}";
            var icerik = GenerateServisKayitEmailHtml(servis, musteri, cihaz);

            return await SendEmailAsync(musteri.Email, konu, icerik);
        }

        public static async Task<(bool success, string message)> SendDurumGuncelleEmailAsync(ServisKaydi servis, Musteri musteri, Cihaz cihaz, string yeniDurum)
        {
            if (string.IsNullOrEmpty(musteri.Email))
                return (false, "Müşteri e-posta adresi tanımlı değil.");

            var konu = $"Servis Durumu Güncellendi - {servis.ServisNo}";
            var icerik = GenerateDurumGuncelleEmailHtml(servis, musteri, cihaz, yeniDurum);

            return await SendEmailAsync(musteri.Email, konu, icerik);
        }

        private static string GenerateServisKayitEmailHtml(ServisKaydi servis, Musteri musteri, Cihaz cihaz)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: 'Segoe UI', Arial, sans-serif; background-color: #f5f5f5; margin: 0; padding: 20px; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); overflow: hidden; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; }}
        .header h1 {{ margin: 0; font-size: 24px; }}
        .header p {{ margin: 10px 0 0 0; opacity: 0.9; }}
        .content {{ padding: 30px; }}
        .info-box {{ background-color: #f8f9fa; border-radius: 8px; padding: 20px; margin-bottom: 20px; }}
        .info-box h3 {{ margin: 0 0 15px 0; color: #333; border-bottom: 2px solid #667eea; padding-bottom: 10px; }}
        .info-row {{ display: flex; margin-bottom: 10px; }}
        .info-label {{ font-weight: bold; color: #666; width: 140px; }}
        .info-value {{ color: #333; }}
        .status {{ display: inline-block; padding: 8px 16px; border-radius: 20px; font-weight: bold; background-color: #ffc107; color: #000; }}
        .footer {{ background-color: #f8f9fa; padding: 20px; text-align: center; color: #666; font-size: 12px; }}
        .servis-no {{ font-size: 28px; font-weight: bold; color: #667eea; text-align: center; margin: 20px 0; padding: 15px; background-color: #f0f4ff; border-radius: 8px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🔧 Teknik Servis</h1>
            <p>Servis Kaydınız Oluşturuldu</p>
        </div>
        
        <div class='content'>
            <p>Sayın <strong>{musteri.AdSoyad}</strong>,</p>
            <p>Cihazınız için servis kaydı başarıyla oluşturulmuştur. Aşağıda servis detaylarını bulabilirsiniz.</p>
            
            <div class='servis-no'>
                📋 {servis.ServisNo}
            </div>

            <div class='info-box'>
                <h3>📱 Cihaz Bilgileri</h3>
                <div class='info-row'><span class='info-label'>Cihaz Türü:</span><span class='info-value'>{cihaz.CihazTuru}</span></div>
                <div class='info-row'><span class='info-label'>Marka/Model:</span><span class='info-value'>{cihaz.Marka} {cihaz.Model}</span></div>
                <div class='info-row'><span class='info-label'>Seri No:</span><span class='info-value'>{cihaz.SeriNo}</span></div>
            </div>

            <div class='info-box'>
                <h3>⚠️ Arıza Bilgileri</h3>
                <div class='info-row'><span class='info-label'>Arıza:</span><span class='info-value'>{servis.Ariza}</span></div>
                <div class='info-row'><span class='info-label'>Detay:</span><span class='info-value'>{(string.IsNullOrEmpty(servis.ArizaDetay) ? "-" : servis.ArizaDetay)}</span></div>
            </div>

            <div class='info-box'>
                <h3>📅 Servis Durumu</h3>
                <div class='info-row'><span class='info-label'>Giriş Tarihi:</span><span class='info-value'>{servis.GirisTarihi:dd.MM.yyyy HH:mm}</span></div>
                <div class='info-row'><span class='info-label'>Durum:</span><span class='status'>{servis.Durum}</span></div>
            </div>

            <div class='info-box'>
                <h3>💰 Ücret Bilgileri</h3>
                <div class='info-row'><span class='info-label'>İşçilik Ücreti:</span><span class='info-value'>₺{servis.IscilikUcreti:N2}</span></div>
                <div class='info-row'><span class='info-label'>Parça Ücreti:</span><span class='info-value'>₺{servis.ParcaUcreti:N2}</span></div>
                <div class='info-row' style='border-top: 2px solid #667eea; padding-top: 10px; margin-top: 10px;'>
                    <span class='info-label' style='font-size: 16px;'>TOPLAM:</span>
                    <span class='info-value' style='font-size: 18px; font-weight: bold; color: #28a745;'>₺{servis.ToplamUcret:N2}</span>
                </div>
            </div>

            <p style='margin-top: 20px; color: #666;'>
                Servis süreciniz hakkında size bilgilendirme yapılacaktır. 
                Sorularınız için bizimle iletişime geçebilirsiniz.
            </p>
        </div>

        <div class='footer'>
            <p>Bu e-posta otomatik olarak gönderilmiştir.</p>
            <p>© {DateTime.Now.Year} Teknik Servis Otomasyonu</p>
        </div>
    </div>
</body>
</html>";
        }

        private static string GenerateDurumGuncelleEmailHtml(ServisKaydi servis, Musteri musteri, Cihaz cihaz, string yeniDurum)
        {
            var durumRenk = yeniDurum switch
            {
                "Tamamlandı" => "#28a745",
                "İşlemde" => "#17a2b8",
                "Teslim Edildi" => "#6c757d",
                "İptal" => "#dc3545",
                _ => "#ffc107"
            };

            var durumMesaj = yeniDurum switch
            {
                "Tamamlandı" => "Cihazınızın onarımı tamamlanmıştır. Teslim almak için servisimize gelebilirsiniz.",
                "İşlemde" => "Cihazınız üzerinde çalışılmaktadır.",
                "Teslim Edildi" => "Cihazınız teslim edilmiştir. Bizi tercih ettiğiniz için teşekkür ederiz.",
                "İptal" => "Servis kaydınız iptal edilmiştir.",
                _ => "Servis kaydınızın durumu güncellenmiştir."
            };

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: 'Segoe UI', Arial, sans-serif; background-color: #f5f5f5; margin: 0; padding: 20px; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); overflow: hidden; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; }}
        .header h1 {{ margin: 0; font-size: 24px; }}
        .content {{ padding: 30px; }}
        .status-box {{ text-align: center; padding: 30px; background-color: #f8f9fa; border-radius: 8px; margin: 20px 0; }}
        .status {{ display: inline-block; padding: 15px 30px; border-radius: 30px; font-weight: bold; font-size: 20px; background-color: {durumRenk}; color: white; }}
        .servis-no {{ font-size: 18px; color: #667eea; margin-top: 10px; }}
        .info-box {{ background-color: #f8f9fa; border-radius: 8px; padding: 20px; margin-bottom: 20px; }}
        .info-row {{ display: flex; margin-bottom: 8px; }}
        .info-label {{ font-weight: bold; color: #666; width: 120px; }}
        .info-value {{ color: #333; }}
        .message {{ padding: 20px; background-color: #e8f4fd; border-left: 4px solid #667eea; margin: 20px 0; }}
        .footer {{ background-color: #f8f9fa; padding: 20px; text-align: center; color: #666; font-size: 12px; }}
        .price-box {{ background-color: #f0fff4; border: 2px solid #28a745; border-radius: 8px; padding: 20px; margin: 20px 0; }}
        .total-price {{ font-size: 24px; font-weight: bold; color: #28a745; text-align: center; margin-top: 10px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🔧 Servis Durumu Güncellendi</h1>
        </div>
        
        <div class='content'>
            <p>Sayın <strong>{musteri.AdSoyad}</strong>,</p>
            
            <div class='status-box'>
                <div class='status'>{yeniDurum}</div>
                <div class='servis-no'>📋 {servis.ServisNo}</div>
            </div>

            <div class='message'>
                <strong>📢 Bilgilendirme:</strong><br>
                {durumMesaj}
            </div>

            <div class='info-box'>
                <h4 style='margin-top:0;'>📱 Cihaz: {cihaz.Marka} {cihaz.Model}</h4>
                <div class='info-row'><span class='info-label'>Arıza:</span><span class='info-value'>{servis.Ariza}</span></div>
                <div class='info-row'><span class='info-label'>Giriş Tarihi:</span><span class='info-value'>{servis.GirisTarihi:dd.MM.yyyy}</span></div>
                {(string.IsNullOrEmpty(servis.YapilanIslemler) ? "" : $"<div class='info-row'><span class='info-label'>Yapılan İşlem:</span><span class='info-value'>{servis.YapilanIslemler}</span></div>")}
            </div>

            <div class='price-box'>
                <h4 style='margin: 0 0 15px 0; color: #333;'>💰 Ücret Bilgileri</h4>
                <div class='info-row'><span class='info-label'>İşçilik:</span><span class='info-value'>₺{servis.IscilikUcreti:N2}</span></div>
                <div class='info-row'><span class='info-label'>Parça:</span><span class='info-value'>₺{servis.ParcaUcreti:N2}</span></div>
                <div class='total-price'>TOPLAM: ₺{servis.ToplamUcret:N2}</div>
                <div style='text-align: center; margin-top: 10px; color: #666;'>Tahsilat: {servis.TahsilatDurumu}</div>
            </div>
        </div>

        <div class='footer'>
            <p>Bu e-posta otomatik olarak gönderilmiştir.</p>
            <p>© {DateTime.Now.Year} Teknik Servis Otomasyonu</p>
        </div>
    </div>
</body>
</html>";
        }

        public static async Task<(bool success, string message)> TestConnectionAsync()
        {
            try
            {
                await LoadAyarlarAsync();
                
                if (_ayarlar == null)
                    return (false, "E-posta ayarları bulunamadı.");

                using var smtpClient = new SmtpClient(_ayarlar.SmtpServer)
                {
                    Port = _ayarlar.SmtpPort,
                    Credentials = new NetworkCredential(_ayarlar.GondericiEmail, _ayarlar.GondericiSifre),
                    EnableSsl = _ayarlar.SslKullan,
                    Timeout = 10000
                };

                // Test bağlantısı - kendine e-posta gönder
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_ayarlar.GondericiEmail, _ayarlar.GondericiAdi),
                    Subject = "Test E-postası - Teknik Servis",
                    Body = "Bu bir test e-postasıdır. E-posta ayarlarınız doğru yapılandırılmış.",
                    IsBodyHtml = false
                };
                mailMessage.To.Add(_ayarlar.GondericiEmail);

                await smtpClient.SendMailAsync(mailMessage);
                return (true, "Bağlantı başarılı! Test e-postası gönderildi.");
            }
            catch (Exception ex)
            {
                return (false, $"Bağlantı hatası: {ex.Message}");
            }
        }
    }
}

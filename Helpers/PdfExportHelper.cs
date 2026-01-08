using DevExpress.XtraPrinting;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using TeknikServisOtomasyon.Data.Repositories;
using TeknikServisOtomasyon.Models;

namespace TeknikServisOtomasyon.Helpers
{
    public static class PdfExportHelper
    {
        public static async Task<string> ExportServisDetayAsync(int servisId)
        {
            var servisRepository = new ServisKaydiRepository();
            var musteriRepository = new MusteriRepository();
            var cihazRepository = new CihazRepository();
            var fotografRepository = new ServisFotografRepository();

            var servis = await servisRepository.GetByIdAsync(servisId);
            if (servis == null)
                throw new Exception("Servis kaydı bulunamadı.");

            var musteri = await musteriRepository.GetByIdAsync(servis.MusteriId);
            var cihaz = await cihazRepository.GetByIdAsync(servis.CihazId);
            var fotograflar = await fotografRepository.GetByServisIdAsync(servisId);

            // PDF klasörünü oluştur
            var pdfKlasoru = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "TeknikServisOtomasyon",
                "PDF"
            );

            if (!Directory.Exists(pdfKlasoru))
                Directory.CreateDirectory(pdfKlasoru);

            var pdfDosyaYolu = Path.Combine(pdfKlasoru, $"Servis_{servis.ServisNo}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

            // PrintingSystem kullanarak PDF oluştur
            using (var ps = new PrintingSystem())
            {
                var link = new Link(ps);
                link.CreateDetailArea += (s, e) =>
                {
                    CreateServisDetayContent(e.Graph, servis, musteri, cihaz, fotograflar);
                };

                link.CreateDocument();

                // PDF olarak export et
                ps.ExportToPdf(pdfDosyaYolu);
            }

            return pdfDosyaYolu;
        }

        private static void CreateServisDetayContent(BrickGraphics graph, ServisKaydi servis, Musteri? musteri, Cihaz? cihaz, System.Collections.Generic.List<ServisFotograf> fotograflar)
        {
            var y = 0f;
            var pageWidth = 750f;
            var margin = 20f;

            // Başlık
            var headerFont = new Font("Segoe UI", 18, FontStyle.Bold);
            var titleFont = new Font("Segoe UI", 12, FontStyle.Bold);
            var normalFont = new Font("Segoe UI", 10);
            var smallFont = new Font("Segoe UI", 9);

            // Şirket Başlığı
            var headerBrick = graph.DrawString("TEKNİK SERVİS", Color.DarkBlue, 
                new RectangleF(margin, y, pageWidth - 2 * margin, 30), BorderSide.None);
            headerBrick.Font = headerFont;
            y += 35;

            graph.DrawString("SERVİS FİŞİ", Color.Black,
                new RectangleF(margin, y, pageWidth - 2 * margin, 25), BorderSide.None).Font = titleFont;
            y += 40;

            // Servis No ve Tarih
            var servisNoBrick = graph.DrawString($"Servis No: {servis.ServisNo}", Color.Black,
                new RectangleF(margin, y, 300, 20), BorderSide.None);
            servisNoBrick.Font = titleFont;

            var tarihBrick = graph.DrawString($"Tarih: {servis.GirisTarihi:dd.MM.yyyy HH:mm}", Color.Black,
                new RectangleF(400, y, 300, 20), BorderSide.None);
            tarihBrick.Font = normalFont;
            y += 30;

            // Durum
            var durumRenk = servis.Durum switch
            {
                "Beklemede" => Color.Orange,
                "İşlemde" => Color.Blue,
                "Tamamlandı" => Color.Green,
                "Teslim Edildi" => Color.DarkGreen,
                "İptal" => Color.Red,
                _ => Color.Black
            };
            graph.DrawString($"Durum: {servis.Durum}", durumRenk,
                new RectangleF(margin, y, 200, 20), BorderSide.None).Font = titleFont;

            graph.DrawString($"Öncelik: {servis.OncelikDurumu}", Color.Black,
                new RectangleF(250, y, 200, 20), BorderSide.None).Font = normalFont;
            y += 40;

            // Çizgi
            graph.DrawLine(new PointF(margin, y), new PointF(pageWidth - margin, y), Color.Gray, 1);
            y += 15;

            // Müşteri Bilgileri
            graph.DrawString("MÜŞTERİ BİLGİLERİ", Color.DarkBlue,
                new RectangleF(margin, y, 300, 20), BorderSide.None).Font = titleFont;
            y += 25;

            graph.DrawString($"Ad Soyad: {musteri?.AdSoyad ?? "-"}", Color.Black,
                new RectangleF(margin, y, 350, 18), BorderSide.None).Font = normalFont;
            graph.DrawString($"Telefon: {musteri?.Telefon ?? "-"}", Color.Black,
                new RectangleF(400, y, 300, 18), BorderSide.None).Font = normalFont;
            y += 22;

            graph.DrawString($"E-posta: {musteri?.Email ?? "-"}", Color.Black,
                new RectangleF(margin, y, 350, 18), BorderSide.None).Font = normalFont;
            y += 22;

            graph.DrawString($"Adres: {musteri?.Adres ?? "-"}", Color.Black,
                new RectangleF(margin, y, pageWidth - 2 * margin, 18), BorderSide.None).Font = normalFont;
            y += 35;

            // Cihaz Bilgileri
            graph.DrawString("CİHAZ BİLGİLERİ", Color.DarkBlue,
                new RectangleF(margin, y, 300, 20), BorderSide.None).Font = titleFont;
            y += 25;

            graph.DrawString($"Cihaz Türü: {cihaz?.CihazTuru ?? "-"}", Color.Black,
                new RectangleF(margin, y, 250, 18), BorderSide.None).Font = normalFont;
            graph.DrawString($"Marka: {cihaz?.Marka ?? "-"}", Color.Black,
                new RectangleF(280, y, 200, 18), BorderSide.None).Font = normalFont;
            graph.DrawString($"Model: {cihaz?.Model ?? "-"}", Color.Black,
                new RectangleF(500, y, 200, 18), BorderSide.None).Font = normalFont;
            y += 22;

            graph.DrawString($"Seri No: {cihaz?.SeriNo ?? "-"}", Color.Black,
                new RectangleF(margin, y, 300, 18), BorderSide.None).Font = normalFont;
            graph.DrawString($"IMEI: {cihaz?.IMEI ?? "-"}", Color.Black,
                new RectangleF(350, y, 300, 18), BorderSide.None).Font = normalFont;
            y += 35;

            // Arıza Bilgileri
            graph.DrawString("ARIZA BİLGİLERİ", Color.DarkBlue,
                new RectangleF(margin, y, 300, 20), BorderSide.None).Font = titleFont;
            y += 25;

            graph.DrawString($"Arıza: {servis.Ariza}", Color.Black,
                new RectangleF(margin, y, pageWidth - 2 * margin, 18), BorderSide.None).Font = normalFont;
            y += 22;

            if (!string.IsNullOrEmpty(servis.ArizaDetay))
            {
                graph.DrawString($"Detay: {servis.ArizaDetay}", Color.Black,
                    new RectangleF(margin, y, pageWidth - 2 * margin, 40), BorderSide.None).Font = smallFont;
                y += 45;
            }
            else
            {
                y += 15;
            }

            // Yapılan İşlemler
            if (!string.IsNullOrEmpty(servis.YapilanIslemler))
            {
                graph.DrawString("YAPILAN İŞLEMLER", Color.DarkBlue,
                    new RectangleF(margin, y, 300, 20), BorderSide.None).Font = titleFont;
                y += 25;

                graph.DrawString(servis.YapilanIslemler, Color.Black,
                    new RectangleF(margin, y, pageWidth - 2 * margin, 60), BorderSide.None).Font = normalFont;
                y += 65;
            }

            // Kullanılan Parçalar
            if (!string.IsNullOrEmpty(servis.KullanilanParcalar))
            {
                graph.DrawString("KULLANILAN PARÇALAR", Color.DarkBlue,
                    new RectangleF(margin, y, 300, 20), BorderSide.None).Font = titleFont;
                y += 25;

                graph.DrawString(servis.KullanilanParcalar, Color.Black,
                    new RectangleF(margin, y, pageWidth - 2 * margin, 40), BorderSide.None).Font = normalFont;
                y += 45;
            }

            // Çizgi
            graph.DrawLine(new PointF(margin, y), new PointF(pageWidth - margin, y), Color.Gray, 1);
            y += 15;

            // Ücret Bilgileri
            graph.DrawString("ÜCRET BİLGİLERİ", Color.DarkBlue,
                new RectangleF(margin, y, 300, 20), BorderSide.None).Font = titleFont;
            y += 25;

            graph.DrawString($"İşçilik Ücreti: ₺{servis.IscilikUcreti:N2}", Color.Black,
                new RectangleF(margin, y, 200, 18), BorderSide.None).Font = normalFont;
            graph.DrawString($"Parça Ücreti: ₺{servis.ParcaUcreti:N2}", Color.Black,
                new RectangleF(250, y, 200, 18), BorderSide.None).Font = normalFont;
            y += 25;

            graph.DrawString($"TOPLAM: ₺{servis.ToplamUcret:N2}", Color.DarkGreen,
                new RectangleF(margin, y, 200, 22), BorderSide.None).Font = titleFont;
            graph.DrawString($"Tahsilat: {servis.TahsilatDurumu} (₺{servis.OdenenTutar:N2})", Color.Black,
                new RectangleF(250, y, 300, 18), BorderSide.None).Font = normalFont;
            y += 40;

            // Teslim Tarihi
            if (servis.TeslimTarihi.HasValue)
            {
                graph.DrawString($"Teslim Tarihi: {servis.TeslimTarihi:dd.MM.yyyy HH:mm}", Color.Black,
                    new RectangleF(margin, y, 300, 18), BorderSide.None).Font = normalFont;
                y += 30;
            }

            // Notlar
            if (!string.IsNullOrEmpty(servis.Notlar))
            {
                graph.DrawString("NOTLAR", Color.DarkBlue,
                    new RectangleF(margin, y, 300, 20), BorderSide.None).Font = titleFont;
                y += 25;

                graph.DrawString(servis.Notlar, Color.Black,
                    new RectangleF(margin, y, pageWidth - 2 * margin, 40), BorderSide.None).Font = smallFont;
                y += 50;
            }

            // Fotoğraflar
            if (fotograflar.Count > 0)
            {
                y += 20;

                // Sayfa geçişlerini manuel kontrol etmek yerine BrickGraphics'in akışını kullanmaya çalışıyoruz
                // Ancak CreateDetailArea bir tek sayfa canvası gibi davranır.
                // DevExpress PrintingSystem'de sayfa sonu eklemek için BrickGraphics yerine Link kullanmak daha doğrudur.
                // Basit çözüm: Y koordinatı çok arttıysa uyaralım veya limiti görmezden gelelim (PrintingSystem kendisi bölebilir)
                
                graph.DrawString($"FOTOĞRAFLAR ({fotograflar.Count} adet)", Color.DarkBlue,
                    new RectangleF(margin, y, 300, 20), BorderSide.None).Font = titleFont;
                y += 25;

                var xPos = margin;
                var imgWidth = 150f;
                var imgHeight = 120f;

                foreach (var foto in fotograflar)
                {
                    if (File.Exists(foto.DosyaYolu))
                    {
                        try
                        {
                            // "Parameter is not valid" hatasını önlemek için güvenli resim yükleme
                            // Resmi yüklerken MemoryStream kullanarak dosya kilidini önlüyoruz ve formatı ensure ediyoruz
                            Image? safeImage = null;
                            using (var fs = new FileStream(foto.DosyaYolu, FileMode.Open, FileAccess.Read))
                            {
                                using (var original = Image.FromStream(fs))
                                {
                                    // Resmi klonla veya yeni bir bitmap oluştur
                                    safeImage = new Bitmap(original);
                                }
                            }

                            if (safeImage != null)
                            {
                                var imgBrick = graph.DrawImage(safeImage, new RectangleF(xPos, y, imgWidth, imgHeight));
                            }
                            
                            graph.DrawString(foto.FotografTipi, Color.Gray,
                                new RectangleF(xPos, y + imgHeight + 2, imgWidth, 15), BorderSide.None).Font = smallFont;
                        }
                        catch 
                        { 
                            graph.DrawString("[Resim Yüklenemedi]", Color.Red,
                                new RectangleF(xPos, y, imgWidth, 20), BorderSide.None).Font = smallFont;
                        }
                    }

                    xPos += imgWidth + 15;
                    if (xPos > pageWidth - imgWidth - margin)
                    {
                        xPos = margin;
                        y += imgHeight + 30;
                    }
                }
            }

            // Alt bilgi
            y += 80;
            graph.DrawLine(new PointF(margin, y), new PointF(pageWidth - margin, y), Color.Gray, 1);
            y += 10;

            graph.DrawString($"Bu belge {DateTime.Now:dd.MM.yyyy HH:mm} tarihinde oluşturulmuştur.", Color.Gray,
                new RectangleF(margin, y, pageWidth - 2 * margin, 15), BorderSide.None).Font = smallFont;
        }

        public static string GetPdfKlasoru()
        {
            var pdfKlasoru = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "TeknikServisOtomasyon",
                "PDF"
            );

            if (!Directory.Exists(pdfKlasoru))
                Directory.CreateDirectory(pdfKlasoru);

            return pdfKlasoru;
        }
    }
}

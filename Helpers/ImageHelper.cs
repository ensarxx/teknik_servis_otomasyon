using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace TeknikServisOtomasyon.Helpers
{
    public static class ImageHelper
    {
        private static readonly string FotoKlasoru = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "TeknikServisOtomasyon",
            "Fotograflar"
        );

        static ImageHelper()
        {
            if (!Directory.Exists(FotoKlasoru))
                Directory.CreateDirectory(FotoKlasoru);
        }

        public static string GetFotografKlasoru()
        {
            return FotoKlasoru;
        }

        public static string SaveImage(string sourceFilePath, int servisId, string fotografTipi)
        {
            try
            {
                var servisKlasoru = Path.Combine(FotoKlasoru, $"Servis_{servisId}");
                if (!Directory.Exists(servisKlasoru))
                    Directory.CreateDirectory(servisKlasoru);

                var extension = Path.GetExtension(sourceFilePath);
                var yeniDosyaAdi = $"{fotografTipi}_{DateTime.Now:yyyyMMdd_HHmmss}{extension}";
                var hedefYol = Path.Combine(servisKlasoru, yeniDosyaAdi);

                // Görseli yeniden boyutlandır ve kaydet
                using (var originalImage = Image.FromFile(sourceFilePath))
                {
                    var resizedImage = ResizeImage(originalImage, 1200, 900);
                    
                    // JPEG olarak kaydet
                    var jpegEncoder = GetEncoder(ImageFormat.Jpeg);
                    var encoderParams = new EncoderParameters(1);
                    encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 85L);
                    
                    hedefYol = Path.ChangeExtension(hedefYol, ".jpg");
                    resizedImage.Save(hedefYol, jpegEncoder, encoderParams);
                    resizedImage.Dispose();
                }

                return hedefYol;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fotoğraf kaydedilirken hata oluştu: {ex.Message}");
            }
        }

        public static Image ResizeImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            if (ratio >= 1)
                return new Bitmap(image);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);
            using (var graphics = Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }

        public static Image CreateThumbnail(string imagePath, int width, int height)
        {
            try
            {
                using (var originalImage = Image.FromFile(imagePath))
                {
                    return ResizeImage(originalImage, width, height);
                }
            }
            catch
            {
                // Hata durumunda placeholder görsel döndür
                var placeholder = new Bitmap(width, height);
                using (var g = Graphics.FromImage(placeholder))
                {
                    g.Clear(Color.Gray);
                    g.DrawString("?", new Font("Segoe UI", 24), Brushes.White, width / 2 - 10, height / 2 - 15);
                }
                return placeholder;
            }
        }

        public static void DeleteServisFotograflari(int servisId)
        {
            var servisKlasoru = Path.Combine(FotoKlasoru, $"Servis_{servisId}");
            if (Directory.Exists(servisKlasoru))
            {
                Directory.Delete(servisKlasoru, true);
            }
        }

        public static void DeleteFotograf(string dosyaYolu)
        {
            if (File.Exists(dosyaYolu))
            {
                File.Delete(dosyaYolu);
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageEncoders();
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                    return codec;
            }
            return null!;
        }
    }
}

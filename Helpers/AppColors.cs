using System;
using System.Drawing;

namespace TeknikServisOtomasyon.Helpers
{
    public static class AppColors
    {
        // Dark Mode Ana renkler
        public static Color Primary = Color.FromArgb(0, 150, 255);        // Daha parlak mavi
        public static Color PrimaryDark = Color.FromArgb(0, 120, 215);
        public static Color Secondary = Color.FromArgb(150, 160, 170);
        public static Color Success = Color.FromArgb(50, 205, 100);       // Daha parlak yeşil
        public static Color Danger = Color.FromArgb(255, 80, 100);        // Daha parlak kırmızı
        public static Color Warning = Color.FromArgb(255, 200, 50);       // Daha parlak sarı
        public static Color Info = Color.FromArgb(50, 180, 220);          // Daha parlak cyan

        // Durum renkleri (Dark mode için optimize edilmiş)
        public static Color Beklemede = Color.FromArgb(255, 200, 50);     // Parlak sarı
        public static Color Islemde = Color.FromArgb(50, 180, 220);       // Parlak cyan
        public static Color Tamamlandi = Color.FromArgb(50, 205, 100);    // Parlak yeşil
        public static Color TeslimEdildi = Color.FromArgb(130, 140, 150);
        public static Color Iptal = Color.FromArgb(255, 80, 100);         // Parlak kırmızı

        // Öncelik renkleri
        public static Color OncelikDusuk = Color.FromArgb(130, 140, 150);
        public static Color OncelikNormal = Color.FromArgb(50, 180, 220);
        public static Color OncelikYuksek = Color.FromArgb(255, 200, 50);
        public static Color OncelikAcil = Color.FromArgb(255, 80, 100);

        // Dark Mode Arka plan renkleri
        public static Color BackgroundDark = Color.FromArgb(30, 30, 30);
        public static Color BackgroundDarkSecondary = Color.FromArgb(45, 45, 48);
        public static Color BackgroundDarkTertiary = Color.FromArgb(60, 60, 65);
        public static Color TextLight = Color.FromArgb(230, 230, 230);
        public static Color TextSecondary = Color.FromArgb(180, 180, 180);
        public static Color BorderDark = Color.FromArgb(70, 70, 75);

        public static Color GetDurumRengi(string durum)
        {
            return durum switch
            {
                "Beklemede" => Beklemede,
                "İşlemde" => Islemde,
                "Tamamlandı" => Tamamlandi,
                "Teslim Edildi" => TeslimEdildi,
                "İptal" => Iptal,
                _ => Secondary
            };
        }

        public static Color GetOncelikRengi(string oncelik)
        {
            return oncelik switch
            {
                "Düşük" => OncelikDusuk,
                "Normal" => OncelikNormal,
                "Yüksek" => OncelikYuksek,
                "Acil" => OncelikAcil,
                _ => Secondary
            };
        }
    }
}
